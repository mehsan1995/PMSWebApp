using Application.AutoMapper;
using Application.DTOs;
using Application.Implementation;
using Application.Interfaces;
using Application.UnitOfWork;
using Common.Helper;
using DAL;
using DAL.Models;
using DAL.TenantProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PMSWebApp.Helper;
using PMSWebApp.Models;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection")
                      ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar-SA") };

    options.DefaultRequestCulture = new RequestCulture("ar-SA");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Important for cross-domain
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // If you're using HTTP
});


// 🔹 Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 🔹 Register AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// 🔹 Register services
builder.Services.AddScoped<ITenantProvider, TenantProvider>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IDepartmentTypes, DepartmentTypes>();
builder.Services.AddScoped<IDepartmentUsersService, DepartmentUsersService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();


// Required for Identity UI
builder.Services.AddRazorPages();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//    var supportedCultures = new[]
//    {
//        new CultureInfo("en-US"),
//        new CultureInfo("ar-SA")
//    };

//    options.DefaultRequestCulture = new RequestCulture("en-US");
//    options.SupportedCultures = supportedCultures;
//    options.SupportedUICultures = supportedCultures;

//    // Optional: Automatically switch based on query string or cookie
//    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
//});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "ar-SA" }
        .Select(c => new CultureInfo(c)).ToList();

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Clear default providers and add your custom provider first
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new CustomUserCultureProvider(builder.Services.BuildServiceProvider().GetRequiredService<ISettingsService>()));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();


builder.Services.AddTransient<IEmailSender, EmailSender>();
//builder.Services.Configure<SendGridSettings>(
//    builder.Configuration.GetSection("SendGrid"));



var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();

//var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
//app.UseRequestLocalization(locOptions.Value);
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.MapPost("/change-culture", async (ChangeCultureRequest request, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, HttpResponse response) =>
{
    var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
        return Results.Unauthorized();

    var settings = await unitOfWork.SettingsService.GetByIdAsync(userId);
    if (settings == null)
    {
        settings = new SettingsDto { UserId = userId, Language = request.Culture };
        await unitOfWork.SettingsService.CreateAsync(settings);
        await unitOfWork.CompleteAsync();
    }
    else
    {
        settings.Language = request.Culture;
        await unitOfWork.SettingsService.Update(userId,request.Culture);
        await unitOfWork.CompleteAsync();
    }

    response.Cookies.Append(
     CookieRequestCultureProvider.DefaultCookieName,
     CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(request.Culture)),
     new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
 );

    return Results.Ok();
});


// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // ⚠️ Required to use /Identity/Account/Login

app.Run();
