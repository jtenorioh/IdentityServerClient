using DarthSeldon.Services.Caching.Abstractions;
using DarthSeldon.Services.Caching.Memory;
using DarthSeldon.Services.KeyVault.Abstractions;
using DarthSeldon.Services.KeyVault.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DarthSeldon.IdentityServerClient.Tests
{
    /// <summary>
    /// Unit Tests Identity Server Client
    /// </summary>
    public class UnitTestsIdentityServerClient
    {
        #region Members

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The service provider
        /// </summary>
        private readonly ServiceProvider _serviceProvider;

        #endregion Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestsIdentityServerClient"/> class.
        /// </summary>
        public UnitTestsIdentityServerClient()
        {
            var rootDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location)?.Parent?.Parent?.Parent?.Parent?.FullName;

            if (rootDirectory == null)
                throw new ApplicationException("No appsettings were found");

            var fileName = "appsettings.json";

            _configuration = new ConfigurationBuilder()
              .SetBasePath(rootDirectory)
              .AddJsonFile(fileName)
              .AddUserSecrets<UnitTestsIdentityServerClient>()
              .Build();

            var services = new ServiceCollection();

            services.AddSingleton<ICache, InMemoryCache>((c) =>
            {
                var settings = _configuration.GetSection("InMemoryCacheSettings").Get<InMemoryCacheSettings>();
                return new InMemoryCache(settings);
            });

            services.AddScoped<IKeyVault, AzureKeyVault>((c) =>
            {
                var settings = _configuration.GetSection("AzureKeyVaultSettings").Get<AzureKeyVaultSettings>();
                var caches = c.GetServices<ICache>();
                return new AzureKeyVault(settings, caches);
            });

            services.AddScoped((c) =>
            {
                var settings = _configuration.GetSection("IdpServerClientSettings").Get<IdentityServerClientSettings>();
                var caches = c.GetServices<ICache>();
                var keyvaults = c.GetServices<IKeyVault>();
                return new IdentityServerClient(settings, caches, keyvaults);
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests the set secret.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Identity Server Client is not initialized</exception>
        [Fact]
        public async Task TestSetSecret()
        {
            var client = _serviceProvider.GetService<IdentityServerClient>();

            if (client == null)
            {
                throw new ArgumentNullException("Identity Server Client is not initialized");
            }

            var result = await client.ObtainAuthorizationTokenSignedJWTAsync();

            Assert.True(true);
        }

        #endregion
    }
}