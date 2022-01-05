using DotNetXtensions.Collections;

namespace DNX.Test;

public class CacheDictionaryTests : DnxTestBase
{
	/// <summary>
	/// As nice as a bunch of small unit-tests would be, testing this is
	/// difficult because it depends on changes of state. To mock those changes of
	/// state takes a lot of setup that has to 
	/// </summary>
	[Fact]
	public void BIGTest_Count_CountPurged_GetItemsEnumerator_ItemGet_AutoRemovesPurged_Etc()
	{
		var cd = getMockCacheDict(out DateTime now);

		// this WILL (Should) hit cd.CopyTo, then GetItems, etc. Test that out 
		// in debugging but no assertions, how would we. This DOES also mean
		// expired items WILL be purged! So ToArray, ToList, ToDictionary, etc
		// all DO purge expired items that haven't been removed yet, we like :)
		var d = cd.ToDictionary(kv => kv.Key, kv => kv.Value);

		True(cd.DictionariesAreEqual(d));

		double addMinutesToExpireApples =
			MockVals1["Apples"] + cd.ExpiresAfter.TotalMinutes;
		// let's mock that "Now" is 1 minute after our start time, and then add the expires time,
		// currently == 1 minute as well. This is exactly at threshold after which first two items
		// will drop off, `{ "Apples", 1 }` etc
		True(addMinutesToExpireApples == 2.0);

		cd.GetDateTimeNow = () => now.AddMinutes(addMinutesToExpireApples - 0.001);
		True(cd.CountPurged() == 5);

		cd.GetDateTimeNow = () => now.AddMinutes(addMinutesToExpireApples);

		int countPurged = cd.CountPurged();

		True(cd.Count == 5 && countPurged == 3);

		string keys = cd.Keys.ToArray().OrderBy(n => n).JoinToString(",");
		string vals = cd.Values.ToArray().OrderBy(n => n).JoinToString(",");

		True(
			keys == "Peaches,Pears,Pineapples" &&
			vals == "3,3,5");

		// Let's demonstrate
		// 1) TryGetValue works, 
		// 2) it DOES remove an expired item when found
		int _val;
		True(
			cd.Count == 5 &&
			!cd.TryGetValue("Apples", out _val) &&
			cd.Count == 4);

		True(cd.TryGetValue("Peaches", out _val) && _val == 3);

		// ------

		double addMinutesToExpirePears =
			MockVals1["Pears"] + cd.ExpiresAfter.TotalMinutes;

		cd.GetDateTimeNow = () => now.AddMinutes(addMinutesToExpirePears);

		cd.PurgeExpiredItems();

		countPurged = cd.CountPurged();
		keys = cd.Keys.ToArray().OrderBy(n => n).JoinToString(",");

		True(cd.Count == 1 && countPurged == 1 && keys == "Pineapples");
	}

	[Fact]
	public void AddAlreadyExistingKeyActsLikeSet_NOException()
	{
		var cd = getMockCacheDict(out DateTime now);

		string keyNm = "Peaches";
		int newVal = 88;

		True(cd.Count == 5 && cd[keyNm] == 3);

		cd.Add(keyNm, newVal);

		True(cd.Count == 5 && cd[keyNm] == newVal);

		cd[keyNm] = newVal;
		cd.Add(keyNm, newVal);

		True(cd.Count == 5 && cd[keyNm] == newVal);

		// --- now DO add a new value, but then add it again

		keyNm = "Mango";
		newVal = 44;

		cd.Add(keyNm, newVal); // make sure to DO do Add first here
		cd[keyNm] = newVal;

		True(cd.Count == 6 && cd[keyNm] == newVal);
	}

	[Fact]
	public void AutoPurgeOnGet()
	{
		var cd = getMockCacheDict(out DateTime now);
		cd.RunPurgeTS = TimeSpan.FromMinutes(1);
		cd.GetDateTimeNow = () => now;

		True(cd.Count == 5);

		True(cd.Count == 5 &&
			cd.TryGetValue("Peaches", out int _val) &&
			_val == 3 &&
			cd.Count == 5);

		now = now.AddMinutes(4);
		DateTime nextPurgeDT = cd.ResetRunNextPurgeDT();

		True(nextPurgeDT == now.AddMinutes(1)); // == cd.RunPurgeTS set above 

		True(
			cd.Count == 5 &&
			!cd.TryGetValue("Peaches", out _val) &&
			cd.TryGetValue("Pineapples", out _val) &&
			cd.Count == 4);
	}

	[Fact]
	public void TriggerFullPurgeOnGet()
	{
		var cd = getMockCacheDict(out DateTime now);
		cd.RunPurgeTS = TimeSpan.FromMinutes(1);

		True(cd.Count == 5);

		DateTime nextPurgeDT = cd.ResetRunNextPurgeDT();
		// MUST run this while Now was plain Now, AFTER mock that its 4 mins later
		True(nextPurgeDT == now.Add(cd.RunPurgeTS)); // == cd.RunPurgeTS set above 

		double minsToExpirePears = MockVals1["Pears"] + cd.RunPurgeTS.TotalMinutes;
		True(minsToExpirePears == 4.0);
		cd.GetDateTimeNow = () => now.AddMinutes(minsToExpirePears);

		int _val;
		True(
			cd.Count == 5 &&
			!cd.TryGetValue("Peaches", out _val) &&
			cd.TryGetValue("Pineapples", out _val) &&
			cd.Count == 1);
	}

	/// <summary>
	/// 2 with 1 min, 2 with 3 min, 1 with 5 min
	/// </summary>
	static Dictionary<string, int> MockVals1 = new Dictionary<string, int>() {
			{ "Apples", 1 },
			{ "Oranges", 1 },
			{ "Peaches", 3 },
			{ "Pears", 3 },
			{ "Pineapples", 5 },
		};


	CacheDictionary<string, int> getMockCacheDict(out DateTime now)
	{
		var nw = now = _getNowRoundedUp();
		DateTime nowP2 = now.AddMinutes(2);

		var cd = new CacheDictionary<string, int>(TimeSpan.FromMinutes(1)) {

			// IMPORTANT NOTE:
			// for mocking, we MUST set this value to greater than highest 
			// mock time we want to test ({ "Pineapples", 5 }), otherwise 
			// purges will be firing when we don't want them to which will
			// fail the test state / make testing impossible
			RunPurgeTS = TimeSpan.FromMinutes(6)
		};

		foreach(var kv in MockVals1) {
			string ky = kv.Key;
			int val = kv.Value;

			DateTime fakeAddedTimeAfterNow = now.AddMinutes(val);
			cd.GetDateTimeNow = () => fakeAddedTimeAfterNow;
			cd.Add(ky, val);
		}

		cd.GetDateTimeNow = () => nw;

		return cd;
	}

	DateTime _getNowRoundedUp(double addMins = 0)
		=> DateTime.Now.RoundUp(TimeSpan.FromMinutes(1)).AddMinutes(addMins);

}
