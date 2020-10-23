using System;
using System.Threading.Tasks;
using Xunit;
using DotNetXtensions;
using DotNetXtensions.Test;

namespace DNX.Test.DateTimes
{
	public class XDateTimes_RoundTo_Tests : DnxTestBase
	{
		[Fact]
		public async Task RoundToWeekday_NxtPrv()
		{
			DateTime dt = new DateTime(2018, 7, 4); // a Wed

			DayOfWeek roundToWk = DayOfWeek.Monday;

			True(dt.DayOfWeek == DayOfWeek.Wednesday);

			DateTime dtNext = dt.RoundToWeekday(roundToWk, roundUp: true);
			True(dtNext == new DateTime(2018, 7, 9));
			True(dtNext.DayOfWeek == roundToWk);

			DateTime dtPrev = dt.RoundToWeekday(roundToWk, roundUp: false);
			DateTime dtTestImplicitArg = dt.RoundToWeekday(roundToWk /* do NOT enter the arg here! */);


			True(dtPrev == dtTestImplicitArg);
			True(dtPrev == new DateTime(2018, 7, 2));
			True(dtPrev.DayOfWeek == roundToWk);
		}

		[Fact]
		public async Task RoundToWeekday_NoChange()
		{
			DateTime dt = new DateTime(2018, 7, 2, 3, 3, 3);

			True(dt.DayOfWeek == DayOfWeek.Monday);

			DateTime dtPrv = dt.RoundToWeekday(DayOfWeek.Monday, roundUp: false);
			DateTime dtNxt = dt.RoundToWeekday(DayOfWeek.Monday, roundUp: true);

			True(dt == dtPrv && dt == dtNxt);
		}

		[Fact]
		public async Task RoundToDay_NxtPrv()
		{
			DateTime dt = new DateTime(2018, 7, 2, 3, 3, 3);

			string expDTStr = "2018-07-02T03:03:03";
			True(dt.DayOfWeek == DayOfWeek.Monday && dt.ToString("yyyy-MM-ddTHH:mm:ss") == expDTStr);

			DateTime dtPrv = dt.RoundToDay();
			DateTime dtPrv_DefParamIsRoundUpFalse = dt.RoundToDay(roundUp: false);

			True(dtPrv_DefParamIsRoundUpFalse == dtPrv);

			True(dtPrv == dt.Date);

			DateTime dtNxt = dt.RoundToDay(roundUp: true);

			True(dtNxt == dt.Date.AddDays(1));

			// no-change when rounding up

			DateTime dtNoTime = new DateTime(2018, 7, 2, 0, 0, 0);
			DateTime dtNxtNoChange = dtNoTime.RoundToDay(roundUp: true);

			True(dtNoTime == dtNxtNoChange);
		}

		[Fact]
		public async Task RoundToMonth_NxtPrv()
		{
			DateTime dt = new DateTime(2018, 7, 4);
			True(dt.Day == 4);

			DateTime dtPrev = dt.RoundToMonth(roundUp: false);
			DateTime dtNext = dt.RoundToMonth(roundUp: true);

			True(dtPrev.Day == 1 && dtPrev == new DateTime(2018, 7, 1));
			True(dtNext.Day == 1 && dtNext == new DateTime(2018, 8, 1));
		}

		[Fact]
		public async Task RoundToYearlyQuarter_NxtPrv()
		{
			DateTime dt = new DateTime(2018, 2, 15);
			True(dt.Day == 15);

			DateTime dtPrev = dt.RoundToQuarterYear(roundUp: false);
			DateTime dtNext = dt.RoundToQuarterYear(roundUp: true);

			True(dtPrev.Day == 1 && dtPrev == new DateTime(2018, 1, 1));
			True(dtNext.Day == 1 && dtNext == new DateTime(2018, 4, 1));
		}

		[Fact]
		public async Task RoundToYear_NxtPrv()
		{
			DateTime dt = new DateTime(2018, 7, 4);

			DateTime dtPrev = dt.RoundToYear(roundUp: false);
			DateTime dtNext = dt.RoundToYear(roundUp: true);

			True(dtPrev.Day == 1 && dtPrev.Month == 1 && dtPrev == new DateTime(2018, 1, 1));
			True(dtNext.Day == 1 && dtNext.Month == 1 && dtNext == new DateTime(2019, 1, 1));
		}

		[Fact]
		public async Task RoundToMonth_NoChange()
		{
			DateTime dt = new DateTime(2018, 7, 1);
			True(dt.Day == 1);

			DateTime dtPrev = dt.RoundToMonth(roundUp: false);
			DateTime dtNext = dt.RoundToMonth(roundUp: true);

			True(dtPrev == dtNext // so after this only have to test 1 of the 2
				&& dtPrev.Day == 1
				&& dtPrev == new DateTime(2018, 7, 1));
		}

	}
}
