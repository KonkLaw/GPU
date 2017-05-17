#include "Header.hlsli"

Vertex main(float4 position : POSITION, float3 color : COLOR, float2 textCoord : TEXTCOORD)
{
	Vertex vertex = (Vertex)0;
	vertex.position = position;
	vertex.color = color;
	//vertex.textCoord = textCoord;
	return vertex;
}