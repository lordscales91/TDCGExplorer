using System;

namespace TDCG.TAHTool
{
    class TAHCryption
    {
        static MT19937ar myMT19937ar = new MT19937ar();

        public static void crypt(ref byte[] data, UInt32 length, UInt32 origin)
        {
            uint[] init_key = new uint[4];
            byte[] rnd = new byte[1024];

            init_key[0] = (origin | 0x80) >> 5;
            init_key[1] = (origin << 9) | 6;
            init_key[2] = (origin << 6) | 4;
            init_key[3] = (origin | 0x48) >> 3;

            myMT19937ar.init_by_array(init_key, 4);

            for (UInt32 i = 0; i < 1024; i++)
            {
                rnd[i] = (byte)(((uint)myMT19937ar.genrand_int32()) >> (int)(i % 7));
            }

            UInt32 seed = (((origin / 1000) % 10) + ((origin / 100) % 10) + ((origin / 10) % 10) + (origin % 10)) & 0x31A;

            for (int i = 0; i < length; ++i)
            {
                seed = (seed + 1) & 0x3ff;
                data[i] ^= rnd[seed];
            }
        }
    }
}
