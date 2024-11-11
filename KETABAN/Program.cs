using KETABAN.CORE.Helper;
using KETABAN.CORE.Sender.EmailSender;
using KETABAN.CORE.Services;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Context;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KETABAN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddMemoryCache();

            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options =>
            //     {
            //    options.AccessDeniedPath = "/Account/AccessDenied?statusCode={0}";
            //    options.LoginPath = "/Account/Login";
            //     });
            #region SQL_Connection

            builder.Services.AddDbContext<_KetabanContext>(option =>
            option.UseSqlServer("Data Source=.;Initial Catalog=KETABAN_DB;Integrated Security=True;TrustServerCertificate=True"));
            #endregion

            #region Identity

            builder.Services.AddIdentity<Librarian, IdentityRole>()
                .AddEntityFrameworkStores<_KetabanContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<CustomIdentityError>();


            builder.Services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireDigit = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
            });

            #endregion

            #region IOC
            builder.Services.AddScoped<ILibrariansManagementServices, LibrariansManagementServices>();
            builder.Services.AddScoped<IAccountManagementServices, AccountManagementServices>();
            builder.Services.AddScoped<IStudentManagementServices, StudentManagementServices>();
            builder.Services.AddScoped<IBookManagementServices, BookManagementServices>();
            builder.Services.AddScoped<ILoanManagementServices, LoanManagementServices>();
            builder.Services.AddScoped<ISettingsManagementServices, SettingsManagementServices>();

            #endregion




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
            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/Home/NotFound");
            //app.UseStatusCodePagesWithReExecute("/Account/AccessDenied", " statusCode={0}" );

            app.MapControllerRoute(
           name: "default",
           pattern: "{controller=Account}/{action=Login}/{id?}");
           
            app.Run();


        }
    }
}
