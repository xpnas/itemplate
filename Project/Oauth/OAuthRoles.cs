namespace Project.Oauth
{
    public static class OAuthRoles
    {
        static OAuthRoles()
        {
            User = "user";
            System = "system";

        }
        public static string User { get; set; }

        public static string System { get; set; }

        public static string SystemOrUser { get; set; }

    }
}
