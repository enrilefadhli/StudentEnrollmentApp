using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using StudentEnrollment.Data;
using StudentEnrollment.Api.DTOs.Course;
using AutoMapper;
namespace StudentEnrollment.Api.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Course").WithTags(nameof(Course));

        group.MapGet("/", async (StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            // Uncomment the following line to return all courses without using DTOs
            // return await db.Courses.ToListAsync();

            // Manual DTO mapping
            //var courses = await db.Courses.ToListAsync();
            //var data = new List<CourseDto>();
            //foreach (var course in courses)
            //{
            //    data.Add(new CourseDto
            //    {
            //        Id = course.Id,
            //        Title = course.Title,
            //        Credits = course.Credits
            //    });
            //
            //return data;

            // Using AutoMapper
            var courses = await db.Courses.ToListAsync();
            var data = mapper.Map<List<CourseDto>>(courses);
            return data;
        })
        .WithName("GetAllCourses")
        .WithOpenApi()
        .Produces<List<CourseDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (int id, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            return await db.Courses.FindAsync(id)
                is Course model
                    ? Results.Ok(mapper.Map<CourseDto>(model))
                    : Results.NotFound();
        })
        .WithName("GetCourseById")
        .WithOpenApi()
        .Produces<CourseDto>(StatusCodes.Status200OK);

        group.MapPut("/{id}", async (int id, CourseDto courseDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var foundModel = await db.Courses.FindAsync(id);

            if (foundModel == null)
            {
                return Results.NotFound();
            }
            mapper.Map(courseDto, foundModel);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateCourse")
        .WithOpenApi()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/", async (CreateCourseDto courseDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {   
            //var course = new Course
            //{
            //    Title = courseDto.Title,
            //    Credits = courseDto.Credits,
            //};

            var course = mapper.Map<Course>(courseDto);
            db.Courses.Add(course);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Course/{course.Id}",course);
        })
        .WithName("CreateCourse")
        .WithOpenApi()
        .Produces<Course>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async (int id, StudentEnrollmentDbContext db) =>
        {   
            if (await db.Courses.FindAsync(id) is Course course)
            {
                db.Courses.Remove(course);
                await db.SaveChangesAsync();
                return Results.Ok(course);
            }
            return Results.NotFound();

            //var affected = await db.Courses
            //    .Where(model => model.Id == id)
            //    .ExecuteDeleteAsync();
            //return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteCourse")
        .WithOpenApi()
        .Produces<Course>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
