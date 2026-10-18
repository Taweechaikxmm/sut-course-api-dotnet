using FastEndpoints;
using Sut_API.Feafure.SutCourse.DataModels;
using Sut_API.Feafure.SutCourse.Infrastructure;

namespace Sut_API.Feafure.SutCourse.Commands
{
    public class GetExamInfo : ExamInfoRequest, ICommand<ExamInfoResponse>
    {
    }

    public class GetExamInfoHandler : ICommandHandler<GetExamInfo, ExamInfoResponse>
    {
        private readonly ISutCoursesRepository _sutCoursesRepository;
        public GetExamInfoHandler(ISutCoursesRepository sutCoursesRepository)
        {
            this._sutCoursesRepository = sutCoursesRepository;
        }
        public async Task<ExamInfoResponse> ExecuteAsync(GetExamInfo request, CancellationToken cancellationToken)
        {
            var response = await _sutCoursesRepository.GetExamInfo(request);
            return response;
        }
    }
}
