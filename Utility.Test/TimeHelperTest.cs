using System;
using FluentAssertions;
using Xunit;

namespace Utility.Test
{
    public class TimeHelperTests
    {
        private readonly TimeSpan _offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

        [Fact]
        public void ConvertLocalToUTCTest()
        {
            // convert local to utc
            var dateTimeLocal = DateTime.Now;

            var isDaylightSavingTime = dateTimeLocal.IsDaylightSavingTime();
            if (isDaylightSavingTime)
                dateTimeLocal.AddHours(_offset.Hours * -1).Should().Be(dateTimeLocal.ToUniversalTime());
            else
                dateTimeLocal.AddHours(_offset.Hours * -1).Should().Be(dateTimeLocal.ToUniversalTime());
        }

        [Fact]
        public void ConvertUndefinedToLocalTest()
        {
            // convert undefined to local. ToLocalTime assume the unspecified time as UTC
            var today = DateTime.Today.ToString();
            var dateTimeUnspecified = DateTime.Parse(today);
            var local = dateTimeUnspecified.ToLocalTime();
            if (_offset.Hours != 0)
                dateTimeUnspecified.AddHours(_offset.TotalHours).Should().Be(local);
        }

        [Fact]
        public void ConvertAESTToUTCTest()
        {
            // convert AEST to UTC
            // When AEST is not DST
            var dtLocal = DateTime.Parse(new DateTime(2014, 4, 30, 0, 0, 0).ToString());
            var dtAESTtoUTC = TimeHelper.ConvertToTimezone(dtLocal, TimeHelper.AEST, TimeHelper.UTC);
            dtLocal.IsDaylightSavingTime().Should().BeFalse();
            dtLocal.Should().Be(dtAESTtoUTC.AddHours(10));

            // use ToUniversalTime
            // When AEST is not DST and GMT is not DST, the difference between AEST and GMT is 10 hours
            dtAESTtoUTC = dtLocal.ToUniversalTime();
            dtLocal.Should().Be(dtAESTtoUTC.AddHours(_offset.Hours));

            // convert to GMT (Daylight saving time)
            // When AEST is not DST and GMT is DST, the difference between AEST and GMT is 9 hours
            var dtAESTtoGMT = TimeHelper.ConvertToTimezone(dtLocal, TimeZoneInfo.Local, TimeHelper.GMT);
            dtAESTtoGMT.IsDaylightSavingTime().Should().BeFalse();
            TimeHelper.GMT.IsDaylightSavingTime(dtAESTtoGMT).Should().BeTrue();
            dtLocal.Should().Be(dtAESTtoGMT.AddHours(_offset.Hours - 1));
        }

        [Fact]
        public void ConvertAEDTToUTCTest()
        {
            var dtLocal = DateTime.Parse(new DateTime(2014, 4, 30, 0, 0, 0).ToString());

            // convert AEDT to UTC
            // When AEST is DST, the difference between AEST and UTC is 11 hours
            // todo: need to find a way to create a AEDT localtime
            dtLocal = DateTime.Parse(new DateTime(2014, 3, 30, 0, 0, 0).ToString());
            var dtAEDTtoUTC = TimeHelper.ConvertToTimezone(dtLocal, TimeHelper.AEST, TimeHelper.UTC);
            // Assert.IsTrue(dtLocal.IsDaylightSavingTime());
            dtLocal.Should().Be(dtAEDTtoUTC.AddHours(11));

            // convert to GMT (non-Daylight saving time)
            // When AEST is DST and GMT is not DST, the difference between two is 11 hours
            var dtAEDSTtoGMT = TimeHelper.ConvertToTimezone(dtLocal, TimeHelper.AEST, TimeHelper.GMT);
            dtLocal.Should().Be(dtAEDSTtoGMT.AddHours(11));
        }

        [Fact]
        public void TestGetOnlyTimeFromString()
        {
            var actual = TimeHelper.GetOnlyTimeFromString("01:00");
            var expect = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 1, 0, 0);
            Assert.Equal(expect, actual);
        }
    }
}