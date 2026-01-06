using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Data;
using Global.App.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

// Get the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure the DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Add exception filter for database-related operations
//The developer exception page runs early in the middleware pipeline
//so that it can catch unhandled exceptions thrown in middleware that follows.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity
// ensure that the necessary pages for user login, logout, and account management are included
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add Razor Pages
builder.Services.AddRazorPages();

// Configure Identity Options
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Configure Authentication with Google and Facebook
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })

    .AddFacebook(facebookOptions =>
    {
        facebookOptions.ClientId = builder.Configuration["Authentication:Facebook:ClientId"];
        facebookOptions.ClientSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); //Shows detailed error information
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Provides a user-friendly error page
    app.UseHsts();  // Enforces HTTPS communication for better security
}

// implement the middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
//re-executes the request pipeline using a specified path and query string


// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();
// Serve static files from the wwwroot folder
app.UseStaticFiles();
// Enable routing to handle URL requests
app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure the default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages
app.MapRazorPages();

// Run the application
app.Run();
