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
static const float PixelSizeWidth = 1.0 / 84.0;
static const float PixelSizeHeight = 1.0 / 112.0;

bool isBlack(float3 color)
{
    return color.r == 0.0 && color.g == 0.0 && color.b == 0.0;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    uv.y += 0.015 * sin(6.28 * distance(uv.x, 0.5) * (uTime % 360.0));
    float4 color = tex2D(uImage0, uv);

    /*float2 standUV = coords * float2(uShaderSpecificData.x, uShaderSpecificData.y);
    float4 standColor = tex2D(uImage2, standUV);
    if (isBlack(standColor.rgb))
        return float4(1.0, 0.0, 0.0, 1.0);*/
    
    float noiseOffset = sin((3.14 / 2.0) * (((uTime * 60.0) % NoiseTravelRate) / NoiseTravelRate)); //only goes up
    
    float noise = tex2D(uImage1, uv + float2(0.0, uTime / 1.2)).r;
    int surroundVal = 0;
    for (int i = 1; i < 4; i++)
    {
        if (!isBlack(tex2D(uImage0, uv + float2(-PixelSizeWidth * i, 0)).rgb) || !isBlack(tex2D(uImage0, uv + float2(PixelSizeWidth * i, 0)).rgb) || !isBlack(tex2D(uImage0, uv + float2(0, -PixelSizeHeight * i)).rgb) || !isBlack(tex2D(uImage0, uv + float2(0, PixelSizeHeight  * i)).rgb))
        {
            surroundVal = i;
            break;
        }

    }
    if (surroundVal >= 1)
    {
        int strength = 3 - surroundVal;
        return float4(uSecondaryColor / 2.5 * noise / (surroundVal * 2.0), noise / (surroundVal * 800.0) * uOpacity);
        
        float3 noiseCol = tex2D(uImage1, uv + float2(0.0, noiseOffset));
        return float4(noiseCol.rgb * uSecondaryColor, 1.0);
    }
    else
        return float4(0.0, 0.0, 0.0, 0.0);
    return color;
}

technique Technique1
{
    pass AuraShaderEffect
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}