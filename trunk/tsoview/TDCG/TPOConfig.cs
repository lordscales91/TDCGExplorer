using System;
//using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TDCG
{
    /// <summary>
    /// 体型レシピ要素
    /// </summary>
public class Proportion
{
    /// <summary>
    /// 体型スクリプト名
    /// </summary>
    public string ClassName { get; set; }
    /// <summary>
    /// 変形比率
    /// </summary>
    public float Ratio { get; set; }
}
/// <summary>
/// 体型レシピを扱います。
/// </summary>
public class TPOConfig
{
    /// <summary>
    /// 体型レシピ要素の配列
    /// </summary>
    public Proportion[] Proportions { get; set; }

    /// <summary>
    /// 体型レシピを生成します。
    /// </summary>
    public TPOConfig()
    {
        this.Proportions = new Proportion[0];
    }

    /// <summary>
    /// 体型レシピを書き出します。
    /// </summary>
    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TPOConfig));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// 体型レシピを読み込みます。
    /// </summary>
    /// <param name="source_file">ファイル名</param>
    /// <returns>体型レシピ</returns>
    public static TPOConfig Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(TPOConfig));
        TPOConfig config = serializer.Deserialize(reader) as TPOConfig;
        reader.Close();
        return config;
    }

    /// <summary>
    /// 体型レシピを保存します。
    /// </summary>
    /// <param name="dest_file">保存ファイル名</param>
    public void Save(string dest_file)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TPOConfig));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(dest_file, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }
}
}
