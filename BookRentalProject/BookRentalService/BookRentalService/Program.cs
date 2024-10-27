using BookRentalService.Common;
using BookRentalService.Data;
using BookRentalService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with connection string
builder.Services.AddDbContext<BookRentalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your BookService
builder.Services.AddScoped<BookService>();

// Register EmailService with your SMTP Service
builder.Services.AddScoped<IEmailService>(sp => new SmtpEmailService("smtp.gmail.com", 465, "testmotorhouse@gmail.com", "Testmotorhouse")); // sample gmail account


// Add logging services
builder.Logging.AddConsole(); // Add console logging
builder.Logging.AddDebug();    // Add debug logging


// Configure Serilog // Used to Serilog to capture the log 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog as the logging provider

// Load errorMessages.json as a configuration source
var errorMessagesConfig = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("errorMessages.json", optional: false, reloadOnChange: true)
    .Build();

// Register it in the services collection
builder.Services.AddSingleton<IConfiguration>(errorMessagesConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
