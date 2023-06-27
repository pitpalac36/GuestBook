using Microsoft.Extensions.Configuration;

namespace GuestBook.WorkerRole
{
    public class AppSettings
    {

        public static IConfiguration LoadAppSettings()
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json")
                .Build();
            return configRoot;
        }
    }
}
