#include "Header.hlsli"

float4 main(Vertex ver) : SV_Target
{
	return float4(ver.col,1);
}