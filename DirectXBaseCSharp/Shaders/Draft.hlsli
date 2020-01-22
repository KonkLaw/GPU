//struct Vertex
//{
//	float4 position : SV_POSITION;
//	float3 color : COLOR;
//	//float2 textCoord : TEXTCOORD;
//};
//
//#include "Header.hlsli"
//
//
//
//#include "Header.hlsli"
//
//Vertex main(float4 position : POSITION, float3 color : COLOR /*, float2 textCoord : TEXTCOORD*/)
//{
//	//if (position.x > 0)
//	//{
//	//	Vertex vertex = (Vertex)0;
//	//	vertex.position = float4(99999, 99999 ,99999, 1);
//	//	return vertex;
//	//}
//	Vertex vertex = (Vertex)0;
//	vertex.position = position;
//	vertex.color = color;
//	//vertex.textCoord = textCoord;
//	return vertex;
//}
//
//
////SamplerState clampSampler
////{
////	Filter = MIN_MAG_MIP_LINEAR;
////	AddressU = Clamp; // border sampling in U
////	AddressV = Clamp; // border sampling in V
////	AddressW = Clamp;
////};
////
////Texture2D text : register(t0);
//
//float4 main(Vertex vertex) : SV_Target
//{
//	//clip(400 - vertex.position.x);
//	return float4(vertex.color, 1);
////float3 color = text.Sample(clampSampler, vertex.textCoord);
////double cos1 = 0;
////double sin1 = 0;
////for (int i = 0; i < 1000; i++)
////{
////	cos1 += cos(color.r + vertex.textCoord.x + i * 0.01);
////	sin1 += sin(color.b + vertex.textCoord.y + i * 0.01);
////}
////return float4(color.r, cos1, sin1, 1);
//
//
//}