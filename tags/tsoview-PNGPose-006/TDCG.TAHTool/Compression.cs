using System;
using System.Collections.Generic;
using System.IO;

namespace TDCG.TAHTool
{
    class Compression
    {
        // Window sizing related stuff compressor
        public static uint HS_LZSS_MINMATCHLEN = 3;
        public static uint HS_LZSS_WINBITS = 12;
        public static uint HS_LZSS_WINLEN = 4091; //otherwise there will be data loops
        public static uint HS_LZSS_MATCHBITS = 4;
        public static uint HS_LZSS_MATCHLEN = 18;
        public static uint HS_LZSS_HASHTABLE_SIZE = 4096;
        public static uint HS_LZSS_HASHTABLE_EMPTY_ENTRY = 0xFFFFFFFF;
        public static byte HS_LZSS_OUTPUT_BUFFER = 0;
        public static UInt32 HS_LZSS_OUTPUT_BUFFER_POS = 0;
        public static byte HS_LZSS_OUTPUT_BUFFER_FLAG = 0;
        public static UInt32 HS_LZSS_OUTPUT_BUFFER_FLAG_POS = 0;

        public struct LZSS_Hash
        {
            public UInt32[] nPos; //4 entries max (no linked lists)
        }
        public static LZSS_Hash[] m_HashTable;

        // modified:
        // (c)2002 Jonathan Bennett (jon@hiddensoft.com)

        /*LZSS compressor*/
        public static void encrypt(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, ref UInt32 output_length)
        {

            // Set up initial values
            UInt32 m_nDataStreamPos = 0;				// We are at the start of the input data
            UInt32 m_nCompressedStreamPos = 0;			// We are at the start of the compressed data
            HS_LZSS_OUTPUT_BUFFER = 0;
            HS_LZSS_OUTPUT_BUFFER_POS = 0;
            data_output = new byte[1024 * 1024]; //1MB buffer

            // If the input file is too small then there is a chance of 
            // buffer overrun, so just abort
            if (input_length < 32)
            {
                data_output = data_input;
                return;
            }

            // Initialize our hash table
            HashTableInit();

            //
            // Jump to our main compression function
            //

            deflate_loop(ref data_input, input_length, ref data_output, ref output_length, ref m_nDataStreamPos, ref m_nCompressedStreamPos);

            TAHCryption.crypt(ref data_output, output_length, input_length);
        }

        public static void deflate_loop(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, ref UInt32 output_length, ref UInt32 nInputPos, ref UInt32 nOutputPos)
        {
            UInt32 nOffset1, nLen1;					// n Offset values
            UInt32 nOffset2, nLen2;					// n+1 Offset values (lazy evaluation)
            UInt32 nDataPos1;						// N data pos temp values
            UInt32 nTempDataPos;
            UInt32 nHash;
            UInt32 nTempUINT;
            UInt32 nWPos;

            nOffset1 = 0;
            nOffset2 = 0;
            nLen1 = 0;
            nLen2 = 0;

            // Loop around until there is no more data
            int flag_loop = 0;
            while (nInputPos < input_length)
            {
                // Store where we are before we go crazy with lazy evaluation stuff
                nTempDataPos = nInputPos;

                // Match 
                FindMatches(ref nOffset1, ref nLen1, ref nInputPos, ref data_input); // Search for matches for current position
                nDataPos1 = nInputPos;			// Store the data pos if we use this match

                // Match + 1
                nInputPos = nTempDataPos + 1;	// Reset the stream pointer for a match at +1

                // Check that we are not at the end of the data for doing a match at +1
                if (nInputPos < input_length)
                    FindMatches(ref nOffset2, ref nLen2, ref nInputPos, ref data_input); // Search for matches for current position +1
                else
                {
                    // We will overrun the end of the data buffer by doing a second match
                    // fool the routine into thinking that 2nd match was blank
                    nOffset2 = 0;
                    nLen2 = 0;
                }

                // Are there ANY good matches?
                if ((nOffset1 != 0) || (nOffset2 != 0))
                {
                    // Which was the best match? N or N+1?
                    if (nLen1 >= nLen2)
                    {
                        // Let's use match 1
                        nInputPos = nDataPos1;	// Restore the data pos for this match
                        //WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos);
                        WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos, flag_loop % 8);
                        WriteBitsToDataOutput(inverse_byte_order(nOffset1 - HS_LZSS_MINMATCHLEN, nInputPos, nLen1), HS_LZSS_WINBITS, ref data_output, ref nOutputPos, -1);
                        WriteBitsToDataOutput(nLen1 - HS_LZSS_MINMATCHLEN, HS_LZSS_MATCHBITS, ref data_output, ref nOutputPos, -1);
                        flag_loop++;
                    }
                    else
                    {
                        // Remember m_nDataStreamPos WILL ALREADY BE VALID for match +1
                        // Let's use match 2 (a little more work required)
                        // First, store the byte at N as a literal
                        //WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos);
                        WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos, flag_loop % 8);
                        WriteBitsToDataOutput(data_input[nTempDataPos], 8, ref data_output, ref nOutputPos, -1);
                        flag_loop++;

                        // Then store match 2
                        //m_nDataStreamPos = nDataPos2;	// Restore the data pos for this match
                        //WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos);
                        WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos, flag_loop % 8);
                        WriteBitsToDataOutput(inverse_byte_order(nOffset2 - HS_LZSS_MINMATCHLEN, nInputPos, nLen2), HS_LZSS_WINBITS, ref data_output, ref nOutputPos, -1);
                        WriteBitsToDataOutput(nLen2 - HS_LZSS_MINMATCHLEN, HS_LZSS_MATCHBITS, ref data_output, ref nOutputPos, -1);
                        flag_loop++;
                    }
                }
                else
                {
                    // No matches, just store the literal byte
                    nInputPos = nTempDataPos;
                    //WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos);
                    WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos, flag_loop % 8);
                    WriteBitsToDataOutput(data_input[nInputPos++], 8, ref data_output, ref nOutputPos, -1);
                    flag_loop++;
                }

                // We have skipped forwards either 1 byte or xxx bytes (if matched) we must now
                // add entries in the hash table for all the entries we've skipped

                if (nTempDataPos < HS_LZSS_WINLEN)
                    nWPos = 0;
                else
                    nWPos = nTempDataPos - HS_LZSS_WINLEN;

                nTempUINT = nInputPos - nTempDataPos;	// How many bytes to hash?
                while ((nTempUINT > 0) && (nTempDataPos + 2 < data_input.Length))
                {
                    nTempUINT--;
                    nHash = (UInt32)((40543 * ((((data_input[nTempDataPos] << 4) ^ data_input[nTempDataPos + 1]) << 4) ^ data_input[nTempDataPos + 2])) >> 4) & 0xFFF;
                    HashTableAdd(nHash, nTempDataPos, nWPos);
                    nTempDataPos++;

                }  // End while

            } // End while
            //there might be data left in the output bit bufer HS_LZSS_OUTPUT_BUFFER... write it out into the data_output buffer
            if (data_output.Length <= nOutputPos + 1)
            {
                //running out of preallocated memory in the data_output array
                //add 1MB to buffer
                byte[] tmp_data = new byte[data_output.Length + 1024 * 1024];
                data_output.CopyTo(tmp_data, 0);
                data_output = tmp_data;
            }
            //flushing remaining data in the buffers
            if (HS_LZSS_OUTPUT_BUFFER_POS != 0)
            {
                while (HS_LZSS_OUTPUT_BUFFER_POS < 8)
                {
                    // Make room for another bit (shift left once)
                    HS_LZSS_OUTPUT_BUFFER = (byte)(HS_LZSS_OUTPUT_BUFFER << 1);
                    // Update how many bits we are using (add 1)
                    HS_LZSS_OUTPUT_BUFFER_POS++;
                }
                data_output[nOutputPos++] = (byte)(HS_LZSS_OUTPUT_BUFFER);
            }
            if (flag_loop % 8 != 0)
            {
                while ((flag_loop % 8) != 0)
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG >> (byte)0x01);
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(0x80);
                    flag_loop++;
                }
                data_output[HS_LZSS_OUTPUT_BUFFER_FLAG_POS] = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG);
            }
            // We've now written out everything... reset the buffer
            HS_LZSS_OUTPUT_BUFFER_POS = 0;
            HS_LZSS_OUTPUT_BUFFER_FLAG_POS = 0;
            // empty HS_LZSS_OUTPUT_BUFFER
            HS_LZSS_OUTPUT_BUFFER = 0;
            HS_LZSS_OUTPUT_BUFFER_FLAG = 0;
            output_length = nOutputPos;
        }

        public static UInt32 inverse_byte_order(UInt32 nValue, UInt32 nInputPos, UInt32 nLen)
        {
            UInt32 ret_value = 0;
            //the decrypter does not move backwards in the
            //search window when retrieving copy data,
            //but moves forward from the offset at 4080.
            //So any backward slide number given in nValue
            //must be transformed into an absolute file
            //offset.
            nValue = nInputPos - (nValue + HS_LZSS_MINMATCHLEN + nLen);
            //prepare nValue for decrypter win offset...
            nValue += 4080;
            nValue &= 4095;
            //assume 12bit data length
            ret_value |= (UInt32)(nValue & 0xFF);
            ret_value <<= 4;
            ret_value |= (UInt32)((nValue >> 8) & 0xF);
            return ret_value;
        }

        public static void WriteBitsToDataOutput(UInt32 nValue, UInt32 nNumBits, ref byte[] bOutputData, ref UInt32 nPosOutput, int flag_loop)
        {
            if (flag_loop != -1)
            {
                if (flag_loop == 0)
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG_POS = nPosOutput;
                    nPosOutput++;
                    HS_LZSS_OUTPUT_BUFFER_FLAG = 0x00;
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(nValue * 0x80);
                }
                else if (flag_loop == 7)
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG >> (byte)0x01);
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(nValue * 0x80);
                    bOutputData[HS_LZSS_OUTPUT_BUFFER_FLAG_POS] = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG);
                }
                else
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG >> (byte)0x01);
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(nValue * 0x80);
                }
            }
            else
            {
                while (nNumBits > 0)
                {
                    nNumBits--;

                    // Make room for another bit (shift left once)
                    HS_LZSS_OUTPUT_BUFFER = (byte)(HS_LZSS_OUTPUT_BUFFER << 1);

                    // Merge (OR) our value into the temporary long
                    HS_LZSS_OUTPUT_BUFFER = (byte)(HS_LZSS_OUTPUT_BUFFER | ((nValue >> (int)nNumBits) & 0x00000001));

                    // Update how many bits we are using (add 1)
                    HS_LZSS_OUTPUT_BUFFER_POS++;

                    // Now check if we have filled our temporary long with bits (32bits)
                    if (HS_LZSS_OUTPUT_BUFFER_POS == 8)
                    {
                        if (bOutputData.Length <= nPosOutput + 1)
                        {
                            //running out of preallocated memory in the data_output array
                            //add 1MB to buffer
                            byte[] tmp_data = new byte[bOutputData.Length + 1024 * 1024];
                            bOutputData.CopyTo(tmp_data, 0);
                            bOutputData = tmp_data;
                        }
                        bOutputData[nPosOutput++] = (byte)(HS_LZSS_OUTPUT_BUFFER);

                        // We've now written out 8 bits
                        HS_LZSS_OUTPUT_BUFFER_POS = 0;
                        HS_LZSS_OUTPUT_BUFFER = 0x00;
                    }

                } // End while
            }

        }

        public static void FindMatches(ref UInt32 nOffset, ref UInt32 nLen, ref UInt32 nInputPos, ref byte[] bInputData)
        {
            UInt32 nTempWPos, nWPos, nDPos;	// Temp Window and Data position markers
            UInt32 nTempLen;					// Temp vars 
            UInt32 nBestOffset, nBestLen;		// Stores the best match so far
            UInt32 nHash;

            // Reset all variables
            nBestOffset = 0;
            nBestLen = HS_LZSS_MINMATCHLEN - 1;

            // Get our window start position, if the window would take us beyond
            // the start of the file, just use 0
            if (nInputPos < HS_LZSS_WINLEN)
                nWPos = 0;
            else
                nWPos = nInputPos - HS_LZSS_WINLEN;

            // Generate a hash of the next three chars
            nHash = (UInt32)((40543 * ((((bInputData[nInputPos] << 4) ^ bInputData[nInputPos + 1]) << 4) ^ bInputData[nInputPos + 2])) >> 4) & 0xFFF;

            // Main loop

            for (int i = 0; i < 4; i++)
            {
                if (m_HashTable[nHash].nPos != null)
                {
                    nTempWPos = m_HashTable[nHash].nPos[i];
                    if ((nTempWPos < nWPos) && (m_HashTable[nHash].nPos[i] != HS_LZSS_HASHTABLE_EMPTY_ENTRY))
                    {
                        //remove it
                        m_HashTable[nHash].nPos[i] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
                    }
                    else if (m_HashTable[nHash].nPos[i] != HS_LZSS_HASHTABLE_EMPTY_ENTRY)
                    {
                        nDPos = nInputPos;
                        nTempLen = 0;
                        while ((bInputData[nTempWPos] == bInputData[nDPos]) && (nTempWPos < nInputPos) &&
                                (nDPos < bInputData.Length - 1) && (nTempLen < HS_LZSS_MATCHLEN))
                        {
                            nTempLen++; nTempWPos++; nDPos++;
                        }
                        // See if this match was better than previous match
                        if (nTempLen > nBestLen)
                        {
                            nBestLen = nTempLen;
                            nBestOffset = nInputPos - m_HashTable[nHash].nPos[i];
                        }
                    }
                }
                else
                {
                    break;
                }
            }


            // Setup our return values of bestoffset and bestlen, bestoffset will be 0
            // if no good matches were match
            nOffset = nBestOffset;
            nLen = nBestLen;

            // Update our data stream pointer if we had a good match
            if (nOffset != 0)
                nInputPos = nInputPos + nLen;

        }

        public static void HashTableInit()
        {
            m_HashTable = new LZSS_Hash[HS_LZSS_HASHTABLE_SIZE];
            m_HashTable.Initialize();
        }

        public static void HashTableAdd(UInt32 nHash, UInt32 nPos, UInt32 nTooOldPos)
        {
            if (m_HashTable[nHash].nPos != null)
            {
                //search for the first empty entry (HS_LZSS_HASHTABLE_EMPTY_ENTRY)
                for (int i = 0; i < 4; i++)
                {
                    if ((m_HashTable[nHash].nPos[i] == HS_LZSS_HASHTABLE_EMPTY_ENTRY) || (m_HashTable[nHash].nPos[i] < nTooOldPos))
                    {
                        m_HashTable[nHash].nPos[i] = nPos;
                        break;
                    }
                    else if (i == 3)
                    {
                        //full, so replace this entry
                        m_HashTable[nHash].nPos[i] = nPos;
                        break;
                    }
                }
            }
            else
            {
                //first entry...
                m_HashTable[nHash].nPos = new UInt32[4];
                m_HashTable[nHash].nPos[0] = nPos;
                m_HashTable[nHash].nPos[1] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
                m_HashTable[nHash].nPos[2] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
                m_HashTable[nHash].nPos[3] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
            }
        }
    }
}
