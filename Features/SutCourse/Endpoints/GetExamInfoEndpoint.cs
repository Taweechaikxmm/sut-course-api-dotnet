using FastEndpoints;
using Sut_API.Feafure.SutCourse.Commands;
using Sut_API.Feafure.SutCourse.DataModels;

// API Endpoint
namespace Sut_API.Feafure.SutCourse.Endpoints
{
    public class GetExamInfoEndpoint : Endpoint<ExamInfoRequest, ExamInfoResponse>
    {
        public override void Configure()
        {
            Post("/api/course-exam");
            Summary(s =>
            {
                s.Summary = "ดึงวันสอบกลางภาค/ประจำภาครายวิชา (SUT Course Exam)";
                s.Description =
                    "ดึงวันสอบกลางภาคและสอบประจำภาคจากระบบทะเบียน มหาวิทยาลัยเทคโนโลยีสุรนารี (SUT) " +
                    "โดยเข้าหน้ารายละเอียดรายวิชา (class_info_2.asp) ของ section นั้นๆ " +
                    "ใช้ courseId ที่ได้จากผลลัพธ์ของ POST /api/courses (ฟิลด์ courseId)";

                s.ExampleRequest = new ExamInfoRequest
                {
                    courseid = "1012371",
                    acadyear = "2569",
                    semester = "2"
                };

                s.Responses[200] = "ดึงวันสอบสำเร็จ (ค่าว่าง = ไม่มีวันสอบ/ยังไม่ประกาศ)";
                s.Responses[500] = "ไม่สามารถเชื่อมต่อระบบทะเบียนได้";
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(ExamInfoRequest req, CancellationToken ct)
        {
            var exam = await new GetExamInfo()
            {
                courseid = req.courseid,
                acadyear = req.acadyear,
                semester = req.semester
            }.ExecuteAsync(ct);
            await SendAsync(exam);
        }
    }
}
