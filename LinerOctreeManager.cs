using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace tso2mqo
{
    public class Cell : LinkedList<int>
    {
        /// nodeをプッシュ
        public bool Push(LinkedListNode<int> node)
        {
            if (node == null)
                return false; // 無効オブジェクトは登録しない
            if (node.List == this)
                return false; // 2重登録チェック
            AddFirst(node);
            return true;
        }
    }

    /// 線形8分木空間管理クラス
    public class LinerOctreeManager
    {
        public const int MaxLevel = 7;

        /// 線形空間ポインタ配列
        public Cell[] cells;
        /// べき乗数値配列
        protected UInt32[] pow = new UInt32[MaxLevel + 1];
        /// 領域の幅
        protected Vector3 size;
        /// 領域の最小値
        protected Vector3 rgn_min;
        /// 領域の最大値
        protected Vector3 rgn_max;
        /// 最小領域の辺の長さ
        protected Vector3 unit;
        /// 空間の数
        public UInt32 ncell;
        /// 最下位レベル
        protected int level;

        /// 線形8分木配列を構築する
        public bool Init(int level, ref Vector3 min, ref Vector3 max)
        {
            // 各レベルでの空間数を算出
            pow[0] = 1;
            for (int i = 1; i < MaxLevel + 1; i++)
                pow[i] = pow[i - 1] * 8;

            // levelレベル（0基点）の配列作成
            ncell = (pow[level + 1] - 1) / 3;
            cells = new Cell[ncell];

            // 領域を登録
            rgn_min = min;
            rgn_max = max;
            size = rgn_max - rgn_min;
            float div = (float)(1 << level);
            Console.WriteLine("div {0}", div);
            unit = size;
            unit.X /= div;
            unit.Y /= div;
            unit.Z /= div;

            this.level = level;

            return true;
        }

        /// オブジェクトを登録する
        public bool Regist(ref Vector3 min, ref Vector3 max, LinkedListNode<int> node)
        {
            // オブジェクトの境界範囲から登録モートン番号を算出
            UInt32 num = GetMortonNumber(ref min, ref max);
            if (num < ncell)
            {
                // 空間が無い場合は新規作成
                if (cells[num] == null)
                    CreateNewCell(num);
                return cells[num].Push(node);
            }
            return false; // 登録失敗
        }

        /// 空間を生成
        public bool CreateNewCell(UInt32 num)
        {
            // 引数の要素番号
            while (num < ncell && cells[num] == null)
            {
                // 指定の要素番号に空間を新規作成
                cells[num] = new Cell();

                // 親空間にジャンプ
                num = (num - 1) >> 2;
            }
            return true;
        }

        /// 座標から空間番号を算出
        public UInt32 GetMortonNumber(ref Vector3 min, ref Vector3 max)
        {
            // 最小レベルにおける各軸位置を算出
            UInt32 LT = GetPointElem(ref min);
            UInt32 RB = GetPointElem(ref max);

            // 空間番号を引き算して
            // 最上位区切りから所属レベルを算出
            UInt32 Def = RB ^ LT;
            int HiLevel = 1;
            int i;
            for (i = 0; i < level; i++)
            {
                UInt32 Check = (Def >> (i * 3)) & 0x7;
                if (Check != 0)
                    HiLevel = i + 1;
            }
            UInt32 SpaceNum = RB >> (HiLevel * 3);
            UInt32 AddNum = (pow[level - HiLevel] - 1) / 7;
            SpaceNum += AddNum;

            if (SpaceNum > ncell)
                return UInt32.MaxValue;

            return SpaceNum;
        }

        /// ビット分割関数
        public static UInt32 BitSeparateFor3D(Byte n)
        {
            UInt32 s = n;
            s = (s | s << 8) & 0x0000f00f;
            s = (s | s << 4) & 0x000c30c3;
            s = (s | s << 2) & 0x00249249;
            return s;
        }

        /// 3Dモートン空間番号算出関数
        public static UInt32 Get3DMortonNumber(Byte x, Byte y, Byte z)
        {
            return BitSeparateFor3D(x) | BitSeparateFor3D(y) << 1 | BitSeparateFor3D(z) << 2;
        }

        /// 座標→線形8分木要素番号変換関数
        public UInt32 GetPointElem(ref Vector3 p)
        {
            return Get3DMortonNumber(
                (Byte)((p.X - rgn_min.X) / unit.X),
                (Byte)((p.Y - rgn_min.Y) / unit.Y),
                (Byte)((p.Z - rgn_min.Z) / unit.Z)
                );
        }
    }
}
