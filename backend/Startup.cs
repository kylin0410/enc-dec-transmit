using DecBackEnd.Filters;
using DecBackEnd.InputFormatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace DecBackEnd
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
            // 新增byte array格式輸入的處理(可處理加密資料上傳，解密後解析資料的關鍵).
            services.AddMvc(o => o.InputFormatters.Insert(0, new DecFormatter()));
            // 例外處理的注入
            services.AddScoped<ExcpFilter>();
            // 日誌處理的注入
            services.AddScoped<LoggingFilter>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    // name: 攸關 SwaggerDocument 的 URL 位置。
                    name: "v1",
                    // info: 是用於 SwaggerDocument 版本資訊的顯示(內容非必填)。
                    info: new Info
                    {
                        Title = "RESTful API",
                        Version = "1.0.0",
                        Description = "This is ASP.NET Core RESTful API Sample.",
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Name = "DecBackEnd"
                        },
                        License = new License
                        {
                            Name = ""
                        }
                    }
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
                    url: "/swagger/v1/swagger.json",
                    // description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                    name: "RESTful API v1.0.0"
                );
            });

            app.UseMvc();
        }
    }
}
