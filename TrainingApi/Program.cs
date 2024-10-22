using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TrainingApi.Services;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using TrainingApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host
    .UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.File(
                "LOG/log-.txt",
                rollingInterval: RollingInterval.Day,  // Roll logs daily
                fileSizeLimitBytes: null,              // No size limit on a single file
                rollOnFileSizeLimit: false,            // Do not create new files based on size
                retainedFileCountLimit: 5,             // Keep only 5 days of logs
                shared: true                           // Allow log sharing between processes
            )
    );



builder.Services.AddDbContext<DataBaseContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("sqlcon")));

builder.Services.AddScoped<IRepository<Trainings>, TrainingsRepository>();
builder.Services.AddScoped<IRepository<TrainingTeam>, TrainingTeamRepository>();
builder.Services.AddScoped<ITrainingsService, TrainingsService>();
builder.Services.AddScoped<ITrainingTeamService, TrainingTeamService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "standard authorization header using the bearer scheme (/*bearer {token}/*)",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    policy =>
    {
        policy.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();
app.UseDeveloperExceptionPage();
app.MapControllers();

app.Run();
