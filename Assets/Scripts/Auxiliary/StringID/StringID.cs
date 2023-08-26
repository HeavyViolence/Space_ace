using System;
using System.Security.Cryptography;
using System.Text;

namespace SpaceAce.Auxiliary
{
	public sealed class StringID : IEquatable<StringID>
	{
		private const string AllowedSymbols = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
		public const string Default = "0000-0000-0000-0000";

		private const char ChunkSeparator = '-';

		private const int IDLength = 16;
		private const int ChunkSize = 4;

		private static readonly RandomNumberGenerator s_generator = RandomNumberGenerator.Create();
		private static readonly byte[] s_randomCryptosafeBytes = new byte[IDLength];
		private static readonly StringBuilder s_cryptosafeIDBuilder = new(Default.Length);

		private readonly Random _random;
		private readonly StringBuilder _idBuilder = new(Default.Length);

		public int Seed { get; }

		public StringID(int seed)
		{
			_random = new(seed);
			Seed = seed;
		}

		public string Next()
		{
			_idBuilder.Clear();

			for (int i = 0; i < IDLength; i++)
			{
				int index = _random.Next(0, AllowedSymbols.Length);
				char symbol = AllowedSymbols[index];

				_idBuilder.Append(symbol);

				if (i % ChunkSize == ChunkSize - 1 && i != IDLength - 1) _idBuilder.Append(ChunkSeparator);
            }

			return _idBuilder.ToString();
		}

		public static string NextCryptosafe()
		{
			s_cryptosafeIDBuilder.Clear();
			s_generator.GetBytes(s_randomCryptosafeBytes);

			for (int i = 0; i < IDLength; i++)
			{
				byte index = (byte)(s_randomCryptosafeBytes[i] % AllowedSymbols.Length);
				char symbol = AllowedSymbols[index];

				s_cryptosafeIDBuilder.Append(symbol);

				if (i % ChunkSize == ChunkSize - 1 && i != IDLength - 1) s_cryptosafeIDBuilder.Append(ChunkSeparator);
            }

			return s_cryptosafeIDBuilder.ToString();
		}

		public static bool IsValid(string candidate)
		{
			if (string.IsNullOrEmpty(candidate) || string.IsNullOrWhiteSpace(candidate)) return false;
			if (candidate.Length != Default.Length) return false;

			for (int i = 0; i < candidate.Length; i++)
			{
				if (candidate[i] == ChunkSeparator && Default[i] != ChunkSeparator) return false;
				if (char.IsLetterOrDigit(candidate[i]) != char.IsLetterOrDigit(Default[i])) return false;
			}

			return true;
		}

		public override bool Equals(object obj) => Equals(obj as StringID);

		public bool Equals(StringID other) => other is not null && other.Seed.Equals(Seed);

		public override int GetHashCode() => Seed.GetHashCode();

		public static bool operator ==(StringID x, StringID y)
		{
			if (x == null)
			{
				if (y == null) return true;

                return false;
			}

			return x.Equals(y);
		}

		public static bool operator !=(StringID x, StringID y) => !(x == y);
	}
}