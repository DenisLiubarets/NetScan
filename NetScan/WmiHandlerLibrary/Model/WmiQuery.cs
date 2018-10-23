using System.Collections.Generic;

namespace WmiHandlerLibrary
{
    /// <summary>
    /// Holds class and corresponding parameters
    /// </summary>
    public class WmiQuery
    {
        #region Public Members

        /// <summary>
        /// WMI class
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// List of WMI properties
        /// </summary>
        public List<string> Properties { get; set; } = new List<string>();

        #endregion
    }
}
