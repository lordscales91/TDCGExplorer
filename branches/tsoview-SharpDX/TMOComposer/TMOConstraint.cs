using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;
using System.Xml;
using System.Xml.Serialization;

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
    public string Name { get; set; }
    /// <summary>
    /// 角度の最小値
    /// </summary>
    public Vector3 Min { get; set; }
    /// <summary>
    /// 角度の最大値
    /// </summary>
    public Vector3 Max { get; set; }
    /// 象限X
    public int SectorX { get; set; }
    /// 象限Y
    public int SectorY { get; set; }
    /// 象限Z
    public int SectorZ { get; set; }

    /// euler角の範囲を制限します。
    public Vector3 Limit(Vector3 angle1)
    {
        Vector3 angle0 = Vector3.Zero;
        Vector3 angle2 = angle1;

        if (angle2.X < 0) angle2.X += 360;
        if (angle2.Y < 0) angle2.Y += 360;
        if (angle2.Z < 0) angle2.Z += 360;

        if (SectorX == 1)
        {
            if (angle1.X < Min.X) angle1.X = Min.X;
            if (angle1.X > Max.X) angle1.X = Max.X;
            angle0.X = angle1.X;
        }
        else
        {
            if (angle2.X < Min.X) angle2.X = Min.X;
            if (angle2.X > Max.X) angle2.X = Max.X;
            if (angle2.X > 180) angle2.X -= 360;
            angle0.X = angle2.X;
        }

        if (SectorY == 1)
        {
            if (angle1.Y < Min.Y) angle1.Y = Min.Y;
            if (angle1.Y > Max.Y) angle1.Y = Max.Y;
            angle0.Y = angle1.Y;
        }
        else
        {
            if (angle2.Y < Min.Y) angle2.Y = Min.Y;
            if (angle2.Y > Max.Y) angle2.Y = Max.Y;
            if (angle2.Y > 180) angle2.Y -= 360;
            angle0.Y = angle2.Y;
        }

        if (SectorZ == 1)
        {
            if (angle1.Z < Min.Z) angle1.Z = Min.Z;
            if (angle1.Z > Max.Z) angle1.Z = Max.Z;
            angle0.Z = angle1.Z;
        }
        else
        {
            if (angle2.Z < Min.Z) angle2.Z = Min.Z;
            if (angle2.Z > Max.Z) angle2.Z = Max.Z;
            if (angle2.Z > 180) angle2.Z -= 360;
            angle0.Z = angle2.Z;
        }

        return angle0;
    }
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

        Dictionary<string, Vector3> min1_dic = new Dictionary<string, Vector3>();
        Dictionary<string, Vector3> max1_dic = new Dictionary<string, Vector3>();

        Dictionary<string, Vector3> min2_dic = new Dictionary<string, Vector3>();
        Dictionary<string, Vector3> max2_dic = new Dictionary<string, Vector3>();

        string[] files = Directory.GetFiles(source_file, "*.tmo");
        foreach (string file in files)
        {
            tmo.Load(file);
            foreach (TMONode node in tmo.nodes)
            {
                TMOMat mat = node.matrices[0];

                string name = node.Name;
                Vector3 angle1 = Helper.ToAngleXYZ(mat.m);
                Vector3 angle2 = angle1;

                if (angle2.X < 0) angle2.X += 360;
                if (angle2.Y < 0) angle2.Y += 360;
                if (angle2.Z < 0) angle2.Z += 360;

                if (! min1_dic.ContainsKey(name))
                    min1_dic[name] = new Vector3(+180.0f, +180.0f, +180.0f);
                if (! max1_dic.ContainsKey(name))
                    max1_dic[name] = new Vector3(-180.0f, -180.0f, -180.0f);

                Vector3 min1 = min1_dic[name];
                Vector3 max1 = max1_dic[name]; 

                if (angle1.X < min1_dic[name].X) min1.X = angle1.X;
                if (angle1.X > max1_dic[name].X) max1.X = angle1.X;

                if (angle1.Y < min1_dic[name].Y) min1.Y = angle1.Y;
                if (angle1.Y > max1_dic[name].Y) max1.Y = angle1.Y;

                if (angle1.Z < min1_dic[name].Z) min1.Z = angle1.Z;
                if (angle1.Z > max1_dic[name].Z) max1.Z = angle1.Z;

                min1_dic[name] = min1;
                max1_dic[name] = max1;

                if (! min2_dic.ContainsKey(name))
                    min2_dic[name] = new Vector3(360.0f, 360.0f, 360.0f);
                if (! max2_dic.ContainsKey(name))
                    max2_dic[name] = new Vector3(0.0f, 0.0f, 0.0f);

                Vector3 min2 = min2_dic[name];
                Vector3 max2 = max2_dic[name]; 

                if (angle2.X < min2_dic[name].X) min2.X = angle2.X;
                if (angle2.X > max2_dic[name].X) max2.X = angle2.X;

                if (angle2.Y < min2_dic[name].Y) min2.Y = angle2.Y;
                if (angle2.Y > max2_dic[name].Y) max2.Y = angle2.Y;

                if (angle2.Z < min2_dic[name].Z) min2.Z = angle2.Z;
                if (angle2.Z > max2_dic[name].Z) max2.Z = angle2.Z;

                min2_dic[name] = min2;
                max2_dic[name] = max2;
            }
        }

        foreach (string name in min1_dic.Keys)
        {
            TMOConstraintItem item = new TMOConstraintItem();
            item.Name = name;

            Vector3 min1 = min1_dic[name];
            Vector3 max1 = max1_dic[name];

            Vector3 min2 = min2_dic[name];
            Vector3 max2 = max2_dic[name];

            Vector3 sub1 = max1 - min1;
            Vector3 sub2 = max2 - min2;

            Vector3 min;
            Vector3 max;

            if (sub1.X <= sub2.X)
            {
                min.X = min1.X;
                max.X = max1.X;
                item.SectorX = 1;
            }
            else
            {
                min.X = min2.X;
                max.X = max2.X;
                item.SectorX = 2;
            }

            if (sub1.Y <= sub2.Y)
            {
                min.Y = min1.Y;
                max.Y = max1.Y;
                item.SectorY = 1;
            }
            else
            {
                min.Y = min2.Y;
                max.Y = max2.Y;
                item.SectorY = 2;
            }

            if (sub1.Z <= sub2.Z)
            {
                min.Z = min1.Z;
                max.Z = max1.Z;
                item.SectorZ = 1;
            }
            else
            {
                min.Z = min2.Z;
                max.Z = max2.Z;
                item.SectorZ = 2;
            }
            item.Min = min;
            item.Max = max;

            items.Add(item);
        }
    }

    /// <summary>
    /// node名（短い形式）に対応する要素を得ます。
    /// </summary>
    /// <param name="name">node名（短い形式）</param>
    public TMOConstraintItem GetItem(string name)
    {
        foreach (TMOConstraintItem item in items)
        {
            if (item.Name == name)
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
