using FastEndpoints;
using Sut_API.Feafure.SutCourse.Infrastructure;
using Sut_API.Feafure.SutCourse.Services;

var builder = WebApplication.CreateBuilder(args);

// ลงทะเบียน FastEndpoints
builder.Services.AddFastEndpoints();
builder.Services.AddSingleton<ScrapersService>();
builder.Services.AddSingleton<ISutCoursesRepository, SutCoursesRepository>();


var app = builder.Build();

// Map FastEndpoints
app.MapFastEndpoints();

app.Run();
