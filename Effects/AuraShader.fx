sampler uImage0 : register(s0); //the screen itself
sampler uImage1 : register(s1);
sampler uImage2;
sampler uImage3;
float3 uColor;
float uOpacity : register(C0);
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float4 uShaderSpecificData;

static const int NoiseTravelRate = 5 * 60;

bool isBlack(float3 color)
{
    return color.r == 0.0 && color.g == 0.0 && color.b == 0.0;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    float4 color = tex2D(uImage0, uv);
    if (!isBlack(color.rgb))
        return color;
    if (uv.y * 66 * 4 > uSourceRect.y)
        return float4(1.0, 0.0, 0.0, 1.0);

    float noiseOffset = sin(3.14 * ((uTime % NoiseTravelRate) / NoiseTravelRate)); //only goes up
    
    float noise = tex2D(uImage1, uv + float2(0.0, noiseOffset)).r;
    float pixelSizeWidth = 1.0 / 62.0;
    float pixelSizeHeight = 1.0 / 66.0;
    int surroundVal = 0;
    for (int i = 1; i < 4; i++)
    {
        if (!isBlack(tex2D(uImage0, uv + float2(-pixelSizeWidth * i, 0)).rgb) || !isBlack(tex2D(uImage0, uv + float2(pixelSizeWidth * i, 0)).rgb) || !isBlack(tex2D(uImage0, uv + float2(0, -pixelSizeHeight * i)).rgb) || !isBlack(tex2D(uImage0, uv + float2(0, pixelSizeHeight  * i)).rgb))
        {
            surroundVal = i;
            break;
        }

    }
    if (surroundVal >= 1)
    {
        if (surroundVal == 3)
            return float4(uSecondaryColor, 1.0);
        else
            return float4(uSecondaryColor * noise, noise * (surroundVal / 4.0));
        
        float3 noiseCol = tex2D(uImage1, uv + float2(0.0, noiseOffset));
        return float4(noiseCol.rgb * uSecondaryColor, 1.0);
    }
    return color;
}

technique Technique1
{
    pass AuraShaderEffect
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}