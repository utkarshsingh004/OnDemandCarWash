var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/auth/v1/swagger.json",
        "AuthService");

    options.SwaggerEndpoint(
        "/swagger/customer/v1/swagger.json",
        "CustomerService");

    options.SwaggerEndpoint(
        "/swagger/washer/v1/swagger.json",
        "WasherService");

    options.SwaggerEndpoint(
        "/swagger/booking/v1/swagger.json",
        "BookingService");

    options.SwaggerEndpoint(
        "/swagger/notification/v1/swagger.json",
        "NotificationService");
});

app.UseCors("AngularClient");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapReverseProxy();

app.Run();