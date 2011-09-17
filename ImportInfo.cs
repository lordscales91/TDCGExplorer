using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Tso2MqoGui
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

            foreach(ImportEffectInfo   i in effects)   i.PostLoad(this);
            foreach(ImportTextureInfo  i in textures)  i.PostLoad(this);
            foreach(ImportMaterialInfo i in materials) i.PostLoad(this);
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

    public class ImportInfoItem
    {
        [XmlIgnore] protected ImportInfo    Owner;

        public virtual void PostLoad(ImportInfo owner)
        {
            this.Owner  = owner;
        }
    }

    public class ImportEffectInfo : ImportInfoItem
    {
        [XmlAttribute] public string   Name;

        public ImportEffectInfo()
        {
        }

        public ImportEffectInfo(TSOEffect eff)
        {
            Name            = eff.Name;
        }

        public override string ToString()
        {
            return "Name:" + Name;
        }
    }

    public class ImportTextureInfo : ImportInfoItem
    {
        [XmlAttribute] public string   Name;
        [XmlAttribute] public string   File;
        [XmlAttribute] public int      BytesPerPixel;
        [XmlAttribute] public int      Width;
        [XmlAttribute] public int      Height;

        public ImportTextureInfo()
        {
        }

        public ImportTextureInfo(TSOTex tex)
        {
            Name            = tex.Name;
            File            = tex.File.Trim('"');
            BytesPerPixel   = tex.Depth;
            Width           = tex.Width;
            Height          = tex.Height;
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

    public class ImportMaterialInfo : ImportInfoItem
    {
        [XmlAttribute] public string   Name;
        [XmlAttribute] public string   File;
        [XmlIgnore]    public Dictionary<string, string>    parameters;
        [XmlIgnore]    public ImportTextureInfo             color;
        [XmlIgnore]    public ImportTextureInfo             shadow;

        public ImportMaterialInfo()
        {
        }

        public ImportMaterialInfo(TSOMaterial mtl)
        {
            Name            = mtl.Name;
            File            = mtl.File.Trim('"');
        }

        public override void PostLoad(ImportInfo owner)
        {
            base.PostLoad(owner);

            string  dir     = Path.GetDirectoryName(owner.filename);
            string  codefile= Path.Combine(dir, Name);

            if(System.IO.File.Exists(codefile))
            {
                TSOMaterialCode code= TSOMaterialCode.GenerateFromFile(codefile);
                TSOParameter    p;

                if(code.TryGetValue("ColorTex", out p))
                    color   = owner.GetTexture(p.Value);

                if(code.TryGetValue("ShadeTex", out p))
                    shadow  = owner.GetTexture(p.Value);
            }
        }

        public override string ToString()
        {
            return "Name:"          + Name
                +", File:"          + File
                ;
        }
    }
}
