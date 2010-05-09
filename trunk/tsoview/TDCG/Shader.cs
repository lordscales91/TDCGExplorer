using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// �V�F�[�_�ݒ�p�����[�^
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderParameter
    {
        /// <summary>
        /// �V�F�[�_�ݒ�̌^��
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// �킩��Ȃ�
            /// </summary>
            Unknown,
            /// <summary>
            /// string
            /// </summary>
            String,
            /// <summary>
            /// float
            /// </summary>
            Float,
            /// <summary>
            /// float3
            /// </summary>
            Float3,
            /// <summary>
            /// float4
            /// </summary>
            Float4,
            /// <summary>
            /// �e�N�X�`��
            /// </summary>
            Texture
        };

        internal Type type;
        internal string name;

        private string str;
        private float f1;
        private float f2;
        private float f3;
        private float f4;
        private int dim = 0;

        /// <summary>
        /// �p�����[�^�̖���
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// float�l1
        /// </summary>
        public float F1 { get { return f1; } set { f1 = value; } }
        /// <summary>
        /// float�l2
        /// </summary>
        public float F2 { get { return f2; } set { f2 = value; } }
        /// <summary>
        /// float�l3
        /// </summary>
        public float F3 { get { return f3; } set { f3 = value; } }
        /// <summary>
        /// float�l4
        /// </summary>
        public float F4 { get { return f4; } set { f4 = value; } }
        /// <summary>
        /// float������
        /// </summary>
        public int Dimension { get { return dim; } }

        /// <summary>
        /// �V�F�[�_�ݒ�t�@�C���̍s����͂��ăV�F�[�_�ݒ�p�����[�^�𐶐����܂��B
        /// </summary>
        /// <param name="line">�V�F�[�_�ݒ�t�@�C���̍s</param>
        /// <returns>�V�F�[�_�ݒ�p�����[�^</returns>
        public static ShaderParameter Parse(string line)
        {
            int m = line.IndexOf('='); if (m < 0) throw new ArgumentException();
            string type_name = line.Substring(0,m);
            string value = line.Substring(m+1).Trim();
            m = type_name.IndexOf(' '); if (m < 0) throw new ArgumentException();
            string type = type_name.Substring(0,m);
            string name = type_name.Substring(m+1).Trim();

            return new ShaderParameter(type, name, value);
        }

        /// <summary>
        /// �V�F�[�_�ݒ�p�����[�^�𐶐����܂��B
        /// </summary>
        public ShaderParameter()
        {
        }

        /// <summary>
        /// �V�F�[�_�ݒ�p�����[�^�𐶐����܂��B
        /// </summary>
        /// <param name="type_string">�^��</param>
        /// <param name="name">����</param>
        /// <param name="value">�l</param>
        public ShaderParameter(string type_string, string name, string value)
        {
            this.name = name;

            switch (type_string)
            {
            case "string":
                type = Type.String;
                SetString(value);
                break;
            case "float":
                type = Type.Float;
                SetFloat(value);
                break;
            case "float3":
                type = Type.Float3;
                SetFloat3(value);
                break;
            case "float4":
                type = Type.Float4;
                SetFloat4(value);
                break;
            case "texture":
                type = Type.Texture;
                SetTexture(value);
                break;
            default:
                type = Type.Unknown;
                break;
            }
        }

        /// <summary>
        /// ��������擾���܂��B
        /// </summary>
        /// <returns>������</returns>
        public string GetString()
        {
            return str;
        }
        /// <summary>
        /// �������ݒ肵�܂��B
        /// </summary>
        /// <param name="value">������\��</param>
        public void SetString(string value)
        {
            str = value.Trim('"', ' ', '\t');
        }

        static Regex re_float_array = new Regex(@"\s+|\s*,\s*");

        /// <summary>
        /// float�l�̔z���ݒ肵�܂��B
        /// </summary>
        /// <param name="value">float�z��l�̕�����\��</param>
        /// <param name="dim">������</param>
        public void SetFloatDim(string value, int dim)
        {
            string[] token = re_float_array.Split(value.Trim('[', ']', ' ', '\t'));
            this.dim = dim;
            if (token.Length > 0)
                f1 = float.Parse(token[0].Trim());
            if (token.Length > 1)
                f2 = float.Parse(token[1].Trim());
            if (token.Length > 2)
                f3 = float.Parse(token[2].Trim());
            if (token.Length > 3)
                f4 = float.Parse(token[3].Trim());
        }

        /// <summary>
        /// float�l���擾���܂��B
        /// </summary>
        /// <returns>float�l</returns>
        public float GetFloat()
        {
            return f1;
        }
        /// <summary>
        /// float�l��ݒ肵�܂��B
        /// </summary>
        /// <param name="value">float�l�̕�����\��</param>
        public void SetFloat(string value)
        {
            SetFloatDim(value, 1);
        }

        /// <summary>
        /// float3�l���擾���܂��B
        /// </summary>
        /// <returns>float3�l</returns>
        public Vector3 GetFloat3()
        {
            return new Vector3(f1, f2, f3);
        }
        /// <summary>
        /// float3�l��ݒ肵�܂��B
        /// </summary>
        /// <param name="value">float3�l�̕�����\��</param>
        public void SetFloat3(string value)
        {
            SetFloatDim(value, 3);
        }

        /// <summary>
        /// float4�l���擾���܂��B
        /// </summary>
        /// <returns>float4�l</returns>
        public Vector4 GetFloat4()
        {
            return new Vector4(f1, f2, f3, f4);
        }
        /// <summary>
        /// float4�l��ݒ肵�܂��B
        /// </summary>
        /// <param name="value">float4�l�̕�����\��</param>
        public void SetFloat4(string value)
        {
            SetFloatDim(value, 4);
        }

        /// <summary>
        /// �e�N�X�`�������擾���܂��B
        /// </summary>
        /// <returns>�e�N�X�`����</returns>
        public string GetTexture()
        {
            return str;
        }
        /// <summary>
        /// �e�N�X�`������ݒ肵�܂��B
        /// </summary>
        /// <param name="value">�e�N�X�`����</param>
        public void SetTexture(string value)
        {
            str = value;
        }
    }

    /// <summary>
    /// �V�F�[�_�ݒ�
    /// </summary>
    public class Shader
    {
        /// <summary>
        /// �V�F�[�_�ݒ�p�����[�^�̔z��
        /// </summary>
        public ShaderParameter[] shader_parameters;

        //internal string     description;     // = "TA ToonShader v0.50"
        //internal string     shader;          // = "TAToonshade_050.cgfx"
        internal string     technique;       // = "ShadowOn"
        internal float      lightDirX;       // = [-0.00155681]
        internal float      lightDirY;       // = [-0.0582338]
        internal float      lightDirZ;       // = [-0.998302]
        internal float      lightDirW;       // = [0]
        //internal Vector4    shadowColor;     // = [0, 0, 0, 1]
        internal string     shadeTex;        // = Ninjya_Ribbon_Toon_Tex
        //internal float      highLight;       // = [0]
        //internal float      colorBlend;      // = [10]
        //internal float      highLightBlend;  // = [10]
        //internal Vector4    penColor;        // = [0.166, 0.166, 0.166, 1]
        //internal float      ambient;         // = [38]
        internal string     colorTex;        // = file24
        //internal float      thickness;       // = [0.018]
        //internal float      shadeBlend;      // = [10]
        //internal float      highLightPower;  // = [100]

        /// <summary>
        /// ���������x�N�g��
        /// </summary>
        public Vector4 LightDir
        {
            get { return new Vector4(lightDirX, lightDirY, lightDirZ, lightDirW); }
        }

        /// <summary>
        /// �A�e�N�X�`���̃t�@�C����
        /// </summary>
        public string ShadeTexName { get { return shadeTex; } }
        /// <summary>
        /// �F�e�N�X�`���̃t�@�C����
        /// </summary>
        public string ColorTexName { get { return colorTex; } }

        /// <summary>
        /// �V�F�[�_�ݒ��ǂݍ��݂܂��B
        /// </summary>
        /// <param name="lines">�e�L�X�g�s�z��</param>
        public void Load(string[] lines)
        {
            shader_parameters = new ShaderParameter[lines.Length];
            int i = 0;
            foreach (string line in lines)
            {
                ShaderParameter p = ShaderParameter.Parse(line);
                switch(p.name)
                {
                    case "description":
                        break;
                    case "shader":
                        break;
                    case "technique":
                        technique = p.GetString();
                        break;
                    case "LightDirX":
                        lightDirX = p.GetFloat();
                        break;
                    case "LightDirY":
                        lightDirY = p.GetFloat();
                        break;
                    case "LightDirZ":
                        lightDirZ = p.GetFloat();
                        break;
                    case "LightDirW":
                        lightDirW = p.GetFloat();
                        break;
                    case "LightDir":
                    {
                        Vector3 v = p.GetFloat3();
                        lightDirX = v.X;
                        lightDirY = v.Y;
                        lightDirZ = v.Z;
                    }
                        break;
                    case "ShadeTex":
                        shadeTex = p.GetTexture();
                        break;
                    case "ColorTex":
                        colorTex = p.GetTexture();
                        break;
                    default:
                        shader_parameters[i++] = p;
                        break;
                }

            }
            Array.Resize(ref shader_parameters, i);
        }
    }
}
