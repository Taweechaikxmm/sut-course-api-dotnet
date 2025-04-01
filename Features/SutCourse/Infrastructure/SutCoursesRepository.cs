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
         public async Task<List<ClassInfoResponse>> GetCourses(ClassInfoRequest data)
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

            var scrapedData = await _scrapersService.ScrapeCourseDataFromPostAsync("http://reg.sut.ac.th/registrar/class_info_1.asp", formData);

            try
            {
                var courses = JsonConvert.DeserializeObject<List<ClassInfoResponse>>(scrapedData);
                return courses != null && courses.Count > 0 ? courses : new List<ClassInfoResponse>();
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }



            // ส่งข้อมูลกลับ
            return new List<ClassInfoResponse>();
        }
    }
}
