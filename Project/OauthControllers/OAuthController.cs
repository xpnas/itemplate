﻿using Project.Data.Models.Project;
using Project.Data.Models.System;
using Project.Oauth;
using Project.OauthThrid.Common;
using Project.ThridOauth.IService;
using Project.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Project.OauthControllers
{
    [ApiController]
    [Route("api/oauth")]
    public class OAuthController : BaseController
    {
        private readonly IGitHubLogin m_gitHubLogin;

        private readonly IConfiguration m_configuration;

        public OAuthController(IGitHubLogin gitHubLogin, IConfiguration configuration)
        {
            m_gitHubLogin = gitHubLogin;
            m_configuration = configuration;
        }

        [HttpPost, Route("Login")]
        public JsonResult Login(string username, string password)
        {
            var md5 = password.ToMd5();

            var userInfo = SystemDBManager.Instance.DBase.Query<ProjectUserInfo>().FirstOrDefault(e => e.UserName == username);
            if (userInfo != null)
            {
                if (!userInfo.Active)
                {
                    return Fail(401, "用户被禁用");
                }

                if (userInfo.Password == password.ToMd5())
                {
                    var token = GenToken(username);
                    var role = GetRole(username);
                    return OK(new { name = username, role, token });
                }
                else
                {
                    return Fail(401, "用户密码错误");
                }
            }
            return Fail(401, "用户名不存在");
        }

        [HttpGet, Route("githubenable")]
        public JsonResult GithubEnable()
        {
            return OK(SystemDBStore.GetSystemValue("githubEnable") == "true");
        }

        [HttpGet, Route("githublogin")]
        public JsonResult GitHubLogin(string? code)
        {
            if (SystemDBStore.GetSystemValue("githubEnable") != "true")
            {
                return Fail(401, "未启用GITHUB登陆");
            }

            if (UserName != null && Token != null)
            {
                var direct = string.Format("/#login?token={0}", Token);
                HttpContext.Response.Redirect(direct, true);
                return OK();
            }
            else
            {
                if (string.IsNullOrEmpty(code))
                {
                    return OK(m_gitHubLogin.GetOauthUrl());
                }
                else
                {
                    var res = m_gitHubLogin.Authorize();
                    if (res != null && res.Result != null && res.Code == Code.Success)
                    {
                        string? githubUserName = null;
                        string? avtar = null;
                        string email = "";
                        if (res.Result.TryGetValue("login", out JToken? jToken))
                        {
                            githubUserName = jToken.ToString();
                        }

                        if (res.Result.TryGetValue("avatar_url", out jToken))
                        {
                            avtar = jToken.ToString();
                        }

                        if (res.Result.TryGetValue("email", out jToken))
                        {
                            email = jToken.ToString();
                        }

                        if (githubUserName != null && avtar != null)
                        {
                            ProjectUserInfo user;
                            if (SystemDBManager.Instance.IsUser(githubUserName))
                            {
                                user = SystemDBManager.Instance.DBase.Query<ProjectUserInfo>().FirstOrDefault(e => e.UserName == githubUserName);
                                user.Avatar = avtar;
                                SystemDBManager.Instance.DBase.Update(user);
                                if (!user.Active)
                                {
                                    return Fail(401, "用户被禁用");
                                }
                            }
                            else
                            {
                                user = new ProjectUserInfo()
                                {
                                    UserName = githubUserName,
                                    Avatar = avtar,
                                    Password = "123456".ToMd5(),
                                    Email = email,
                                    Active = true,
                                    Token = Guid.NewGuid().ToString("N").ToUpper(),
                                    CreateTime = DateTime.Now
                                };
                                SystemDBManager.Instance.DBase.Insert(user);
                            }

                            var token = GenToken(user.UserName);
                            var direct = string.Format("/#login?token={0}", token);
                            HttpContext.Response.Redirect(direct, true);
                            return OK(user);
                        }
                    }
                }
            }
            return Fail(401, "Github登陆失败");
        }

        [HttpPost, Route("resetpassword")]
        public JsonResult ResetPassword(string password)
        {
            if (UserName != null)
            {
                var userInfo = SystemDBManager.Instance.DBase.Query<ProjectUserInfo>().FirstOrDefault(e => e.UserName == UserName);

                if (userInfo != null)
                {
                    var md5 = password.ToMd5();
                    userInfo.Password = md5;
                    SystemDBManager.Instance.DBase.Update(userInfo);
                    return OK();
                }
            }
            return Fail(401);
        }

        [HttpGet, Route("info"), Authorize(OAuthPolicys.All)]
        public JsonResult Info(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException($"“{nameof(token)}”不能为 Null 或空白", nameof(token));
            }

            if (UserName != null)
            {
                var user = SystemDBManager.Instance.DBase.Query<ProjectUserInfo>().FirstOrDefault(e => e.UserName == UserName);
                if (user != null)
                {
                    var role = GetRole(user.UserName);
                    return OK(new { name = user.UserName, role, avatar = user.Avatar });
                }
            }

            return Fail(401, "登陆失败");
        }

        [HttpPost, Route("logout"), Authorize(OAuthPolicys.All)]
        public JsonResult Logout()
        {
            if (UserName != null)
            {
                HttpContext.SignOutAsync();
                return OK("登出成功");
            }
            return Fail(401, "您还未登录");
        }

        private string GenToken(string userName)
        {
            string role = GetRole(userName);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddMinutes(Convert.ToInt32(m_configuration.GetSection("JWT")["Expires"]))).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SystemDBManager.Instance.JWT.IssuerSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                issuer: SystemDBManager.Instance.JWT.Issuer,
                audience: SystemDBManager.Instance.JWT.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(SystemDBManager.Instance.JWT.Expiration)),
                signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }

        private string GetRole(string userName)
        {
            string role = "user";
            var admins = SystemDBStore.GetSystemValue("administrators");
            if (admins != null)
            {
                var adminNames = admins.Split(",");
                if (adminNames.Contains(userName))
                {
                    role = "system";
                }
            }

            return role;
        }
    }
}
