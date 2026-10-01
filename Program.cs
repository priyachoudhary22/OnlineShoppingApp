// Creates the web application builder.
var builder = WebApplication.CreateBuilder(args);

// Adds MVC controller and view support.
builder.Services.AddControllersWithViews();

// Adds memory caching.
builder.Services.AddMemoryCache();

// Adds HttpClientFactory.
builder.Services.AddHttpClient();

// Adds session support.
builder.Services.AddSession(options =>
{
    // Sets session timeout to 30 minutes.
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Makes the session cookie inaccessible to JavaScript.
    options.Cookie.HttpOnly = true;

    // Marks the cookie as essential.
    options.Cookie.IsEssential = true;
});

// Builds the application.
var app = builder.Build();

// Enables static files.
app.UseStaticFiles();

// Enables routing.
app.UseRouting();

// Enables session.
app.UseSession();

// Enables authorization.
app.UseAuthorization();

// Defines the default MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Starts the application.
app.Run();