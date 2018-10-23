using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WmiLight;

namespace WmiHandlerLibrary
{
    public class WmiHandler
    {
        #region Private Members

        /// <summary>
        /// Number of hosts scanned by current <see cref="WmiHandler"/>
        /// </summary>
        private int _hostsScanned;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public WmiHandler()
        {

        }

        #endregion

        #region Events

        /// <summary>
        /// When received <see cref="WmiParameter"/> from given host, fire this event
        /// </summary>
        public event EventHandler<HostDataReceivedEventArgs> HostDataReceived;
        protected virtual void OnHostDataReceived(HostDataReceivedEventArgs e)
        {
            HostDataReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Host scan complete event
        /// </summary>
        public event EventHandler HostScanComplete;
        protected virtual void OnHostScanComplete()
        {
            Interlocked.Increment(ref _hostsScanned);
            HostScanComplete?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Query host with given list of <see cref="WmiQuery"/> and fire <see cref="OnHostDataReceived(HostDataReceivedEventArgs)"/>
        /// for each received <see cref="WmiParameter"/>
        /// </summary>
        /// <param name="host">Host to query</param>
        /// <param name="credential">Administrator credential that is able to run WMI requests</param>
        /// <param name="wmiQueries">List of WMI queries to run</param>
        public void ScanHostAsync(IPAddress host, NetworkCredential credential, List<WmiQuery> wmiQueries)
        {
            // Enable encryption for WMI packets for security
            var options = new WmiConnectionOptions { EnablePackageEncryption = true };

            // Initialize new WMI connection
            using (var connection = new WmiConnection($"\\\\{host}\\root\\cimv2", credential, options))
            {
                // For each query in list of queries
                Parallel.ForEach(wmiQueries, wmiQuery =>
                {
                    try
                    {
                        // This foreach loop runs once, creates and executes WMI query and stores result of it in wmiObject 
                        foreach (var wmiObject in connection.CreateQuery($"SELECT * FROM {wmiQuery.Class}"))
                        {
                            // Gets all available properties in WmiObject for current WMI class
                            var availableProperties = new List<string>(wmiObject.GetPropertyNames());

                            // For each property in given WMI query
                            foreach (var property in wmiQuery.Properties)
                            {
                                // If current class contains requested property then get it's value, if not - send message that it was not found
                                var value = availableProperties.Contains(property) ? wmiObject.GetPropertyValue<string>(property) : $"{property} query not found";

                                // Prepare OnHostDataReceived and fire it
                                ExecuteOnHostDataReceived(host, property, value);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        // Prepare OnHostDataReceived and fire it with NULL parameter and exception message as a value
                        ExecuteOnHostDataReceived(host, "NULL", exception.Message.Split('.')[0]);
                        return;
                    }
                });
            }

            // This host was completely scanned
            OnHostScanComplete();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Prepares event args for OnHostDataReceived and fires the event
        /// </summary>
        /// <param name="host">IPAddress of host</param>
        /// <param name="property">WMI property</param>
        /// <param name="value">WMI value</param>
        private void ExecuteOnHostDataReceived(IPAddress host, string property, string value)
        {
            OnHostDataReceived(new HostDataReceivedEventArgs
            {
                Host = host,
                Parameter = new WmiParameter
                {
                    Property = property,
                    Value = value
                }
            });
        }

        #endregion
    }
}
