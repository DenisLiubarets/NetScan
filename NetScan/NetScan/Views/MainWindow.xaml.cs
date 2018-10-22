using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetScan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members

        /// <summary>
        /// Total number of hosts that responded to ICMP requests
        /// </summary>
        private int _totalOnlineHosts;

        /// <summary>
        /// Total number of hosts scanned with WmiHandler, this includes failed scans
        /// </summary>
        private int _totalScannedHosts;

        #endregion

        // TESTING TESTING TESTING
        List<IPAddress> OnlineHosts { get; set; } = new List<IPAddress>();
        WmiHandler wmiHandler = new WmiHandler();
        IcmpHandler icmpHandler = new IcmpHandler();
        Progress progress = new Progress();
        Stopwatch sw = new Stopwatch();

        NetworkCredential credential;
        List<WmiQuery> wmiQueries;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            sw.Start();

            Preconfigure();

            ConfigureIcmpHandler();
            ConfigureWmiHandler();

        }

        private void Preconfigure()
        {
            credential = new NetworkCredential
            {
                //Domain = "",
                UserName = "",
                Password = ""
            };

            wmiQueries = new List<WmiQuery>()
            {
                new WmiQuery
                {
                    Class = "Win32_BIOS",
                    Properties = { "SerialNumber" }
                },
                new WmiQuery
                {
                    Class = "Win32_OperatingSystem",
                    Properties = { "Caption" }
                },
                new WmiQuery
                {
                    Class = "Win32_ComputerSystem",
                    Properties = { "Name", "Model", "Manufacturer", "UserName", "ChassisSKUNumber" }
                }
            };
        }

        private void ConfigureWmiHandler()
        {
            wmiHandler.HostDataReceived += this.WmiHandler_HostDataReceived;
            wmiHandler.HostScanComplete += this.WmiHandler_HostScanComplete;
        }

        private void ConfigureIcmpHandler()
        {
            string rangeStr = "10.161.240.1-254";
            var range = IPAddressRange.GetIPAddressList(rangeStr);
            CancellationToken cts = new CancellationToken();

            progress.Start = IPAddressRange.GetStartIPAddress(rangeStr).GetByteAtIndex(3);
            progress.Finish = IPAddressRange.GetEndIPAddress(rangeStr).GetByteAtIndex(3);

            icmpHandler.HostFound += this.IcmpHandler_HostFound;
            icmpHandler.ScanComplete += this.IcmpHandler_ScanComplete;
            icmpHandler.ProgressChanged += this.IcmpHandler_ProgressChanged;

            icmpHandler.ScanAsync(range, cts);
        }

        private void IcmpHandler_ProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
        }

        private void IcmpHandler_ScanComplete(object sender, EventArgs e)
        {
        }

        private void IcmpHandler_HostFound(object sender, HostFoundEventArgs e)
        {
            Interlocked.Increment(ref _totalOnlineHosts);
            wmiHandler.ScanHostAsync(e.Host, credential, wmiQueries);
        }

        private void WmiHandler_HostDataReceived(object sender, HostDataReceivedEventArgs e)
        {
            var host = e.Host;
            var data = e.Parameter;

            Console.WriteLine($"{host}: {data.Property}: {data.Value}");
        }

        private void WmiHandler_HostScanComplete(object sender, EventArgs e)
        {
            Interlocked.Increment(ref _totalScannedHosts);

            var percent = _totalScannedHosts * 100 / _totalOnlineHosts;
            this.Dispatcher.Invoke(() =>
            {
                var text = $"Progress: {percent}% | totalOnlineHosts: {_totalOnlineHosts} | totalScannedOnlineHosts: {_totalScannedHosts}\n";
                Display.Text = text;
            });
            
            if (_totalScannedHosts == _totalOnlineHosts)
            {
                sw.Stop();
                MessageBox.Show($"Scan complete!\n\nTime elapsed: {sw.Elapsed}");
            }
        }

    }
}
