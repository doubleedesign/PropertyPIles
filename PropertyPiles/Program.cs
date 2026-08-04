using PropertyPiles.Components;
using PropertyPiles.Containers;
using PropertyPiles.Services;
using Spectre.Console;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Environment.SetEnvironmentVariable("DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION", "1");
Environment.SetEnvironmentVariable("ENABLE_VIRTUAL_TERMINAL_PROCESSING", "1");
AnsiConsole.Profile.Capabilities.Ansi = true;
AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.TrueColor;
AnsiConsole.Profile.Width = 160;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
// Note: Loading order matters here. If a service depends on another one, the dependency must be loaded first.
builder.Services.AddSingleton<AppState>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<ListingDataService>();
builder.Services.AddSingleton<InternetCoverageService>();
builder.Services.AddSingleton<ShortlistService>();

var app = builder.Build();

// Enable loading variables from .env locally
if (app.Environment.IsDevelopment()) {
	DotNetEnv.Env.Load(".env");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();