using DBro.Web.Components;
using DBro.Web.Services;
using DBro.Web.Validators;
using FluentValidation;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.PreventDuplicates = false;
});

builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", options =>
{
    options.Cookie.Name = "Cookies";
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.Cookie.MaxAge = options.ExpireTimeSpan;
    options.SlidingExpiration = false;
});

builder.Services.AddAuthorizationCore(options =>
{
    //options.AddPolicy("AkunRead", policy => policy.RequireAssertion(context => ReadOnlyAccess(context, "Akun")));
    options.AddPolicy("User", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Aktivitas", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Menu", policy => policy.RequireRole("Karyawan", "Admin"));
    options.AddPolicy("Sales", policy => policy.RequireRole("Karyawan", "Admin"));
    options.AddPolicy("Pesanan", policy => policy.RequireRole("Karyawan", "Admin"));
    options.AddPolicy("Dashboard", policy => policy.RequireRole("Karyawan", "Admin"));
    options.AddPolicy("Laporan", policy => policy.RequireRole("Karyawan", "Admin"));
    options.AddPolicy("Home", policy => policy.RequireRole("Pelanggan"));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorPages();

#region API Services

builder.Services.AddHttpClient<IAuthService, AuthService>(client => client.BaseAddress = new("https://localhost:7117/"));
builder.Services.AddHttpClient<IMenuService, MenuService>(client => client.BaseAddress = new("https://localhost:7117/"));
builder.Services.AddHttpClient<IUserService, UserService>(client => client.BaseAddress = new("https://localhost:7117/"));
builder.Services.AddHttpClient<ISalesService, SalesService>(client => client.BaseAddress = new("https://localhost:7117/"));
builder.Services.AddHttpClient<IAktivitasService, AktivitasService>(client => client.BaseAddress = new("https://localhost:7117/"));

#endregion API Services

#region Validators

builder.Services.AddScoped<IValidator<Menu>, MenuValidator>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();

#endregion Validators

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
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRequestLocalization("id-ID");

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapRazorPages();

app.Run();
