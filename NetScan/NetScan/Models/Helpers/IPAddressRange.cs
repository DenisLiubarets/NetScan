using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace NetScan
{
    /// <summary>
    /// Helper class that handles all the work related to composing and decomposing range. 
    /// 'Range' in terms of this class is a range of IP addresses writen in specific forms.
    /// Now supports only one form (e.g. "192.168.0.1-254")
    /// </summary>
    public static class IPAddressRange
    {
        /// <summary>
        /// Gets starting <see cref="IPAddress"/> from given range
        /// For example, if "192.168.0.1-254" was passed, method will return "192.168.0.1"
        /// </summary>
        /// <param name="range">Range of IP address in specific form (e.g. "192.168.0.1-254")</param>
        /// <returns>First IP address from <param name="range"></returns>
        public static IPAddress GetStartIPAddress(string range)
        {
            // Range is not just one IPAddress
            if (range.Contains("-"))
            {
                string ip = range.Split('-')[0];
                return IPAddress.Parse(ip);
            }

            // Consider range as only one IPAddress
            return IPAddress.Parse(range);
        }

        /// <summary>
        /// Gets ending <see cref="IPAddress"/> from range
        /// For example, if "192.168.0.1-254" was passed, method will return "192.168.0.254"
        /// </summary>
        /// <param name="range">Range of IP address in specific form (e.g. "192.168.0.1-254")</param>
        /// <returns>Last IP address from <param name="range">.</returns>
        public static IPAddress GetEndIPAddress(string range)
        {
            // Range is not just one IPAddress
            if (range.Contains("-"))
            {
                // Get starting IPAddress to work with
                var ip = GetStartIPAddress(range);
                // Get ending byte out of range
                byte b = byte.Parse(range.Split('-')[1]);
                // Change the last byte of starting IPAddress, thus creating ending IPAddress
                return ip.ChangeByteAtIndex(b, 3);
            }

            // Consider range as only one IPAddress.
            return IPAddress.Parse(range);
        }

        /// <summary>
        /// Composes a List of <see cref="IPAddress"/> that belong to a range
        /// </summary>
        /// <param name="range">Range of IP address in specific form (e.g. "192.168.0.1-254")</param>
        /// <returns>List of IPAddress</returns>
        public static List<IPAddress> GetIPAddressList(string range)
        {
            // List of IPAddress to return
            var addrList = new List<IPAddress>();

            // Range is not just one IPAddress.
            if (range.Contains("-"))
            {
                // Convert range string to starting IPAddress, e.g "192.168.0.1"
                var ip = GetStartIPAddress(range);
                // Get last byte, e.g "1"
                var start = ip.GetByteAtIndex(3);
                // Convert range to ending IPAddress, e.g "192.168.0.254"
                ip = GetEndIPAddress(range);
                // Get last byte, e.g "254"
                byte end = ip.GetByteAtIndex(3);

                // Loop from start IP to end IP
                for (byte i = start; i <= end; i++)
                {
                    // Set last byte of ip to current i
                    var addr = ip.ChangeByteAtIndex(i, 3);
                    // And add it to the list
                    addrList.Add(addr);
                }
            }
            // Consider range as only one IPAddress
            else
            {
                var ip = IPAddress.Parse(range);
                addrList.Add(ip);
            }
            
            return addrList;
        }

        /// <summary>
        /// Converts IPAddress to valid range that starts from 1 and ends with 254
        /// 
        /// For example: If '192.168.0.34' was passed, method will return a string '192.168.0.1-254'
        /// </summary>
        /// <param name="addr">IPAddress to convert</param>
        /// <returns>Valid range from 1 to 254</returns>
        public static string ConvertIPAddressToDefaultRange(IPAddress addr)
        {
            // Change last byte of IPAddress to 1
            addr.ChangeByteAtIndex(1, 3);

            // Return range e.g. "192.168.0.1-254"
            return $"{addr.ToString()}-254";
        }

        /// <summary>
        /// Validates that range is written in specific form.
        /// For now there is only one forms - "192.168.0.1-254" or "192.168.0.1"
        /// </summary>
        /// <param name="range">Range string to validate</param>
        /// <returns>True if range is valid</returns>
        public static bool ValidateRange(string range)
        {
            var match = Regex.Match(range, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");
            if (!match.Success) return false;
            match = Regex.Match(range, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)-(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
            if (!match.Success) return false;

            // Also checks if ending IP is larger than starting IP
            byte start = GetStartIPAddress(range).GetByteAtIndex(3);
            byte end = GetEndIPAddress(range).GetByteAtIndex(3);
            return (end > start);
        }
    }
}
