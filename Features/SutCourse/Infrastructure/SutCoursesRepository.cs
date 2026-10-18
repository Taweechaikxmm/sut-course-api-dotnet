using Newtonsoft.Json;
using Sut_API.Feafure.SutCourse.DataModels;
using Sut_API.Feafure.SutCourse.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sut_API.Feafure.SutCourse.Infrastructure
{
    public class SutCoursesRepository : ISutCoursesRepository
    {
        private readonly ScrapersService _scrapersService;
        public SutCoursesRepository(ScrapersService _scrapersService)
        {
            this._scrapersService = _scrapersService;
        }
         public async Task<CoursesPageResponse> GetCourses(ClassInfoRequest data)
        {
            var formData = new Dictionary<string, string>
            {
                { "coursestatus", data.coursestatus },
                { "facultyid", data.facultyid },
                { "maxrow", data.maxrow },
                { "acadyear", data.acadyear },
                { "semester", data.semester },
                { "CAMPUSID", data.CAMPUSID },
                { "LEVELID", data.LEVELID },
                { "coursecode", data.coursecode },
                { "coursename", data.coursename },
                { "cmd", data.cmd }
            };

            // Check and add weekdays, timefrom, timeto only if they are not null
            if (!string.IsNullOrEmpty(data.weekdays))
            {
                formData.Add("weekdays", data.weekdays);
            }

            if (!string.IsNullOrEmpty(data.timefrom))
            {
                formData.Add("timefrom", data.timefrom);
            }

            if (!string.IsNullOrEmpty(data.timeto))
            {
                formData.Add("timeto", data.timeto);
            }

            var scrapedData = await _scrapersService.ScrapeCoursePageAsync(
                "http://reg.sut.ac.th/registrar/class_info_1.asp",
                formData,
                data.nextUrl ?? ""
            );

            try
            {
                var page = JsonConvert.DeserializeObject<CoursesPageResponse>(scrapedData);
                if (page != null)
                    return new CoursesPageResponse
                    {
                        Items = page.Items ?? new List<ClassInfoResponse>(),
                        Next = string.IsNullOrWhiteSpace(page.Next) ? null : page.Next
                    };
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }



            // send data back
            return new CoursesPageResponse();
        }

        public async Task<ExamInfoResponse> GetExamInfo(ExamInfoRequest data)
        {
            var exams = await _scrapersService.ScrapeExamFromDetailPageAsync(
                data.courseid,
                data.acadyear,
                data.semester
            );

            return new ExamInfoResponse
            {
                Midterm = exams.TryGetValue("midterm", out var mid) ? mid : "",
                Final = exams.TryGetValue("final", out var fin) ? fin : ""
            };
        }
    }
}
