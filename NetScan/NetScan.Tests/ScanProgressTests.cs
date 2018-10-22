using System;
using Xunit;

namespace NetScan.Tests
{
    public class ScanProgressTests
    {
        [Theory]
        [InlineData(0,125,250,50)]
        [InlineData(0,50,100,50)]
        [InlineData(30,60,90,50)]
        [InlineData(30,30,90,0)]
        [InlineData(30,90,90,100)]
        [InlineData(30,29,90,0)]
        [InlineData(30,91,90,100)]
        [InlineData(90,90,90,100)]
        [InlineData(90,0,30,-1)]
        [InlineData(90,199,30,-1)]
        [InlineData(90,90,30,-1)]
        [InlineData(90,30,30,-1)]
        [InlineData(49,50,51,50)]
        [InlineData(11,12,15,25)]
        [InlineData(11,14,15,75)]
        public void GetCurrentProgressPercentage_ReturnsCorrectValue(int start, int progress, int finish, int expected)
        {
            var scanProgress = new Progress { Start = start, Current = progress, Finish = finish };
            int actual = scanProgress.GetCurrentProgressPercentage();

            Assert.Equal(expected, actual);
        }
    }
}
