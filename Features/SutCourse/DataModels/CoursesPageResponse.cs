namespace Sut_API.Feafure.SutCourse.DataModels
{
    public class CoursesPageResponse
    {
        public List<ClassInfoResponse> Items { get; set; } = new();

        /// <summary>URL หน้าถัดไป (null = หน้าสุดท้าย)</summary>
        public string? Next { get; set; }
    }
}
