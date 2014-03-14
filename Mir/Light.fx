PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 LightColours1;
float4x4 Lights1;
float4x4 LightColours2;
float4x4 Lights2;
float4x4 LightColours3;
float4x4 Lights3;
float4x4 LightColours4;
float4x4 Lights4;

float3 Ambient;
float3 AmbientRemaining;
float Alpha;

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

float3 GetLightImpactAtPixel1(VertexShaderOutput input, int idx)
{
	float lightDistance = Lights1[idx].w;

	if (lightDistance == 0)
	{
		return float3(0, 0, 0);
	}

	float3 lightPosition = Lights1[idx].xyz;
	float3 lightColor = LightColours1[idx].xyz;

	float distance = length(input.WorldPosition - lightPosition);
	float invDistance = max(0, lightDistance - distance) / lightDistance;
	return float3(invDistance * lightColor.r, invDistance * lightColor.g, invDistance * lightColor.b);
}

float3 GetLightImpactAtPixel2(VertexShaderOutput input, int idx)
{
	float lightDistance = Lights2[idx].w;

	if (lightDistance == 0)
	{
		return float3(0, 0, 0);
	}

	float3 lightPosition = Lights2[idx].xyz;
	float3 lightColor = LightColours2[idx].xyz;

	float distance = length(input.WorldPosition - lightPosition);
	float invDistance = max(0, lightDistance - distance) / lightDistance;
	return float3(invDistance * lightColor.r, invDistance * lightColor.g, invDistance * lightColor.b);
}

float3 GetLightImpactAtPixel3(VertexShaderOutput input, int idx)
{
	float lightDistance = Lights3[idx].w;

	if (lightDistance == 0)
	{
		return float3(0, 0, 0);
	}

	float3 lightPosition = Lights3[idx].xyz;
	float3 lightColor = LightColours3[idx].xyz;

	float distance = length(input.WorldPosition - lightPosition);
	float invDistance = max(0, lightDistance - distance) / lightDistance;
	return float3(invDistance * lightColor.r, invDistance * lightColor.g, invDistance * lightColor.b);
}

float3 GetLightImpactAtPixel4(VertexShaderOutput input, int idx)
{
	float lightDistance = Lights4[idx].w;

	if (lightDistance == 0)
	{
		return float3(0, 0, 0);
	}

	float3 lightPosition = Lights4[idx].xyz;
	float3 lightColor = LightColours4[idx].xyz;

	float distance = length(input.WorldPosition - lightPosition);
	float invDistance = max(0, lightDistance - distance) / lightDistance;
	return float3(invDistance * lightColor.r, invDistance * lightColor.g, invDistance * lightColor.b);
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	// Calculate lights
	float3 lightResult = float3(0, 0, 0);
	lightResult += GetLightImpactAtPixel1(input, 0);
	lightResult += GetLightImpactAtPixel1(input, 1);
	lightResult += GetLightImpactAtPixel1(input, 2);
	lightResult += GetLightImpactAtPixel1(input, 3);
	lightResult += GetLightImpactAtPixel2(input, 0);
	lightResult += GetLightImpactAtPixel2(input, 1);
	lightResult += GetLightImpactAtPixel2(input, 2);
	lightResult += GetLightImpactAtPixel2(input, 3);
	lightResult += GetLightImpactAtPixel3(input, 0);
	lightResult += GetLightImpactAtPixel3(input, 1);
	lightResult += GetLightImpactAtPixel3(input, 2);
	lightResult += GetLightImpactAtPixel3(input, 3);
	lightResult += GetLightImpactAtPixel4(input, 0);
	lightResult += GetLightImpactAtPixel4(input, 1);
	lightResult += GetLightImpactAtPixel4(input, 2);
	lightResult += GetLightImpactAtPixel4(input, 3);

	// Calculate final lighting impact.
	float4 lightingRemaining = float4(Ambient + (lightResult * AmbientRemaining), Alpha);

	output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord) * lightingRemaining;
    
    return output;
}

technique
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER DefaultPixelShader();
	}
}