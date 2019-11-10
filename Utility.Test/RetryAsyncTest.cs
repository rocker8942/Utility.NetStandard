using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Utility.Test
{
    public class RetryAsyncTest
    {
        private int _retries;

        private async Task<bool> SucceedAlways(int anything)
        {
            _retries++;

            return await Task.FromResult(true);
        }

        private async Task<bool> SuccessOnSubsequentTry(int anything)
        {
            _retries++;
            if (_retries == 1)
                throw new Exception("first try always fail");

            return await Task.FromResult(true);
        }

        private async Task<bool> FailAlways()
        {
            _retries++;

            throw new InvalidOperationException("first try always fail");
        }

        [Fact]
        public async Task RetrySucceedsOnFirstTryAsync()
        {
            _retries = 0;

            var completedDelegate = await Retry.DoAsync(
                async () => await SucceedAlways(1)
                , TimeSpan.FromSeconds(1));

            Debug.WriteLine(_retries.ToString());
            _retries.Should().Be(1);
        }

        [Fact]
        public async Task RetrySucceedsOnSubsequentTryAsync()
        {
            _retries = 0;
            var completedDelegate = await Retry.DoAsync(
                async () => await SuccessOnSubsequentTry(1)
                , TimeSpan.FromSeconds(1));

            Debug.WriteLine(_retries.ToString());
            _retries.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task RetryFail()
        {
            _retries = 0;

            Func<Task<bool>> act = async () => await Retry.DoAsync(
                async () => await FailAlways()
                , TimeSpan.FromSeconds(1));

            await Assert.ThrowsAsync<AggregateException>(act);
            Debug.WriteLine(_retries.ToString());
            _retries.Should().Be(3);
            
        }
    }
}
