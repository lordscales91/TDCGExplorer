/* 
 * PROJECT: MMD for Java
 * --------------------------------------------------------------------------------
 * This work is based on the ARTK_MMD v0.1 
 *   PY
 * http://ppyy.hp.infoseek.co.jp/
 * py1024<at>gmail.com
 * http://www.nicovideo.jp/watch/sm7398691
 *
 * The MMD for Java is Java version MMD class library.
 * Copyright (C)2009 nyatla
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/
 *	<airmail(at)ebony.plala.or.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace jp.nyatla.nymmd.cs.struct_type
{
    public class DataWriter
    {
        private BinaryWriter _buf;
        public DataWriter(BinaryWriter i_writer)
        {
            this._buf = i_writer;
        }
        public void writeByte(int x)
        {
            this._buf.Write((byte)x);
        }
        public void write(int x)
        {
            this._buf.Write((sbyte)x);
        }
        public void writeShort(int x)
        {
            this._buf.Write((Int16)x);
        }
        public void writeUnsignedShort(int x)
        {
            this._buf.Write((UInt16)x);
        }
        public void writeInt(int x)
        {
            this._buf.Write((Int32)x);
        }
        public void writeFloat(float x)
        {
            this._buf.Write((float)x);
        }
        public void writeFloat(double x)
        {
            this._buf.Write((float)x);
        }
        public void writeDouble(float x)
        {
            this._buf.Write((double)x);
        }
        public void writeDouble(double x)
        {
            this._buf.Write((double)x);
        }
        public void writeAscii(String str, int i_length)
        {
            if (str != null)
            {
                this._buf.Write(Encoding.GetEncoding(932).GetBytes(str)); // Shift JISとしてbyte配列に変換
                if (i_length > Encoding.GetEncoding(932).GetBytes(str).Length)
                    this._buf.Write(Convert.ToByte("00", 16)); // 文字列終端の記号
                for (int i = 0; i < (i_length - 1 - Encoding.GetEncoding(932).GetBytes(str).Length); i++)
                    this._buf.Write(Convert.ToByte("FD", 16)); // ストリームの位置を合わせるため、意味のないByteを書き込む
            }
            else
            {
                this._buf.Write(Convert.ToByte("00", 16)); // 文字列終端の記号
                for (int i = 0; i < (i_length - 1); i++)
                    this._buf.Write(Convert.ToByte("FD", 16)); // ストリームの位置を合わせるため、意味のないByteを書き込む
            }
        }
    }
}
