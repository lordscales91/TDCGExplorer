using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using System.Xml;
using System.Xml.Serialization;
using TDCG;

namespace TDCG
{
/// <summary>
/// nodeの回転角度制限要素
/// </summary>
public class TMOConstraintItem
{
    /// <summary>
    /// node名（短い形式）
    /// </summary>
    public string ShortName { get; set; }
    /// <summary>
    /// 角度の最小値
    /// </summary>
    public Vector3 Min { get; set; }
    /// <summary>
    /// 角度の最大値
    /// </summary>
    public Vector3 Max { get; set; }
}

/// <summary>
/// nodeの回転角度制限
/// </summary>
public class TMOConstraint
{
    /// 要素の配列
    public List<TMOConstraintItem> items = new List<TMOConstraintItem>();

    /// <summary>
    /// xml形式で出力します。
    /// </summary>
    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TMOConstraint));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// xml形式ファイルから生成します。
    /// </summary>
    /// <param name="source_file">xmlファイル名</param>
    public static TMOConstraint Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(TMOConstraint));
        TMOConstraint program = serializer.Deserialize(reader) as TMOConstraint;
        reader.Close();
        return program;
    }

    /// <summary>
    /// tmoファイルを含むディレクトリから要素を追加します。
    /// </summary>
    /// <param name="source_file">tmoファイルを含むディレクトリ名</param>
    public void AddItemFromTMODirectory(string source_file)
    {
        TMOFile tmo = new TMOFile();

        Dictionary<string, Vector3> min_dic = new Dictionary<string, Vector3>();
        Dictionary<string, Vector3> max_dic = new Dictionary<string, Vector3>();

        string[] files = Directory.GetFiles(source_file, "*.tmo");
        foreach (string file in files)
        {
            tmo.Load(file);
            foreach (TMONode node in tmo.nodes)
            {
                TMOMat mat = node.frame_matrices[0];

                string sname = node.ShortName;
                Vector3 angle = TMOMat.ToAngle(mat.m);

                if (! min_dic.ContainsKey(sname))
                    min_dic[sname] = Vector3.Empty;
                if (! max_dic.ContainsKey(sname))
                    max_dic[sname] = Vector3.Empty;

                Vector3 min = min_dic[sname];
                Vector3 max = max_dic[sname]; 

                if (angle.X < min_dic[sname].X)
                    min.X = angle.X;
                if (angle.X > max_dic[sname].X)
                    max.X = angle.X;

                if (angle.Y < min_dic[sname].Y)
                    min.Y = angle.Y;
                if (angle.Y > max_dic[sname].Y)
                    max.Y = angle.Y;

                if (angle.Z < min_dic[sname].Z)
                    min.Z = angle.Z;
                if (angle.Z > max_dic[sname].Z)
                    max.Z = angle.Z;

                min_dic[sname] = min;
                max_dic[sname] = max;
            }
        }

        foreach (string sname in min_dic.Keys)
        {
            TMOConstraintItem item = new TMOConstraintItem();
            item.ShortName = sname;
            item.Min = min_dic[sname];
            item.Max = max_dic[sname];
            items.Add(item);
            /*
            Console.WriteLine("node {0} x {1:F2}..{2:F2} y {3:F2}..{4:F2} z {5:F2}..{6:F2}", sname,
                    min_dic[sname].X,
                    max_dic[sname].X,
                    min_dic[sname].Y,
                    max_dic[sname].Y,
                    min_dic[sname].Z,
                    max_dic[sname].Z);
                    */
        }
    }

    /// <summary>
    /// node名（短い形式）に対応する要素を得る。
    /// </summary>
    /// <param name="sname">node名（短い形式）</param>
    public TMOConstraintItem GetItem(string sname)
    {
        foreach (TMOConstraintItem item in items)
        {
            if (item.ShortName == sname)
                return item;
        }
        return null;
    }

    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("TMOConstraint.exe <tmo directory>");
            return;
        }
        string source_file = args[0];

        TMOConstraint program = new TMOConstraint();
        program.AddItemFromTMODirectory(source_file);
        program.Dump();
    }
}
}
