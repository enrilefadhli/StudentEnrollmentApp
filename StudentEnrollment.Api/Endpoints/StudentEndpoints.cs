using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using StudentEnrollment.Data;
using AutoMapper;
using StudentEnrollment.Api.DTOs.Student;
namespace StudentEnrollment.Api.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Student").WithTags(nameof(Student));

        group.MapGet("/", async (StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var student = await db.Students.ToListAsync();
            var data = mapper.Map<List<StudentDto>>(student);
            return data;
        })
        .WithName("GetAllStudents")
        .WithOpenApi()
        .Produces<List<StudentDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (int id, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            return await db.Students.FindAsync(id)
                is Student model
                    ? Results.Ok(mapper.Map<StudentDto>(model))
                    : Results.NotFound();
        })
        .WithName("GetStudentById")
        .WithOpenApi()
        .Produces<StudentDto>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, StudentDto studentDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var foundModel = await db.Students.FindAsync(id);
            if (foundModel == null)
            {
                return Results.NotFound();
            }
            mapper.Map(studentDto, foundModel);
            await db.SaveChangesAsync();
            return Results.Ok();
        })
        .WithName("UpdateStudent")
        .WithOpenApi()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/", async (CreateStudentDto studentDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var student = mapper.Map<Student>(studentDto);
            db.Students.Add(student);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Student/{student.Id}", student);
        })
        .WithName("CreateStudent")
        .WithOpenApi()
        .Produces<Student>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async (int id, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            if (await db.Students.FindAsync(id) is Student student)
            {
                db.Students.Remove(student);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }
            return Results.NotFound();
        })
        .WithName("DeleteStudent")
        .WithOpenApi()
        .Produces<Student>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);
    }
}
