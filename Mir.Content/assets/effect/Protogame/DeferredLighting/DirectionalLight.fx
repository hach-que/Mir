float3 LightDirection;
float3 Color;
float3 CameraPosition;
float4x4 InvertViewProjection;
float2 HalfPixel;

PROTOGAME_DECLARE_TEXTURE(ColorMap, 0) = sampler_state
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

PROTOGAME_DECLARE_TEXTURE(NormalMap, 1) = sampler_state
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

PROTOGAME_DECLARE_TEXTURE(DepthMap, 2) = sampler_state
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
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
    //get normal data from the normalMap
    float4 normalData = PROTOGAME_SAMPLE_TEXTURE(NormalMap, input.TexCoord);
    //tranform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;
    //get specular power, and get it into [0,255] range]
    float specularPower = normalData.a * 255;
    //get specular intensity from the colorMap
    float specularIntensity = PROTOGAME_SAMPLE_TEXTURE(ColorMap, input.TexCoord).a;
    
    //read depth
    float depthVal = PROTOGAME_SAMPLE_TEXTURE(DepthMap, input.TexCoord).r;

    //compute screen-space position
    float4 position;
    position.x = input.TexCoord.x * 2.0f - 1.0f;
    position.y = -(input.TexCoord.x * 2.0f - 1.0f);
    position.z = depthVal;
    position.w = 1.0f;
    //transform to world space
    position = mul(position, InvertViewProjection);
    position /= position.w;
    
    //surface-to-light vector
    float3 lightVector = -normalize(LightDirection);

    //compute diffuse light
    float NdL = max(0, dot(normal, lightVector));
    float3 diffuseLight = NdL * Color.rgb;

    //reflexion vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));
    //camera-to-surface vector
    float3 directionToCamera = normalize(CameraPosition - position);
    //compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);

    //output the two lights
    return float4(diffuseLight.rgb, specularLight);
}

technique Technique0
{
    pass Pass0
    {
        VertexShader = compile PROTOGAME_VERTEX_SHADER VertexShaderFunction();
        PixelShader = compile PROTOGAME_PIXEL_SHADER PixelShaderFunction();
    }
}