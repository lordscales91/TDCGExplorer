using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TDCG
{
/// <summary>
/// TPOファイルのリストです。
/// </summary>
public class TPOFileList
{
    /// TPOファイルリスト
    public List<TPOFile> files = new List<TPOFile>();

    //初期モーション行列値を保持するフレーム配列
    private TMOFrame[] frames;

    private TMOFile tmo = null;

    /// <summary>
    /// インデクサ
    /// </summary>
    /// <param name="i">index</param>
    /// <returns>tpo</returns>
    public TPOFile this[int i]
    {
        get
        {
            return files[i];
        }
    }

    /// <summary>
    /// 要素数
    /// </summary>
    public int Count
    {
        get
        {
            return files.Count;
        }
    }
    
    /// <summary>
    /// tpoを追加します。
    /// </summary>
    /// <param name="tpo"></param>
    public void Add(TPOFile tpo)
    {
        tpo.Tmo = tmo;
        files.Add(tpo);
    }

    /// <summary>
    /// TPOファイルリストを消去します。
    /// </summary>
    public void Clear()
    {
        files.Clear();
    }

    /// <summary>
    /// 体型リストを設定します。
    /// </summary>
    /// <param name="pro_list">体型リスト</param>
    public void SetProportionList(ProportionList pro_list)
    {
        Clear();
        foreach (IProportion pro in pro_list.items)
        {
            TPOFile tpo = new TPOFile();
            tpo.Proportion = pro;
            Add(tpo);
        }
    }

    public void SetRatiosFromConfig(TPOConfig config)
    {
        Dictionary<string, Proportion> proportion_map = new Dictionary<string, Proportion>();

        foreach (Proportion proportion in config.Proportions)
            proportion_map[proportion.ClassName] = proportion;

        foreach (TPOFile tpo in files)
        {
            Debug.Assert(tpo.Proportion != null, "tpo.Proportion should not be null");
            Proportion proportion;
            if (proportion_map.TryGetValue(tpo.ProportionName, out proportion))
                tpo.Ratio = proportion.Ratio;
        }
    }

    /// <summary>
    /// 全てのフレームに含まれるモーション行列値を変形します。
    /// </summary>
    public void Transform()
    {
        LoadMatrix();
        foreach (TPOFile tpo in files)
            tpo.Transform();
    }

    /// <summary>
    /// 指定番号のフレームに含まれるモーション行列値を変形します。
    /// </summary>
    /// <param name="frame_index">フレーム番号</param>
    public void Transform(int frame_index)
    {
        LoadMatrix();
        foreach (TPOFile tpo in files)
            tpo.Transform(frame_index);
    }

    /// <summary>
    /// tmo
    /// </summary>
    public TMOFile Tmo
    {
        get
        {
            return tmo;
        }
        set
        {
            tmo = value;

            foreach (TPOFile tpo in files)
                tpo.Tmo = tmo;

            CreateFrames();
            SaveMatrix();
        }
    }

    //初期モーション行列値を保持する領域を確保する。
    private void CreateFrames()
    {
        if (tmo.frames == null)
            return;

        int frame_count = tmo.frames.Length;
        frames = new TMOFrame[frame_count];
        for (int i = 0; i < frame_count; i++)
        {
            int matrix_count = tmo.frames[i].matrices.Length;
            frames[i] = new TMOFrame();
            frames[i].id = i;
            frames[i].matrices = new TMOMat[matrix_count];
            for (int j = 0; j < matrix_count; j++)
            {
                frames[i].matrices[j] = new TMOMat();
            }
        }
    }

    /// <summary>
    /// 退避モーション行列値をtmoに戻します。
    /// </summary>
    public void LoadMatrix()
    {
        if (frames == null)
            return;

        if (tmo.frames == null)
            return;

        int frame_count = frames.Length;
        for (int i = 0; i < frame_count; i++)
        {
            int matrix_count = frames[i].matrices.Length;
            for (int j = 0; j < matrix_count; j++)
                tmo.frames[i].matrices[j].m = frames[i].matrices[j].m;
        }
    }

    /// <summary>
    /// モーション行列値をtmoから退避します。
    /// </summary>
    public void SaveMatrix()
    {
        if (frames == null)
            return;

        if (tmo.frames == null)
            return;

        int frame_count = frames.Length;
        for (int i = 0; i < frame_count; i++)
        {
            int matrix_count = frames[i].matrices.Length;
            for (int j = 0; j < matrix_count; j++)
                frames[i].matrices[j].m = tmo.frames[i].matrices[j].m;
        }
    }
}
}
