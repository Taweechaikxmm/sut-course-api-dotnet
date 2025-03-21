namespace Sut_API.Feafure.SutCourse.Services
{
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public class ScrapersService
    {
        private static readonly HttpClient client = new HttpClient();
        public  async Task<string> ScrapeCourseDataFromPostAsync(string url, Dictionary<string, string> formData)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var courses = new List<object>(); // List to store all courses data
                while (url != "")
                {
                    // Send POST request with form data
                    var content = new FormUrlEncodedContent(formData);
                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        return JsonConvert.SerializeObject(new { Error = $"Error: {response.StatusCode}" });
                    }

                    // Get response as byte array and convert it to TIS-620 encoding
                    byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
                    string contentTIS620 = Encoding.GetEncoding("TIS-620").GetString(responseBytes);

                    // Load the HTML content using HtmlAgilityPack
                    var document = new HtmlDocument();
                    document.LoadHtml(contentTIS620);

                    // Process table rows and add courses to the list
                    var rows = document.DocumentNode.SelectNodes("//table//tr");
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes("td");
                        if (cells != null && cells.Count >= 12)
                        {
                            var courseName = HttpUtility.HtmlDecode(cells[2].SelectSingleNode("font").InnerText.Trim()); // Extract course name from <font>

                            // Extract professors from <li> tags under <font>
                            var professors = new List<string>();
                            var professorNodes = cells[2].SelectNodes(".//li");
                            if (professorNodes != null)
                            {
                                foreach (var professor in professorNodes)
                                {
                                    professors.Add(HttpUtility.HtmlDecode(professor.InnerText.Trim()));
                                }
                            }
                            var course = new
                            {
                                CourseCodeVersion = HttpUtility.HtmlDecode(cells[1].InnerText.Trim()),
                                CourseName = courseName,
                                Professors = professors,
                                Credit = HttpUtility.HtmlDecode(cells[3].InnerText.Trim()),
                                Language = HttpUtility.HtmlDecode(cells[4].InnerText.Trim()),
                                Degree = HttpUtility.HtmlDecode(cells[5].InnerText.Trim()),
                                Schedule = HttpUtility.HtmlDecode(cells[6].InnerText.Trim()),
                                TotalSeats = HttpUtility.HtmlDecode(cells[7].InnerText.Trim()),
                                Registered = HttpUtility.HtmlDecode(cells[8].InnerText.Trim()),
                                RemainingSeats = HttpUtility.HtmlDecode(cells[9].InnerText.Trim()),
                                Status = HttpUtility.HtmlDecode(cells[10].InnerText.Trim())
                            };

                            courses.Add(course);
                        }
                    }
                    // Find the "Next" button or link
                    var nextButton = document.DocumentNode.SelectSingleNode("//a[contains(text(),'หน้าต่อไป')]");

                    if (nextButton != null)
                    {
                        // Get the URL for the next page from the 'href' attribute
                        var nextUrl = nextButton.GetAttributeValue("href", "");

                        if (nextUrl != "")
                        {
                            // Append the base URL to the next page URL
                            url = "http://reg.sut.ac.th/registrar/" + nextUrl;

                            // Continue scraping the next page
                            Console.WriteLine("Moving to next page...");
                        }
                        else
                        {
                            url = ""; // No more pages to load
                        }
                    }
                    else
                    {
                        url = ""; // No "Next" button, so stop
                    }
                }

                // Display the courses data in Console as formatted JSON
                Console.WriteLine(JsonConvert.SerializeObject(courses, Formatting.Indented));

                // Return the courses data as JSON
                return JsonConvert.SerializeObject(courses);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Error = $"Error occurred: {ex.Message}" });
            }
        }
    }
}
