using BX.Repository;
using BX.Repository.Base;
using BX.Service;
using BX.Web.Model;
using BX.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using static BX.Web.Security.Auth_Middle;

namespace WebAPI_DEMO
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddControllers();

            services.AddCors(options =>
            {
                // CorsPolicy 是自訂的 Policy 名稱
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:8787")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<ISQLServerConnectionBase, SQLServerConnectionBase>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMailInfoService, MailInfoService>();
            services.AddScoped<IRedisService, RedisService>();

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //   .AddJwtBearer(options =>
            //   {
            //       options.TokenValidationParameters = new TokenValidationParameters
            //       {
            //           ValidateIssuer = true,
            //           ValidateAudience = false,
            //           ValidateLifetime = true,
            //           ValidateIssuerSigningKey = true,
            //           ValidIssuer = "admin",
            //            //ValidAudience = "lilibuy.com",
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:SecurityKey"]))
            //       };
            //   });
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("User",
            //        policy => policy.RequireClaim("CompletedBasicTraining")  //身分
            //        .AddRequirements(new Auth_Middle(1))   //驗證額外參數
            //        );
            //});
            services.AddSingleton<IAuthorizationHandler,Auth_MiddleHandler>();


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
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });


            // 起始路徑 測試
            app.Run(ctx =>
            {
                ctx.Response.Redirect("/Account/Test"); 
                return Task.FromResult(0);
            });
        }
    }
}
