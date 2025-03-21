using FastEndpoints;
using Sut_API.Feafure.SutCourse.DataModels;
using Sut_API.Feafure.SutCourse.Infrastructure;

namespace Sut_API.Feafure.SutCourse.Commands
{
    public class GetCourses : ClassInfoRequest, ICommand<List<ClassInfoResponse>>
    {
    }

    public class GetCoursesHandler : ICommandHandler<GetCourses, List<ClassInfoResponse>>
    {
        private readonly ISutCoursesRepository _sutCoursesRepository;
        public GetCoursesHandler(ISutCoursesRepository _sutCoursesRepository)
        {
            this._sutCoursesRepository = _sutCoursesRepository;
        }
        public async Task<List<ClassInfoResponse>> ExecuteAsync(GetCourses request, CancellationToken cancellationToken)
        {
            var response = await _sutCoursesRepository.GetCourses(request);
            return response;
        }
    }
}
