#include "Header.hlsli"

struct PixelInfo
{
	float x;
	float y;
	uint coverage;
	uint sampleIndex;
	float2 textCoord;
};

AppendStructuredBuffer<PixelInfo> ress : register(u1);

float4 main(Vertex vertex, uint coverage : SV_Coverage, uint sampleIndex : SV_SampleIndex) : SV_Target
{
	PixelInfo info = (PixelInfo)0;
	info.x = vertex.position.x;
	info.y = vertex.position.y;
	info.coverage = coverage;
	info.sampleIndex = sampleIndex;
	info.textCoord = vertex.textCoord;

	ress.Append(info);
	return float4(vertex.color, 1);
}