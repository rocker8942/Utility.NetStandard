using System;
using System.Diagnostics;
using FluentAssertions;
using Xunit;

namespace Utility.Test
{
    public class RetryTests
    {
        private int _retries;

        private bool SucceedAlways()
        {
            _retries++;

            return true;
        }

        private bool SucessOnSubsequentTry()
        {
            _retries++;
            if (_retries == 1)
                throw new Exception("first try always fail");
            //long tick = DateTime.Now.Ticks;
            //int seed = int.Parse(tick.ToString().Substring(tick.ToString().Length - 4, 4));
            //var random = new Random(seed);
            //int value = random.Next();
            //int even = value % 2;

            //if (even.ToString().EndsWith("0"))
            return true;
            // throw new Exception("fail scenario in second trial as a random");
        }

        private bool FailAlways()
        {
            _retries++;

            throw new Exception("first try always fail");
        }

        [Fact]
        public void RetrySucceedsOnFirstTry()
        {
            _retries = 0;
            var completedDelegate = Retry.Do(
                () => SucceedAlways()
                , TimeSpan.FromSeconds(1));

            Debug.WriteLine(_retries.ToString());
            _retries.Should().Be(1);
        }

        [Fact]
        public void RetrySucceedsOnSubsequentTry()
        {
            _retries = 0;
            var completedDelegate = Retry.Do(
                () => SucessOnSubsequentTry()
                , TimeSpan.FromSeconds(1));

            Debug.WriteLine(_retries.ToString());
            _retries.Should().BeGreaterThan(1);
        }

        [Fact]
        public void RetryFail()
        {
            _retries = 0;
            try
            {
                var completedDelegate = Retry.Do(
                    FailAlways
                    , TimeSpan.FromSeconds(1)
                    , 3);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            Debug.WriteLine(_retries.ToString());
            _retries.Should().Be(3);
        }
    }
}