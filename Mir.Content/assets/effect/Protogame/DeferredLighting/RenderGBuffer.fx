float4x4 World;
float4x4 View;
float4x4 Projection;
float specularIntensity = 0.8f;
float specularPower = 0.5f;

PROTOGAME_DECLARE_TEXTURE(Texture, 0) = sampler_state
{
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : PROTOGAME_POSITION;
    float3 Normal : PROTOGAME_NORMAL(0);
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
    float3 Normal : PROTOGAME_TEXCOORD(1);
    float2 Depth : PROTOGAME_TEXCOORD(2);
};

struct PixelShaderOutput
{
    float4 Color : PROTOGAME_TARGET(0);
    float4 Normal : PROTOGAME_TARGET(1);
    float4 Depth : PROTOGAME_TARGET(2);
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoord = input.TexCoord;
	output.Normal = mul(input.Normal, World);
    
	output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;
	
    output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord);
	output.Color.a = specularIntensity;
	
    output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);
	output.Normal.a = specularPower;

	output.Depth = input.Depth.x / input.Depth.y;
    
	return output;
}

technique Basic
{
	pass Pass1
	{
		VertexShader = compile PROTOGAME_VERTEX_SHADER VertexShaderFunction();
		PixelShader = compile PROTOGAME_PIXEL_SHADER PixelShaderFunction();
	}
}