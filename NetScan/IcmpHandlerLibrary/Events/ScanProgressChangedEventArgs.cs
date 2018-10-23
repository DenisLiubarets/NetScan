namespace IcmpHandlerLibrary
{
    /// <summary>
    /// Provides data for ScanProgressChanged event
    /// </summary>
    public class ScanProgressChangedEventArgs
    {
        #region Public Members

        /// <summary>
        /// Current progress value
        /// </summary>
        public int ProgressValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScanProgressChangedEventArgs() { }

        /// <summary>
        /// Constructor that sets current progress
        /// </summary>
        /// <param name="progress">Current progress value</param>
        public ScanProgressChangedEventArgs(int progress)
        {
            ProgressValue = progress;
        }

        #endregion
    }
}
