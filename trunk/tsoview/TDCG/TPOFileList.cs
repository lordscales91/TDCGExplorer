using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TDCG
{
/// <summary>
/// TPO�t�@�C���̃��X�g�ł��B
/// </summary>
public class TPOFileList
{
    /// TPO�t�@�C�����X�g
    public List<TPOFile> files = new List<TPOFile>();

    //�������[�V�����s��l��ێ�����t���[���z��
    private TMOFrame[] frames;

    private TMOFile tmo = null;

    /// <summary>
    /// �C���f�N�T
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
    /// �v�f��
    /// </summary>
    public int Count
    {
        get
        {
            return files.Count;
        }
    }
    
    /// <summary>
    /// tpo��ǉ����܂��B
    /// </summary>
    /// <param name="tpo"></param>
    public void Add(TPOFile tpo)
    {
        tpo.Tmo = tmo;
        files.Add(tpo);
    }

    /// <summary>
    /// TPO�t�@�C�����X�g���������܂��B
    /// </summary>
    public void Clear()
    {
        files.Clear();
    }

    /// <summary>
    /// �̌^���X�g��ݒ肵�܂��B
    /// </summary>
    /// <param name="pro_list">�̌^���X�g</param>
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
    /// �S�Ẵt���[���Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    public void Transform()
    {
        LoadMatrix();
        foreach (TPOFile tpo in files)
            tpo.Transform();
    }

    /// <summary>
    /// �w��ԍ��̃t���[���Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
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

    //�������[�V�����s��l��ێ�����̈���m�ۂ���B
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
    /// �ޔ����[�V�����s��l��tmo�ɖ߂��܂��B
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
    /// ���[�V�����s��l��tmo����ޔ����܂��B
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
