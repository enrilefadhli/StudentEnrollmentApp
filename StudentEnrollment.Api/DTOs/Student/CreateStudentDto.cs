namespace StudentEnrollment.Api.DTOs.Student
{
    public class CreateStudentDto
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string idNumber { get; set; }
        public string picture { get; set; }
    }
}
