#pragma once
#include <d3d11_1.h>

class ResourceHelper
{

public:

	template <class T>
	static ID3D11Buffer* CreateVertexBuffer(ID3D11Device* device, size_t count, T* pointer)
	{
		D3D11_BUFFER_DESC bufferDescr;
		ZeroMemory(&bufferDescr, sizeof(bufferDescr));
		bufferDescr.Usage = D3D11_USAGE_DEFAULT;
		bufferDescr.ByteWidth = sizeof(T) * count;
		bufferDescr.BindFlags = D3D11_BIND_VERTEX_BUFFER;
		bufferDescr.CPUAccessFlags = 0;

		D3D11_SUBRESOURCE_DATA subresourceData;
		ZeroMemory(&subresourceData, sizeof(subresourceData));
		subresourceData.pSysMem = pointer;

		ID3D11Buffer* _pointsBuffer = nullptr;
		CheckResult(device->CreateBuffer(&bufferDescr, &subresourceData, &_pointsBuffer));
		return _pointsBuffer;
	}
};

