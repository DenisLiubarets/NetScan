using System.Net;

namespace IcmpHandlerLibrary
{
    /// <summary>
    /// Provides data for HostFound event
    /// </summary>
    public class HostFoundEventArgs : System.EventArgs
    {
        /// <summary>
        /// IPAddress of found host
        /// </summary>
        public IPAddress Host { get; set; }
    }
}
