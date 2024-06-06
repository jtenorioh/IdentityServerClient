using DarthSeldon.Settings.Abstractions;

namespace DarthSeldon.Services.KeyVault.Azure
{
    /// <summary>
    /// Azure Key Vault Settings
    /// </summary>
    /// <seealso cref="DarthSeldon.Settings.Abstractions.ISettings" />
    public class AzureKeyVaultSettings : ISettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// Name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// Type.
        /// </value>
        public string Type { get; set; } = "AzureKeyVaultSettings";

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the azure key vault URI.
        /// </summary>
        /// <value>
        /// The azure key vault URI.
        /// </value>
        public string AzureKeyVaultUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is use cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is use cache; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseCache { get; set; } = false;

        /// <summary>
        /// Gets or sets the expiration in days. Default value 7.
        /// </summary>
        /// <value>
        /// The expiration in days.
        /// </value>
        public int DefaultExpirationInDays { get; set; } = 7;

        /// <summary>
        /// Gets or sets the name of the cache settings.
        /// </summary>
        /// <value>
        /// The name of the cache settings.
        /// </value>
        public string CacheSettingsName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is managed service identity.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is managed service identity; otherwise, <c>false</c>.
        /// </value>
        public bool IsManagedServiceIdentity { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is use visual studio identity.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is use visual studio identity; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseVisualStudioIdentity { get; set; } = false;

        #endregion Properties
    }
}