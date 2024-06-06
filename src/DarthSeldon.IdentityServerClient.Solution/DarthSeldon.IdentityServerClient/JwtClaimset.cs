namespace DarthSeldon.IdentityServerClient
{
    /// <summary>
    /// JWT Claimset
    /// </summary>
    public class JwtClaimset
    {
        #region Properties

        /// <summary>
        /// Gets or sets the iss.
        /// </summary>
        /// <value>
        /// The iss.
        /// </value>
        public string iss { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sub.
        /// </summary>
        /// <value>
        /// The sub.
        /// </value>
        public string sub { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the aud.
        /// </summary>
        /// <value>
        /// The aud.
        /// </value>
        public string aud { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the jti.
        /// </summary>
        /// <value>
        /// The jti.
        /// </value>
        public string jti { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the exp.
        /// </summary>
        /// <value>
        /// The exp.
        /// </value>
        public long exp { get; set; } = 0;

        /// <summary>
        /// Gets or sets the iat.
        /// </summary>
        /// <value>
        /// The iat.
        /// </value>
        public long iat { get; set; } = 0;

        #endregion Properties
    }
}