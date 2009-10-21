float4x4 g_mWorld : World;
float4x4 g_mWorldIT : WorldInverseTranspose;
float4 g_vEyePos : EyePos;
float4x4 g_mWorldViewProj : WorldViewProj;

const float4 g_vLightPos = float4(1.0f, 2.0f, 0.0f, 1.0f);
const float4 g_vLightColor = float4(0.8f, 0.8f, 0.8f, 1.0f);
const float g_fSpecExpon = 0.7f;

float4 g_vAmbiColor =float4(0.3f, 0.3f, 0.3f, 1.0f);
float4 g_vSurfColor =float4(0.5f, 0.5f, 0.5f, 1.0f);

struct VS_INPUT
{
  float4 Position : POSITION;
  float3 Normal : NORMAL;
};

struct PHONG_VS_OUTPUT
{
  float4 vPos : POSITION;
  float3 Normal : TEXCOORD0;
  float3 ViewVec : TEXCOORD1;
  float3 LightVec : TEXCOORD2;
};

struct PLAIN_VS_OUTPUT
{
  float4 vPos : POSITION;
};

PHONG_VS_OUTPUT PhongVS(VS_INPUT In)
{
  PHONG_VS_OUTPUT Out = (PHONG_VS_OUTPUT)0;

  float3 Pw = mul(In.Position, g_mWorld);

  Out.vPos = mul(In.Position, g_mWorldViewProj);
  Out.ViewVec = normalize(g_vEyePos - Pw);
  Out.LightVec = normalize(g_vLightPos - Pw);
  Out.Normal = mul(In.Normal, g_mWorldIT);
  Out.Normal = normalize(Out.Normal);
  
  return Out;
}

float4 PhongPS(PHONG_VS_OUTPUT In) : COLOR0
{
  float3 Hn = normalize(In.ViewVec + In.LightVec);

  float4 litV = lit(dot(In.Normal, In.LightVec), dot(In.Normal, Hn), g_fSpecExpon);
  float4 diffContrib = g_vSurfColor * (0.5 * litV.y * g_vLightColor + g_vAmbiColor);
  float4 specContrib = litV.y * litV.z * g_vLightColor;
  return diffContrib + specContrib;
}

PLAIN_VS_OUTPUT PlainVS(VS_INPUT In)
{
  PLAIN_VS_OUTPUT Out = (PLAIN_VS_OUTPUT)0;

  Out.vPos = mul(In.Position, g_mWorldViewProj);
  
  return Out;
}

float4 PlainPS(PHONG_VS_OUTPUT In) : COLOR0
{
  return g_vSurfColor;
}

technique PhongShader
{
  pass P0
  {
    VertexShader = compile vs_2_0 PhongVS();
    PixelShader = compile ps_2_0 PhongPS();
  }
}

technique PlainShader
{
  pass P0
  {
    VertexShader = compile vs_2_0 PlainVS();
    PixelShader = compile ps_2_0 PlainPS();
  }
}
