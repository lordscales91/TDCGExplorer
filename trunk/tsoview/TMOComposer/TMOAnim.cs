using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using TDCG;

namespace TMOComposer
{
    public class TMOAnimItem
    {
        public int Length { get; set; }
        public float Accel { get; set; }

        public TMOAnimItem()
        {
            this.Length = 30;
            this.Accel = 0.5f;
        }
        [XmlIgnore]
        public TMOFile Tmo { get; set; }

        int save_id;
        public int SaveID { get { return save_id; } }

        int id;
        public int ID { get { return id; } }

        public void UpdateID(int save_id, int id)
        {
            this.save_id = save_id;
            this.id = id;
        }

        public static string PoseRoot { get; set; }

        public static string FaceRoot { get; set; }

        public string GetTmoPath()
        {
            return Path.Combine(Application.StartupPath, String.Format(@"motion\{0}\{1}.tmo", save_id, id));
        }
        
        public string PoseFile
        {
            get { return String.Format(@"tmo-{0}-{1:D3}.tdcgpose.png", save_id, id); }
        }

        public string GetPngPath()
        {
            return Path.Combine(PoseRoot, PoseFile);
        }

        public void LoadPoseFile(string pose_file)
        {
            if (!string.IsNullOrEmpty(pose_file))
            {
                Console.WriteLine("Load File: " + pose_file);
                Tmo = TMOAnim.LoadPNGFile(Path.Combine(PoseRoot, pose_file));
                Tmo.LoadTransformationMatrix(0);
            }
            Tmo.TruncateFrame(0); // forced pose
        }

        public void CopyFaceFile(string face_file)
        {
            if (Tmo.frames == null)
                return;

            List<string> except_snames = new List<string>();
            except_snames.Add("Kami_Oya");

            if (!string.IsNullOrEmpty(face_file))
            {
                Console.WriteLine("Load File: " + face_file);
                TMOFile face_tmo = TMOAnim.LoadPNGFile(Path.Combine(FaceRoot, face_file));
                if (face_tmo.frames != null)
                {
                    Tmo.SaveTransformationMatrix(0);
                    Tmo.CopyChildrenNodeFrom(face_tmo, "face_oya", except_snames);
                    Tmo.LoadTransformationMatrix(0);
                }
            }
        }

        public TMOAnimItem Dup()
        {
            TMOAnimItem item = new TMOAnimItem();
            item.Length = Length;
            item.Accel = Accel;
            if (Tmo != null)
            {
                item.Tmo = Tmo.Dup();
                item.Tmo.LoadTransformationMatrix(0);
            }

            return item;
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

        int save_id;

        public void UpdateID(int save_id)
        {
            this.save_id = save_id;

            for (int i = 0; i < items.Count; i++)
                items[i].UpdateID(save_id, i);
        }

        public static string FaceRoot { get; set; }

        public static string GetFacePath(string face_file)
        {
            return Path.Combine(FaceRoot, face_file);
        }

        public void LoadSource()
        {
            source = CreateTmo(SourceItem);
        }

        public string GetTmoPath()
        {
            return String.Format(@"out-{0:D3}.tmo", save_id);
        }

        public void SaveSourceToFile()
        {
            if (source.frames != null)
            {
                string tmo_path = GetTmoPath();
                Console.WriteLine("Save File: " + tmo_path);
                source.Save(tmo_path);
            }
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

            string png_file = item.GetPngPath();
            string tmo_file = item.GetTmoPath();
            if (File.Exists(png_file))
            {
                Console.WriteLine("Load File: " + png_file);
                tmo = LoadPNGFile(png_file);
            }
            else if (File.Exists(tmo_file))
            {
                Console.WriteLine("Load File: " + tmo_file);
                tmo.Load(tmo_file);
            }
            tmo.TruncateFrame(0); // forced pose

            return tmo;
        }
        
        public void SavePoseToFile()
        {
            foreach (TMOAnimItem item in items)
            {
                TMOFile tmo = GetTmo(item);

                if (tmo.frames != null)
                {
                    /*
                    string tmo_file = item.GetTmoPath();
                    Console.WriteLine("Save File: " + tmo_file);
                    Directory.CreateDirectory(Path.GetDirectoryName(tmo_file));
                    tmo.Save(tmo_file);
                    */

                    string png_file = item.GetPngPath();
                    Console.WriteLine("Save File: " + png_file);
                    PNGFile png = CreatePNGFile(item);
                    png.WriteTaOb += delegate(BinaryWriter bw)
                    {
                        PNGWriter pw = new PNGWriter(bw);
                        WritePose(pw, tmo);
                    };
                    png.Save(png_file);
                }
            }
        }

        public PNGFile CreatePNGFile(TMOAnimItem item)
        {
            MemoryStream ms = new MemoryStream();
            using (Bitmap bmp = new Bitmap(180, 180, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                Graphics g = Graphics.FromImage(bmp);
                Brush brush = new SolidBrush(Color.FromArgb(0xfb, 0xc6, 0xc6));
                g.FillRectangle(brush, 0, 0, 180, 180);
                Font font = new Font(FontFamily.GenericSerif, 36, FontStyle.Bold);
                g.DrawString(string.Format("{0:D1}-{1:D3}", item.SaveID, item.ID), font, Brushes.Black, 0, 0);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            }
            ms.Seek(0, SeekOrigin.Begin);

            PNGFile png = new PNGFile();
            png.Load(ms);

            return png;
        }

        protected byte[] ReadFloats(string dest_file)
        {
            List<float> floats = new List<float>();
            string line;
            using (StreamReader source = new StreamReader(File.OpenRead(dest_file)))
            while ((line = source.ReadLine()) != null)
            {
                floats.Add(Single.Parse(line));
            }

            byte[] data = new byte[ sizeof(Single) * floats.Count ];
            int offset = 0;
            foreach (float flo in floats)
            {
                byte[] buf_flo = BitConverter.GetBytes(flo);
                buf_flo.CopyTo(data, offset);
                offset += buf_flo.Length;
            }
            return data;
        }

        string GetCameraPath()
        {
            return Path.Combine(Application.StartupPath, "Camera.txt");
        }

        string GetLightAPath()
        {
            return Path.Combine(Application.StartupPath, "LightA.txt");
        }

        void WritePose(PNGWriter pw, TMOFile tmo)
        {
            byte[] cami = ReadFloats(GetCameraPath());
            byte[] lgta = ReadFloats(GetLightAPath());

            pw.WriteTDCG();
            pw.WritePOSE();
            pw.WriteCAMI(cami);
            pw.WriteLGTA(lgta);
            pw.WriteFTMO(tmo);
        }

        public void Process()
        {
            foreach (TMOAnimItem item in items)
            {
                TMOFile tmo = GetTmo(item);

                if (tmo.frames == null)
                    continue;

                source.SlerpFrameEndTo(tmo, item.Length, item.Accel);
            }
        }
    }
}
