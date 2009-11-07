//---------------------------------------------------------------
// ���U�V���h�E�}�b�v
//---------------------------------------------------------------

texture		texRender;			// �����_�����O��e�N�X�`��
float4x4	matWVP;				// �ϊ��s��
float4x4	matLS;				// �e�N�X�`�����e�s��
float4		vLightPos;			// ���C�g�ʒu�i���[�J���j
float4		vLightCol;			// ���C�g�J���[
float		aWeight[8];			// �K�E�X�t�B���^�̃E�F�C�g�l
float		fEpsilon = 0.0001f;	// �I�t�Z�b�g
float		fMaxDepth = 300.0f;	// �ő�[�x
float		MAP_WIDTH = 512.0f;
float		MAP_HEIGHT = 512.0f;

//---------------------------------------------------------------
// ���_�V�F�[�_����
struct VS_INPUT0{
	float4	vPos	: POSITION;
	float3	vNorm	: NORMAL0;
};
struct VS_INPUT1{
	float4	vPos	: POSITION;
	float2	vTex	: TEXCOORD0;
};

//---------------------------------------------------------------
// ���_�V�F�[�_�o��
struct VS_OUTPUT1{
	float4	vPos	: POSITION;
	float4	vCol	: COLOR0;
	float4	vTex	: TEXCOORD0;
	float4	vDepth	: TEXCOORD1;
};
struct VS_OUTPUT2{
	float4	vPos	: POSITION;
	float2	vTex0	: TEXCOORD0;
	float2	vTex1	: TEXCOORD1;
	float2	vTex2	: TEXCOORD2;
	float2	vTex3	: TEXCOORD3;
	float2	vTex4	: TEXCOORD4;
	float2	vTex5	: TEXCOORD5;
	float2	vTex6	: TEXCOORD6;
	float2	vTex7	: TEXCOORD7;
};

//---------------------------------------------------------------
// �T���v��
sampler s0 = sampler_state{
	texture = <texRender>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MipFilter = NONE;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

//---------------------------------------------------------------
// ���_�V�F�[�_
// �ʏ�`��
VS_OUTPUT1 vsNormalDraw(const VS_INPUT0 v)
{
	VS_OUTPUT1	o = (VS_OUTPUT1)0;

	// ���W�ϊ�
	o.vPos = mul(v.vPos, matWVP);

	// ���C�g�x�N�g�������߂�
	float3		lit;
	lit = normalize(v.vPos.xyz - vLightPos.xyz);

	// ���C�g�v�Z
//	o.vCol.xyz = vLightCol.xyz * dot(-lit, v.vNorm);
	o.vCol.xyz = 1.0f;
	o.vCol.w = 1.0f;

	return o;
}
// �e�}�b�v�`��
VS_OUTPUT1 vsShadowMapDraw(const VS_INPUT0 v)
{
	VS_OUTPUT1	o = (VS_OUTPUT1)0;

	// ���W�ϊ�
	float4	pos = mul(v.vPos, matLS);
	o.vPos = pos;

	// �[�x
//	o.vDepth = pos;
	o.vDepth.xy = pos.xy;
//	o.vDepth.z = length( v.vPos - vLightPos ) / fMaxDepth;
	o.vDepth.z = (length( v.vPos - vLightPos ) / fMaxDepth) * 2.0f - 1.0f;

	return o;
}
// �e���e�`��
VS_OUTPUT1 vsShadowDraw(const VS_INPUT0 v)
{
	VS_OUTPUT1	o = (VS_OUTPUT1)0;

	// ���W�ϊ�
	o.vPos = mul(v.vPos, matWVP);

	// �[�x
	float4	pos = mul(v.vPos, matLS);
//	o.vDepth = pos;
	o.vDepth.xy = pos.xy;
//	o.vDepth.z = length( v.vPos - vLightPos ) / fMaxDepth;
	o.vDepth.z = (length( v.vPos - vLightPos ) / fMaxDepth) * 2.0f - 1.0f;

	// �e�N�X�`�����W�v�Z
	o.vTex.x = 0.5f * (pos.x + pos.w);
	o.vTex.y = 0.5f * (-pos.y + pos.w);
	o.vTex.w = pos.w;

	return o;
}
// �e�e�N�X�`���\��
VS_OUTPUT1 vsTexDraw(const VS_INPUT1 v)
{
	VS_OUTPUT1	o = (VS_OUTPUT1)0;

	// ���W�ϊ�
	o.vPos = v.vPos;

	// �J���[
	o.vCol = 1.0f;

	// �e�N�X�`�����W�v�Z
	o.vTex.xy = v.vTex.xy;
	o.vTex.zw = 0.0f;

	return o;
}
// �K�E�X�t�B���^X�ڂ���
VS_OUTPUT2 vsGaussXDraw(const VS_INPUT1 v)
{
	VS_OUTPUT2	o = (VS_OUTPUT2)0;

	// ���W�ϊ�
	o.vPos = v.vPos;

	// �e�N�X�`�����W
	o.vTex0.xy = v.vTex + float2(- 1.0f/MAP_WIDTH, 0.0f);
	o.vTex1.xy = v.vTex + float2(- 3.0f/MAP_WIDTH, 0.0f);
	o.vTex2.xy = v.vTex + float2(- 5.0f/MAP_WIDTH, 0.0f);
	o.vTex3.xy = v.vTex + float2(- 7.0f/MAP_WIDTH, 0.0f);
	o.vTex4.xy = v.vTex + float2(- 9.0f/MAP_WIDTH, 0.0f);
	o.vTex5.xy = v.vTex + float2(-11.0f/MAP_WIDTH, 0.0f);
	o.vTex6.xy = v.vTex + float2(-13.0f/MAP_WIDTH, 0.0f);
	o.vTex7.xy = v.vTex + float2(-15.0f/MAP_WIDTH, 0.0f);

	return o;
}
// �K�E�X�t�B���^Y�ڂ���
VS_OUTPUT2 vsGaussYDraw(const VS_INPUT1 v)
{
	VS_OUTPUT2	o = (VS_OUTPUT2)0;

	// ���W�ϊ�
	o.vPos = v.vPos;

	// �e�N�X�`�����W
	o.vTex0.xy = v.vTex + float2(0.0f, - 1.0f/MAP_HEIGHT);
	o.vTex1.xy = v.vTex + float2(0.0f, - 3.0f/MAP_HEIGHT);
	o.vTex2.xy = v.vTex + float2(0.0f, - 5.0f/MAP_HEIGHT);
	o.vTex3.xy = v.vTex + float2(0.0f, - 7.0f/MAP_HEIGHT);
	o.vTex4.xy = v.vTex + float2(0.0f, - 9.0f/MAP_HEIGHT);
	o.vTex5.xy = v.vTex + float2(0.0f, -11.0f/MAP_HEIGHT);
	o.vTex6.xy = v.vTex + float2(0.0f, -13.0f/MAP_HEIGHT);
	o.vTex7.xy = v.vTex + float2(0.0f, -15.0f/MAP_HEIGHT);

	return o;
}

//---------------------------------------------------------------
// �s�N�Z���V�F�[�_
// �ʏ�`��
float4 psNormalDraw(const VS_OUTPUT1 v) : COLOR
{
	return v.vCol;
}
// �e�}�b�v�`��
float4 psShadowMapDraw(const VS_OUTPUT1 v) : COLOR
{
	float4	o;

	o.x = v.vDepth.z;
	o.y = o.x * o.x;
	o.z = o.w = 1.0f;

	return o;
}
// �e���e�`��
float4 psShadowDraw(const VS_OUTPUT1 v) : COLOR
{
	float4	o = {1.0f, 1.0f, 1.0f, 1.0f};
	float4	depth;
	float	d = v.vDepth.z;

	depth = tex2Dproj(s0, v.vTex);
	if(depth.x < d - 0.01f){
		o.xyz = 0.2f;
	}

	return o;
}
// �e�N�X�`���`��
float4 psTexDraw(const VS_OUTPUT1 v) : COLOR
{
	return tex2D(s0, v.vTex) * v.vCol.w;
}
// �K�E�X�t�B���^�ڂ���
float4 psGaussXDraw(VS_OUTPUT2 v) : COLOR
{
	float4	col;

	float2	offset = {16.0f / MAP_WIDTH, 0.0f};

	col  = aWeight[0] * (tex2D(s0, v.vTex0) + tex2D(s0, v.vTex7 + offset));
	col += aWeight[1] * (tex2D(s0, v.vTex1) + tex2D(s0, v.vTex6 + offset));
	col += aWeight[2] * (tex2D(s0, v.vTex2) + tex2D(s0, v.vTex5 + offset));
	col += aWeight[3] * (tex2D(s0, v.vTex3) + tex2D(s0, v.vTex4 + offset));
	col += aWeight[4] * (tex2D(s0, v.vTex4) + tex2D(s0, v.vTex3 + offset));
	col += aWeight[5] * (tex2D(s0, v.vTex5) + tex2D(s0, v.vTex2 + offset));
	col += aWeight[6] * (tex2D(s0, v.vTex6) + tex2D(s0, v.vTex1 + offset));
	col += aWeight[7] * (tex2D(s0, v.vTex7) + tex2D(s0, v.vTex0 + offset));

	return col;
}
float4 psGaussYDraw(VS_OUTPUT2 v) : COLOR
{
	float4	col;

	float2	offset = {0.0f, 16.0f / MAP_HEIGHT};

	col  = aWeight[0] * (tex2D(s0, v.vTex0) + tex2D(s0, v.vTex7 + offset));
	col += aWeight[1] * (tex2D(s0, v.vTex1) + tex2D(s0, v.vTex6 + offset));
	col += aWeight[2] * (tex2D(s0, v.vTex2) + tex2D(s0, v.vTex5 + offset));
	col += aWeight[3] * (tex2D(s0, v.vTex3) + tex2D(s0, v.vTex4 + offset));
	col += aWeight[4] * (tex2D(s0, v.vTex4) + tex2D(s0, v.vTex3 + offset));
	col += aWeight[5] * (tex2D(s0, v.vTex5) + tex2D(s0, v.vTex2 + offset));
	col += aWeight[6] * (tex2D(s0, v.vTex6) + tex2D(s0, v.vTex1 + offset));
	col += aWeight[7] * (tex2D(s0, v.vTex7) + tex2D(s0, v.vTex0 + offset));

	return col;
}
// VSM�`��
#if 1
float4 psVsmDraw(const VS_OUTPUT1 v) : COLOR
{
	float4	o = {1.0f, 1.0f, 1.0f, 1.0f};
	float4	depth = tex2Dproj(s0, v.vTex);
	float	d = v.vDepth.z;

	float	lit_fact = (d <= depth.x);

	float	depth_sq = depth.x * depth.x;
	float	variance = min( max( depth.y - depth_sq, 0.0f ) + fEpsilon, 1.0f );
	float	md = d - depth.x;
	float	pmax = variance / (variance + (md * md));

	o.xyz *= max( lit_fact, pmax );

	return o;
}
#else
float4 psVsmDraw(const VS_OUTPUT1 v) : COLOR
{
	float4	o = {1.0f, 1.0f, 1.0f, 1.0f};
	float4	depth = tex2Dproj(s0, v.vTex);
	float	d = v.vDepth.z;

	if( d <= depth.x ){
		float	depth_sq = depth.x * depth.x;
		float	variance = min( max( depth.y - depth_sq, 0.0f ) + fEpsilon, 1.0f );
		float	md = depth.x - d;
		float	pmax = variance / (variance + (md * md));

		o.xyz *= (1.0f - pmax);
	}
	else{
		o.xyz *= 0.0f;
	}

	return o;
}
#endif
//---------------------------------------------------------------
// �e�N�j�b�N
// �ʏ�`��
technique Tec0_NormalDraw
{
	pass Pas0
	{
		VertexShader = compile vs_2_0 vsNormalDraw();
		PixelShader = compile ps_2_0 psNormalDraw();

		ZEnable = TRUE;
		AlphaBlendEnable = False;
	}
}
// �e�}�b�v�`��
technique Tec1_ShadowMapDraw
{
	pass Pas0
	{
		VertexShader = compile vs_2_0 vsShadowMapDraw();
		PixelShader = compile ps_2_0 psShadowMapDraw();

		ZEnable = TRUE;
		AlphaBlendEnable = False;
	}
}
// �e���e�`��
technique Tec2_ShadowDraw
{
	pass Pas0
	{
		VertexShader = compile vs_2_0 vsShadowDraw();
		PixelShader = compile ps_2_0 psShadowDraw();

		Sampler[0] = <s0>;

		ZEnable = TRUE;
		AlphaBlendEnable = False;
	}
}
// �e�e�N�X�`���\��
technique Tec3_TexDraw
{
	pass Pas0
	{
		VertexShader = compile vs_2_0 vsTexDraw();
		PixelShader = compile ps_2_0 psTexDraw();

		Sampler[0] = <s0>;

		ZEnable = FALSE;
		AlphaBlendEnable = False;
	}
}
// �K�E�X�t�B���^�ڂ���
technique Tec4_GaussDraw
{
	pass Pas0
	{
		VertexShader = compile vs_2_0 vsGaussXDraw();
		PixelShader = compile ps_2_0 psGaussXDraw();

		ZEnable = False;
		AlphaBlendEnable = False;
	}
	pass Pas1
	{
		VertexShader = compile vs_2_0 vsGaussYDraw();
		PixelShader = compile ps_2_0 psGaussYDraw();

		ZEnable = False;
		AlphaBlendEnable = False;
	}
}
// VSM�`��
technique Tec5_ShadowDraw
{
	pass Pas0
	{
		VertexShader = compile vs_2_0 vsShadowDraw();
		PixelShader = compile ps_2_0 psVsmDraw();

		Sampler[0] = <s0>;

		ZEnable = True;
		AlphaBlendEnable = False;
	}
}
