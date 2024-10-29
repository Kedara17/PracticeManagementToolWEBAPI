using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using LeadEnquiryApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewLeadApi.Services;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;


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

builder.Services.AddScoped<IRepository<NewLeadEnquiry>, NewLeadEnquiryRepository>();
builder.Services.AddScoped<INewLeadEnquiryService, NewLeadEnquiryService>();
builder.Services.AddScoped<IRepository<NewLeadEnquiryTechnology>, NewLeadEnquiryTechnologyRepository>();
builder.Services.AddScoped<INewLeadEnquiryTechnologyService, NewLeadEnquiryTechnologyService>();
builder.Services.AddScoped<IRepository<NewLeadEnquiryFollowup>, NewLeadEnquiryFollowupRepository>();
builder.Services.AddScoped<INewLeadEnquiryFollowupService, NewLeadEnquiryFollowupService>();
builder.Services.AddScoped<IRepository<NewLeadEnquiryDocuments>, NewLeadEnquiryDocumentsRepository>();
builder.Services.AddScoped<INewLeadEnquiryDocumentsService, NewLeadEnquiryDocumentsService>();

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Configure the HTTP request pipeline.
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
