using INTERNMvc.DAL;
using INTERNMvc.Services;
using INTERNMVCNew.DAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace INTERNMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            // Add session services
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout value
                options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
                options.Cookie.IsEssential = true; // Mark the cookie as essential for the session to work even under certain privacy regulations
            });

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddScoped<Employee_DAL>();
            builder.Services.AddScoped<UserDAL>();
          

            builder.Services.AddSession();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<UserRepository>(provider => new UserRepository(connectionString));

           

            
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
            app.UseSession(); // Enable session middleware
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
