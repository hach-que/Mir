PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float4 WorldPosition : PROTOGAME_TARGET(0);
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

VertexShaderOutput DefaultVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.WorldPosition = worldPosition;

    output.TexCoord = input.TexCoord;

    return output;
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	float4 LightPosition = float4(16, 8, 16, 0);
	float LightDistance = 30;
	float4 Ambient = float4(0.2, 0.2, 0.2, 1);
	float4 AmbientRemaining = float4(1, 1, 1, 1) - Ambient;

	// Calculate light distance
	float4 relative = input.WorldPosition - LightPosition;
	float distance = length(relative);

	// Calculate lighting effect
	float invDistance = max(0, LightDistance - distance) / LightDistance;
	float4 lightingModifier = float4(invDistance, invDistance, invDistance, 1);
	float4 lightingRemaining = Ambient + (lightingModifier * AmbientRemaining);

	output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord) * lightingRemaining;
    
    return output;
}

technique
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_SHADER DefaultPixelShader();
	}
}