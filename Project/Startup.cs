using Project.Data.Models.System;
using Project.Oauth;
using Project.ThridOauth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Project
{
    public class Startup
    {

        public Startup()
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromSeconds(SystemDBManager.Instance.JWT.ClockSkew),
                        ValidAudience = SystemDBManager.Instance.JWT.Audience,
                        ValidIssuer = SystemDBManager.Instance.JWT.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SystemDBManager.Instance.JWT.IssuerSigningKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var payload = JsonConvert.SerializeObject(new { message = "认证失败", code = 403 });
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.WriteAsync(payload);
                            return Task.FromResult(1);

                        },
                        OnForbidden = context =>
                         {
                             var payload = JsonConvert.SerializeObject(new { message = "未经授权", code = 405 });
                             context.Response.ContentType = "application/json";
                             context.Response.StatusCode = StatusCodes.Status200OK;
                             context.Response.WriteAsync(payload);
                             return Task.FromResult(1);
                         }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(OAuthPolicys.Users, policy => policy.RequireRole(OAuthRoles.User).Build());
                options.AddPolicy(OAuthPolicys.Systems, policy => policy.RequireRole(OAuthRoles.System).Build());
                options.AddPolicy(OAuthPolicys.SystemOrUsers, policy => policy.RequireRole(OAuthRoles.User, OAuthRoles.System).Build());
                options.AddPolicy(OAuthPolicys.All, policy => policy.RequireRole(OAuthRoles.User, OAuthRoles.System).Build());
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddGitHubLogin(p =>
            {
                p.ClientId = SystemDBStore.GetSystemValue("githubClientID");
                p.ClientSecret = SystemDBStore.GetSystemValue("githubClientSecret");
            });
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });

            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });

            var options = new RewriteOptions();
            options.Add(rewriteContext =>
            {
                Match match;
                if (rewriteContext.HttpContext.Request.Path == "/")
                {
                    var queryValue = rewriteContext.HttpContext.Request.QueryString.Value;
                    match = Regex.Match(queryValue, @"^\?act=(.*)/(.*)/(.*)/(.*)$");
                 
                    if (match.Success)
                    {
                        var groups = match.Groups;
                        rewriteContext.HttpContext.Request.Path = @"/api/send";
                        rewriteContext.HttpContext.Request.QueryString = new QueryString($"?token={groups[2]}&title={groups[3]}&data={groups[4]}");
                    }
                    else
                    {
                        match = Regex.Match(queryValue, @"^\?act=(.*)/(.*)/(.*)$");
                        if (match.Success)
                        {
                            var groups = match.Groups;
                            rewriteContext.HttpContext.Request.Path = @"/api/send";
                            rewriteContext.HttpContext.Request.QueryString = new QueryString($"?token={groups[2]}&title={groups[3]}");
                        }
                        else
                        {
                            match = Regex.Match(queryValue, @"^\?act=(.*)/(.*)$");
                            if (match.Success)
                            {
                                var groups = match.Groups;
                                rewriteContext.HttpContext.Request.Path = @"/api/send";
                                rewriteContext.HttpContext.Request.QueryString = new QueryString($"?token={groups[2]}");
                            }
                            else if (rewriteContext.HttpContext.Request.QueryString.Value.StartsWith("?"))
                            {
                                var groups = match.Groups;
                                rewriteContext.HttpContext.Request.Path = @"/info";
                                rewriteContext.HttpContext.Request.QueryString = new QueryString();
                            }
                        }
                    }

                }
                rewriteContext.Result = RuleResult.ContinueRules;
            });
            options.AddRewrite(@"^api/(.*).send/(.*)/(.*)", "api/send?token=$1&title=$2&data=$3", true);
            options.AddRewrite(@"^api/(.*).send/(.*)", "api/send?token=$1&title=$2", true);

            options.AddRewrite(@"^(.*).send/(.*)/(.*)", "api/send?token=$1&title=$2&data=$3", true);
            options.AddRewrite(@"^(.*).send/(.*)", "api/send?token=$1&title=$2", true);

            app.UseRewriter(options);
            app.UseRouting();

            app.UseStaticFiles();
            app.UseFileServer();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
