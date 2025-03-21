using Sut_API.Feafure.SutCourse.DataModels;

namespace Sut_API.Feafure.SutCourse.Infrastructure
{
    public interface ISutCoursesRepository
    {
         public Task<List<ClassInfoResponse>> GetCourses(ClassInfoRequest data);
    }
}