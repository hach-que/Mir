PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 LightColours;
float4x4 Lights;

float3 Ambient;
float3 AmbientRemaining;

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

float3 GetLightImpactAtPixel(VertexShaderOutput input, int idx)
{
	float lightDistance = Lights[idx].w;

	if (lightDistance == 0)
	{
		return float3(0, 0, 0);
	}

	float3 lightPosition = Lights[idx].xyz;
	float3 lightColor = LightColours[idx].xyz;

	float distance = length(input.WorldPosition - lightPosition);
	float invDistance = max(0, lightDistance - distance) / lightDistance;
	return float3(invDistance * lightColor.r, invDistance * lightColor.g, invDistance * lightColor.b);
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	// Calculate lights
	float3 light1 = GetLightImpactAtPixel(input, 0);
	float3 light2 = GetLightImpactAtPixel(input, 1);
	float3 light3 = GetLightImpactAtPixel(input, 2);
	
	// Combine light values.
	float3 lightResult = light1 + light2 + light3;

	// Normalize values so that they don't exceed 1.
	//lightResult.r = min(1, lightResult.r);
	//lightResult.g = min(1, lightResult.g);
	//lightResult.b = min(1, lightResult.b);

	// Calculate final lighting impact.
	float4 lightingRemaining = float4(Ambient + (lightResult * AmbientRemaining), 1);

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