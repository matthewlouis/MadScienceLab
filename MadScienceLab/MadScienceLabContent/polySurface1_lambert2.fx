//TODO: Modify the folowing code: 

int GlobalParameter : SasGlobal 
< 
   int3 SasVersion = {1, 1, 0}; 
   string SasEffectDescription = "This is a sample effect"; 
   string SasEffectCompany = "cgdev.net"; 
>; 

float4x4 World      : WORLD <string SasBindAddress = "Sas.Skeleton.MeshToJointToWorld[0]";>; 
float4x4 View       : VIEW <string SasBindAddress = "Sas.Camera.WorldToView";>; 
float4x4 Projection : PROJECTION <string SasBindAddress = "Sas.Camera.Projection";>; 

bool bEmissiveColor = false; 
bool bLight = true; 
float3 lightDirection = { 0, 0, -1 }; 

float4 DiffuseColor  = { 0.800000f, 0.800000f, 0.800000f, 1.000000f }; 
float  SpecularPower = 0.000000f; 
float4 SpecularColor = { 0.000000f, 0.000000f, 0.000000f, 1.000000f }; 
float4 EmissiveColor = { 0.000000f, 0.000000f, 0.000000f, 1.000000f }; 

texture file2; 
texture file2_; 

sampler2D file2Sampler = sampler_state  // TexCoord0 
{ 
   Texture = <file2>; 
   MinFilter = Linear; 
   MagFilter = Linear; 
   MipFilter = Linear; 
   AddressU  = Wrap;     
   AddressV  = Wrap;     
}; 

sampler2D file2_Sampler = sampler_state  // TexCoord0 
{ 
   Texture = <file2_>; 
   MinFilter = Linear; 
   MagFilter = Linear; 
   MipFilter = Linear; 
   AddressU  = Wrap;     
   AddressV  = Wrap;     
}; 

struct VS_INPUT 
{ 
   float4 Position  : POSITION; 
   float2 TexCoord0 : TEXCOORD0;
   float3 N         : NORMAL;
   float3 T         : TANGENT;
   float3 B         : BINORMAL;
   float4 VertexColor : COLOR0;
}; 

struct VS_OUTPUT 
{ 
   float4 Position  : POSITION; 
   float2 TexCoord0 : TEXCOORD0;
   float4 VertexColor : COLOR0;
   float LightStrength     : COLOR1;
}; 

VS_OUTPUT VS( VS_INPUT IN ) 
{ 
   //TODO: Modify the folowing code: 
   VS_OUTPUT OUT;	 
   float4 oPos = mul( mul( IN.Position, World ), View ); 
   OUT.Position = mul( oPos, Projection ); 
   OUT.VertexColor = IN.VertexColor; 
   OUT.TexCoord0.xy = IN.TexCoord0.xy; 
   float3 WorldNormal = normalize( mul( IN.N, (float3x3)World ) );
   OUT.LightStrength =  max(0, dot( WorldNormal, lightDirection ));
   return OUT;
}; 

float4  PS( VS_OUTPUT vout ) : COLOR 
{ 
   //TODO: Modify the folowing code: 
   float4 color = tex2D( file2Sampler, vout.TexCoord0 ); 
   color.rgb = color.rgb * vout.LightStrength; 
   color = color * DiffuseColor; 
   if(bEmissiveColor)
     color.rgb += EmissiveColor.rgb;
   color.rgb = vout.VertexColor.rgb * vout.VertexColor.a + color.rgb * ( 1.0f - vout.VertexColor.a );
   return  color; 
}; 

technique DefaultTechnique 
{ 
   pass p0 
   { 
      VertexShader = compile vs_2_0  VS(); 
      PixelShader  = compile ps_2_0  PS(); 
   } 
} 
