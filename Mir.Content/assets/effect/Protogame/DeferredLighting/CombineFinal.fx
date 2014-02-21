float2 HalfPixel;

PROTOGAME_DECLARE_TEXTURE(ColorMap, 0) = sampler_state
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

PROTOGAME_DECLARE_TEXTURE(LightMap, 1) = sampler_state
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

struct VertexShaderInput
{
    float3 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    output.TexCoord = input.TexCoord - HalfPixel;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : PROTOGAME_TARGET(0)
{
    float3 diffuseColor = PROTOGAME_SAMPLE_TEXTURE(ColorMap, input.TexCoord).rgb;
    float4 light = PROTOGAME_SAMPLE_TEXTURE(LightMap, input.TexCoord);
    float3 diffuseLight = light.rgb;
    float specularLight = light.a;
    return float4((diffuseColor * diffuseLight + specularLight),1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile PROTOGAME_VERTEX_SHADER VertexShaderFunction();
        PixelShader = compile PROTOGAME_PIXEL_SHADER PixelShaderFunction();
    }
}
