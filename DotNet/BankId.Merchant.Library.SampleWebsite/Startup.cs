using System.Diagnostics;
using BankId.Merchant.Library.AppConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankId.Merchant.Library.SampleWebsite
{
    public class Startup
    {
        private readonly string _contentRoot;
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _contentRoot = env.WebRootPath;
        }
        

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMvcCore()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Insert(0, new DecimalModelBinder());
                });
            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration.GetSection("iDIN.Merchant.Library.Settings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            //Trace.Listeners.Add(new CustomTraceListener(env.WebRootPath));
            Trace.Listeners.Add(new CustomTraceListener(_contentRoot));
        }
    }
}
