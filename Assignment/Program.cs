using Assignment.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CompanyContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("CompanyContext")));

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opt => {
        opt.InvalidModelStateResponseFactory = context =>
        {
            var responseObj = new
            {
                path = context.HttpContext.Request.Path.Value,
                controller = (context.ActionDescriptor as ControllerActionDescriptor)?.ControllerName,
                action = (context.ActionDescriptor as ControllerActionDescriptor)?.ActionName,
                errors = context.ModelState.Keys.Select(k =>
                {
                    return new { field = k, Messages = context.ModelState[k]?.Errors.Select(e => e.ErrorMessage) };
                })
            };

            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var errorMessage = JsonSerializer.Serialize(responseObj, options);

            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation($"Invalid modelstate request: {errorMessage}");

            return new BadRequestObjectResult(responseObj);
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Assignment",
        policy =>
        {
            policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseCors("Assignment");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
