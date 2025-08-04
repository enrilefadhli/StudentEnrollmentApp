namespace StudentEnrollment.Api.DTOs.Student
{
    public class StudentDto
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string idNumber { get; set; }
        public string picture { get; set; }
    }
}
