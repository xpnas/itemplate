using Project.OauthThrid.Common;

namespace Project.ThridOauth.IService
{
    public interface ILogin
    {
        AuthorizeResult Authorize();

        string AuthorizeCode { get; }
    }
}