using Microsoft.Extensions.Configuration;

namespace GuestBook.WebRole
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
