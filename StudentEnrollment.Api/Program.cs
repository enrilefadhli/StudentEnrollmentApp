using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Data;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("StudentEnrollmentDbConnection");
builder.Services.AddDbContext<StudentEnrollmentDbContext>(options =>
{
    options.UseSqlServer(conn);
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.MapGet("/api/courses", async (StudentEnrollmentDbContext context) =>
{
    return await context.Courses.ToListAsync();
});

app.MapGet("/api/courses/{id}", async (StudentEnrollmentDbContext context, int id) =>
{
    return await context.Courses.FindAsync(id) is Course course ? Results.Ok(course) : Results.NotFound();
});

app.MapPost("/api/courses", async (StudentEnrollmentDbContext context, Course course) =>
{
    context.Courses.Add(course);
    await context.SaveChangesAsync();
    return Results.Created($"/api/courses/{course.Id}", course);
});

app.MapPut("/api/courses/{id}", async (StudentEnrollmentDbContext context, int id, Course course) =>
{
    var recordExists = await context.Courses.AnyAsync(c => c.Id == id);
    if (!recordExists) return Results.NotFound();

    context.Update(course);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/courses/{id}", async (StudentEnrollmentDbContext context, int id) =>
{
    var record = await context.Courses.FindAsync(id);
    if (record is null) return Results.NotFound();

    context.Courses.Remove(record);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
