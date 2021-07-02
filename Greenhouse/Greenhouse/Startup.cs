using Greenhouse.Data.Contexts;
using Greenhouse.Data.Models;
using Greenhouse.Models.Options;
using Greenhouse.Services;
using Greenhouse.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Greenhouse
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Greenhouse", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                            }
                        },
                    
                        new string[] { }
                    }
                });
            });


            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            #region Добавляем AspNetCore Identity сервисы
            var builder = services.AddIdentityCore<User>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);            
            identityBuilder.AddEntityFrameworkStores<AppDbContext>();
            identityBuilder.AddSignInManager<SignInManager<User>>();
            identityBuilder.AddUserManager<AspNetUserManager<User>>();
            identityBuilder.AddDefaultTokenProviders();
            #endregion

            services.AddSingleton<ILogger>(LoggerFactory.Create(options =>
            {
                options.AddDebug();
                options.AddConsole();
                options.AddEventLog();                
            }).CreateLogger("Logger"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IJwtHelper, JwtHelper>();
            services.AddScoped<IEmailService, SendGridEmailService>();

            var sendGridConfig = Configuration.GetSection("SendGridOptions");
            services.Configure<SendGridOptions>(sendGridConfig);

            var jwtOptionsConfig = Configuration.GetSection("JwtOptions");
            services.Configure<JwtOptions>(jwtOptionsConfig);
            // Получаем JWT настройки сейчас чтобы сконфигурировать с их помощью аутентификацию.
            var options = jwtOptionsConfig.Get<JwtOptions>(); 

            // Добавляем аутентификацию.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configure =>
            {
                configure.RequireHttpsMetadata = false;
                configure.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = options.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = options.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Greenhouse v1"));
            }

            app.UseCors(options =>
            {
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
            });
            app.UseRouting();      
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
