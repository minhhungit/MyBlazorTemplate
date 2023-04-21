using BlazorAppWithSwagger.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using static System.Net.Mime.MediaTypeNames;

namespace BlazorAppWithSwagger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddExceptional(builder.Configuration.GetSection("Exceptional"), settings => {
                settings.Store.ApplicationName = "BlazorAppWithSwaggerEx";
                //settings.UseExceptionalPageOnThrow = Environment.IsDevelopment();

                settings.GetCustomData = (exception, data) =>
                {
                    foreach (var key in exception.Data.Keys)
                    {
                        if (key as string != null && (key as string).StartsWith("log:", StringComparison.OrdinalIgnoreCase))
                        {
                            string v;
                            var value = exception.Data[key];
                            if (value == null)
                                v = "[null]";
                            else
                                v = value.ToString();

                            data.Add((key as string)[4..], v);
                        }
                    }
                };

                settings.OnBeforeLog += (sender, args) =>
                {
                    if (args.Error.Cookies != null)
                    {
                        args.Error.Cookies.Remove("Authentication");
                        args.Error.Cookies.Remove(".AspNetAuth");
                    }

                    ReplaceKey(args.Error.Form, "Password");
                    ReplaceKey(args.Error.Form, "PasswordConfirm");
                    ReplaceKey(args.Error.Form, "PasswordConfirm");
                    ReplaceKey(args.Error.Form, "SmtpUsername");
                    ReplaceKey(args.Error.Form, "SmtpPassword");
                    ReplaceKey(args.Error.Form, "ImapUsername");
                    ReplaceKey(args.Error.Form, "ImapPassword");
                };
            });

            builder.Services.AddSingleton<WeatherForecastService>();
            
            // add this to fix style.css 404 on production environment
            builder.WebHost.UseStaticWebAssets();
            var app = builder.Build();

            app.UseExceptional();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            app.UseRouting();

            

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapBlazorHub();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("/_Host");
            });

            //app.MapBlazorHub();
            //app.MapFallbackToPage("/_Host");

            
            app.Run();
        }

        private static void ReplaceKey(NameValueCollection collection, string key)
        {
            if (collection == null)
                return;

            var item = collection[key];
            if (item != null)
                collection[item] = "***";
        }
    }
}