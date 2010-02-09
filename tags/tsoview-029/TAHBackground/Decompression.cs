using System;
using System.Collections.Generic;
using System.IO;

namespace TAHTool
{
    class Decompression
    {
        static MT19937ar myMT19937ar = new MT19937ar();

        /*LZSS decompressor*/
        public static void decrypt(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, UInt32 output_length)
        {
            UInt32 act_output_length = 0;
            UInt32 act_byte = 0;
            UInt32 nActWindowByte = 0xff0;
            const UInt32 win_size = 4096;
            byte[] win = new byte[4096];
            ushort flag = 0;
            uint[] init_key = new uint[4];
            byte[] rnd = new byte[1024];

            init_key[0] = (output_length | 0x80) >> 5;
            init_key[1] = (output_length << 9) | 6;
            init_key[2] = (output_length << 6) | 4;
            init_key[3] = (output_length | 0x48) >> 3;
            myMT19937ar.init_by_array(init_key, 4);
            for (UInt32 i = 0; i < 1024; i++)
            {
                rnd[i] = (byte)(((uint)myMT19937ar.genrand_int32()) >> (int)(i % 7));
            }

            UInt32 seed = (((output_length / 1000) % 10) + ((output_length / 100) % 10) + ((output_length / 10) % 10) + (output_length % 10)) & 0x31A;
            win.Initialize();

            while (act_output_length < output_length)
            {
                flag >>= 1;
                if ((flag & 0x0100) == 0)
                {
                    seed = (seed + 1) & 0x3ff;
                    flag = (ushort)((((ushort)(data_input[act_byte++] ^ rnd[seed]))) | 0xff00);
                }

                seed = (seed + 1) & 0x3ff;

                if ((flag & 1) != 0)
                {
                    byte data;

                    data = (byte)(data_input[act_byte++] ^ rnd[seed]);
                    data_output[act_output_length++] = data;
                    win[nActWindowByte++] = data;
                    nActWindowByte &= win_size - 1;
                }
                else
                {
                    UInt32 copy_bytes, win_offset;
                    UInt32 i;

                    win_offset = (UInt32)(data_input[act_byte++] ^ rnd[seed]);
                    seed = (seed + 1) & 0x3ff;
                    copy_bytes = (UInt32)(data_input[act_byte++] ^ rnd[seed]);
                    win_offset |= (copy_bytes & 0xf0) << 4;
                    copy_bytes &= 0x0f;
                    copy_bytes += 3;

                    for (i = 0; i < copy_bytes; i++)
                    {
                        byte data;

                        data = win[(win_offset + i) & (win_size - 1)];
                        data_output[act_output_length++] = data;
                        win[nActWindowByte++] = data;
                        nActWindowByte &= win_size - 1;
                        if (act_output_length >= output_length)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}