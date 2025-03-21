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
            AllowAnonymous();
        }

        // public override async Task HandleAsync(ClassInfoRequest req, CancellationToken ct)
        // {
        //     // var courses = new ClassInfoResponse()
        //     // {
        //     //     coursestatus = req.coursestatus,
        //     //     facultyid = req.facultyid,
        //     //     maxrow = req.maxrow,
        //     //     acadyear = req.acadyear,
        //     //     semester = req.semester,
        //     //     CAMPUSID = req.CAMPUSID,
        //     //     LEVELID = req.LEVELID,
        //     //     weekdays = req.weekdays,
        //     //     timefrom = req.timefrom,
        //     //     timeto = req.timeto,
        //     //     coursecode = req.coursecode,
        //     //     coursename = req.coursename,
        //     //     cmd = req.cmd,
        //     // };
        //     await SendAsync(courses);
        // }

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
            // ส่งข้อมูล Mock กลับไป
            // var mockResponse = new ClassInfoResponse
            // {
            //     CourseCodeVersion = "V1.0",
            //     CourseName = "Mock Course",
            //     Professors = "Prof. John Doe",
            //     Credit = "3",
            //     Language = "English",
            //     Degree = "Bachelor",
            //     Schedule = "Mon-Wed-Fri 9:00-12:00",
            //     TotalSeats = "30",
            //     Registered = "25",
            //     RemainingSeats = "5",
            //     Status = "Active"
            // };

            // ส่งข้อมูล mockResponse กลับไปยัง client
            await SendAsync(courses);
        }
    }
}

