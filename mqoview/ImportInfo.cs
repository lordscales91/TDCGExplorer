using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace mqoview
{
    [XmlRoot("TsoImportInfo")]
    public class ImportInfo
    {
        [XmlIgnore] public string filename;
        [XmlElement("Effect")]    public List<ImportEffectInfo>     effects     = new List<ImportEffectInfo>();
        [XmlElement("Textures")]  public List<ImportTextureInfo>    textures    = new List<ImportTextureInfo>();
        [XmlElement("Materials")] public List<ImportMaterialInfo>   materials   = new List<ImportMaterialInfo>();
        
        private Dictionary<string, ImportEffectInfo>    effectmap;
        private Dictionary<string, ImportTextureInfo>   texturemap;
        private Dictionary<string, ImportMaterialInfo>  materialmap;

        public static Type      Type            { get { return typeof(ImportInfo); } }

        public void PostLoad()
        {
            effectmap   = new Dictionary<string, ImportEffectInfo>();
            texturemap  = new Dictionary<string, ImportTextureInfo>();
            materialmap = new Dictionary<string, ImportMaterialInfo>();

            foreach(ImportEffectInfo   i in effects)   effectmap  .Add(i.Name, i);
            foreach(ImportTextureInfo  i in textures)  texturemap .Add(i.Name, i);
            foreach(ImportMaterialInfo i in materials) materialmap.Add(i.Name, i);

        }

        public ImportEffectInfo GetEffect(string name)
        {
            ImportEffectInfo    info= null;
            effectmap.TryGetValue(name, out info);
            return info;
        }

        public ImportTextureInfo GetTexture(string name)
        {
            ImportTextureInfo   info= null;
            texturemap.TryGetValue(name, out info);
            return info;
        }

        public ImportMaterialInfo GetMaterial(string name)
        {
            ImportMaterialInfo  info= null;
            materialmap.TryGetValue(name, out info);
            return info;
        }

        public static ImportInfo Load(string file)
        {
            using(FileStream fs= File.OpenRead(file))
            {
                ImportInfo  ii  = new XmlSerializer(Type).Deserialize(fs) as ImportInfo;
                ii.filename     = file;
                ii.PostLoad();
                return ii;
            }
        }

        public static void Save(string file, ImportInfo ii)
        {
            using(FileStream fs= File.OpenWrite(file))
            {
                fs.SetLength(0);
                new XmlSerializer(Type).Serialize(fs, ii);
            }
        }
    }

    public class ImportEffectInfo
    {
        [XmlAttribute] public string   Name;

        public ImportEffectInfo()
        {
        }

        public override string ToString()
        {
            return "Name:" + Name;
        }
    }

    public class ImportTextureInfo
    {
        [XmlAttribute] public string   Name;
        [XmlAttribute] public string   File;
        [XmlAttribute] public int      BytesPerPixel;
        [XmlAttribute] public int      Width;
        [XmlAttribute] public int      Height;

        public ImportTextureInfo()
        {
        }

        public override string ToString()
        {
            return "Name:"          + Name
                +", File:"          + File
                +", BytesPerPixel:" + BytesPerPixel
                +", Width:"         + Width
                +", Height:"        + Height
                ;
        }
    }

    public class ImportMaterialInfo
    {
        [XmlAttribute] public string   Name;
        [XmlAttribute] public string   File;

        public ImportMaterialInfo()
        {
        }

        public override string ToString()
        {
            return "Name:"          + Name
                +", File:"          + File
                ;
        }
    }
}
