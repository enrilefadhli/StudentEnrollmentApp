using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Data.Contracts;

namespace StudentEnrollment.Data.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(StudentEnrollmentDbContext db) : base(db)
        {

        }
        public async Task<Student> GetStudentDetails(int studentId)
        {
            var student = await _db.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);
            return student;
        }
    }
}
