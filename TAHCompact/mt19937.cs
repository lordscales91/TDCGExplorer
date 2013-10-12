/* 
   A C-program for MT19937, with initialization improved 2002/1/26.
   Coded by Takuji Nishimura and Makoto Matsumoto.

   Before using, initialize the state by using init_genrand(seed)  
   or init_by_array(init_key, key_length).

   Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
   All rights reserved.                          

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


   Any feedback is very welcome.
   http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
   email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
*/

public class mt19937
{
    /* Period parameters */
    public const int N = 624;
    public const int M = 397;
    public const uint MATRIX_A = 0x9908b0dfU; /* constant vector a */
    public const uint UPPER_MASK = 0x80000000U; /* most significant w-r bits */
    public const uint LOWER_MASK = 0x7fffffffU; /* least significant r bits */

    static uint[] mt = new uint[N]; /* the array for the state vector  */
    static int mti = N + 1; /* mti==N+1 means mt[N] is not initialized */

    /* initializes mt[N] with a seed */
    public static void init_genrand(uint s)
    {
        mt[0] = s & 0xffffffffU;
        for (mti = 1; mti < N; mti++)
        {
            mt[mti] = (uint)
            (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
            /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
            /* In the previous versions, MSBs of the seed affect   */
            /* only MSBs of the array mt[].                        */
            /* 2002/01/09 modified by Makoto Matsumoto             */
            mt[mti] &= 0xffffffffU;
            /* for >32 bit machines */
        }
    }

    /* initialize by an array with array-length */
    /* init_key is the array for initializing keys */
    /* key_length is its length */
    /* slight change for C++, 2004/2/26 */
    public static void init_by_array(uint[] init_key, int key_length)
    {
        int i, j, k;
        init_genrand(19650218U);
        i = 1; j = 0;
        k = (N > key_length ? N : key_length);
        for (; k != 0; k--)
        {
            mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U))
              + init_key[j] + (uint)j; /* non linear */
            mt[i] &= 0xffffffffU; /* for WORDSIZE > 32 machines */
            i++; j++;
            if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
            if (j >= key_length) j = 0;
        }
        for (k = N - 1; k != 0; k--)
        {
            mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U))
              - (uint)i; /* non linear */
            mt[i] &= 0xffffffffU; /* for WORDSIZE > 32 machines */
            i++;
            if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
        }

        mt[0] = 0x80000000U; /* MSB is 1; assuring non-zero initial array */
    }

    static uint[] mag01 = new uint[] { 0x0U, MATRIX_A };

    /* generates a random number on [0,0xffffffff]-interval */
    public static uint genrand_int32()
    {
        uint y;
        /* mag01[x] = x * MATRIX_A  for x=0,1 */

        if (mti >= N)
        { /* generate N words at one time */
            int kk;

            if (mti == N + 1)   /* if init_genrand() has not been called, */
                init_genrand(5489U); /* a default initial seed is used */

            for (kk = 0; kk < N - M; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1UL];
            }
            for (; kk < N - 1; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1UL];
            }
            y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
            mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1UL];

            mti = 0;
        }

        y = mt[mti++];

        /* Tempering */
        y ^= (y >> 11);
        y ^= (y << 7) & 0x9d2c5680U;
        y ^= (y << 15) & 0xefc60000U;
        y ^= (y >> 18);

        return y;
    }

    /* generates a random number on [0,0x7fffffff]-interval */
    public static int genrand_int31()
    {
        return (int)(genrand_int32() >> 1);
    }

    /* generates a random number on [0,1]-real-interval */
    public static double genrand_real1()
    {
        return genrand_int32() * (1.0 / 4294967295.0);
        /* divided by 2^32-1 */
    }

    /* generates a random number on [0,1)-real-interval */
    public static double genrand_real2()
    {
        return genrand_int32() * (1.0 / 4294967296.0);
        /* divided by 2^32 */
    }

    /* generates a random number on (0,1)-real-interval */
    public static double genrand_real3()
    {
        return (((double)genrand_int32()) + 0.5) * (1.0 / 4294967296.0);
        /* divided by 2^32 */
    }

    /* generates a random number on [0,1) with 53-bit resolution*/
    public static double genrand_res53()
    {
        uint a = genrand_int32() >> 5, b = genrand_int32() >> 6;
        return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
    }
    /* These real versions are due to Isaku Wada, 2002/01/09 added */

    public static int main()
    {
        int i;
        uint[] init = new uint[] { 0x123, 0x234, 0x345, 0x456 };
        int length = 4;
        init_by_array(init, length);
        System.Diagnostics.Debug.Write("1000 outputs of genrand_int32()\n");
        for (i = 0; i < 1000; i++)
        {
            System.Diagnostics.Debug.Write("{0} ", genrand_int32().ToString().PadLeft(10));
            if (i % 5 == 4) System.Diagnostics.Debug.Write("\n");
        }
        System.Diagnostics.Debug.Write("\n1000 outputs of genrand_real2()\n");
        for (i = 0; i < 1000; i++)
        {
            System.Diagnostics.Debug.Write("{0} ", genrand_real2().ToString("N8").PadLeft(10));
            if (i % 5 == 4) System.Diagnostics.Debug.Write("\n");
        }
        return 0;
    }
}
