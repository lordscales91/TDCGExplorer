using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using TDCG;

namespace TMOKinect
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
                Tmo.LoadTransformationMatrixFromFrame(0);
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
                    Tmo.SaveTransformationMatrixToFrame(0);
                    Tmo.CopyChildrenNodeFrom(face_tmo, "face_oya", except_snames);
                    Tmo.LoadTransformationMatrixFromFrame(0);
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
                item.Tmo.LoadTransformationMatrixFromFrame(0);
            }

            return item;
        }
    }
}
