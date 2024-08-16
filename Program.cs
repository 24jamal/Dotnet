using INTERNMvc.DAL;
using INTERNMvc.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace INTERNMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<Employee_DAL>();
            builder.Services.AddScoped<UserDAL>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<UserRepository>(provider => new UserRepository(connectionString));

            builder.Services.AddSession();

            
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
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
