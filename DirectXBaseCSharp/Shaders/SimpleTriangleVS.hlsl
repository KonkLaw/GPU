#include "SimpleTriangleHeader.hlsli"

Vertex main(float4 position : POSITION, float3 color : COLOR)
{		
	Vertex vertex = (Vertex)0;
	vertex.position = position;
	vertex.color = color;
	return vertex;
}