using HSS.ERP.API.Data;
using HSS.ERP.API.Services;
using HSS.ERP.API.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTeamsApp",
        policy =>
        {
            policy.WithOrigins(
                    "https://localhost:53000", // Teams app local development
                    "https://localhost:44302", // Teams app SSL port
                    "https://*.teams.microsoft.com",
                    "https://*.microsoft.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Configure PostgreSQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Services with DI container - using factory pattern for switching
var useApiServices = builder.Configuration.GetValue<bool>("UseApiServices");

if (useApiServices)
{
    // Register API-based service implementations
    builder.Services.AddScoped<IBookingService, ApiBookingService>();
}
else
{
    // Register direct database service implementations
    builder.Services.AddScoped<IBookingService, PostgreSqlBookingService>();
}

// Register common services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInvoiceHistoryService, InvoiceHistoryService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ITmsBodyService, TmsBodyService>();

// Configure Application Insights
if (builder.Configuration.GetValue<bool>("ApplicationInsights:Enabled"))
{
    var aiOptions = new ApplicationInsightsServiceOptions
    {
        ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"]
    };
    builder.Services.AddApplicationInsightsTelemetry(aiOptions);
    builder.Services.AddScoped<IAppInsightsService, AppInsightsService>();
}
else
{
    // Register a null implementation when Application Insights is disabled
    builder.Services.AddSingleton<Microsoft.ApplicationInsights.TelemetryClient>(new Microsoft.ApplicationInsights.TelemetryClient(new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration()));
    builder.Services.AddScoped<IAppInsightsService, AppInsightsService>();
}

// Configure JWT Authentication (for future use)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<InvoiceDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowTeamsApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Unable to apply migrations. Database may not be configured.");
    }
}

app.Run();