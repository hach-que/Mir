struct VertexShaderInput
{
    float3 Position : PROTOGAME_POSITION;
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    return output;
}

struct PixelShaderOutput
{
    float4 Color : PROTOGAME_TARGET(0);
    float4 Normal : PROTOGAME_TARGET(1);
    float4 Depth : PROTOGAME_TARGET(2);
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
    //black color
    output.Color = 0.0f;
    output.Color.a = 0.0f;
    //when transforming 0.5f into [-1,1], we will get 0.0f
    output.Normal.rgb = 0.5f;
    //no specular power
    output.Normal.a = 0.0f;
    //max depth
    output.Depth = 0.0f;
    return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile PROTOGAME_VERTEX_SHADER VertexShaderFunction();
        PixelShader = compile PROTOGAME_PIXEL_SHADER PixelShaderFunction();
    }
}