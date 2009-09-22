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

        string pose_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG\pose";

        string GetSourcePath(string source_file)
        {
            return pose_path + @"\" + source_file;
        }

        string GetMotionPath(string motion_file)
        {
            return pose_path + @"\" + motion_file;
        }

        public void LoadSource()
        {
            List<string> except_snames = new List<string>();
            except_snames.Add("Kami_Oya");

            source = LoadPNGFile(GetSourcePath(SourceFile));
            source.TruncateFrame(0); // forced pose

            if (SourceItem.FaceFile != null)
            {
                Console.WriteLine("Load File: " + SourceItem.FaceFile);
                TMOFile face_motion = LoadPNGFile(GetMotionPath(SourceItem.FaceFile));
                if (face_motion.frames != null)
                    source.CopyChildrenNodeFrom(face_motion, "face_oya", except_snames);
            }
        }

        public void SaveSourceToFile(string dest_path)
        {
            if (source.frames != null)
                source.Save(dest_path);
        }

        public TMOFile LoadPNGFile(string source_file)
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

        public void Process()
        {
            List<string> except_snames = new List<string>();
            except_snames.Add("Kami_Oya");

            foreach (TMOAnimItem item in items)
            {
                Console.WriteLine("Load File: " + item.PoseFile);
                TMOFile motion = LoadPNGFile(GetMotionPath(item.PoseFile));
                motion.TruncateFrame(0); // forced pose

                if (item.FaceFile != null)
                {
                    Console.WriteLine("Load File: " + item.FaceFile);
                    TMOFile face_motion = LoadPNGFile(GetMotionPath(item.FaceFile));
                    if (face_motion.frames != null)
                        motion.CopyChildrenNodeFrom(face_motion, "face_oya", except_snames);
                }

                source.SlerpFrameEndTo(motion, item.Length);
                Console.WriteLine("source nodes Length {0}", source.nodes.Length);
                Console.WriteLine("motion nodes Length {0}", motion.nodes.Length);
                Console.WriteLine("source frames Length {0}", source.frames.Length);
                Console.WriteLine("motion frames Length {0}", motion.frames.Length);
            }
        }
    }
}
