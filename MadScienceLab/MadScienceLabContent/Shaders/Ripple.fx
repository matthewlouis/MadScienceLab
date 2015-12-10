sampler2D Texture0;
float fT; //semantic - Time0_X


float4 ps_main(float2 texCoord  : TEXCOORD0) : COLOR
{
	return tex2D(Texture0, texCoord + float2(0, sin(texCoord.y * 16 + fT) * 0.04));
}

technique Desaturate
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 ps_main();
	}
}