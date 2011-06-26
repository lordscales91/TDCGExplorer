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

namespace jp.nyatla.nymmd.cs.types
{
    public class MmdColor4
    {
        public float r, g, b, a;

        public MmdColor4()
        {
            this.r = 0.0f;
            this.g = 0.0f;
            this.b = 0.0f;
            this.a = 0.0f;
            return;
        }
        public MmdColor4(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            return;
        }
        public void setValue(MmdColor4 v)
        {
            this.r = v.r;
            this.g = v.g;
            this.b = v.b;
            this.a = v.a;
            return;
        }
        public void getValue(float[] v, int i_st)
        {
            v[i_st + 0] = this.r;
            v[i_st + 1] = this.g;
            v[i_st + 2] = this.b;
            v[i_st + 3] = this.a;
            return;
        }
    }
}
