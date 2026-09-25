using ApiConstants = BookyServer.Api.Constants;
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
        options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue(
            ApiConstants.Configuration.RequireConfirmedEmail,
            false);
    })
    .AddEntityFrameworkStores<BookyServerDbContext>();

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
        options.SwaggerEndpoint(ApiConstants.OpenApi.Endpoint, ApiConstants.OpenApi.Title);
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

app.MapGroup(ApiConstants.Routes.Authentication).MapIdentityApi<BookyUser>();
app.MapControllers();

app.MapGet(ApiConstants.Routes.HealthLive, () =>
    {
        return Results.Ok(new { status = ApiConstants.Statuses.Live });
    })
    .AllowAnonymous();

app.MapGet(
        ApiConstants.Routes.HealthReady,
        async (BookyServerDbContext db, CancellationToken cancellationToken) =>
        {
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? Results.Ok(new { status = ApiConstants.Statuses.Ready })
                : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        })
    .AllowAnonymous();

app.Run();
