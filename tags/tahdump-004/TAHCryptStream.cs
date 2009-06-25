using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TAHdecrypt
{
	public class TAHCryptStream : Stream
	{
		private const int MASK = 1023;
		private byte[] key = new byte[MASK + 1];
		private Stream basestream;
		private int seed;

		public TAHCryptStream(Stream basestream, int length)
		{
			this.basestream = basestream;
			uint[] init_key = new uint[4]
            {
                ((uint)length | 0x80) >> 5,
                ((uint)length << 9) | 0x06,
                ((uint)length << 6) | 0x04,
                ((uint)length | 0x48) >> 3,
            };

			mt19937.init_by_array(init_key, 4);

			for (int i = 0; i < 1024; ++i) {
				key[i] = (byte)((mt19937.genrand_int32()) >> (int)(i % 7));
				//DbgPrint(key[i].ToString("X").PadLeft(2, '0'));
			}

			seed = (((length / 1000) % 10)
						+ ((length / 100) % 10)
						+ ((length / 10) % 10)
						+ ((length) % 10)) & 0x31A;
			++seed;
		}

		public override bool CanRead { get { return basestream.CanRead; } }
		public override bool CanSeek { get { return basestream.CanSeek; } }
		public override bool CanWrite { get { return basestream.CanWrite; } }
		public override long Length { get { return basestream.Length; } }
		public override long Position
		{
			get { return basestream.Position; }
			set { basestream.Position = value; }
		}

		public override void Flush() { basestream.Flush(); }

		public override long Seek(long offset, SeekOrigin origin)
		{
			return basestream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			basestream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (Position >= Length)
				return 0;

			long pos = Position;
			count = basestream.Read(buffer, offset, count);
			Crypt(buffer, offset, count, pos);
			return count;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			long pos = Position;
			Crypt(buffer, offset, count, pos);
			basestream.Write(buffer, offset, count);
			Crypt(buffer, offset, count, pos);
		}

		public void Crypt(byte[] buffer, int offset, int count, long origin)
		{
			int kp = (int)((origin + seed) & MASK);

			for (int i = 0; i < count; ++i) {
				buffer[offset + i] ^= key[kp];
				kp = (kp + 1) & MASK;
			}
		}
	}
}
