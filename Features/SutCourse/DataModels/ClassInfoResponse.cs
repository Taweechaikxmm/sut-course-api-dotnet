namespace Sut_API.Feafure.SutCourse.DataModels
{
    public class ClassInfoResponse
    {
        public string? CourseCodeVersion { get; set; }
        public string? CourseName { get; set; }
        public List<string?>? Professors { get; set; }
        public string? Credit { get; set; }
        public string? Language { get; set; }
        public string? Degree { get; set; }
        public string? Schedule { get; set; }
        public string? TotalSeats { get; set; }
        public string? Registered { get; set; }
        public string? RemainingSeats { get; set; }
        public string? Status { get; set; }

    }
}