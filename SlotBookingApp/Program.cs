using SlotBookingApp.Infrastructure.Helpers;
using SlotBookingApp.Infrastructure.Interfaces;
using SlotBookingApp.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IScheduleService, ScheduleService>();

builder.Services.AddScoped<DateHelper>();
builder.Services.AddScoped<SlotsHelper>();

builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri("https://draliatest.azurewebsites.net/api/");
});
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
