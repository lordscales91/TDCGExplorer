using System;
using System.Collections.Generic;
using System.IO;

namespace TDCG.TAHTool
{
    class Decompression
    {
        public static void decrypt(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, UInt32 output_length)
        {
            TAHCryption.crypt(ref data_input, input_length, output_length);

            infrate(ref data_input, input_length, ref data_output, output_length);
        }

        /*LZSS decompressor*/
        public static void infrate(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, UInt32 output_length)
        {
            UInt32 act_output_length = 0;
            UInt32 act_byte = 0;
            UInt32 nActWindowByte = 0xff0;
            const UInt32 win_size = 4096;
            byte[] win = new byte[4096];
            ushort flag = 0;

            win.Initialize();

            while (act_output_length < output_length)
            {
                flag >>= 1;
                if ((flag & 0x0100) == 0)
                {
                    flag = (ushort)((((ushort)(data_input[act_byte++]))) | 0xff00);
                }

                if ((flag & 1) != 0)
                {
                    byte data;

                    data = (byte)(data_input[act_byte++]);
                    data_output[act_output_length++] = data;
                    win[nActWindowByte++] = data;
                    nActWindowByte &= win_size - 1;
                }
                else
                {
                    UInt32 copy_bytes, win_offset;
                    UInt32 i;

                    win_offset = (UInt32)(data_input[act_byte++]);
                    copy_bytes = (UInt32)(data_input[act_byte++]);
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
