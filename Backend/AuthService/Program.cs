using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using AuthService.Services;
using AuthService.Clients;
using AuthService.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();                                                                                                                                                                                                                                              

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "AuthDbConnection")));

builder.Services.AddScoped<
    IAuthService,
    AuthService.Services.AuthService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration[

        "ServiceUrls:CustomerService"
        ]!);
});

builder.Services.AddHttpClient<WasherApiClient>(client =>
{
    client.BaseAddress = new Uri( builder.Configuration[

        "ServiceUrls:WasherService"
        ]!);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AdminDbContext>();

    await AdminSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();