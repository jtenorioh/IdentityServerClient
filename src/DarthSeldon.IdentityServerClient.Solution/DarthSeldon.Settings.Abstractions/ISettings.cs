namespace DarthSeldon.Settings.Abstractions
{
    /// <summary>
    /// Settings Interface
    /// </summary>
    public interface ISettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// Name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// Type.
        /// </value>
        string Type { get; set; }

        #endregion Properties
    }
}