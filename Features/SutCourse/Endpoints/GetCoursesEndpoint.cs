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

