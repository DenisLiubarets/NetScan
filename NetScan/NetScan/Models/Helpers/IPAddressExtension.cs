using System;
using System.Net;

namespace NetScan
{
    /// <summary>
    /// Helper to adjust <see cref="IPAddress"/> class with extension methods.
    /// </summary>
    public static class IPAddressExtension
    {
        /// <summary>
        /// Allows to change byte at given index.
        /// </summary>
        /// <param name="b">New byte value.</param>
        /// <param name="index">Index within IP address.</param>
        /// <returns></returns>
        public static IPAddress ChangeByteAtIndex(this IPAddress ip, byte b, byte index)
        {
            byte[] bytes = ip.GetAddressBytes();
            bytes[index] = b;
            return new IPAddress(bytes);
        }

        /// <summary>
        /// Allows to get a byte at given index.
        /// </summary>
        /// <param name="index">Index within IP address.</param>
        /// <returns>Byte at given index.</returns>
        public static byte GetByteAtIndex(this IPAddress ip, byte index)
        {
            byte[] bytes = ip.GetAddressBytes();
            return bytes[index];
        }
    }
}
