using Microsoft.AspNetCore.Mvc;
using samplesManagment.DTOs;
using samplesManagment.Middleware;
using samplesManagment.Repositories;
using samplesManagment.Services;

var builder = WebApplication.CreateBuilder(args);

// Los errores de validación usan la misma estructura que el resto de errores
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(ErrorResponse.FromModelState(context.ModelState)));

builder.Services.AddSwaggerGen();

// CORS para el frontend Angular
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddScoped<ISampleRepository, SampleRepository>();
builder.Services.AddScoped<ISampleService, SampleService>();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();