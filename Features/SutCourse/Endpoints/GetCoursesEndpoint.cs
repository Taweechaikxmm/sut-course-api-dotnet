using FastEndpoints;
using Sut_API.Feafure.SutCourse.Commands;
using Sut_API.Feafure.SutCourse.DataModels;
using Sut_API.Feafure.SutCourse.Infrastructure;

// API Endpoint
namespace Sut_API.Feafure.SutCourse.Endpoints
{
    public class GetCoursesEndpoint : Endpoint<ClassInfoRequest, List<ClassInfoResponse>>
    {
        public override void Configure()
        {
            Post("/api/courses");
            Summary(s =>
            {
                s.Summary = "ค้นหารายวิชา (SUT Course)";
                s.Description =
                    "ดึงข้อมูลรายวิชาจากระบบทะเบียน มหาวิทยาลัยเทคโนโลยีสุรนารี (SUT) " +
                    "โดยทำการ scrape ข้อมูลผ่านระบบ reg.sut.ac.th";

                s.ExampleRequest = new ClassInfoRequest
                {
                    facultyid = "all",
                    maxrow = "50",
                    acadyear = "2567",
                    semester = "3",
                    CAMPUSID = "",
                    LEVELID = "1",
                    coursecode = "103122",
                    coursename = "*",
                    cmd = "1",
                    weekdays = "2",
                    timefrom = "1",
                    timeto = "288"
                };

                s.Responses[200] = "ค้นหารายวิชาสำเร็จ";
                s.Responses[400] = "รูปแบบข้อมูลไม่ถูกต้อง";
                s.Responses[500] = "ไม่สามารถเชื่อมต่อระบบทะเบียนได้";
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(ClassInfoRequest req, CancellationToken ct)
        {
            var courses = await new GetCourses()
            {
                coursestatus = req.coursestatus,
                facultyid = req.facultyid,
                maxrow = req.maxrow,
                acadyear = req.acadyear,
                semester = req.semester,
                CAMPUSID = req.CAMPUSID,
                LEVELID = req.LEVELID,
                weekdays = req.weekdays,
                timefrom = req.timefrom,
                timeto = req.timeto,
                coursecode = req.coursecode,
                coursename = req.coursename,
                cmd = req.cmd,
            }.ExecuteAsync();
            await SendAsync(courses);
        }
    }
}

