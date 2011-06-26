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
    public class MmdMatrix
    {
        //NyARToolkitと統合かな。
        public double[,] m = new double[4, 4];
        public static MmdMatrix[] createArray(int i_length)
        {
            MmdMatrix[] ret = new MmdMatrix[i_length];
            for (int i = 0; i < i_length; i++)
            {
                ret[i] = new MmdMatrix();
            }
            return ret;
        }
        public void MatrixIdentity()
        {
            this.m[0, 1] = this.m[0, 2] = this.m[0, 3] = this.m[1, 0] = this.m[1, 2] = this.m[1, 3] = this.m[2, 0] = this.m[2, 1] = this.m[2, 3] = this.m[3, 0] = this.m[3, 1] = this.m[3, 2] = 0.0f;
            this.m[0, 0] = this.m[1, 1] = this.m[2, 2] = this.m[3, 3] = 1.0f;
            return;
        }
        private double[,] _array_temp = new double[4, 4];
        public void MatrixMultiply(MmdMatrix matSrc1, MmdMatrix matSrc2)
        {
            double[,] matTemp = this._array_temp;
            int i;

            for (i = 0; i < 4; i++)
            {
                matTemp[i, 0] = matSrc1.m[i, 0] * matSrc2.m[0, 0] + matSrc1.m[i, 1] * matSrc2.m[1, 0] + matSrc1.m[i, 2] * matSrc2.m[2, 0] + matSrc1.m[i, 3] * matSrc2.m[3, 0];
                matTemp[i, 1] = matSrc1.m[i, 0] * matSrc2.m[0, 1] + matSrc1.m[i, 1] * matSrc2.m[1, 1] + matSrc1.m[i, 2] * matSrc2.m[2, 1] + matSrc1.m[i, 3] * matSrc2.m[3, 1];
                matTemp[i, 2] = matSrc1.m[i, 0] * matSrc2.m[0, 2] + matSrc1.m[i, 1] * matSrc2.m[1, 2] + matSrc1.m[i, 2] * matSrc2.m[2, 2] + matSrc1.m[i, 3] * matSrc2.m[3, 2];
                matTemp[i, 3] = matSrc1.m[i, 0] * matSrc2.m[0, 3] + matSrc1.m[i, 1] * matSrc2.m[1, 3] + matSrc1.m[i, 2] * matSrc2.m[2, 3] + matSrc1.m[i, 3] * matSrc2.m[3, 3];
            }

            for (i = 0; i < 4; i++)
            {
                this.m[i, 0] = matTemp[i, 0];
                this.m[i, 1] = matTemp[i, 1];
                this.m[i, 2] = matTemp[i, 2];
                this.m[i, 3] = matTemp[i, 3];
            }
        }

        public void MatrixInverse(MmdMatrix matSrc)
        {
            double[,] matTemp = this._array_temp;
            for (int i = 0; i < 4; i++)
            {
                for (int i2 = 0; i2 < 4; i2++)
                {
                    matTemp[i, i2] = matSrc.m[i, i2];
                }
            }
            this.MatrixIdentity();

            //掃き出し法
            for (int i = 0; i < 4; i++)
            {
                double buf = 1.0 / matTemp[i, i];
                for (int j = 0; j < 4; j++)
                {
                    matTemp[i, j] *= buf;
                    this.m[i, j] *= buf;
                }
                for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                    {
                        buf = matTemp[j, i];
                        for (int k = 0; k < 4; k++)
                        {
                            matTemp[j, k] -= matTemp[i, k] * buf;
                            this.m[j, k] -= this.m[i, k] * buf;
                        }
                    }
                }
            }
            return;
        }

        public void MatrixLerp(MmdMatrix matSrc1, MmdMatrix matSrc2, float fLerpValue)
        {
            double[,] sm1 = matSrc1.m;
            double[,] sm2 = matSrc2.m;
            double[,] dm = this.m;
            double fT = 1.0 - fLerpValue;
            for (int i = 0; i < 4; i++)
            {
                dm[i, 0] = sm1[i, 0] * fLerpValue + sm2[i, 0] * fT;
                dm[i, 1] = sm1[i, 1] * fLerpValue + sm2[i, 1] * fT;
                dm[i, 2] = sm1[i, 2] * fLerpValue + sm2[i, 2] * fT;
                dm[i, 3] = sm1[i, 3] * fLerpValue + sm2[i, 3] * fT;
            }
            return;
        }
        public void QuaternionToMatrix(MmdVector4 pvec4Quat)
        {
            double x2 = pvec4Quat.x * pvec4Quat.x * 2.0f;
            double y2 = pvec4Quat.y * pvec4Quat.y * 2.0f;
            double z2 = pvec4Quat.z * pvec4Quat.z * 2.0f;
            double xy = pvec4Quat.x * pvec4Quat.y * 2.0f;
            double yz = pvec4Quat.y * pvec4Quat.z * 2.0f;
            double zx = pvec4Quat.z * pvec4Quat.x * 2.0f;
            double xw = pvec4Quat.x * pvec4Quat.w * 2.0f;
            double yw = pvec4Quat.y * pvec4Quat.w * 2.0f;
            double zw = pvec4Quat.z * pvec4Quat.w * 2.0f;

            double[,] mt = this.m;
            mt[0, 0] = 1.0f - y2 - z2;
            mt[0, 1] = xy + zw;
            mt[0, 2] = zx - yw;
            mt[1, 0] = xy - zw;
            mt[1, 1] = 1.0f - z2 - x2;
            mt[1, 2] = yz + xw;
            mt[2, 0] = zx + yw;
            mt[2, 1] = yz - xw;
            mt[2, 2] = 1.0f - x2 - y2;

            mt[0, 3] = mt[1, 3] = mt[2, 3] = mt[3, 0] = mt[3, 1] = mt[3, 2] = 0.0f;
            mt[3, 3] = 1.0f;
            return;
        }
    }

}
