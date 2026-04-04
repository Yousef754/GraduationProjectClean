using ECommerce.API.CustomMiddlewares;
using ECommerce.API.Extensions;
using ECommerce.API.Factories;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Persistence.Data.DataSeed;
using ECommerce.Persistence.Data.DbContexts;
using ECommerce.Persistence.IdentityData.DataSeed;
using ECommerce.Persistence.IdentityData.DbContexts;
using ECommerce.Persistence.Repositories;
using ECommerce.Services;
using ECommerce.Services.Abstraction;
using ECommerce.Services.MappingProfiles;
using ECommerce.Services.Specifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Text.Json.Serialization;

namespace ECommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Register DI Container
            // Add services to the container.

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
            options.JsonSerializerOptions.Converters.Add(
             new JsonStringEnumConverter()
                );
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce.API", Version = "v1" });

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new string[] { }
                        },
                    }
                );
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "DevelopmentPolicy",
                    builder =>
                    {
                        builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                    }
                );
            });

            //builder.Services.AddDbContext<StoreDbContext>(options =>
            //{
            //    options.UseSqlServer(
            //        builder.Configuration.GetConnectionString("DefaultConnection")
            //    );
            //});

            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });

            // DbContext الخاص بالـ Identity يشترك في نفس الـ DB
            builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });

            // إعداد الـ Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<StoreIdentityDbContext>()
                .AddDefaultTokenProviders();

            //builder
            //   .Services.AddIdentityCore<ApplicationUser>()
            //   .AddRoles<IdentityRole>()
            //   .AddEntityFrameworkStores<StoreIdentityDbContext>();

            //builder.Services.AddKeyedScoped<IDataIntializer, DataIntializer>("Default");
            //builder.Services.AddKeyedScoped<IDataIntializer, IdentityDataIntializer>("Identity");

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddAutoMapper(typeof(ServiceAssemblyReference).Assembly);

            //builder.Services.AddScoped<IProductService, ProductService>();


            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("Redis");

                var options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false;

                return ConnectionMultiplexer.Connect(options);
            });

            //var redisConnection = builder.Configuration["RedisConnection"];
            //if (!string.IsNullOrWhiteSpace(redisConnection))
            //{
            //    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            //        ConnectionMultiplexer.Connect(
            //            new ConfigurationOptions
            //            {
            //                EndPoints = { redisConnection },
            //                AbortOnConnectFail = false
            //            }
            //        )
            //    );
            //}

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDataIntializer, IdentityDataIntializer>();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory =
                    ApiResponseFactory.GenerateApiValidationResponse;
            });

            
            //builder
            //    .Services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<StoreIdentityDbContext>();

            //builder
            //    .Services.AddIdentityCore<ApplicationUser>()
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<StoreIdentityDbContext>();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddAutoMapper(typeof(OrderProfile));   

            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
                        ValidAudience = builder.Configuration["JWTOptions:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!)
                        ),
                    };
                });

            //builder.Services.AddScoped<IOrderService, OrderService>();
            //builder.Services.AddScoped<IPaymentService, PaymentService>();
            #endregion

            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var identityInitializer = services.GetRequiredService<IDataIntializer>();
                await identityInitializer.IntializeAsync();
            }

            //await app.MigrateDataBaseAsync();
            //await app.MigratIdentityeDataBaseAsync();

            //await app.SeedDataAsync();
            //await app.SeedIdentityDataAsync();

            #region Configure PipeLine [Middlewares]
            #region Custom Middleware
            // Configure the HTTP request pipeline.

            //app.Use(
            //    async (context, next) =>
            //    {
            //        try
            //        {
            //            await next();
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message); //Logging console

            //            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //            await context.Response.WriteAsJsonAsync(
            //                new
            //                {
            //                    StatusCode = StatusCodes.Status500InternalServerError,
            //                    Error = $"An unexpected error Occured:{ex.Message}",
            //                }
            //            );
            //        }
            //    }
            //);
            #endregion


            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseRouting();
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.DocExpansion(DocExpansion.None);
            });

            if (app.Environment.IsDevelopment())
            {

                

            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("DevelopmentPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            await app.RunAsync();
        }
    }
}
