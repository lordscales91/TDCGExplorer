using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TDCGExplorer
{
    public static class TDCGTbnUtil
    {
        private static TBNCategoryData[] categoryTable;

        static TDCGTbnUtil()
        {
            categoryTable = new TBNCategoryData[30];
            categoryTable[0] = new TBNCategoryData('A', TextResource.TbnCatA, 0x00, 0x04, 0x03);
            categoryTable[1] = new TBNCategoryData('B', TextResource.TbnCatB, 0x01, 0x09, 0x08);
            categoryTable[2] = new TBNCategoryData('C', TextResource.TbnCatC, 0x02, 0x0E, 0x0D);
            categoryTable[3] = new TBNCategoryData('D', TextResource.TbnCatD, 0x03, 0x13, 0x12);
            categoryTable[4] = new TBNCategoryData('E', TextResource.TbnCatE, 0x04, 0x18, 0x17);
            categoryTable[5] = new TBNCategoryData('F', TextResource.TbnCatF, 0x05, 0x1D, 0x1C);
            categoryTable[6] = new TBNCategoryData('G', TextResource.TbnCatG, 0x06, 0x22, 0x21);
            categoryTable[7] = new TBNCategoryData('H', TextResource.TbnCatH, 0x07, 0x27, 0x26);
            categoryTable[8] = new TBNCategoryData('I', TextResource.TbnCatI, 0x08, 0x2C, 0x2B);
            categoryTable[9] = new TBNCategoryData('J', TextResource.TbnCatJ, 0x09, 0x31, 0x30);
            categoryTable[10] = new TBNCategoryData('K', TextResource.TbnCatK, 0x0A, 0x36, 0x35);
            categoryTable[11] = new TBNCategoryData('L', TextResource.TbnCatL, 0x0B, 0x3B, 0x3A);
            categoryTable[12] = new TBNCategoryData('M', TextResource.TbnCatM, 0x0C, 0x40, 0x3F);
            categoryTable[13] = new TBNCategoryData('N', TextResource.TbnCatN, 0x0D, 0x45, 0x44);
            categoryTable[14] = new TBNCategoryData('O', TextResource.TbnCatO, 0x0E, 0x4A, 0x49);
            categoryTable[15] = new TBNCategoryData('P', TextResource.TbnCatP, 0x0F, 0x4F, 0x4E);
            categoryTable[16] = new TBNCategoryData('Q', TextResource.TbnCatQ, 0x10, 0x54, 0x53);
            categoryTable[17] = new TBNCategoryData('R', TextResource.TbnCatR, 0x11, 0x59, 0x58);
            categoryTable[18] = new TBNCategoryData('S', TextResource.TbnCatS, 0x12, 0x5E, 0x5D);
            categoryTable[19] = new TBNCategoryData('T', TextResource.TbnCatT, 0x13, 0x63, 0x62);
            categoryTable[20] = new TBNCategoryData('U', TextResource.TbnCatU, 0x14, 0x68, 0x67);
            categoryTable[21] = new TBNCategoryData('V', TextResource.TbnCatV, 0x15, 0x6D, 0x6C);
            categoryTable[22] = new TBNCategoryData('W', TextResource.TbnCatW, 0x16, 0x72, 0x71);
            categoryTable[23] = new TBNCategoryData('X', TextResource.TbnCatX, 0x17, 0x77, 0x76);
            categoryTable[24] = new TBNCategoryData('Y', TextResource.TbnCatY, 0x18, 0x7C, 0x7B);
            categoryTable[25] = new TBNCategoryData('Z', TextResource.TbnCatZ, 0x19, 0x00, 0x00);
            categoryTable[26] = new TBNCategoryData('0', TextResource.TbnCat0, 0x1A, 0x86, 0x85);
            categoryTable[27] = new TBNCategoryData('1', TextResource.TbnCat1, 0x1B, 0x8B, 0x8A);
            categoryTable[28] = new TBNCategoryData('2', TextResource.TbnCat2, 0x1C, 0x90, 0x8F);
            categoryTable[29] = new TBNCategoryData('3', TextResource.TbnCat3, 0x1D, 0x95, 0x94);
        }

        public static TBNCategoryData[] CategoryData
        {
            get { return categoryTable; }
        }

        public static string GetCategoryText(string pathname)
        {
            string file = Path.GetFileNameWithoutExtension(pathname);
            string ext = Path.GetExtension(pathname).ToLower();
            if(file.Length!=12) return "";
            if (ext == ".psd" || ext == ".tbn" || ext == ".tso")
            {
                string typechar = file.Substring(9, 1);
                int type = typechartotype(typechar[0]);
                return CategoryData[type].name;
            }
            return "";
        }

        public static int typechartotype(char typecode)
        {
            int type = 0;
            string typestring = new string(typecode, 1);
            switch (typestring.ToUpper())
            {
                case "A": type = 0; break;
                case "B": type = 1; break;
                case "C": type = 2; break;
                case "D": type = 3; break;
                case "E": type = 4; break;
                case "F": type = 5; break;
                case "G": type = 6; break;
                case "H": type = 7; break;
                case "I": type = 8; break;
                case "J": type = 9; break;
                case "K": type = 10; break;
                case "L": type = 11; break;
                case "M": type = 12; break;
                case "N": type = 13; break;
                case "O": type = 14; break;
                case "P": type = 15; break;
                case "Q": type = 16; break;
                case "R": type = 17; break;
                case "S": type = 18; break;
                case "T": type = 19; break;
                case "U": type = 20; break;
                case "V": type = 21; break;
                case "W": type = 22; break;
                case "X": type = 23; break;
                case "Y": type = 24; break;
                case "Z": type = 25; break;
                case "0": type = 26; break;
                case "1": type = 27; break;
                case "2": type = 28; break;
                case "3": type = 29; break;
            }
            return type;
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
            if (offset == -1) return;
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
            if (offset == -1) return null;
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
        public static string ext(byte[] data)
        {
            if (data.Length > 4 && data[0] == 'F' && data[1] == 'O' && data[2] == 'N' && data[3] == 'T') return ".font";
            if (data.Length > 4 && data[0] == '8' && data[1] == 'B' && data[2] == 'P' && data[3] == 'S') return ".psd";
            if (data.Length > 4 && data[0] == 0x89 && data[1] == 'P' && data[2] == 'N' && data[3] == 'G') return ".png";
            if (data.Length > 4 && data[0] == 'T' && data[1] == 'S' && data[2] == 'O' && data[3] == '1') return ".tso";
            if (data.Length > 4 && data[0] == 'T' && data[1] == 'M' && data[2] == 'O' && data[3] == '1') return ".tmo";
            if (data.Length > 4 && data[0] == '/' && data[1] == '*' && data[2] == '*' && data[3] == '*') return ".cgfx";
            if (data.Length > 4 && data[0] == 'B' && data[1] == 'B' && data[2] == 'B' && data[3] == 'B') return ".tbn";
            if (data.Length > 4 && data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S') return ".ogg";
            return "";
        }
    }

    public class TBNCategoryData
    {
        public char symbol;
        public string name;
        public byte byte1;
        public byte byte2;
        public byte byte3;
        public TBNCategoryData(char sym, string nam, byte b1, byte b2, byte b3)
        {
            symbol=sym;
            name=nam;
            byte1=b1;
            byte2=b2;
            byte3=b3;
        }
    }
}
