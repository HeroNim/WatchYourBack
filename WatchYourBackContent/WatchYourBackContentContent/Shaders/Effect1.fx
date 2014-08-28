sampler s0;
texture lightMask;
sampler lightSampler = sampler_state{ Texture = lightMask; };


float4 SceneryFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = tex2D(s0, texCoord);
	float4 colorScale = color * 0.5f;
	float4 lightColor = tex2D(lightSampler, texCoord);
	return colorScale * lightColor + colorScale;
}

float4 ObjectFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 clear = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 color = tex2D(s0, texCoord);
	float4 lightColor = tex2D(lightSampler, texCoord);
	if (any(lightColor.rgb))
	{
		return color;
	}
	else
	{
		return clear;
	}
}

technique Technique1
{
	pass DrawScenery
	{
		PixelShader = compile ps_4_0_level_9_1 SceneryFunction();
	}

	pass DrawObjects
	{
		PixelShader = compile ps_4_0_level_9_1 ObjectFunction();
	}
}