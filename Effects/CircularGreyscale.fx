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

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 newColor = color;
    newColor.b = newColor.g = newColor.r;
    return lerp(color, newColor, distance(float2(0.5, 0.5), coords) * 2 * uProgress);
}

technique Technique1
{
    pass CircularGreyscale
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}