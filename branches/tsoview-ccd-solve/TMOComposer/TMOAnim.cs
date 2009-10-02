using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TDCG;

namespace TMOComposer
{
    public class TMOAnimItem
    {
        public string PoseFile { get; set; }
        public int Length { get; set; }
        public string FaceFile { get; set; }
        public float Accel { get; set; }

        public TMOAnimItem()
        {
            this.Length = 30;
            this.Accel = 0.5f;
        }
        [XmlIgnore]
        public TMOFile Tmo { get; set; }

        public string GetPoseFileWithTmoExtension()
        {
            return Path.GetFileNameWithoutExtension(this.PoseFile) + ".tmo";
        }

        public void CopyFace()
        {
            if (Tmo.frames == null)
                return;

            List<string> except_snames = new List<string>();
            except_snames.Add("Kami_Oya");

            if (this.FaceFile != null)
            {
                Console.WriteLine("Load File: " + this.FaceFile);
                TMOFile face_tmo = TMOAnim.LoadPNGFile(TMOAnim.GetFacePath(this.FaceFile));
                if (face_tmo.frames != null)
                    Tmo.CopyChildrenNodeFrom(face_tmo, "face_oya", except_snames);
            }
        }
    }

    public class TMOAnim
    {
        public TMOAnimItem SourceItem
        {
            get
            {
                if (items.Count != 0)
                    return items[items.Count - 1];
                else
                    return null;
            }
        }

        public string SourceFile
        {
            get
            {
                if (SourceItem != null)
                    return SourceItem.PoseFile;
                else
                    return null;
            }
        }

        public List<TMOAnimItem> items = new List<TMOAnimItem>();

        public void Dump(string dest_file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TMOAnim));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.GetEncoding("Shift_JIS");
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(dest_file, settings);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static TMOAnim Load(string source_file)
        {
            XmlReader reader = XmlReader.Create(source_file);
            XmlSerializer serializer = new XmlSerializer(typeof(TMOAnim));
            TMOAnim program = serializer.Deserialize(reader) as TMOAnim;
            reader.Close();
            return program;
        }

        TMOFile source;
        public TMOFile SourceTmo { get { return source;  } }

        public TMOAnim()
        {
            source = new TMOFile();
        }

        public static string PoseRoot { get; set; }
        public static string FaceRoot { get; set; }

        public static string GetPosePath(string pose_file)
        {
            return PoseRoot + @"\" + pose_file;
        }

        public static string GetFacePath(string face_file)
        {
            return FaceRoot + @"\" + face_file;
        }

        public void LoadSource()
        {
            source = CreateTmo(SourceItem);
        }

        public void SaveSourceToFile(string dest_path)
        {
            if (source.frames != null)
                source.Save(dest_path);
        }

        public static TMOFile LoadPNGFile(string source_file)
        {
            TMOFile tmo = new TMOFile();
            if (File.Exists(source_file))
                try
                {
                    PNGFile png = new PNGFile();

                    png.Ftmo += delegate(Stream dest, int extract_length)
                    {
                        tmo.Load(dest);
                    };
                    png.Load(source_file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
            return tmo;
        }

        public TMOFile GetTmo(TMOAnimItem item)
        {
            TMOFile tmo;

            if (item.Tmo != null)
            {
                tmo = item.Tmo;
                tmo.SaveTransformationMatrix(0);
            }
            else
            {
                tmo = CreateTmo(item);
                tmo.LoadTransformationMatrix(0);
                item.Tmo = tmo;
            }

            return tmo;
        }

        public TMOFile CreateTmo(TMOAnimItem item)
        {
            TMOFile tmo = new TMOFile();

            if (item == null)
                return tmo;

            string tmo_file = item.GetPoseFileWithTmoExtension();
            if (File.Exists(tmo_file))
            {
                Console.WriteLine("Load File: " + tmo_file);
                tmo.Load(tmo_file);
            }
            else
            {
                Console.WriteLine("Load File: " + item.PoseFile);
                tmo = LoadPNGFile(GetPosePath(item.PoseFile));
            }

            if (tmo.frames == null)
                return tmo;

            tmo.TruncateFrame(0); // forced pose

            return tmo;
        }
        
        public void SavePoseToFile()
        {
            foreach (TMOAnimItem item in items)
            {
                TMOFile tmo = GetTmo(item);

                if (tmo.frames != null)
                    tmo.Save(item.GetPoseFileWithTmoExtension());
            }
        }

        public void Process()
        {
            foreach (TMOAnimItem item in items)
            {
                TMOFile tmo = GetTmo(item);

                if (tmo.frames == null)
                    continue;

                source.SlerpFrameEndTo(tmo, item.Length, item.Accel);
                Console.WriteLine("source nodes Length {0}", source.nodes.Length);
                Console.WriteLine("motion nodes Length {0}", tmo.nodes.Length);
                Console.WriteLine("source frames Length {0}", source.frames.Length);
                Console.WriteLine("motion frames Length {0}", tmo.frames.Length);
            }
        }
    }
}
