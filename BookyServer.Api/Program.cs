using BookyServer.Api.Middlewares;
using BookyServer.Application;
using BookyServer.Infrastructure;
using BookyServer.Infrastructure.Identity;
using BookyServer.Infrastructure.Persistence;

using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

using ApiConstants = BookyServer.Api.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddCors();

builder.Services.AddBookyApplication();
builder.Services.AddBookyInfrastructure(builder.Configuration);

builder.Services.AddIdentityCore<BookyUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<BookyServerDbContext>()
    .AddSignInManager();

var authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
});
authentication.AddIdentityCookies();

var googleClientId = builder.Configuration[ApiConstants.Configuration.GoogleClientId];
var googleClientSecret = builder.Configuration[ApiConstants.Configuration.GoogleClientSecret];
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authentication.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.SaveTokens = false;
        options.SignInScheme = IdentityConstants.ExternalScheme;
    });
}

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ApiConstants.Cookies.AuthenticationName;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ApiConstants.Authorization.AdministratorPolicy, policy =>
        policy.RequireRole(ApiConstants.Authorization.AdministratorRole));

var allowedOrigins = builder.Configuration.GetSection(ApiConstants.Configuration.AllowedCorsOrigins).Get<string[]>() ?? [];
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

app.MapControllers();

app.MapGet("/health/live", () =>
    {
        return Results.Ok(new { status = "live" });
    })
    .AllowAnonymous();

app.MapGet(
        "/health/ready",
        async (BookyServerDbContext db, CancellationToken cancellationToken) =>
        {
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? Results.Ok(new { status = "ready" })
                : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        })
    .AllowAnonymous();

app.Run();
