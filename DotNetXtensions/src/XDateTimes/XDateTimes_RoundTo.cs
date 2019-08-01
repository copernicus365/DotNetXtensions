// Ideas herein inspired by Jon Skeet / SO's answer to this related question here:
// https://stackoverflow.com/questions/6346119/datetime-get-next-tuesday

// By the way: this marks the VERY FIRST instance in which we split up one of our 
// big source files into partial class units! Far better, allows for more cleaner
// and targeted git commits, etc. This also was inspired by Jon Skeet! Namely, when
// I took a look at his NodaTime source code and their unit tests, which are split like
// this. Note: This is just a start of that journey...

using System;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static partial class XDateTimes
	{
		/// <summary>
		/// Rounds this DateTime to the day, so resultant TimeOfDay will equal Zero. 
		/// When rounding down this simply returns dt.Date. For <paramref name="roundUp"/>, 
		/// directly returns input dt if it already has a zero TimeOfDay, else adds a day 
		/// and returns the date. For more documentation, see similar principles
		/// documented on <see cref="RoundToWeekday(DateTime, DayOfWeek, bool)"/>.
		/// </summary>
		public static DateTime RoundToDay(this DateTime dt, bool roundUp = false)
		{
			if(!roundUp)
				return dt.Date;

			return dt.TimeOfDay == TimeSpan.Zero
				? dt
				: dt.AddDays(1).Date;
		}

		/// <summary>
		/// Rounds this DateTime to the specified day of the week. If it is already a match, returns the 
		/// input value, in which case the <see cref="DateTime.TimeOfDay"/> value is not cleared
		/// (simply call <see cref="DateTime.Date"/> to clear). 
		/// If <paramref name="roundUp"/> is 
		/// false (default), and if a Jul 4 date on a Wed was input and the specified day was a Monday,
		/// then it would be rounded down to Jul 2. If true, up to Jul 9.
		/// <para />
		/// Ideas herein inspired by Jon Skeet / SO's answer to this related question here:
		/// https://stackoverflow.com/questions/6346119/datetime-get-next-tuesday
		/// </summary>
		/// <param name="dt">The DateTime.</param>
		/// <param name="day">Day to round to.</param>
		/// <param name="roundUp">True to round up if not already a match, else rounds down.</param>
		public static DateTime RoundToWeekday(this DateTime dt, DayOfWeek day, bool roundUp = false)
		{
			int seven = roundUp ? 7 : -7; // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
			int daysToAdd = ((int)day - (int)dt.DayOfWeek + seven) % 7;

			if(daysToAdd == 0)
				return dt;

			return dt.AddDays(daysToAdd);
		}

		/// <summary>
		/// Rounds this DateTime to the 1rst day of the current or following Month, depending on 
		/// <paramref name="roundUp"/>. For more documentation, see similar principles
		/// documented on <see cref="RoundToWeekday(DateTime, DayOfWeek, bool)"/>.
		/// </summary>
		public static DateTime RoundToMonth(this DateTime dt, bool roundUp = false)
		{
			if(dt.Day == 1)
				return dt;

			if(roundUp) {
				var v = dt.AddMonths(1); // could change year, so can't just increment month
				return new DateTime(v.Year, v.Month, 1, 0, 0, 0, dt.Kind);
			}
			else
				return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind);
		}

		/// <summary>
		/// Rounds this DateTime to Jan 1 of the current or upcoming Year, depending on 
		/// <paramref name="roundUp"/>. For more documentation, see similar principles
		/// documented on <see cref="RoundToWeekday(DateTime, DayOfWeek, bool)"/>.
		/// </summary>
		public static DateTime RoundToYear(this DateTime dt, bool roundUp = false)
		{
			if(dt.Month == 1 && dt.Day == 1)
				return dt;

			int year = roundUp ? dt.Year + 1 : dt.Year;
			return new DateTime(year, 1, 1, 0, 0, 0, dt.Kind);
		}

		/// <summary>
		/// Rounds this DateTime to the current or upcoming quarter Year value, i.e. one of:
		/// Jan 1, Apr 1, Jul 1, or Oct 1. For more documentation, see similar principles documented on 
		/// <see cref="RoundToWeekday(DateTime, DayOfWeek, bool)"/>.
		/// </summary>
		public static DateTime RoundToQuarterYear(this DateTime dt, bool roundUp = false)
		{
			int month = dt.Month - 1; // get a zero-based month, then allows division by 3 / 
			int remainder = month % 3;

			if(remainder == 0) {
				if(dt.Day == 1)
					return dt;

				return new DateTime(dt.Year, dt.Year, 1, 0, 0, 0, dt.Kind);
			}

			int monthsToAdd = roundUp
				? 3 - remainder
				: -remainder;
			// note: remainder will ALWAYS be: `monthsToAdd.InRange(-2,2) == true` (not even 0 at this point)

			var v = dt.AddMonths(monthsToAdd);
			return new DateTime(v.Year, v.Month, 1, 0, 0, 0, dt.Kind);
		}

	}
}