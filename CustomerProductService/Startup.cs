using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using Polly;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using IdentityModel.Client;
using ProductRepository;
using ProductOrderFacade;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HttpManager;

namespace CustomerProductService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }

        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer()
                .AddJwtBearer("ProductAuth", options =>
                {
                    options.Authority = Configuration.GetValue<string>("StaffAuthServerUrl");
                    options.Audience = "customer_product_api";
                })
                .AddJwtBearer("CustomerAuth", options =>
                {
                    options.Authority = Configuration.GetValue<string>("CustomerAuthServerUrl");
                    options.Audience = "customer_product_api";
                });

            services.AddAuthorization(OptionsBuilderConfigurationExtensions =>
            {
                OptionsBuilderConfigurationExtensions.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("ProductAuth")
                .Build();
                OptionsBuilderConfigurationExtensions.AddPolicy("CustomerOnly", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("CustomerAuth")
                    .Build());
                OptionsBuilderConfigurationExtensions.AddPolicy("StaffProductAPIOnly", new AuthorizationPolicyBuilder()
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "client_id" && c.Value == "staff_product_api"))
                    .AddAuthenticationSchemes("ProductAuth")
                    .Build());
            });


            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddDbContext<ProductDb>(options =>
            {
                var cs = Configuration.GetConnectionString("ProductConnection");
                options.UseSqlServer(cs);
            });

            services.AddScoped<IProductRepository, ProductRepository.ProductRepository>();
            services.AddScoped<IHttpHandler, HttpHandler>();
            services.AddScoped<IUnmockablesWrapper, UnmockablesWrapper>();

            services.AddSingleton(new ClientCredentialsTokenRequest
            {
                Address = "",
                ClientId = Configuration.GetValue<string>("ClientId"),
                ClientSecret = Configuration.GetValue<string>("ClientSecret"),
                Scope = ""
            });

            if (Env.IsDevelopment())
            {
                services.AddScoped<IProductOrderFacade, ProductOrderFacade.ProductOrderFacade>();
            }
            else
            {
                services.AddScoped<IProductOrderFacade, ProductOrderFacade.ProductOrderFacade>();
            }

            services.AddHttpClient("CustomerOrderingAPI", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("CustomerOrderingUrl").Value);
            })
                    .AddTransientHttpErrorPolicy(p => p.OrResult(
                        msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(60)));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

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
