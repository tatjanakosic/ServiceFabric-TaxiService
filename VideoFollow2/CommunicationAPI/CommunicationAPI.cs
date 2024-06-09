using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Communication.Mapping;
using Communication.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Communication;

namespace CommunicationAPI
{
    internal sealed class CommunicationAPI : StatelessService
    {
        public CommunicationAPI(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
            new ServiceInstanceListener(serviceContext =>
                new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                {
                    ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                    var builder = WebApplication.CreateBuilder();

                    // Configuration from appsettings.json
                    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    var jwtSettings = new JwtSettings();
                    builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
                    builder.Services.AddSingleton(jwtSettings);

                    // Configure CORS
                    builder.Services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(policy =>
                        {
                            policy.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader();
                        });
                    });

                    // Add AutoMapper
                    builder.Services.AddAutoMapper(typeof(MappingProfile));

                    // Add controllers and Swagger
                    builder.Services.AddControllers();
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();

                    // Add JWT Service
                    builder.Services.AddScoped<JWTService>();

                    // Configure Authentication
                    builder.Services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                ServiceEventSource.Current.ServiceMessage(serviceContext, $"Authentication failed: {context.Exception.Message}");
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                ServiceEventSource.Current.ServiceMessage(serviceContext, "Token validated successfully");
                                return Task.CompletedTask;
                            },
                            OnMessageReceived = context =>
                            {
                                ServiceEventSource.Current.ServiceMessage(serviceContext, $"Token received: {context.Token}");
                                return Task.CompletedTask;
                            }
                        };
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                            ValidAudience = builder.Configuration["JwtSettings:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
                            ClockSkew = TimeSpan.Zero // Optional: reduce the default clock skew tolerance
                        };
                    });

                    // Configure Authorization
                    builder.Services.AddAuthorization(options =>
                    {
                        options.FallbackPolicy = new AuthorizationPolicyBuilder()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                            .RequireAuthenticatedUser()
                            .Build();

                        options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                        options.AddPolicy("User", policy => policy.RequireRole("User"));
                        options.AddPolicy("Driver", policy => policy.RequireRole("Driver"));

                        options.AddPolicy("VerifiedCorrect", policy => policy.RequireClaim("VerificationStatus", "Accepted"));
                        options.AddPolicy("BlockedCorrect", policy => policy.RequireClaim("BlockingStatus", "Unblocked"));
                    });

                    // Add SMTP settings and email service
                    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
                    builder.Services.AddSingleton<IEmailService, EmailService>();

                    builder.WebHost
                           .UseKestrel()
                           .UseContentRoot(Directory.GetCurrentDirectory())
                           .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                           .UseUrls(url);

                    var app = builder.Build();

                    if (app.Environment.IsDevelopment())
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();
                    }

                    // Use middleware
                    app.UseCors();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.MapControllers();

                    return app;
                }))
            };
        }
    }

}
