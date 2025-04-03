using PouringPuzzle.Components;
using PouringPuzzle.Services;
using PouringPuzzle.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Pouring Puzzle services
builder.Services.AddScoped<IPuzzleGeneratorService, PuzzleGeneratorService>();
builder.Services.AddScoped<IPuzzleSolverService, PuzzleSolverService>();

// ViewModels
builder.Services.AddScoped<PuzzleViewModel>();
builder.Services.AddScoped<PuzzleSetupViewModel>();

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
