using System;
using System.Security.Cryptography;
using System.Text;

namespace SpaceAce
{
    namespace Auxiliary
    {
        public sealed class StringID : IEquatable<StringID>
		{
			private const string Symbols = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
			private const int IDLength = 16;
			private const int ChunkSize = 4;
			private const int ChunkSeparatorsPerID = IDLength / ChunkSize - 1;
			private const char ChunkSeparator = '-';

			private static readonly RandomNumberGenerator s_generator = RandomNumberGenerator.Create();
			private static readonly byte[] s_randomCryptoBytes = new byte[IDLength];
			private static readonly StringBuilder s_cryptoIDBuilder = new(IDLength + ChunkSeparatorsPerID);

			private Random _random;
			private StringBuilder _idBuilder = new(IDLength + ChunkSeparatorsPerID);

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
					int index = _random.Next(0, Symbols.Length);
					char symbol = Symbols[index];

					_idBuilder.Append(symbol);

					if (i != IDLength - 1 && i % ChunkSize == ChunkSize - 1)
					{
						_idBuilder.Append(ChunkSeparator);
					}
				}

				return _idBuilder.ToString();
			}

			public static string NextCryptosafe()
			{
				s_cryptoIDBuilder.Clear();
				s_generator.GetBytes(s_randomCryptoBytes);

				for (int i = 0; i < IDLength; i++)
				{
					byte index = (byte)(s_randomCryptoBytes[i] % Symbols.Length);
					char symbol = Symbols[index];

					s_cryptoIDBuilder.Append(symbol);

					if (i != IDLength - 1 && i % ChunkSize == ChunkSize - 1)
					{
						s_cryptoIDBuilder.Append(ChunkSeparator);
					}
				}

				return s_cryptoIDBuilder.ToString();
			}

			public override bool Equals(object obj) => Equals(obj as StringID);

			public bool Equals(StringID other) => other is not null && other.Seed.Equals(Seed);

			public override int GetHashCode() => _random.GetHashCode();

			public static bool operator ==(StringID x, StringID y)
			{
				if (x == null)
				{
					if (y == null)
					{
						return true;
					}

					return false;
				}

				return x.Equals(y);
			}

			public static bool operator !=(StringID x, StringID y) => !(x == y);
		}
	}
}