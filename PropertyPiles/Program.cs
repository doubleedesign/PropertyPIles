using PropertyPiles.Components;
using PropertyPiles.Containers;
using PropertyPiles.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddSingleton<AppState>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<ListingDataService>();
builder.Services.AddSingleton<ShortlistService>();
builder.Services.AddSingleton<NbnCoverageService>();

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