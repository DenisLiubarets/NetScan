using System;

namespace NetScan
{
    /// <summary>
    /// Holds values of current scan progress
    /// </summary>
    public class Progress
    {
        #region Public Members

        /// <summary>
        /// Value that progress starts from
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Value that progress ends with
        /// </summary>
        public int Finish { get; set; }

        /// <summary>
        /// Current progress value
        /// </summary>
        public int Current { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns current progress from 0 to 100
        /// </summary>
        /// <returns></returns>
        public int GetCurrentProgressPercentage()
        {
            // Validate input values
            if (Start > Finish) return -1;
            if (Current >= Finish) return 100;
            if (Current <= Start) return 0;

            // Convert ints to doubles, and also
            // change names to lettes for formula simplicity
            double s = Start;
            double p = Current;
            double f = Finish;

            // Formula to get percentage of progress between two numbers
            double percentage = (p - s) * 100 / (f - s);

            // Convert to integer with rounding
            int result = Convert.ToInt32(percentage);

            return result;
        }

        #endregion
    }
}
