using Project.Data.Models.Project;
using Project.Data.Models.System;
using Project.Oauth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Project.OauthControllers
{
    [ApiController]
    [Route("api/settingsys")]
    public class OAuthSettingController : BaseController
    {
        [HttpGet, Route("GetGlobal"), Authorize(OAuthPolicys.Systems)]
        public IActionResult GetGlobal()
        {
            var proxyenable = SystemDBStore.GetSystemValue("proxyenable");
            var githubEnable = SystemDBStore.GetSystemValue("githubEnable");
            return OK(new
            {
                proxy = SystemDBStore.GetSystemValue("proxy"),
                proxyenable = !string.IsNullOrEmpty(proxyenable) && bool.Parse(proxyenable),
                administrators = SystemDBStore.GetSystemValue("administrators"),
                githubClientID = SystemDBStore.GetSystemValue("githubClientID"),
                githubClientSecret = SystemDBStore.GetSystemValue("githubClientSecret"),
                githubEnable = !string.IsNullOrEmpty(githubEnable) && bool.Parse(githubEnable),
                barkKeyId = SystemDBStore.GetSystemValue("barkKeyId"),
                barkTeamId = SystemDBStore.GetSystemValue("barkTeamId"),
                barkPrivateKey = SystemDBStore.GetSystemValue("barkPrivateKey"),
            });
        }

        [HttpPost, Route("SetGlobal"), Authorize(OAuthPolicys.Systems)]
        public IActionResult SetGlobal(
            string? sendthread,
            string? administrators,
            string? proxy,
            string? proxyenable,
            string? githubClientID,
            string? githubClientSecret,
            string? githubEnable)
        {
            SystemDBStore.SetSystemValue("sendthread", sendthread);
            SystemDBStore.SetSystemValue("administrators", administrators);
            SystemDBStore.SetSystemValue("proxy", proxy);
            SystemDBStore.SetSystemValue("proxyenable", proxyenable);
            SystemDBStore.SetSystemValue("githubClientID", githubClientID);
            SystemDBStore.SetSystemValue("githubClientSecret", githubClientSecret);
            SystemDBStore.SetSystemValue("githubEnable", githubEnable);

            return OK();
        }

        [HttpGet, Route("GetJWT"), Authorize(OAuthPolicys.Systems)]
        public IActionResult GetJWT()
        {
            return OK(SystemDBManager.Instance.JWT);
        }

        [HttpPost, Route("SetJWT"), Authorize(OAuthPolicys.Systems)]
        public IActionResult SetJWT(SystemJwtInfo jwt)
        {
            SystemDBManager.Instance.JWT = jwt;
            StartUpManager.Load().Restart();
            return OK();
        }

        [HttpPost, Route("DeleteUser"), Authorize(OAuthPolicys.Systems)]
        public IActionResult DeleteUser(string userName)
        {
            var userInfo = SystemDBManager.Instance.GetUser(userName);
            var userId = userInfo.Id;
            if (userInfo != null)
            {
                SystemDBManager.Instance.DBase.Delete(userInfo);
                return OK();
            }
            return Fail();
        }

        [HttpPost, Route("ActiveUser"), Authorize(OAuthPolicys.Systems)]
        public IActionResult ActiveUser(string userName, bool active)
        {
            var userInfo = SystemDBManager.Instance.GetUser(userName);
            if (userInfo != null)
            {
                userInfo.Active = active;
                SystemDBManager.Instance.DBase.Update(userInfo, e => e.Active);
                return OK();
            }
            return Fail();
        }

        [HttpGet, Route("GetUsers"), Authorize(OAuthPolicys.Systems)]
        public IActionResult GetUsers(string? query, int page, int pageSize)
        {
            if (query == null)
            {
                return OK(SystemDBManager.Instance.DBase.Query<ProjectUserInfo>().ToPage(page, pageSize));
            }
            else
            {
                return OK(SystemDBManager.Instance.DBase.Query<ProjectUserInfo>().Where(e => e.UserName.Contains(query) || e.Email.Contains(query)).ToPage(page, pageSize));
            }
        }

    }
}
