// Effect dynamically changes color saturation.

sampler TextureSampler : register(s0);
float healthState;

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	// Look up the texture color.
	float4 tex = tex2D(TextureSampler, texCoord);
	float3 colrgb = tex.rgb;
	float greycolor = dot(colrgb, float3(0.3, 0.59, 0.11));

	colrgb.rgb = lerp(dot(greycolor, float3(0.3, 0.59, 0.11)), colrgb, healthState);
	return float4(colrgb.rgb, tex.a);
}


technique Desaturate
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}