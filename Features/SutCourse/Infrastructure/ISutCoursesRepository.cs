using Sut_API.Feafure.SutCourse.DataModels;

namespace Sut_API.Feafure.SutCourse.Infrastructure
{
    public interface ISutCoursesRepository
    {
         public Task<CoursesPageResponse> GetCourses(ClassInfoRequest data);
         public Task<ExamInfoResponse> GetExamInfo(ExamInfoRequest data);
    }
}