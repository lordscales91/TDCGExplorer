using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace tso2mqo
{
    [XmlRoot("TsoMqoConfig")]
    [Serializable]
    public class Config
    {
        public struct KeyAndValue
        {
            [XmlAttribute("key")]   public string   Key;
            [XmlAttribute("value")] public string   Value;

            public KeyAndValue(string k, string v)
            {
                Key     = k;
                Value   = v;
            }
        }

        public static Config    Instance;

        public static string    AssemblyPath    { get { return Path.GetDirectoryName(Type.Assembly.Location); } }
        public static string    ConfigFile      { get { return Path.Combine(AssemblyPath, "config.xml"); } }
        public static Type      Type            { get { return typeof(Config); } }

        [XmlElement("object_bone_list")] public List<KeyAndValue>   object_bone_list    = new List<KeyAndValue>();
        [XmlIgnore] public Dictionary<string, string>               object_bone_map     = new Dictionary<string,string>();

        static Config()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                using(FileStream fs= File.OpenRead(ConfigFile))
                {
                    XmlSerializer   s   = new XmlSerializer(Type);
                    Instance            = s.Deserialize(fs) as Config;
                    Instance.AfterDeserialize();
                }
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                Instance                = new Config();
            }
        }

        public static void Save()
        {
            try
            {
                using(FileStream fs= File.OpenWrite(ConfigFile))
                {
                    fs.SetLength(0);
                    XmlSerializer   s   = new XmlSerializer(Type);
                    Instance.BeforeSerialize();
                    s.Serialize(fs, Instance);
                    fs.Flush();
                }
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        public void BeforeSerialize()
        {
            object_bone_list.Clear();

            foreach(string i in object_bone_map.Keys)
                object_bone_list.Add(new KeyAndValue(i, object_bone_map[i]));
        }

        public void AfterDeserialize()
        {
            object_bone_map.Clear();

            foreach(KeyAndValue i in object_bone_list)
                object_bone_map.Add(i.Key, i.Value);
        }
    }
}
