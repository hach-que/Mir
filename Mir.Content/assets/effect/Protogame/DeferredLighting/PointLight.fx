float4x4 World;
float4x4 View;
float4x4 Projection;
float3 Color; 
float3 CameraPosition; 
float4x4 InvertViewProjection; 
float3 LightPosition;
float LightRadius;
float LightIntensity = 1.0f;
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
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
    float4 ScreenPosition : PROTOGAME_TEXCOORD(0);
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    //processing geometry coordinates
    float4 worldPosition = mul(float4(input.Position,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.ScreenPosition = output.Position;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : PROTOGAME_TARGET(0)
{
    //obtain screen position
    input.ScreenPosition.xy /= input.ScreenPosition.w;

    //obtain textureCoordinates corresponding to the current pixel
    //the screen coordinates are in [-1,1]*[1,-1]
    //the texture coordinates need to be in [0,1]*[0,1]
    float2 texCoord = 0.5f * (float2(input.ScreenPosition.x,-input.ScreenPosition.y) + 1);
    //allign texels to pixels
    texCoord -= HalfPixel;

    //get normal data from the normalMap
    float4 normalData = PROTOGAME_SAMPLE_TEXTURE(NormalMap, texCoord);
    //tranform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;
    //get specular power
    float specularPower = normalData.a * 255;
    //get specular intensity from the colorMap
    float specularIntensity = PROTOGAME_SAMPLE_TEXTURE(ColorMap, texCoord).a;

    //read depth
    float depthVal = PROTOGAME_SAMPLE_TEXTURE(DepthMap, texCoord).r;

    //compute screen-space position
    float4 position;
    position.xy = input.ScreenPosition.xy;
    position.z = depthVal;
    position.w = 1.0f;
    //transform to world space
    position = mul(position, InvertViewProjection);
    position /= position.w;

    //surface-to-light vector
    float3 lightVector = LightPosition - position;

    //compute attenuation based on distance - linear attenuation
    float attenuation = saturate(1.0f - length(lightVector)/LightRadius); 

    //normalize light vector
    lightVector = normalize(lightVector); 

    //compute diffuse light
    float NdL = max(0,dot(normal,lightVector));
    float3 diffuseLight = NdL * Color.rgb;

    //reflection vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));
    //camera-to-surface vector
    float3 directionToCamera = normalize(CameraPosition - position);
    //compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);

    //take into account attenuation and lightIntensity.
    return attenuation * LightIntensity * float4(diffuseLight.rgb,specularLight);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile PROTOGAME_VERTEX_SHADER VertexShaderFunction();
        PixelShader = compile PROTOGAME_PIXEL_SHADER PixelShaderFunction();
    }
}
