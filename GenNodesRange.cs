using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TDCG;

public static class GenNodesRange
{
    public static void Main()
    {
        NodesRange range = new NodesRange();

        range.root_names.Clear();
        range.root_names.Add("mayu_1_R");
        range.root_names.Add("mayu_1_L");
        range.Save(@"Debug\表情\まゆ\NodesRange.xml");

        range.root_names.Clear();
        range.root_names.Add("sitakuti_oya");
        range.root_names.Add("sita_01");
        range.root_names.Add("kutiyoko_r");
        range.root_names.Add("kutiyoko_l");
        range.root_names.Add("uekuti_oya");
        range.Save(@"Debug\表情\リップ\NodesRange.xml");

        range.root_names.Clear();
        range.root_names.Add("eyeline_sita_R");
        range.root_names.Add("eyeline_sita_L");
        range.root_names.Add("L_eyeline_oya_L");
        range.root_names.Add("R_eyeline_oya_R");
        range.Save(@"Debug\表情\目\NodesRange.xml");

        range.root_names.Clear();
        range.root_names.Add("sitakuti_oya");
        range.root_names.Add("sita_01");
        range.root_names.Add("kutiyoko_r");
        range.root_names.Add("kutiyoko_l");
        range.root_names.Add("uekuti_oya");

        range.root_names.Add("eyeline_sita_R");
        range.root_names.Add("eyeline_sita_L");
        range.root_names.Add("L_eyeline_oya_L");
        range.root_names.Add("R_eyeline_oya_R");

        range.root_names.Add("mayu_1_R");
        range.root_names.Add("mayu_1_L");
        range.Save(@"Debug\表情\その他\NodesRange.xml");
    }
}
