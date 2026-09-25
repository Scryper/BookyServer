using BookyServer.Api.Middlewares;
using BookyServer.Application;
using BookyServer.Infrastructure;
using BookyServer.Infrastructure.Identity;
using BookyServer.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddCors();

builder.Services.AddBookyApplication();
builder.Services.AddBookyInfrastructure(builder.Configuration);

builder.Services.AddIdentityApiEndpoints<BookyUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue("Authentication:RequireConfirmedEmail", false);
    })
    .AddEntityFrameworkStores<BookyServerDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-BookyServer.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
if (allowedOrigins.Length > 0)
{
    builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BookyServer API v1");
    });
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();

app.UseMiddleware<CspMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<AntiXssMiddleware>();

app.MapGroup("/api/v1/auth").MapIdentityApi<BookyUser>();
app.MapControllers();

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }))
    .AllowAnonymous();

app.MapGet("/health/ready", async (BookyServerDbContext db, CancellationToken cancellationToken) =>
{
    var canConnect = await db.Database.CanConnectAsync(cancellationToken);
    return canConnect
        ? Results.Ok(new { status = "ready" })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.Run();
