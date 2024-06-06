using DarthSeldon.Settings.Abstractions;

namespace DarthSeldon.Services.Caching.Memory
{
    /// <summary>
    /// In Memory Settings
    /// </summary>
    /// <seealso cref="DarthSeldon.Settings.Abstractions.ISettings" />
    public class InMemoryCacheSettings : ISettings
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
        public string Type { get; set; } = "MemoryCache";

        /// <summary>
        /// Gets or sets a value indicating whether this instance uses expiration of the cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is use expiration; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseExpiration { get; set; }

        /// <summary>
        /// Gets or sets the default expiration in days. Default value 7.
        /// </summary>
        /// <value>
        /// The default expiration in days.
        /// </value>
        public int DefaultExpirationInDays { get; set; } = 7;

        #endregion Properties
    }
}