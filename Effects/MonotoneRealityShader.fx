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
    //float2 uv = coords / uScreenResolution;
    //float amplitude = 0.1;
    //float period = 2 * 3.14;
    //coords.x += amplitude * sin((coords.y + uTime));
    float4 color = tex2D(uImage0, coords);
    //Color.rgb = 1 - Color.rgb;
    float3 white = (1.0, 1.0, 1.0);
    float3 black = (0.0, 0.0, 0.0);
    
    float average = (color.r + color.g + color.b) / 3.0;
  
    if (average <= 0.3 || average >= 0.55)
    {
        color.rgb = black;
    }
    else
    {
        color.rgb = white;
    }
    
	return color; 
}

technique Technique1
{
    pass MonotoneRealityEffect
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}