using Project.Data.Models.System;
using Newtonsoft.Json;
using System;

namespace Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SystemDBManager.Instance.Run();

            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii };
            };
            try
            {
                var appManager = StartUpManager.Load();
                do
                {
                    appManager.Start(args);
                } while (appManager.Restarting);
            }
            catch (Exception)
            {

            }
        }
    }
}
