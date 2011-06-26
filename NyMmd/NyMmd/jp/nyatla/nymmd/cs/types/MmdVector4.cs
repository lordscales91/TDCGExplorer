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
    public class MmdVector4
    {
        public double x, y, z, w;

        public void setValue(MmdVector4 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
            return;
        }
        public void QuaternionSlerp(MmdVector4 pvec4Src1, MmdVector4 pvec4Src2, double fLerpValue)
        {

            // Qlerp
            double qr = pvec4Src1.x * pvec4Src2.x + pvec4Src1.y * pvec4Src2.y + pvec4Src1.z * pvec4Src2.z + pvec4Src1.w * pvec4Src2.w;
            double t0 = 1.0f - fLerpValue;

            if (qr < 0)
            {
                this.x = pvec4Src1.x * t0 - pvec4Src2.x * fLerpValue;
                this.y = pvec4Src1.y * t0 - pvec4Src2.y * fLerpValue;
                this.z = pvec4Src1.z * t0 - pvec4Src2.z * fLerpValue;
                this.w = pvec4Src1.w * t0 - pvec4Src2.w * fLerpValue;
            }
            else
            {
                this.x = pvec4Src1.x * t0 + pvec4Src2.x * fLerpValue;
                this.y = pvec4Src1.y * t0 + pvec4Src2.y * fLerpValue;
                this.z = pvec4Src1.z * t0 + pvec4Src2.z * fLerpValue;
                this.w = pvec4Src1.w * t0 + pvec4Src2.w * fLerpValue;
            }
            QuaternionNormalize(this);
            return;
        }
        public void QuaternionNormalize(MmdVector4 pvec4Src)
        {
            double fSqr = 1.0 / Math.Sqrt((pvec4Src.x * pvec4Src.x + pvec4Src.y * pvec4Src.y + pvec4Src.z * pvec4Src.z + pvec4Src.w * pvec4Src.w));

            this.x = (pvec4Src.x * fSqr);
            this.y = (pvec4Src.y * fSqr);
            this.z = (pvec4Src.z * fSqr);
            this.w = (pvec4Src.w * fSqr);
        }
        public void QuaternionCreateAxis(MmdVector3 pvec3Axis, double fRotAngle)
        {
            if (Math.Abs(fRotAngle) < 0.0001f)
            {
                this.x = this.y = this.z = 0.0f;
                this.w = 1.0f;
            }
            else
            {
                fRotAngle *= 0.5f;
                double fTemp = Math.Sin(fRotAngle);

                this.x = pvec3Axis.x * fTemp;
                this.y = pvec3Axis.y * fTemp;
                this.z = pvec3Axis.z * fTemp;
                this.w = Math.Cos(fRotAngle);
            }
            return;
        }
        public void QuaternionMultiply(MmdVector4 pvec4Src1, MmdVector4 pvec4Src2)
        {
            double px, py, pz, pw;
            double qx, qy, qz, qw;

            px = pvec4Src1.x; py = pvec4Src1.y; pz = pvec4Src1.z; pw = pvec4Src1.w;
            qx = pvec4Src2.x; qy = pvec4Src2.y; qz = pvec4Src2.z; qw = pvec4Src2.w;

            this.x = pw * qx + px * qw + py * qz - pz * qy;
            this.y = pw * qy - px * qz + py * qw + pz * qx;
            this.z = pw * qz + px * qy - py * qx + pz * qw;
            this.w = pw * qw - px * qx - py * qy - pz * qz;
        }
        public void QuaternionCreateEuler(MmdVector3 pvec3EulerAngle)
        {
            double xRadian = pvec3EulerAngle.x * 0.5;
            double yRadian = pvec3EulerAngle.y * 0.5;
            double zRadian = pvec3EulerAngle.z * 0.5;
            double sinX = Math.Sin(xRadian);
            double cosX = Math.Cos(xRadian);
            double sinY = Math.Sin(yRadian);
            double cosY = Math.Cos(yRadian);
            double sinZ = Math.Sin(zRadian);
            double cosZ = Math.Cos(zRadian);

            // XYZ
            this.x = sinX * cosY * cosZ - cosX * sinY * sinZ;
            this.y = cosX * sinY * cosZ + sinX * cosY * sinZ;
            this.z = cosX * cosY * sinZ - sinX * sinY * cosZ;
            this.w = cosX * cosY * cosZ + sinX * sinY * sinZ;
        }
    }

}
