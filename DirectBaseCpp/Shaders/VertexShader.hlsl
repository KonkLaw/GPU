#include "Header.hlsli"

Vertex main(float4 Pos : POSITION, float3 Col : COLOR)
{
	Vertex v = (Vertex)0;
	v.position = Pos;
	v.color = Col;
	//v.textCoord = Pos.xy / 2 + float2(0.5, 0.5);
	return v;
}