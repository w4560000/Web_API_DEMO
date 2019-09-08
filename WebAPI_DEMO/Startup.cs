using BX.Repository;
using BX.Repository.Base;
using BX.Service;
using BX.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
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
            services.AddControllers();
            services.AddCors();

            //services.AddCors(options =>
            //{
            //    // CorsPolicy 是自訂的 Policy 名稱
            //    options.AddPolicy("CorsPolicy", policy =>
            //    {
            //        policy.WithOrigins("http://localhost:44319")
            //              .AllowAnyHeader()
            //              .AllowAnyMethod()
            //              .AllowCredentials();
            //    });
            //});

            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<ISQLServerConnectionBase, SQLServerConnectionBase>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMailInfoService, MailInfoService>();
            services.AddTransient<IRedisService, RedisService>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = "admin",
                       ValidateAudience = false,
                       ClockSkew = TimeSpan.Zero,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:SecurityKey"]))
                   };

                   // 改寫 若驗證失敗後的response結果
                   // 目前前端抓到401的話自動跳轉頁面，這段code目前用不到先保留
                   //options.Events = new JwtBearerEvents()
                   //{
                   //    OnChallenge = context =>
                   //    {
                   //        context.HttpContext.Response.StatusCode = 401;
                   //        var payload = new JObject
                   //        {
                   //            ["error"] = context.Error,
                   //            ["error_description"] = context.ErrorDescription,
                   //            ["error_uri"] = context.ErrorUri
                   //        };

                   //        return context.Response.WriteAsync(payload.ToString());
                   //    }
                   //};
               });

            services.AddMvc(options =>
            {
                // All endpoints need authorization using our custom authorization filter
                options.Filters.Add(new AuthorizationFilter());
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.AddRequirements(new Auth_Middle()));
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddSingleton<IAuthorizationHandler, Auth_MiddleHandler>();
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
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