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
    float2 uv = coords;
    float4 color = tex2D(uImage0, uv);
    
    float2 uv2 = coords;
    uv2.x += 0.064 * sin((128.0 * uv.y) + (uTime * 72.0));
    color += tex2D(uImage0, uv2) * 0.2;
    
    return color;
}

technique Technique1
{
    pass BackInBlackDistortionEffect
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}