using Newtonsoft.Json.Linq;



namespace Project.OauthThrid.Common
{
    public class AuthorizeResult
    {
        public Code Code { get; set; }

        public string Error { get; set; }

        public JObject Result { get; set; }

        public string Token { get; set; }
    }

    public enum Code
    {
        Success,
        Exception,
        UserInfoErrorMsg,
        AccessTokenErrorMsg
    }
}