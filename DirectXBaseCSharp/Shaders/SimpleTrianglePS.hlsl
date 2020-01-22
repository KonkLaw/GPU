#include "SimpleTriangleHeader.hlsli"

float4 main(Vertex vertex) : SV_Target
{
	return float4(vertex.color, 1);
}