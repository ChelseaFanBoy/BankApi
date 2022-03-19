using BankApi.Models;
using BankApi.Services;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.HttpLogging;

namespace BankApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpLogging(httpLogging =>
            {
                httpLogging.LoggingFields = HttpLoggingFields.All;
                httpLogging.MediaTypeOptions.
                AddText("application/json");
            });
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = 
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
            services.Configure<BankDatabaseSettings>(Configuration.GetSection("BankDatabase"));

            services.AddScoped<IBankService, BankService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpLogging();
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            //app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseHttpsRedirection();
        }
    }
}
