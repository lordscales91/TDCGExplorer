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
using System.IO;
using System.Collections;
using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.struct_type;
using jp.nyatla.nymmd.cs.struct_type.pmd;
using jp.nyatla.nymmd.cs.struct_type.vmd;


namespace jp.nyatla.nymmd.cs
{
    //------------------------------
    //ボーンキーフレームソート用比較関数
    //------------------------------
    class BoneCompare : IComparer<BoneKeyFrame>
    {
        public int Compare(BoneKeyFrame o1, BoneKeyFrame o2)
        {
            return (int)(o1.fFrameNo - o2.fFrameNo);
        }
    }


    //------------------------------
    //表情キーフレームソート用比較関数
    //------------------------------
    class FaceCompare : IComparer<FaceKeyFrame>
    {
        public int Compare(FaceKeyFrame o1, FaceKeyFrame o2)
        {
            return (int)(o1.fFrameNo - o2.fFrameNo);
        }
    }

    public class MmdVmdMotion
    {
        private MotionData[] _motion_data_array;	// ボーンごとのキーフレームデータのリスト
        private FaceData[] _face_data_array;	// 表情ごとのキーフレームデータのリスト
        private float m_fMaxFrame;		// 最後のフレーム番号

        public MmdVmdMotion(BinaryReader i_reader)
        {
            initialize(i_reader);
            return;
        }
        public MmdVmdMotion(StreamReader i_reader)
        {
            initialize(new BinaryReader(i_reader.BaseStream));
            return;
        }
        public MmdVmdMotion(Stream i_stream)
        {
            initialize(new BinaryReader(i_stream));
            return;
        }

        public MotionData[] refMotionDataArray()
        {
            return this._motion_data_array;
        }
        public FaceData[] refFaceDataArray()
        {
            return this._face_data_array;
        }

        public float getMaxFrame()
        {
            return this.m_fMaxFrame;
        }


        private void initialize(BinaryReader i_reader)
        {

            DataReader reader = new DataReader(i_reader);

            // ヘッダのチェック
            VMD_Header tmp_vmd_header = new VMD_Header();
            tmp_vmd_header.read(reader);
            if (!tmp_vmd_header.szHeader.Equals("Vocaloid Motion Data 0002",StringComparison.CurrentCultureIgnoreCase))
            {
                throw new MmdException();
            }
            //ボーンと最大フレームを取得
            float[] max_frame = new float[1];
            this._motion_data_array = createMotionDataList(reader, max_frame);
            this.m_fMaxFrame = max_frame[0];

            //表情と最大フレームを再取得
            this._face_data_array = createFaceDataList(reader, max_frame);
            this.m_fMaxFrame = this.m_fMaxFrame > max_frame[0] ? this.m_fMaxFrame : max_frame[0];

            return;
        }

        private static FaceData[] createFaceDataList(DataReader i_reader, float[] o_max_frame)
        {
            //-----------------------------------------------------
            // 表情のキーフレーム数を取得
            List<FaceData> result = new List<FaceData>();
            int ulNumFaceKeyFrames = i_reader.readInt();

            // モーションデータ中の表情ごとのキーフレーム数をカウント
            VMD_Face[] tmp_vmd_face = new VMD_Face[ulNumFaceKeyFrames];
            for (int i = 0; i < ulNumFaceKeyFrames; i++)
            {
                tmp_vmd_face[i] = new VMD_Face();
                tmp_vmd_face[i].read(i_reader);
            }
            float max_frame = 0.0f;

            for (int i = 0; i < ulNumFaceKeyFrames; i++)
            {
                if (max_frame < (float)tmp_vmd_face[i].ulFrameNo)
                {
                    max_frame = (float)tmp_vmd_face[i].ulFrameNo;	// 最大フレーム更新
                }
                bool is_found = false;
                for (int i2 = 0; i2 < result.Count; i2++)
                {
                    FaceData pFaceTemp = result[i2];
                    if (pFaceTemp.szFaceName.Equals(tmp_vmd_face[i].szFaceName))
                    {
                        // リストに追加済み
                        pFaceTemp.ulNumKeyFrames++;
                        is_found = true;
                        break;
                    }
                }

                if (!is_found)
                {
                    // リストにない場合は新規ノードを追加
                    FaceData pNew = new FaceData();
                    pNew.szFaceName = tmp_vmd_face[i].szFaceName;
                    pNew.ulNumKeyFrames = 1;
                    result.Add(pNew);
                }
            }

            // キーフレーム配列を確保
            for (int i = 0; i < result.Count; i++)
            {
                FaceData pFaceTemp = result[i];
                pFaceTemp.pKeyFrames = FaceKeyFrame.createArray(pFaceTemp.ulNumKeyFrames);
                pFaceTemp.ulNumKeyFrames = 0;		// 配列インデックス用にいったん0にする
            }

            // 表情ごとにキーフレームを格納
            for (int i = 0; i < ulNumFaceKeyFrames; i++)
            {
                for (int i2 = 0; i2 < result.Count; i2++)
                {
                    FaceData pFaceTemp = result[i2];
                    if (pFaceTemp.szFaceName.Equals(tmp_vmd_face[i].szFaceName))
                    {
                        FaceKeyFrame pKeyFrame = pFaceTemp.pKeyFrames[pFaceTemp.ulNumKeyFrames];

                        pKeyFrame.fFrameNo = (float)tmp_vmd_face[i].ulFrameNo;
                        pKeyFrame.fRate = tmp_vmd_face[i].fFactor;

                        pFaceTemp.ulNumKeyFrames++;
                        break;
                    }
                }
            }

            // キーフレーム配列を昇順にソート
            for (int i = 0; i < result.Count; i++)
            {
                FaceData pFaceTemp = result[i];
                System.Array.Sort<FaceKeyFrame>(pFaceTemp.pKeyFrames, new FaceCompare());
            }
            o_max_frame[0] = max_frame;
            return result.ToArray();
        }
        private static MotionData[] createMotionDataList(DataReader i_reader, float[] o_max_frame)
        {
            List<MotionData> result = new List<MotionData>();
            int ulNumBoneKeyFrames = i_reader.readInt();

            // まずはモーションデータ中のボーンごとのキーフレーム数をカウント
            VMD_Motion[] tmp_vmd_motion = new VMD_Motion[ulNumBoneKeyFrames];
            for (int i = 0; i < ulNumBoneKeyFrames; i++)
            {
                tmp_vmd_motion[i] = new VMD_Motion();
                tmp_vmd_motion[i].read(i_reader);
            }		

            float max_frame = 0.0f;

            for (int i = 0; i < ulNumBoneKeyFrames; i++)
            {
                if (max_frame < tmp_vmd_motion[i].ulFrameNo)
                {
                    max_frame = tmp_vmd_motion[i].ulFrameNo;	// 最大フレーム更新
                }
                bool is_found = false;
                for (int i2 = 0; i2 < result.Count; i2++)
                {
                    MotionData pMotTemp = result[i2];
                    if (pMotTemp.szBoneName.Equals(tmp_vmd_motion[i].szBoneName))
                    {
                        // リストに追加済みのボーン
                        pMotTemp.ulNumKeyFrames++;
                        is_found = true;
                        break;
                    }
                }

                if (!is_found)
                {
                    // リストにない場合は新規ノードを追加
                    MotionData pNew = new MotionData();
                    pNew.szBoneName = tmp_vmd_motion[i].szBoneName;
                    pNew.ulNumKeyFrames = 1;
                    result.Add(pNew);
                }
            }


            // キーフレーム配列を確保
            for (int i = 0; i < result.Count; i++)
            {
                MotionData pMotTemp = result[i];
                pMotTemp.pKeyFrames = BoneKeyFrame.createArray(pMotTemp.ulNumKeyFrames);
                pMotTemp.ulNumKeyFrames = 0;		// 配列インデックス用にいったん0にする
            }

            // ボーンごとにキーフレームを格納
            for (int i = 0; i < ulNumBoneKeyFrames; i++)
            {
                for (int i2 = 0; i2 < result.Count; i2++)
                {
                    MotionData pMotTemp = result[i2];
                    if (pMotTemp.szBoneName.Equals(tmp_vmd_motion[i].szBoneName))
                    {
                        BoneKeyFrame pKeyFrame = pMotTemp.pKeyFrames[pMotTemp.ulNumKeyFrames];

                        pKeyFrame.fFrameNo = (float)tmp_vmd_motion[i].ulFrameNo;
                        pKeyFrame.vec3Position.setValue(tmp_vmd_motion[i].vec3Position);
                        pKeyFrame.vec4Rotate.QuaternionNormalize(tmp_vmd_motion[i].vec4Rotate);

                        pMotTemp.ulNumKeyFrames++;

                        break;
                    }
                }
            }

            // キーフレーム配列を昇順にソート

            for (int i = 0; i < result.Count; i++)
            {
                MotionData pMotTemp = result[i];
                System.Array.Sort<BoneKeyFrame>(pMotTemp.pKeyFrames, new BoneCompare());
            }

            o_max_frame[0] = max_frame;
            return result.ToArray();

        }
    }
}
