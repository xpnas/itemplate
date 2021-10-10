namespace Project.ThridOauth.IService
{
    public interface IGitHubLogin : ILogin
    {
        public string GetOauthUrl();
    }
}