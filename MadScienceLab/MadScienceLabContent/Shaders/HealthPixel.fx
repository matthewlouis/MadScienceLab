// Effect dynamically changes color saturation.

sampler TextureSampler : register(s0);
float healthState;

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	// Look up the texture color.
	float4 tex = tex2D(TextureSampler, texCoord);
	float3 colrgb = tex.rgb;
	float3 grey = float3(0.2, 0.49, 0.01);
	float greycolor = dot(colrgb, grey);

	colrgb.rgb = lerp(dot(greycolor, grey), colrgb, healthState);
	return float4(colrgb.rgb, tex.a);
}


technique Desaturate
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}