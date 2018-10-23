using System.Net;

namespace WmiHandlerLibrary
{
    /// <summary>
    /// Provides data for HostDetailsReceived event
    /// </summary>
    public class HostDataReceivedEventArgs
    {
        #region Public Members

        /// <summary>
        /// IP address of the host from which data received
        /// </summary>
        public IPAddress Host { get; set; }

        /// <summary>
        /// The actual data that was received from host
        /// </summary>
        public WmiParameter Parameter { get; set; }

        #endregion
    }
}
