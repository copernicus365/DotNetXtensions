using System;
using System.Collections.Generic;

namespace DotNetXtensions
{
	/// <summary>
	/// Serializes and deserializes a datetime + guid + long number into and back
	/// from bytes of precisely 32 bytes long, which has many useful applications.
	/// </summary>
	public class GuidTimeStamp : IEqualityComparer<GuidTimeStamp>, IEquatable<GuidTimeStamp>
	{
		public const int Size = 32;

		// FIELDS

		public DateTime Date { get; set; }

		public Guid Guid { get; set; }

		public long Number { get; set; }



		// CONSTRUCTORS

		public static byte[] SerializedStampToDataBytes(string stamp)
		{
			if (stamp.NotInRange(Size, Size + 12))
				return null;
			byte[] data = stamp.FromUrlSafeBase64ToBytes();
			return data;
		}


		public GuidTimeStamp(string stamp)
			: this(SerializedStampToDataBytes(stamp))
		{ }

		public GuidTimeStamp(byte[] data)
		{
			if (data.CountN() != Size)
				throw new ArgumentOutOfRangeException();
			
			Date = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
			Guid = new Guid(data.CopyBytes(8, 16));
			Number = BitConverter.ToInt64(data, 24);
		}

		public GuidTimeStamp(DateTime date, Guid guid, long number = 0)
		{
			Date = date;
			Guid = guid;
			Number = number;
		}



		public byte[] SerializeRaw()
		{
			byte[] arr1 = BitConverter.GetBytes(Date.Ticks);
			byte[] arr2 = Guid.ToByteArray();
			byte[] arr3 = BitConverter.GetBytes(Number);

			byte[] data = arr1.ConcatToArray(arr2, arr3);
			return data;
		}

		public string Serialize()
		{
			byte[] data = SerializeRaw();
			string val = data.ToUrlSafeBase64();
			return val;
		}

		public override string ToString()
		{
			return Serialize();
		}



		public bool Equals(GuidTimeStamp x, GuidTimeStamp y)
		{
			if (x == null)
				return y == null ? true : false;
			if (y == null)
				return false;
			return x.Date == y.Date && x.Guid == y.Guid && x.Number == y.Number;
		}

		public int GetHashCode(GuidTimeStamp obj)
		{
			return (obj.Date.GetHashCode() + 173) * (obj.Guid.GetHashCode() + 173) * (obj.Number.GetHashCode() + 173);
		}

		public bool Equals(GuidTimeStamp other)
		{
			return Equals(this, other);
		}
	}
}
