#pragma once

#include "BaseTypes.h"

#include <d3d11_1.h>
#include <d3dcompiler.h>
#include <directxmath.h>
#include <vector>

using namespace DirectX;

struct SimpleVertex
{
	XMFLOAT4 Pos;
};

class Renderer
{
	const DXGI_FORMAT textureFormat = DXGI_FORMAT_R8G8B8A8_UNORM;

	ID3D11InputLayout*      _vertexLayout = nullptr;
	ID3D11Buffer*           _pointsBuffer = nullptr;
	ID3D11Buffer*           _colorsBuffer = nullptr;

	D3D11_VIEWPORT _viewPort;

	std::vector<XMFLOAT4>* points;
	std::vector<XMFLOAT3>* colors;

	void InitDeviceContextSwapChain(Size size, HWND windowHandle);
	void InitRenderTargetAndDepthStencil(Size size);
	void InitViewPort(Size size);
	void InitShadersAndLayout();
	void CreateData();

	const float BackgoundColor[4] = { 0.0f, 0.125f, 0.3f, 1.0f }; //red, green, blue, alpha

protected:
	const DXGI_SAMPLE_DESC sampleDescription;

	ID3D11Device*           _device = nullptr;
	ID3D11DeviceContext*    _immediateContext = nullptr;
	IDXGISwapChain*			_swapChain = nullptr;

	ID3D11RenderTargetView* _renderTargetView = nullptr;
	ID3D11DepthStencilView* _depthStencilView = nullptr;

	ID3D11VertexShader*     _vertexShader = nullptr;
	ID3D11PixelShader*      _pixelShader = nullptr;

public:
	Renderer(Size size, HWND windowHandle);
	virtual void Render();

	virtual ~Renderer()
	{
		//_immediateContext->OMSetRenderTargets(1, nullptr, nullptr); // remove pointer to renderTarget and depthSenticil
		_device->Release();
		_immediateContext->Release();
		_swapChain->Release();

		_renderTargetView->Release();
		_depthStencilView->Release();
		_vertexShader->Release();
		_pixelShader->Release();
		_vertexLayout->Release();
		_pointsBuffer->Release();
		_colorsBuffer->Release();

		delete points;
		delete colors;
	}
};

