var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

app.MapReverseProxy();

app.Run();