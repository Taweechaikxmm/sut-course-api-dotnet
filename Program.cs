using FastEndpoints;
using FastEndpoints.Swagger;
using Sut_API.Feafure.SutCourse.Infrastructure;
using Sut_API.Feafure.SutCourse.Services;

var builder = WebApplication.CreateBuilder(args);

// ลงทะเบียน FastEndpoints
builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "SUT Course API";
        s.Version = "v1";
        s.Description = "API for scraping and managing SUT course schedules";
    };
});

builder.Services.AddSingleton<ScrapersService>();
builder.Services.AddSingleton<ISutCoursesRepository, SutCoursesRepository>();


var app = builder.Build();

// Map FastEndpoints
app.MapFastEndpoints();
app.UseSwaggerGen();

app.Run();
