using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace tso2mqo
{
    public class RDBJoint
    {
        public int      No;
        public string   Name;

        public RDBJoint(int no, string name)
        {
            No  = no;
            Name= name;
        }

        public override string ToString()
        {
            return No.ToString().PadLeft(4, '0') + ":" + Name;
        }
    }

    public class RDBBone
    {
        public int      Begin;
        public int      End;

        public RDBBone(int begin, int end)
        {
            Begin   = begin;
            End     = end;
        }

        public override string ToString()
        {
            return "0003," + Begin.ToString().PadLeft(4, '0')
                     + "," + End  .ToString().PadLeft(4, '0');
        }
    }

    public class RDBBonFile
    {
        public int                          no      = 0;
        public List<RDBJoint>               joints  = new List<RDBJoint>();
        public Dictionary<string, RDBJoint> jointmap= new Dictionary<string, RDBJoint>();
        public List<RDBBone>                bones   = new List<RDBBone>();

        public void AddJoint(string name)
        {
            RDBJoint    j   = new RDBJoint(no++, name);
            joints.Add(j);
            jointmap.Add(name, j);
        }

        public void AddBone(int begin, int end)
        {
            bones.Add(new RDBBone(begin, end));
        }

        public void Save(string file)
        {
            StringBuilder   sb  = new StringBuilder();

            sb.AppendLine("BoneFile : type separated : ver1001")
              .AppendLine("")
              .AppendLine("NAMEPART_START");

            foreach(RDBJoint i in joints)
                sb.AppendLine(i.ToString());

            sb.AppendLine("NAMEPART_END")
              .AppendLine("")
              .AppendLine("")
              .AppendLine("TREEPART_START")
              .AppendLine("");

            foreach(RDBBone i in bones)
                sb.AppendLine(i.ToString());
            
            sb.AppendLine("TREEPART_END");

            File.WriteAllText(file, sb.ToString(), Encoding.Default);
        }
    }
}
