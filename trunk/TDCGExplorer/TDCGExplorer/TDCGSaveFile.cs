using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO;

namespace TDCGExplorer
{
    public static class TDCGSaveFileStatic
    {
        public static readonly DataPair[] PARTS;
        public static readonly DataPair[] SLIDER;

        public const int PARTS_SIZE = 32;
        private const int PARTS_FILE_NAME_SIZE = 32;
        public const int SLIDER_SIZE = 7;

        static TDCGSaveFileStatic()
        {
            PARTS = new DataPair[PARTS_SIZE];
            PARTS[0].Set("身体", 0 * PARTS_FILE_NAME_SIZE);
            PARTS[1].Set("頭髪前", 1 * PARTS_FILE_NAME_SIZE);
            PARTS[2].Set("頭髪後", 2 * PARTS_FILE_NAME_SIZE);
            PARTS[3].Set("頭髪生え際", 3 * PARTS_FILE_NAME_SIZE);
            PARTS[4].Set("瞳", 4 * PARTS_FILE_NAME_SIZE);
            PARTS[5].Set("ブラ", 5 * PARTS_FILE_NAME_SIZE);
            PARTS[6].Set("全身下着", 6 * PARTS_FILE_NAME_SIZE);
            PARTS[7].Set("パンツ", 7 * PARTS_FILE_NAME_SIZE);
            PARTS[8].Set("靴下", 8 * PARTS_FILE_NAME_SIZE);
            PARTS[9].Set("上着", 9 * PARTS_FILE_NAME_SIZE);
            PARTS[10].Set("全身上着", 10 * PARTS_FILE_NAME_SIZE);
            PARTS[11].Set("上着オプション", 11 * PARTS_FILE_NAME_SIZE);
            PARTS[12].Set("スカート", 12 * PARTS_FILE_NAME_SIZE);
            PARTS[13].Set("尻尾", 13 * PARTS_FILE_NAME_SIZE);
            PARTS[14].Set("靴", 14 * PARTS_FILE_NAME_SIZE);
            PARTS[15].Set("頭部装備", 15 * PARTS_FILE_NAME_SIZE);
            PARTS[16].Set("眼鏡", 16 * PARTS_FILE_NAME_SIZE);
            PARTS[17].Set("首輪", 17 * PARTS_FILE_NAME_SIZE);
            PARTS[18].Set("腕装備", 18 * PARTS_FILE_NAME_SIZE);
            PARTS[19].Set("背中", 19 * PARTS_FILE_NAME_SIZE);
            PARTS[20].Set("頭髪オプション", 20 * PARTS_FILE_NAME_SIZE);
            PARTS[21].Set("眼帯", 21 * PARTS_FILE_NAME_SIZE);
            PARTS[22].Set("タイツ・ガーター", 22 * PARTS_FILE_NAME_SIZE);
            PARTS[23].Set("手首", 23 * PARTS_FILE_NAME_SIZE);
            PARTS[24].Set("リボン", 24 * PARTS_FILE_NAME_SIZE);
            PARTS[25].Set("手持ち", 25 * PARTS_FILE_NAME_SIZE);
            PARTS[26].Set("眉", 26 * PARTS_FILE_NAME_SIZE);
            PARTS[27].Set("八重歯", 27 * PARTS_FILE_NAME_SIZE);
            PARTS[28].Set("ほくろ", 28 * PARTS_FILE_NAME_SIZE);
            PARTS[29].Set("イヤリング", 29 * PARTS_FILE_NAME_SIZE);
            PARTS[30].Set("背景", 30 * PARTS_FILE_NAME_SIZE);
            PARTS[31].Set("Ｈシーン", 31 * PARTS_FILE_NAME_SIZE);

            SLIDER = new DataPair[SLIDER_SIZE];
            SLIDER[0].Set("乳", PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 0);
            SLIDER[1].Set("姉＆妹", PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 4);
            SLIDER[2].Set("二の腕", PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 5);
            SLIDER[3].Set("太もも", PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 6);
            SLIDER[4].Set("ウエスト", PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 7);
            SLIDER[5].Set("ツリ眼たれ眼", PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 8);
            SLIDER[6].Set("やわらか", PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 11);
        }
    }

    public class TDCGSaveFileInfo : IDisposable
    {
        private byte[] m_saveData;
        private Bitmap m_bitmap;

        public byte[] SaveData
        {
            get { return m_saveData; }
        }
        public Bitmap Bitmap
        {
            get { return m_bitmap; }
        }
        public bool IsValid
        {
            get { return IsValidFile(); }
        }

        public TDCGSaveFileInfo(string saveFileName)
        {
            m_bitmap = new Bitmap(saveFileName);
            if (IsValidFile())
            {
                m_saveData = ExtractSaveData(m_bitmap);
            }
        }

        public TDCGSaveFileInfo(Stream stream)
        {
            m_bitmap = new Bitmap(stream);
            if (IsValidFile())
            {
                m_saveData = ExtractSaveData(m_bitmap);
            }
        }

        public void Dispose()
        {
            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
                m_bitmap = null;
            }
        }
#if false
        public override string ToString()
        {
            unsafe
            {
                string str = "";
                // パーツ等ファイル名
                for (int i = 0; i < PARTS.Length; ++i)
                {
                    str += PARTS[i].Name;
                    str += " ";
                    str += GetString(PARTS[i].Offset);
                    str += "\r\n";
                }

                // スライダー等float
                for (int i = 0; i < SLIDER.Length; ++i)
                {
                    str += SLIDER[i].Name;
                    str += " ";
                    str += GetFloat(SLIDER[i].Offset);
                    str += "\r\n";
                }


                // あと時間やフラグ？

                str += "？ ";
                str += GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 1);
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 2));
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 3));
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 9));
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 10));
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 12));
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 13));
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 14));
                str += "\r\n";

                str += "？ ";
                str += String.Format("0x{0:X8} ", GetUInt32(PARTS_FILE_NAME_SIZE * PARTS_SIZE + 4 * 15));
                str += "\r\n";

                // 全バイナリ値
                for (int j = 0; j < m_saveData.Length; ++j)
                {
                    str += String.Format("{0:X2} ", m_saveData[j]);

                    if (15 == j % 16)
                    {
                        str += "\r\n";
                    }
                }

                return str;
            }
        }
#endif

        public string GetPartsName(int index)
        {
            return TDCGSaveFileStatic.PARTS[index].Name;
        }
        public string GetPartsFileName(int index)
        {
            return GetString(TDCGSaveFileStatic.PARTS[index].Offset);
        }
        public string GetSliderName(int index)
        {
            return TDCGSaveFileStatic.SLIDER[index].Name;
        }
        public string GetSliderValue(int index)
        {
            return GetFloat(TDCGSaveFileStatic.SLIDER[index].Offset).ToString();
        }

        public string GetFileName(int index)
        {
            unsafe
            {
                fixed (byte* pb = m_saveData)
                {
                    return new String((sbyte*)pb + TDCGSaveFileStatic.PARTS[index].Offset);
                }
            }
        }
        private string GetString(int offset)
        {
            unsafe
            {
                fixed (byte* pb = m_saveData)
                {
                    return new String((sbyte*)pb + offset);
                }
            }
        }
        private float GetFloat(int offset)
        {
            float f;
            unsafe
            {
                Marshal.Copy(m_saveData, offset, (IntPtr)(void*)&f, sizeof(float));
            }
            return f;
        }
        private uint GetUInt32(int offset)
        {
            uint u32;
            unsafe
            {
                Marshal.Copy(m_saveData, offset, (IntPtr)(void*)&u32, sizeof(uint));
            }
            return u32;
        }
        private byte[] ExtractSaveData(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int w = bitmap.Width * 3;
            int h = bitmap.Height;
            byte[] saveData = new byte[w * h / 8];
            unsafe
            {
                byte* pByte = (byte*)bmpData.Scan0;
                int dataOffset = 0;
                // BMPはファイル上では下段から並べられるので下段から処理
                for (int y = h - 1; 0 <= y; --y)
                {
                    for (int x = 0; x < w; x += 8)
                    {
                        int i = y * w + x;
                        byte data = (byte)(pByte[i + 0] & 0x1);
                        data |= (byte)((pByte[i + 1] & 0x1) << 1);
                        data |= (byte)((pByte[i + 2] & 0x1) << 2);
                        data |= (byte)((pByte[i + 3] & 0x1) << 3);
                        data |= (byte)((pByte[i + 4] & 0x1) << 4);
                        data |= (byte)((pByte[i + 5] & 0x1) << 5);
                        data |= (byte)((pByte[i + 6] & 0x1) << 6);
                        data |= (byte)((pByte[i + 7] & 0x1) << 7);
                        saveData[dataOffset] = data;
                        ++dataOffset;
                    }
                }
            }

            bitmap.UnlockBits(bmpData);

            return saveData;
        }

        private bool IsValidFile()
        {
            if (null == m_bitmap)
            {
                return false;
            }
            if ((128 != m_bitmap.Width) || (256 != m_bitmap.Height) || (PixelFormat.Format24bppRgb != m_bitmap.PixelFormat))
            {
                return false;
            }
            return true;
        }

    }

    public struct DataPair
    {
        public string Name;
        public int Offset;

        public DataPair(string name, int offset)
        {
            Name = name;
            Offset = offset;
        }
        public void Set(string name, int offset)
        {
            Name = name;
            Offset = offset;
        }
    }
}
