#include "Header.hlsli"

Vertex main(float4 position : POSITION, float3 color : COLOR /*, float2 textCoord : TEXTCOORD*/)
{		
	if (position.x > 0)
	{
		Vertex vertex = (Vertex)0;
		vertex.position = float4(99999, 99999 ,99999, 1);
		return vertex;
	}
	Vertex vertex = (Vertex)0;
	vertex.position = position;
	vertex.color = color;
	//vertex.textCoord = textCoord;
	return vertex;
}