using System;

namespace DarthSeldon.IdentityServerClient
{
    /// <summary>
    /// Authorization token Class
    /// </summary>
    public class AuthorizationToken
    {
        #region Properties

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        /// <value>
        /// AccessToken
        /// </value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date time
        /// </summary>
        /// <value>
        /// ExpiresInDateTime
        /// </value>
        public DateTime ExpiresInDateTime { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        public string TokenType { get; set; }

        #endregion Properties
    }
}