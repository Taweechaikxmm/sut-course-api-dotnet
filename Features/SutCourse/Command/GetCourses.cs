using FastEndpoints;
using Sut_API.Feafure.SutCourse.DataModels;
using Sut_API.Feafure.SutCourse.Infrastructure;

namespace Sut_API.Feafure.SutCourse.Commands
{
    public class GetCourses : ClassInfoRequest, ICommand<CoursesPageResponse>
    {
    }

    public class GetCoursesHandler : ICommandHandler<GetCourses, CoursesPageResponse>
    {
        private readonly ISutCoursesRepository _sutCoursesRepository;
        public GetCoursesHandler(ISutCoursesRepository sutCoursesRepository)
        {
            this._sutCoursesRepository = sutCoursesRepository;
        }
        public async Task<CoursesPageResponse> ExecuteAsync(GetCourses request, CancellationToken cancellationToken)
        {
            var response = await _sutCoursesRepository.GetCourses(request);
            return response;
        }
    }
}
