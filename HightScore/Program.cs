using HighScore.BL.Managers.Abstract;
using HighScore.BL.Managers.Concrete;
using HighScore.Entities.DbContexts;
using HighScore.Entities.Model.Concrete;
using HighScore.Models;
using HighScore.Models.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>(i =>
   new SmtpEmailSender(
       builder.Configuration["EmailSender:Host"],
       builder.Configuration.GetValue<int>("EmailSender:Port"),
       builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
       builder.Configuration["EmailSender:Username"],
       builder.Configuration["EmailSender:Password"])
);

builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<AppDbContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("default");
    options.UseMySQL(connectionString);
});

builder.Services.AddIdentity<MetaUser, Role>()
        .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

    options.User.RequireUniqueEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(10);
}
);



builder.Services.AddScoped<IitemManager, ItemManager>();
builder.Services.AddScoped<ICategoryManager, CategoryManager>();
builder.Services.AddScoped<IPlatformManager, PlatformManager>();
builder.Services.AddScoped<IitemCategoryManager, ItemCategoryManager>();
builder.Services.AddScoped<IitemPlatformManager, itemPlatformManager>();
builder.Services.AddScoped<IUserReviewManager, UserReviewManager>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

