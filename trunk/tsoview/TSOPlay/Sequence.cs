using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TDCG;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

//
// 一定時間ごとに指定されたスクリプトを実行する。
//
namespace TSOPlay
{
    class Sequence
    {
        private List<string> seqlist = null;
        private int seqcount;
        private int nextperiod;
        PlayViewer viewer = null;
        Vector3 camrotate;

        // シーケンスファイルを指定して読み込む.
        public void SetSequence(string FileName, PlayViewer v)
        {
            seqlist = new List<string>();
            seqcount = 0;
            nextperiod = 0;
            viewer = v;
            camrotate = new Vector3(0, 0, 0);

            string line = "";
            using (StreamReader sr = new StreamReader(FileName, Encoding.GetEncoding("Shift_JIS")))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    seqlist.Add(line);
                }
            }
        }

        // シーケンスを実行する.
        public void DoAction(int interval)
        {
            // シーケンスが空なら何もしない.
            if (seqlist == null) return;

            nextperiod -= interval;
            DoInterval();

            if (nextperiod <= 0)
            {
                for (; ; )
                {
                    // 文字列を切り出す.
                    string seq = seqlist[seqcount];
                    char[] splitter = { ' ', '\t' };
                    string[] args = seq.Split(splitter);

                    // コマンドを実行していく.
                    bool breakflag = DoCommand(args);

                    // 次の処理を設定.
                    seqcount++;
                    if (seqlist.Count <= seqcount) seqcount = 0; // 頭に戻る.
                    if (breakflag == true) break;
                }
            }
        }

        public bool DoCommand(string[] args)
        {
            try
            {
                switch (args[0].ToLower())
                {
                    case "clear":
                        // 全て削除する.
                        viewer.ClearFigureList();
                        viewer.MotionEnabled = false;
                        camrotate = new Vector3(0, 0, 0);
                        break;
                    case "load":
                        // ファイルを読み込む.
                        for (int i = 1; i < args.Length; i++)
                        {
                            viewer.LoadAnyFile(args[i]);
                            //viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);
                        }
                        break;
                    case "run":
                        // アニメーションを実行する.
                        viewer.MotionEnabled = true;
                        break;
                    case "translation":
                        {
                            float x = float.Parse(args[1]);
                            float y = float.Parse(args[2]);
                            float z = float.Parse(args[3]);
                            viewer.Camera.SetTranslation(x, y, z);
                        }
                        break;
                    case "angle":
                        {
                            float x = Geometry.DegreeToRadian(float.Parse(args[1]));
                            float y = Geometry.DegreeToRadian(float.Parse(args[2]));
                            float z = Geometry.DegreeToRadian(float.Parse(args[3]));
                            viewer.Camera.SetAngle(x, y, z);
                        }
                        break;
                    case "camtrack":
                        {
                            float x = Geometry.DegreeToRadian(float.Parse(args[1]));
                            float y = Geometry.DegreeToRadian(float.Parse(args[2]));
                            float z = Geometry.DegreeToRadian(float.Parse(args[3]));
                            camrotate = new Vector3(x, y, z);
                        }
                        break;
                    case "transition":
                        viewer.ReadyTransition();
                        break;
                    case "wait":
                        // 次の待ち時間を設定する.
                        nextperiod += int.Parse(args[1]);
                        return true;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message);
            }

            return false;
        }

        // 時間指定コマンドの実行.
        public void DoInterval()
        {
            viewer.Camera.SetAngle(viewer.Camera.Angle + camrotate);
        }

        // プレイリストの消去.
        public void Clear()
        {
            seqlist.Clear();
        }
    }
}
