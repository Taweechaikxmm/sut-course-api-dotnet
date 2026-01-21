namespace Sut_API.Feafure.SutCourse.Services
{
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using System.Net;
    using System.Text;
    using System.Web;

    public class ScrapersService
    {
        public async Task<string> ScrapeCourseDataFromPostAsync(
            string url,
            Dictionary<string, string> formData)
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
            await client.GetAsync(
                "https://reg.sut.ac.th/registrar/class_info.asp"
            );

            var courses = new List<object>();

            while (!string.IsNullOrEmpty(url))
            {
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

                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                string html = Encoding.GetEncoding("TIS-620").GetString(bytes);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var rows = doc.DocumentNode.SelectNodes("//table//tr");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null || cells.Count < 11)
                            continue;

                        var courseName =
                            HttpUtility.HtmlDecode(
                                cells[2].SelectSingleNode("font")?.InnerText?.Trim()
                            );

                        var professors = new List<string>();
                        var profNodes = cells[2].SelectNodes(".//li");

                        if (profNodes != null)
                        {
                            foreach (var p in profNodes)
                                professors.Add(HttpUtility.HtmlDecode(p.InnerText.Trim()));
                        }

                        courses.Add(new
                        {
                            CourseCodeVersion = HttpUtility.HtmlDecode(cells[1].InnerText.Trim()),
                            CourseName = courseName,
                            Professors = professors,
                            Credit = cells[3].InnerText.Trim(),
                            Language = cells[4].InnerText.Trim(),
                            Degree = cells[5].InnerText.Trim(),
                            Schedule = cells[6].InnerText.Trim(),
                            TotalSeats = cells[7].InnerText.Trim(),
                            Registered = cells[8].InnerText.Trim(),
                            RemainingSeats = cells[9].InnerText.Trim(),
                            Status = cells[10].InnerText.Trim()
                        });
                    }
                }

                // ----------------------------
                // pagination
                // ----------------------------
                var next = doc.DocumentNode
                    .SelectSingleNode("//a[contains(text(),'หน้าต่อไป')]");

                if (next != null)
                {
                    var href = next.GetAttributeValue("href", "");
                    url = string.IsNullOrEmpty(href)
                        ? ""
                        : "https://reg.sut.ac.th/registrar/" + href;
                }
                else
                {
                    url = "";
                }
            }

            return JsonConvert.SerializeObject(courses, Formatting.Indented);
        }
    }
}
