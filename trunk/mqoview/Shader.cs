using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace mqoview
{
    /// <summary>
    /// シェーダ設定パラメータ
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderParameter
    {
        /// <summary>
        /// シェーダ設定の型名
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// わからない
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
            /// テクスチャ
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

        internal bool system_p = false;

        /// <summary>
        /// パラメータの名称
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// float値1
        /// </summary>
        public float F1 { get { return f1; } set { f1 = value; } }
        /// <summary>
        /// float値2
        /// </summary>
        public float F2 { get { return f2; } set { f2 = value; } }
        /// <summary>
        /// float値3
        /// </summary>
        public float F3 { get { return f3; } set { f3 = value; } }
        /// <summary>
        /// float値4
        /// </summary>
        public float F4 { get { return f4; } set { f4 = value; } }
        /// <summary>
        /// float次元数
        /// </summary>
        public int Dimension { get { return dim; } }

        /// <summary>
        /// シェーダ設定ファイルの行を解析してシェーダ設定パラメータを生成します。
        /// </summary>
        /// <param name="line">シェーダ設定ファイルの行</param>
        /// <returns>シェーダ設定パラメータ</returns>
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
        /// シェーダ設定パラメータを生成します。
        /// </summary>
        public ShaderParameter()
        {
        }

        /// <summary>
        /// シェーダ設定パラメータを生成します。
        /// </summary>
        /// <param name="type_string">型名</param>
        /// <param name="name">名称</param>
        /// <param name="value">値</param>
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

        /// 文字列として表現します。
        public override string ToString()
        {
            return GetTypeName() + " " + name + " = " + GetValueString();
        }

        /// 型名を文字列として得ます。
        public string GetTypeName()
        {
            switch (type)
            {
                case Type.String:
                    return "string";
                case Type.Float:
                    return "float";
                case Type.Float3:
                    return "float3";
                case Type.Float4:
                    return "float4";
                case Type.Texture:
                    return "texture";
            }
            return null;
        }

        /// <summary>
        /// 値を文字列として得ます。
        /// </summary>
        public string GetValueString()
        {
            switch (type)
            {
                case Type.String:
                    return "\"" + str + "\"";
                case Type.Float:
                    return string.Format("[{0}]", f1);
                case Type.Float3:
                    return string.Format("[{0}, {1}, {2}]", f1, f2, f3);
                case Type.Float4:
                    return string.Format("[{0}, {1}, {2}, {3}]", f1, f2, f3, f4);
                case Type.Texture:
                    return str;
            }
            return str;
        }

        /// <summary>
        /// 文字列を取得します。
        /// </summary>
        /// <returns>文字列</returns>
        public string GetString()
        {
            return str;
        }

        /// <summary>
        /// 文字列を設定します。
        /// </summary>
        /// <param name="value">文字列表現</param>
        public void SetString(string value)
        {
            str = value.Trim('"', ' ', '\t');
        }

        static Regex re_float_array = new Regex(@"\s*,\s*|\s+");

        /// <summary>
        /// float値の配列を設定します。
        /// </summary>
        /// <param name="value">float配列値の文字列表現</param>
        /// <param name="dim">次元数</param>
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
        /// float値を取得します。
        /// </summary>
        /// <returns>float値</returns>
        public float GetFloat()
        {
            return f1;
        }
        /// <summary>
        /// float値を設定します。
        /// </summary>
        /// <param name="value">float値の文字列表現</param>
        public void SetFloat(string value)
        {
            try
            {
                SetFloatDim(value, 1);
            }
            catch (FormatException)
            {
                Console.WriteLine("shader format error (type float): " + value);
            }
        }

        /// <summary>
        /// float3値を取得します。
        /// </summary>
        /// <returns>float3値</returns>
        public Vector3 GetFloat3()
        {
            return new Vector3(f1, f2, f3);
        }
        /// <summary>
        /// float3値を設定します。
        /// </summary>
        /// <param name="value">float3値の文字列表現</param>
        public void SetFloat3(string value)
        {
            try
            {
                SetFloatDim(value, 3);
            }
            catch (FormatException)
            {
                Console.WriteLine("shader format error (type float3): " + value);
            }
        }

        /// <summary>
        /// float4値を取得します。
        /// </summary>
        /// <returns>float4値</returns>
        public Vector4 GetFloat4()
        {
            return new Vector4(f1, f2, f3, f4);
        }
        /// <summary>
        /// float4値を設定します。
        /// </summary>
        /// <param name="value">float4値の文字列表現</param>
        public void SetFloat4(string value)
        {
            try
            {
                SetFloatDim(value, 4);
            }
            catch (FormatException)
            {
                Console.WriteLine("shader format error (type float4): " + value);
            }
        }

        /// <summary>
        /// テクスチャ名を取得します。
        /// </summary>
        /// <returns>テクスチャ名</returns>
        public string GetTexture()
        {
            return str;
        }
        /// <summary>
        /// テクスチャ名を設定します。
        /// </summary>
        /// <param name="value">テクスチャ名</param>
        public void SetTexture(string value)
        {
            str = value;
        }
    }

    /// <summary>
    /// シェーダ設定
    /// </summary>
    public class Shader
    {
        /// <summary>
        /// シェーダ設定パラメータの配列
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
        internal ShaderParameter shadeTex;        // = Ninjya_Ribbon_Toon_Tex
        //internal float      highLight;       // = [0]
        //internal float      colorBlend;      // = [10]
        //internal float      highLightBlend;  // = [10]
        //internal Vector4    penColor;        // = [0.166, 0.166, 0.166, 1]
        //internal float      ambient;         // = [38]
        internal ShaderParameter colorTex;        // = file24
        //internal float      thickness;       // = [0.018]
        //internal float      shadeBlend;      // = [10]
        //internal float      highLightPower;  // = [100]

        /// <summary>
        /// 光源方向ベクトル
        /// </summary>
        public Vector4 LightDir
        {
            get { return new Vector4(lightDirX, lightDirY, lightDirZ, lightDirW); }
        }

        /// <summary>
        /// 陰テクスチャのファイル名
        /// </summary>
        public string ShadeTexName
        {
            get
            {
                return shadeTex.GetTexture();
            }
            set
            {
                shadeTex.SetTexture(value);
            }
        }
        /// <summary>
        /// 色テクスチャのファイル名
        /// </summary>
        public string ColorTexName
        {
            get
            {
                return colorTex.GetTexture();
            }
            set
            {
                colorTex.SetTexture(value);
            }
        }

        /// <summary>
        /// シェーダ設定を読み込みます。
        /// </summary>
        /// <param name="lines">テキスト行配列</param>
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
                        p.system_p = true;
                        break;
                    case "shader":
                        p.system_p = true;
                        break;
                    case "technique":
                        p.system_p = true;
                        technique = p.GetString();
                        break;
                    case "LightDirX":
                        p.system_p = true;
                        lightDirX = p.GetFloat();
                        break;
                    case "LightDirY":
                        p.system_p = true;
                        lightDirY = p.GetFloat();
                        break;
                    case "LightDirZ":
                        p.system_p = true;
                        lightDirZ = p.GetFloat();
                        break;
                    case "LightDirW":
                        p.system_p = true;
                        lightDirW = p.GetFloat();
                        break;
                    case "LightDir":
                        p.system_p = true;
                    {
                        Vector3 v = p.GetFloat3();
                        lightDirX = v.X;
                        lightDirY = v.Y;
                        lightDirZ = v.Z;
                    }
                        break;
                    case "ShadeTex":
                        p.system_p = true;
                        shadeTex = p;
                        break;
                    case "ColorTex":
                        p.system_p = true;
                        colorTex = p;
                        break;
                }
                shader_parameters[i++] = p;
            }
            Array.Resize(ref shader_parameters, i);
        }

        /// <summary>
        /// シェーダ設定を文字列の配列として得ます。
        /// </summary>
        public string[] GetLines()
        {
            string[] lines = new string[shader_parameters.Length];
            int i = 0;
            foreach (ShaderParameter p in shader_parameters)
            {
                lines[i++] = p.ToString();
            }
            Array.Resize(ref lines, i);
            return lines;
        }
    }
}
