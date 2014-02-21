//-----------------------------------------------------------------------------
// MACROS
//-----------------------------------------------------------------------------

#ifdef SM4

// Macros for targetting shader model 4.0 (DX11)

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_4_0_level_9_1 vsname (); PixelShader = compile ps_4_0_level_9_1 psname(); } }

#define BEGIN_CONSTANTS     cbuffer Parameters : register(b0) {
#define MATRIX_CONSTANTS
#define END_CONSTANTS       };

#define _vs(r)
#define _ps(r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    Texture2D<float4> Name : register(t##index); \
    sampler Name##Sampler : register(s##index)

#define DECLARE_CUBEMAP(Name, index) \
    TextureCube<float4> Name : register(t##index); \
    sampler Name##Sampler : register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)


#else


// Macros for targetting shader model 2.0 (DX9)

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_2_0 vsname (); PixelShader = compile ps_2_0 psname(); } }

#define BEGIN_CONSTANTS
#define MATRIX_CONSTANTS
#define END_CONSTANTS

#define _vs(r)  : register(vs, r)
#define _ps(r)  : register(ps, r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    sampler2D Name : register(s##index);

#define DECLARE_CUBEMAP(Name, index) \
    samplerCUBE Name : register(s##index);

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  texCUBE(Name, texCoord)


#endif

//-----------------------------------------------------------------------------
// END MACROS
//-----------------------------------------------------------------------------


DECLARE_TEXTURE(Texture, 0);

BEGIN_CONSTANTS

MATRIX_CONSTANTS

	float4x4 WorldViewProj _vs(c0);

END_CONSTANTS

struct VSInputTx
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutputTx
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float4 Specular   : COLOR1;
    float2 TexCoord   : TEXCOORD0;
};

struct CommonVSOutput
{
    float4 Pos_ps;
    float4 Diffuse;
    float3 Specular;
    float  FogFactor;
};

#define SetCommonVSOutputParams \
    vout.PositionPS = cout.Pos_ps; \
    vout.Diffuse = cout.Diffuse; \
    vout.Specular = float4(cout.Specular, cout.FogFactor);

float ComputeFogFactor(float4 position)
{
	float4 FogVector = float4(0,0,0,0);

    return saturate(dot(position, FogVector));
}

void ApplyFog(inout float4 color, float fogFactor)
{
	float3 FogColor = float3(0,0,0);

    color.rgb = lerp(color.rgb, FogColor * color.a, fogFactor);
}

void AddSpecular(inout float4 color, float3 specular)
{
    color.rgb += specular * color.a;
}

CommonVSOutput ComputeCommonVSOutput(float4 position)
{
    CommonVSOutput vout;

    vout.Pos_ps = mul(position, WorldViewProj);
    vout.Diffuse = float4(1,1,1,1);
    vout.Specular = 0;
    vout.FogFactor = ComputeFogFactor(position);
    
    return vout;
}

VSOutputTx VSBasicTx(VSInputTx vin)
{
    VSOutputTx vout;
    
    CommonVSOutput cout = ComputeCommonVSOutput(vin.Position);
    SetCommonVSOutputParams;
    
    vout.TexCoord = vin.TexCoord;

    return vout;
}

float4 PSBasicTx(VSOutputTx pin) : SV_Target0
{
    float4 color = SAMPLE_TEXTURE(Texture, pin.TexCoord) * pin.Diffuse;
    
    ApplyFog(color, pin.Specular.w);
    
    return color;
}

TECHNIQUE(Basic, VSBasicTx, PSBasicTx)