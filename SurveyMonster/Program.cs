var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure HttpClient
builder.Services.AddHttpClient("SurveyMonsterApi", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
    var timeout = builder.Configuration.GetValue<int>("ApiSettings:Timeout");
    client.Timeout = TimeSpan.FromSeconds(timeout > 0 ? timeout : 30);
});

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

// Register services
builder.Services.AddScoped<SurveyMonster.Services.IApiClient, SurveyMonster.Services.ApiClient>();
builder.Services.AddScoped<SurveyMonster.Services.IAuthService, SurveyMonster.Services.AuthService>();
builder.Services.AddScoped<SurveyMonster.Services.ISurveyService, SurveyMonster.Services.SurveyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
