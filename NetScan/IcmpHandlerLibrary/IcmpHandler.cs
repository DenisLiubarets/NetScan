using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace IcmpHandlerLibrary
{
    /// <summary>
    /// PingHandler takes care of asynchronous ICMP requests to network
    /// </summary>
    public class IcmpHandler
    {
        #region Private Members

        /// <summary>
        /// Current scan progress value. This is not percantage based, 
        /// this is just a counter of occured ICMP requests.
        /// </summary>
        private int _progressCounter;

        #endregion

        #region Events

        /// <summary>
        /// Host was found event handler
        /// </summary>
        public event EventHandler<HostFoundEventArgs> HostFound;

        protected virtual void OnHostFound(HostFoundEventArgs e)
        {
            HostFound?.Invoke(this, e);
        }

        /// <summary>
        /// User has canceled scan event handler
        /// </summary>
        public event EventHandler ScanCanceled;

        protected virtual void OnScanCanceled()
        {
            ScanCanceled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Scan was complete event handler
        /// </summary>
        public event EventHandler ScanComplete;

        protected virtual void OnScanComplete()
        {
            ScanComplete?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Scan progress was changed event handler
        /// </summary>
        public event EventHandler<ScanProgressChangedEventArgs> ProgressChanged;

        protected virtual void OnProgressChanged()
        {
            var scanProgressEventArgs = new ScanProgressChangedEventArgs();
            ProgressChanged?.Invoke(this, new ScanProgressChangedEventArgs(_progressCounter));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public IcmpHandler()
        {
            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Scans given list of <see cref="IPAddress"/> and broadcasts events when host is online
        /// </summary>
        /// <param name="addrList">List of hosts to scan</param>
        /// <param name="token"></param>
        public void ScanAsync(List<IPAddress> addrList, CancellationToken token)
        {
            Task.Run(() =>
            {
                // Reset current progress value
                _progressCounter = 0;

                // Increase multithread perfomance
                ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);
                ThreadPool.SetMinThreads(workerThreads + 256, completionPortThreads + 256);

                try
                {
                    var parallelOptions = new ParallelOptions
                    {
                        CancellationToken = token,
                        MaxDegreeOfParallelism = 256
                    };

                    Parallel.ForEach(addrList, parallelOptions, addr => 
                    {
                        // Flag to check against that we have found online host
                        var hostOnline = false;

                        using (var ping = new Ping())
                        {
                            // Try to ping same host multiple times to assure ping results
                            for (int i = 0; i < 5; i++)
                            {
                                try
                                {
                                    // Send ICMP request to host with smalles possible size
                                    var pingReply = ping.Send(addr, 1000);

                                    // If host responded to ICMP request
                                    if (pingReply != null && pingReply.Status == IPStatus.Success)
                                    {
                                        hostOnline = true;
                                        break;
                                    }
                                }
                                catch (PingException ex) { Console.WriteLine(ex.Message); }

                                // Scan was canceled
                                if (token.IsCancellationRequested) break;
                            }

                            // Check if we found host on this iteration
                            if (hostOnline)
                            {
                                OnHostFound(new HostFoundEventArgs { Host = addr });
                            }
                        }

                        // Increment current scanning progress
                        IncrementProgress();
                    });

                    // Scan was complete
                    OnScanComplete();
                }
                catch (OperationCanceledException)
                {
                    // Check if the scan was already complete
                    if (addrList.Count == _progressCounter)
                        OnScanComplete();
                    else
                        OnScanCanceled();
                }
                finally
                {
                    // Reset the ThreadPool to default
                    ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
                    ThreadPool.SetMinThreads(workerThreads - 256, completionPortThreads - 256);
                }
            }, token);
        }

        #endregion

        #region Private Methods

        private void IncrementProgress()
        {
            Interlocked.Increment(ref _progressCounter);
            OnProgressChanged();
        }

        #endregion
    }
}
