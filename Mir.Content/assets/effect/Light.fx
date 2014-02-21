PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 WorldViewProj;

struct VSInputTx
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct VSOutputTx
{
    float4 PositionPS : PROTOGAME_POSITION;
    float4 Diffuse    : PROTOGAME_TARGET(0);
    float4 Specular   : PROTOGAME_TARGET(1);
    float2 TexCoord   : PROTOGAME_TEXCOORD(0);
};

struct CommonVSOutput
{
    float4 Pos_ps;
    float4 Diffuse;
    float3 Specular;
    float  FogFactor;
};

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
    vout.PositionPS = cout.Pos_ps;
    vout.Diffuse = cout.Diffuse;
    vout.Specular = float4(cout.Specular, cout.FogFactor);
    
    vout.TexCoord = vin.TexCoord;

    return vout;
}

float4 PSBasicTx(VSOutputTx pin) : PROTOGAME_TARGET(0)
{
    float4 color = PROTOGAME_SAMPLE_TEXTURE(Texture, pin.TexCoord) * pin.Diffuse;
    
    ApplyFog(color, pin.Specular.w);
    
    return color;
}

technique
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_SHADER VSBasicTx();
		PixelShader = compile PROTOGAME_PIXEL_SHADER PSBasicTx();
	}
}