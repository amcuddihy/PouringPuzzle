using PouringPuzzle.Components;
using PouringPuzzle.Services;
using PouringPuzzle.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// My additions

builder.Services.AddScoped<PouringPuzzleService>();

builder.Services.AddScoped<PuzzleViewModel>();

// End my additions

if (!builder.Environment.IsDevelopment()) {
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
