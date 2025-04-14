using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;

namespace Recrut.API.Configuration
{
    public static class DockerConfiguration
    {
        public static void AddDockerSecurityConfiguration(this IServiceCollection services, IWebHostEnvironment environment, ConfigurationManager configuration)
        {
            // Configuration for persistence of protection keys
            // Use an absolute path for Docker
            string keyDirectory = "/app/keys";

            try
            {
                if (!Directory.Exists(keyDirectory))
                {
                    Directory.CreateDirectory(keyDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating key directory : {ex.Message}");
            }

            // Basic data protection configuration
            var dataProtectionBuilder = services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory))
                .SetApplicationName($"Recrut.API.{environment.EnvironmentName}");

            // Add certificate protection only in production or if specified
            if (environment.IsProduction() || configuration.GetValue<bool>("UseDataProtectionCertificate"))
            {
                string certPath = Path.Combine(environment.ContentRootPath, "certificates", "dataprotection.pfx");
                if (File.Exists(certPath))
                {
                    try
                    {
                        dataProtectionBuilder.ProtectKeysWithCertificate(
                            new X509Certificate2(certPath, "S3ceBexXOsRCcLnIbwMA9pvP5KiMI02tcwZ1Kqvp"));
                        Console.WriteLine("Certificate key protection enabled");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to configure certificate protection : {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Certificate not found at location : {certPath}");
                }
            }
        }
    }
}
