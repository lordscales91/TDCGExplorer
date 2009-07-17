using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public partial class EditSystemDatabase : Form
    {
        public EditSystemDatabase()
        {
            InitializeComponent();
        }

        public string textArcPath
        {
            get { return tbArcsDirectory.Text; }
            set { tbArcsDirectory.Text = value; }
        }

        public string textZipPath
        {
            get { return tbZipDirectory.Text; }
            set { tbZipDirectory.Text = value; }
        }

        public string textModDbUrl
        {
            get { return tbModRefServer.Text; }
            set { tbModRefServer.Text = value; }
        }

        public string textZipRegexp
        {
            get { return tbZipRegexp.Text; }
            set { tbZipRegexp.Text = value; }
        }

        public string textArcnamesServer
        {
            get { return tbArcnamesServer.Text; }
            set { tbArcnamesServer.Text = value; }
        }

        public string textWorkPath
        {
            get { return tbWorkPath.Text; }
            set { tbWorkPath.Text = value; }
        }

        public bool lookupmodref
        {
            get { return checkBoxLookupModRef.Checked; }
            set { checkBoxLookupModRef.Checked = value; }
        }

        public string textModRegexp
        {
            get { return textBoxModRegexp.Text; }
            set { textBoxModRegexp.Text = value; }
        }

        public string textTagnamesServer
        {
            get { return textBoxTagNameServer.Text; }
            set { textBoxTagNameServer.Text = value; }
        }

        public string uiBehavior
        {
            get
            {
                if (radioButtonBehaviorServer.Checked) return "server";
                if (radioButtonBehaviorImage.Checked) return "image";
                if (radioButtonBehaviorText.Checked) return "text";
                return "none";
            }
            set
            {
                switch(value)
                {
                    case "server":
                        radioButtonBehaviorServer.Checked=true;
                        break;
                    case "image":
                        radioButtonBehaviorImage.Checked=true;
                        break;
                    case "text":
                        radioButtonBehaviorText.Checked=true;
                        break;
                    case "none":
                        radioButtonBehaviorNone.Checked = true;
                        break;
                }
            }
        }
        public string saveDirectory
        {
            get { return textBoxSaveFile.Text; }
            set { textBoxSaveFile.Text = value; }
        }
        public bool initializeCamera
        {
            get { return checkBoxCameraReset.Checked; }
            set { checkBoxCameraReset.Checked = value; }
        }
        public string centerBone
        {
            get { return textBoxCenterBone.Text; }
            set { textBoxCenterBone.Text = value; }
        }
        public string translateBone
        {
            get { return textBoxTranslateBone.Text; }
            set { textBoxTranslateBone.Text = value; }
        }
        public string tahEditorPath
        {
            get { return textBoxTahEditor.Text; }
            set { textBoxTahEditor.Text = value; }
        }
        public string collisionDetectLevel
        {
            get
            {
                if (checkBoxDuplicate.Checked) return "duplicate";
                else return "collision";
            }
            set
            {
                if (value == "duplicate") checkBoxDuplicate.Checked = true;
                else checkBoxDuplicate.Checked = false;
            }
        }
        public bool findziplevel
        {
            get { return checkBoxFindZipLevel.Checked; }
            set { checkBoxFindZipLevel.Checked = value; }
        }
        public bool delete_tahcache
        {
            get { return checkBoxDeleteTahCache.Checked; }
            set { checkBoxDeleteTahCache.Checked = value; }
        }
    }
}
