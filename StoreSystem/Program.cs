using Microsoft.EntityFrameworkCore;
using StoreSystem.Interfaces;
using StoreSystem.Models;
using StoreSystem.Realizations;
using System;
using testproject.Models;

namespace StoreSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication("CookieAuth")
                            .AddCookie("CookieAuth", options =>
                             {
                                 
                                  options.LoginPath = "/Account/Login"; // Путь к странице входа
                                  options.AccessDeniedPath = "/Account/AccessDenied"; // Путь к странице отказа в доступе
                             });
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IBonusService, BonusService>();
            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Account}/{action=Profile}/{id?}");
            app.MapPost("/api/createOrder", async (HttpContext context, ApplicationDbContext db) =>
            {
                var requestBody = await context.Request.ReadFromJsonAsync<OrderRequest>();
                if (requestBody == null || !requestBody.OrderDetails.Any())
                {
                    return Results.BadRequest("Корзина пуста или данные некорректны.");
                }

                // Вычисляем общую сумму
                decimal totalAmount = requestBody.OrderDetails.Sum(od => od.Price * od.Quantity);

                var order = new Order
                {
                    CustomerId = requestBody.CustomerId,
                    EmployeeId = requestBody.EmployeeId,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    OrderDetails = requestBody.OrderDetails
                };

                db.Orders.Add(order);
                await db.SaveChangesAsync();

                return Results.Ok(new { orderId = order.Id, message = "Заказ успешно создан!" });
            });
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
