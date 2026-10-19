using NotificationService.Services;
using NotificationService.Models;
using NotificationService.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddHttpClient("CustomerService", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["CustomerService:BaseUrl"] ??
        "http://localhost:5180/");
});

builder.Services.AddSingleton<CustomerApiClient>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddSingleton<
    INotificationService,
    NotificationService.Services.NotificationService>();
    
var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();