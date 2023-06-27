using Microsoft.Extensions.Configuration;

namespace GuestBook.Data
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
