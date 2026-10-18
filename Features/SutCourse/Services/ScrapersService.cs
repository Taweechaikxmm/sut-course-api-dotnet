namespace Sut_API.Feafure.SutCourse.Services
{
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public class ScrapersService
    {
        /// <summary>
        /// ดึงหน้าเดียว (ไม่ไล่ pagination เอง):
        /// - nextUrl ว่าง → POST ค้นหาใหม่ (หน้าแรก)
        /// - nextUrl มีค่า → GET หน้าถัดไป (whitelist reg.sut.ac.th เท่านั้น)
        /// คืน JSON: { Items: [...], Next: "url หน้าถัดไป" | null }
        /// </summary>
        public async Task<string> ScrapeCoursePageAsync(
            string url,
            Dictionary<string, string> formData,
            string nextUrl)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var cookies = new CookieContainer();

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookies,
                AutomaticDecompression =
                    DecompressionMethods.GZip |
                    DecompressionMethods.Deflate |
                    DecompressionMethods.Brotli
            };

            using var client = new HttpClient(handler);

            // ✅ Browser headers
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/141.0.0.0 Safari/537.36");

            client.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            client.DefaultRequestHeaders.Add("Accept-Language", "th-TH,th;q=0.9,en-US;q=0.8");

            byte[] bytes;

            if (!string.IsNullOrWhiteSpace(nextUrl))
            {
                // pagination: ใช้ URL ที่ server ให้มา อนุญาตเฉพาะ host ของ reg เท่านั้น
                if (!Uri.TryCreate(nextUrl, UriKind.Absolute, out var nextUri) ||
                    (nextUri.Host != "reg.sut.ac.th" && nextUri.Host != "reg4.sut.ac.th") ||
                    (nextUri.Scheme != Uri.UriSchemeHttps && nextUri.Scheme != Uri.UriSchemeHttp))
                {
                    return JsonConvert.SerializeObject(new { Items = new List<object>(), Next = (string?)null });
                }

                var getResp = await client.GetAsync(nextUri);
                if (!getResp.IsSuccessStatusCode)
                    return JsonConvert.SerializeObject(new { Items = new List<object>(), Next = (string?)null });

                bytes = await getResp.Content.ReadAsByteArrayAsync();
            }
            else
            {
                await client.GetAsync(
                    "https://reg.sut.ac.th/registrar/class_info.asp"
                );

                client.DefaultRequestHeaders.Referrer =
                    new Uri("https://reg.sut.ac.th/registrar/class_info.asp");

                client.DefaultRequestHeaders.Add("Origin", "https://reg.sut.ac.th");

                var content = new FormUrlEncodedContent(formData);

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        Error = response.StatusCode.ToString()
                    });
                }

                bytes = await response.Content.ReadAsByteArrayAsync();
            }

            string html = Encoding.GetEncoding("TIS-620").GetString(bytes);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var courses = new List<object>();
            var rows = doc.DocumentNode.SelectNodes("//table//tr");

            if (rows != null)
            {
                // หน้า list ปัจจุบันมีคอลัมน์ "กลุ่ม" นำหน้า รับ/ลง/เหลือ/สถานะ
                // ถ้าไม่มี (หน้าเก่า/รูปแบบอื่น) ใช้แมปเดิม
                var hasGroupColumn = html.Contains("กลุ่ม");

                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td");
                    if (cells == null || cells.Count < 11)
                        continue;

                    var groupOffset = hasGroupColumn ? 1 : 0;
                    // cells[7+groupOffset]=รับ [8+]=ลง [9+]=เหลือ [10+]=สถานะ
                    if (cells.Count < 11 + groupOffset)
                        continue;

                    // ชื่อหลัก (font แรก เช่น "ART APPRECIATION")
                    var mainFont = cells[2].SelectSingleNode("font");
                    var mainName = HttpUtility.HtmlDecode(mainFont?.InnerText?.Trim());

                    // subtitle (font size=1 color=#660000 เช่น "( กลุ่มศึกษาทั่วไปแบบเลือก... )")
                    var subtitleNode = cells[2].SelectSingleNode(".//font[@size='1' and @color='#660000']");
                    var subtitle = subtitleNode != null
                        ? HttpUtility.HtmlDecode(subtitleNode.InnerText?.Trim())
                        : null;

                    // รวม subtitle เข้าชื่อ เพื่อให้ client-side filter ค้น keyword กลางชื่อได้
                    var courseName = !string.IsNullOrWhiteSpace(subtitle)
                        ? $"{mainName} | {subtitle}"
                        : mainName;

                    var professors = new List<string>();
                    var profNodes = cells[2].SelectNodes(".//li");

                    if (profNodes != null)
                    {
                        foreach (var p in profNodes)
                            professors.Add(HttpUtility.HtmlDecode(p.InnerText.Trim()));
                    }

                    var courseIdNode = cells[1].SelectSingleNode(".//a[contains(@href,'class_info_2')]");
                    var courseId = "";
                    if (courseIdNode != null)
                    {
                        var href = courseIdNode.GetAttributeValue("href", "");
                        var m = System.Text.RegularExpressions.Regex.Match(href, @"courseid=(\d+)");
                        if (m.Success)
                            courseId = m.Groups[1].Value;
                    }

                    courses.Add(new
                    {
                        CourseId = courseId,
                        CourseCodeVersion = HttpUtility.HtmlDecode(cells[1].InnerText.Trim()),
                        CourseName = courseName,
                        Professors = professors,
                        Credit = cells[3].InnerText.Trim(),
                        Language = cells[4].InnerText.Trim(),
                        Degree = cells[5].InnerText.Trim(),
                        Schedule = cells[6].InnerText.Trim(),
                        Group = hasGroupColumn
                            ? cells[7].InnerText.Trim()
                            : "",
                        TotalSeats = cells[7 + groupOffset].InnerText.Trim(),
                        Registered = cells[8 + groupOffset].InnerText.Trim(),
                        RemainingSeats = cells[9 + groupOffset].InnerText.Trim(),
                        Status = cells[10 + groupOffset].InnerText.Trim()
                    });
                }
            }

            // ----------------------------
            // pagination: ส่ง URL "หน้าต่อไป" กลับให้ client เอง
            // ----------------------------
            string? next = null;
            var nextNode = doc.DocumentNode
                .SelectSingleNode("//a[contains(text(),'หน้าต่อไป')]");
            if (nextNode != null)
            {
                var href = nextNode.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(href))
                {
                    next = href.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? href
                        : "https://reg.sut.ac.th/registrar/" + href;
                }
            }

            return JsonConvert.SerializeObject(new { Items = courses, Next = next });
        }

        /// <summary>
        /// ดึงวันสอบกลางภาค/สอบประจำภาคจากหน้า detail (class_info_2.asp?courseid=...)
        /// หน้ารายการค้นไม่มีข้อมูลสอบ ต้องเข้าหน้านี้ต่อวิชา
        /// </summary>
        public async Task<Dictionary<string, string>> ScrapeExamFromDetailPageAsync(
            string courseId,
            string acadYear,
            string semester)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var result = new Dictionary<string, string>
            {
                ["midterm"] = "",
                ["final"] = ""
            };

            if (!System.Text.RegularExpressions.Regex.IsMatch(courseId, @"^\d+$"))
                return result;

            var url =
                "https://reg.sut.ac.th/registrar/class_info_2.asp" +
                $"?backto=home&option=0&courseid={courseId}" +
                $"&acadyear={Uri.EscapeDataString(acadYear)}&semester={Uri.EscapeDataString(semester)}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/141.0.0.0 Safari/537.36");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return result;

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            string html = Encoding.GetEncoding("TIS-620").GetString(bytes);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rows = doc.DocumentNode.SelectNodes("//table//tr");
            if (rows == null)
                return result;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                if (cells == null || cells.Count < 2)
                    continue;

                foreach (var cell in cells)
                {
                    var text = HttpUtility.HtmlDecode(cell.InnerText ?? "");
                    if (text.Contains("สอบกลางภาค") && result["midterm"] == "")
                    {
                        var value = NextCellText(cell);
                        if (value != "")
                            result["midterm"] = value;
                    }
                    else if (text.Contains("สอบประจำภาค") && result["final"] == "")
                    {
                        var value = NextCellText(cell);
                        if (value != "")
                            result["final"] = value;
                    }
                }
            }

            return result;
        }

        /// <summary>ค่าใน &lt;td&gt; ถัดไปของ cell ที่ระบุ (label อยู่ใน cell ซ้าย ค่าอยู่ใน cell ขวา)</summary>
        private static string NextCellText(HtmlNode cell)
        {
            var next = cell.NextSibling;
            while (next != null && next.Name != "td" && next.Name != "tr")
                next = next.NextSibling;

            if (next == null || next.Name != "td")
                return "";

            return Regex.Replace(
                HttpUtility.HtmlDecode(next.InnerText ?? ""),
                @"\s+",
                " "
            ).Trim();
        }
    }
}
