// Effect dynamically changes color saturation.

sampler TextureSampler : register(s0);
float healthState;

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	// Look up the texture color.
	float4 tex = tex2D(TextureSampler, texCoord);

	if (texCoord.y > 1 - healthState)
		return tex;
	return float4 (0, 0, 0, 0);
}


technique Desaturate
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}