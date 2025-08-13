using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using StudentEnrollment.Data;
using AutoMapper;
using StudentEnrollment.Api.DTOs.Enrollment;
namespace StudentEnrollment.Api.Endpoints;

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Enrollment").WithTags(nameof(Enrollment));

        group.MapGet("/", async (StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var enrollments = await db.Enrollments.ToListAsync();
            var data = mapper.Map<List<EnrollmentDto>>(enrollments);
            return data;
        })
        .WithName("GetAllEnrollments")
        .WithOpenApi()
        .Produces<List<EnrollmentDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (int id, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            return await db.Enrollments.FindAsync(id)
                is Enrollment model
                    ? Results.Ok(mapper.Map<EnrollmentDto>(model))
                    : Results.NotFound();
        })
        .WithName("GetEnrollmentById")
        .WithOpenApi()
        .Produces<EnrollmentDto>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, EnrollmentDto enrollmentDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var foundModel = await db.Enrollments.FindAsync(id);
            if (foundModel == null)
            {
                return Results.NotFound();
            }
            mapper.Map(enrollmentDto, foundModel);
            await db.SaveChangesAsync();
            return Results.Ok();
        })
        .WithName("UpdateEnrollment")
        .WithOpenApi()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/", async (CreateEnrollmentDto enrollmentDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var enrollment = mapper.Map<Enrollment>(enrollmentDto);
            db.Enrollments.Add(enrollment);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Enrollment/{enrollment.Id}", enrollment);
        })
        .WithName("CreateEnrollment")
        .WithOpenApi()
        .Produces<Enrollment>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async (int id, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            if (await db.Enrollments.FindAsync(id) is Enrollment enrollment)
            {
                db.Enrollments.Remove(enrollment);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }
            return Results.NotFound();
        })
        .WithName("DeleteEnrollment")
        .WithOpenApi()
        .Produces<Student>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status200OK);
    }
}
