using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetScan
{
    public static class IPTools
    {
        /// <summary>
        /// Scans local host network interfaces for IPAddress.
        /// </summary>
        /// <returns>List of active IP addresses of local host.</returns>
        public static List<IPAddress> GetLocalHostAddressList()
        {
            var addrList = new List<IPAddress>();

            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection)
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                // Read the IP configuration for each network
                IPInterfaceProperties properties = network.GetIPProperties();

                // Each network interface may have multiple IP addresses
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    // We're only interested in IPv4 addresses for now
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    // Ignore loopback addresses (e.g., 127.0.0.1)
                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    // Ignore 'unable to talk to DHCP' reserved addresses
                    if (address.Address.ToString().Contains("169.254"))
                        continue;

                    addrList.Add(address.Address);
                }
            }

            return addrList;
        }
    }
}
