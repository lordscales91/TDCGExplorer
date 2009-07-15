using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDCGExplorer
{
    public static class TDCGTbnUtil
    {
        private static TBNCategoryData[] categoryTable;

        static TDCGTbnUtil()
        {
            categoryTable = new TBNCategoryData[30];
            categoryTable[0].set('A', "身体",0x00,0x04,0x03);
            categoryTable[1].set('B', "前髪", 0x01, 0x09, 0x08);
            categoryTable[2].set('C', "後髪", 0x02, 0x0E, 0x0D);
            categoryTable[3].set('D', "頭皮", 0x03, 0x13, 0x12);
            categoryTable[4].set('E', "瞳", 0x04, 0x18, 0x17);
            categoryTable[5].set('F', "ブラ", 0x05, 0x1D, 0x1C);
            categoryTable[6].set('G', "全身下着・水着", 0x06, 0x22, 0x21);
            categoryTable[7].set('H', "パンツ", 0x07, 0x27, 0x26);
            categoryTable[8].set('I', "靴下", 0x08, 0x2C, 0x2B);
            categoryTable[9].set('J', "上衣", 0x09, 0x31, 0x30);
            categoryTable[10].set('K', "全身衣装", 0x0A, 0x36, 0x35);
            categoryTable[11].set('L', "上着オプション", 0x0B, 0x3B, 0x3A);
            categoryTable[12].set('M', "下衣", 0x0C, 0x40, 0x3F);
            categoryTable[13].set('N', "尻尾", 0x0D, 0x45, 0x44);
            categoryTable[14].set('O', "靴", 0x0E, 0x4A, 0x49);
            categoryTable[15].set('P', "頭部装備", 0x0F, 0x4F, 0x4E);
            categoryTable[16].set('Q', "眼鏡", 0x10, 0x54, 0x53);
            categoryTable[17].set('R', "首輪", 0x11, 0x59, 0x58);
            categoryTable[18].set('S', "手首", 0x12, 0x5E, 0x5D);
            categoryTable[19].set('T', "背中", 0x13, 0x63, 0x62);
            categoryTable[20].set('U', "アホ毛類", 0x14, 0x68, 0x67);
            categoryTable[21].set('V', "眼帯", 0x15, 0x6D, 0x6C);
            categoryTable[22].set('W', "タイツ・ガーター", 0x16, 0x72, 0x71);
            categoryTable[23].set('X', "腕装備", 0x17, 0x77, 0x76);
            categoryTable[24].set('Y', "リボン", 0x18, 0x7C, 0x7B);
            categoryTable[25].set('Z', "手持ち", 0x19, 0x00, 0x00);
            categoryTable[26].set('0', "眉毛", 0x1A, 0x86, 0x85);
            categoryTable[27].set('1', "八重歯", 0x1B, 0x8B, 0x8A);
            categoryTable[28].set('2', "ほくろ", 0x1C, 0x90, 0x8F);
            categoryTable[29].set('3', "イヤリング類", 0x1D, 0x95, 0x94);
        }

        public static void SetTsoName(byte[] tbndata, string tsoname)
        {
            int offset=-1;
            switch (tbndata.Length)
            {
                case 6040:
                    offset = 0x177c;
                    break;
                case 8884:
                    offset = 0x2298;
                    break;
                case 8904:
                    offset = 0x22ac;
                    break;
                case 8980:
                    offset = 0x22e8;
                    break;
            }
            int index=0;
            foreach (char code in tsoname)
            {
                tbndata[offset + index] = (byte) code;
                index++;
            }
            tbndata[offset + index] = 0;
        }
        public static string GetTsoName(byte[] tbndata)
        {
            int offset = -1;
            switch (tbndata.Length)
            {
                case 6040:
                    offset = 0x177c;
                    break;
                case 8884:
                    offset = 0x2298;
                    break;
                case 8904:
                    offset = 0x22ac;
                    break;
                case 8980:
                    offset = 0x22e8;
                    break;
            }
            string tsoname = "";
            for (int index = 0; tbndata[offset + index] != 0; index++)
            {
                tsoname += (char)tbndata[offset + index];
            }
            return tsoname;
        }
        public static void SetTsoSignature(byte[] tbndata, TBNCategoryData category)
        {
            if (category.byte1 != 0x1a)
            {
                tbndata[0x6f0] = category.byte1;
                tbndata[0x73a] = category.byte2;
                tbndata[0x756] = category.byte3;
            }
        }
    }

    public class TBNCategoryData
    {
        public char symbol;
        public string name;
        public byte byte1;
        public byte byte2;
        public byte byte3;
        public void set(char sym,string nam,byte b1,byte b2,byte b3)
        {
            symbol=sym;
            name=nam;
            byte1=b1;
            byte2=b2;
            byte3=b3;
        }
    }
}
