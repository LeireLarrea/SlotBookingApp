using SlotBookingApp.Helpers;
using SlotBookingApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();  
builder.Logging.AddDebug();

var configuration = builder.Configuration;
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<DateHelper>();
builder.Services.AddScoped<SlotsHelper>();

builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri("https://draliatest.azurewebsites.net/api/");
});

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
