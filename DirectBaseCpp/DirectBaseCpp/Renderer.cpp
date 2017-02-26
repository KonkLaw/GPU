#include "Renderer.h"

Renderer::Renderer(Size size, HWND windowHandle) : sampleDescription(DXGI_SAMPLE_DESC{ 1, 0 })
{
	InitDeviceContextSwapChain(size, windowHandle);
	InitRenderTargetAndDepthStencil(size);
	InitViewPort(size);
	InitShadersAndLayout();
	CreateData();
	_immediateContext->OMSetRenderTargets(1, &_renderTargetView, _depthStencilView);
}

void Renderer::InitDeviceContextSwapChain(Size size, HWND windowHandle)
{
	DXGI_SWAP_CHAIN_DESC swapChainDesc;
	ZeroMemory(&swapChainDesc, sizeof(swapChainDesc));
	swapChainDesc.BufferCount = 1;
	swapChainDesc.BufferDesc.Format = textureFormat;
	swapChainDesc.BufferDesc.Width = size.Width;
	swapChainDesc.BufferDesc.Height = size.Height;
	swapChainDesc.BufferDesc.Scaling = DXGI_MODE_SCALING_UNSPECIFIED;
	swapChainDesc.BufferDesc.RefreshRate.Numerator = 60;
	swapChainDesc.BufferDesc.RefreshRate.Denominator = 1;
	swapChainDesc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
	swapChainDesc.SampleDesc = sampleDescription;
	swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
	swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	swapChainDesc.Windowed = TRUE;
	swapChainDesc.OutputWindow = windowHandle;

	D3D_FEATURE_LEVEL featureLevels[] = { D3D_FEATURE_LEVEL_11_0 };
	D3D_FEATURE_LEVEL featureLevel = D3D_FEATURE_LEVEL_11_0;
	CheckResult(
		D3D11CreateDeviceAndSwapChain(
			0,
			D3D_DRIVER_TYPE_HARDWARE,
			0,
			0, // flag
			featureLevels,
			1,
			D3D11_SDK_VERSION,
			&swapChainDesc,
			&_swapChain,
			&_device,
			&featureLevel,
			&_immediateContext));
	
	//UINT ql1; _device->CheckMultisampleQualityLevels(textureFormat, 1, &ql1);
	//UINT ql2; _device->CheckMultisampleQualityLevels(textureFormat, 2, &ql2);
	//UINT ql4; _device->CheckMultisampleQualityLevels(textureFormat, 4, &ql4);
	//UINT ql8; _device->CheckMultisampleQualityLevels(textureFormat, 8, &ql8);
}

void Renderer::InitRenderTargetAndDepthStencil(Size size)
{
	// Create a render target view
	ID3D11Texture2D* backBuffer = nullptr;
	_swapChain->GetBuffer(
		0, __uuidof(ID3D11Texture2D),
		reinterpret_cast<void**>(&backBuffer));
	_device->CreateRenderTargetView(
		backBuffer, nullptr, &_renderTargetView);
	backBuffer->Release();

	// Create depth stencil texture
	ID3D11Texture2D* depthStencil = nullptr;
	D3D11_TEXTURE2D_DESC descDepth;
	ZeroMemory(&descDepth, sizeof(descDepth));
	descDepth.Width = size.Width;
	descDepth.Height = size.Height;
	descDepth.MipLevels = 1;
	descDepth.ArraySize = 1;
	descDepth.Format = DXGI_FORMAT_D32_FLOAT;
	descDepth.SampleDesc = sampleDescription;
	descDepth.Usage = D3D11_USAGE_DEFAULT;
	descDepth.BindFlags = D3D11_BIND_DEPTH_STENCIL;
	descDepth.CPUAccessFlags = 0;
	descDepth.MiscFlags = 0;
	CheckResult(
		_device->CreateTexture2D(&descDepth, NULL, &depthStencil));

	// Create the depth stencil view
	D3D11_DEPTH_STENCIL_VIEW_DESC descDSV;
	ZeroMemory(&descDSV, sizeof(descDSV));
	descDSV.Format = descDepth.Format;
	descDSV.ViewDimension = sampleDescription.Count == 1 ?
		D3D11_DSV_DIMENSION_TEXTURE2D : D3D11_DSV_DIMENSION_TEXTURE2DMS;
	descDSV.Texture2D.MipSlice = 0;
	CheckResult(
		_device->CreateDepthStencilView(depthStencil, &descDSV, &_depthStencilView));
	depthStencil->Release();
}

void Renderer::InitViewPort(Size size)
{
	_viewPort.Width = (FLOAT)size.Width;
	_viewPort.Height = (FLOAT)size.Height;
	_viewPort.MinDepth = 0.0f;
	_viewPort.MaxDepth = 1.0f;
	_viewPort.TopLeftX = 0;
	_viewPort.TopLeftY = 0;
}

void Renderer::InitShadersAndLayout()
{
	std::vector<char>* psBytecode = Helper::ReadAllFile(L"CompiledShaders\\PixelShader.cso");
	_device->CreatePixelShader(
		psBytecode->data(),
		psBytecode->size(),
		NULL,
		&_pixelShader);
	delete psBytecode;

	std::vector<char>* vsBytecode = Helper::ReadAllFile(L"CompiledShaders\\VertexShader.cso");
	_device->CreateVertexShader(
		vsBytecode->data(),
		vsBytecode->size(),
		NULL,
		&_vertexShader);

	// Define the input layout
	D3D11_INPUT_ELEMENT_DESC layout[] =
	{
		{ "POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
		{ "COLOR", 0, DXGI_FORMAT_R32G32B32_FLOAT, 1, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 }
	};
	UINT numElements = ARRAYSIZE(layout);
	_device->CreateInputLayout(
		layout, numElements,
		vsBytecode->data(),
		vsBytecode->size(),
		&_vertexLayout);

	delete vsBytecode;
}

void Renderer::Render()
{
	_immediateContext->RSSetViewports(1, &_viewPort);
	_immediateContext->ClearRenderTargetView(_renderTargetView, BackgoundColor);
	_immediateContext->ClearDepthStencilView(_depthStencilView, D3D11_CLEAR_DEPTH, 1.0f, 0);

	_immediateContext->IASetInputLayout(_vertexLayout);
	_immediateContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

	UINT stride = sizeof(XMFLOAT4);
	UINT offset = 0;
	_immediateContext->IASetVertexBuffers(
		0,
		1,
		&_pointsBuffer, &stride, &offset);
	stride = sizeof(XMFLOAT3);
	_immediateContext->IASetVertexBuffers(
		1,
		1,
		&_colorsBuffer, &stride, &offset);

	_immediateContext->VSSetShader(_vertexShader, nullptr, 0);
	_immediateContext->PSSetShader(_pixelShader, nullptr, 0);
	_immediateContext->Draw(points->size(), 0);

	_swapChain->Present(0, 0);
}

//=========================== DATA ==============================
std::vector<XMFLOAT4>* CreatePoints();

void Renderer::CreateData()
{
	//points = CreatePoints();
	points = new std::vector<XMFLOAT4>();
	//points->push_back(XMFLOAT4(-0.5, -0.5, 0, 1));
	//points->push_back(XMFLOAT4(0, +0.5, 0, 1));
	//points->push_back(XMFLOAT4(+0.5, -0.5, 0, 1));
	points->push_back(XMFLOAT4(-1.1f, -1.1f, 0, 1.f));
	points->push_back(XMFLOAT4(-1.1f, 1.f, 0, 1.f));
	points->push_back(XMFLOAT4(1.f, -1.1f, 0, 1.f));

	colors = new std::vector<XMFLOAT3>();
	colors->reserve(points->size());
	for (size_t i = 0; i < points->size() / 3; i++)
	{
		float randR = (static_cast <float> (rand()) / static_cast <float> (RAND_MAX));
		float randG = (static_cast <float> (rand()) / static_cast <float> (RAND_MAX));
		float randB = (static_cast <float> (rand()) / static_cast <float> (RAND_MAX));
		colors->push_back(XMFLOAT3(randR, randG, randB));
		colors->push_back(XMFLOAT3(randR, randG, randB));
		colors->push_back(XMFLOAT3(randR, randG, randB));
	}

	D3D11_BUFFER_DESC bufferDescr;
	ZeroMemory(&bufferDescr, sizeof(bufferDescr));
	bufferDescr.Usage = D3D11_USAGE_DEFAULT;
	bufferDescr.ByteWidth = sizeof(XMFLOAT4) * points->size();
	bufferDescr.BindFlags = D3D11_BIND_VERTEX_BUFFER;
	bufferDescr.CPUAccessFlags = 0;

	D3D11_SUBRESOURCE_DATA data;
	ZeroMemory(&data, sizeof(data));
	data.pSysMem = points->data();

	CheckResult(_device->CreateBuffer(&bufferDescr, &data, &_pointsBuffer));

	bufferDescr.ByteWidth = sizeof(XMFLOAT3) * colors->size();
	data.pSysMem = colors->data();
	CheckResult(_device->CreateBuffer(&bufferDescr, &data, &_colorsBuffer));
}

void AddTraingles(std::vector<XMFLOAT4>* data,
	float xBegin, float xEnd, size_t countX,
	float yBegin, float yEnd, size_t countY,
	float z)
{
	float stepX = (xEnd - xBegin) / countX;
	float stepY = (yEnd - yBegin) / countY;

	data->reserve(data->size() + countX * countY);
	for (size_t j = 0; j < countY; j++)
	{
		float currY = yBegin + j * stepY + stepY / 2;
		for (size_t i = 0; i < countX; i++)
		{
			float currX = xBegin + i * stepX + stepX / 2;
			data->push_back(XMFLOAT4(currX - stepX / 2, currY - stepY / 2, z, 1.0f));
			data->push_back(XMFLOAT4(currX, currY + stepY / 2, z, 1.0f));
			data->push_back(XMFLOAT4(currX + stepX / 2, currY - stepY / 2, z, 1.0f));
		}
	}
}

std::vector<XMFLOAT4>* CreatePoints()
{
	//return new std::vector<XMFLOAT4>
	//{
	//	XMFLOAT4(0.0f, 0.5f, 0.5f, 1.0f),
	//	XMFLOAT4(0.5f, -0.5f, 0.5f, 1.0f),
	//	XMFLOAT4(-0.5f, -0.5f, 0.5f, 1.0f),
	//	XMFLOAT4(0.0f, 0.25f, 0.25f, 1.0f),
	//	XMFLOAT4(0.25f, -0.25f, 0.25f, 1.0f),
	//	XMFLOAT4(-0.25f, -0.25f, 0.25f, 1.0f),
	//};

	auto result = new std::vector<XMFLOAT4>();

	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.90f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.85f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.80f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.75f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.70f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.65f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.60f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.55f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.50f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.45f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.40f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.35f);
	AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.30f);

	return result;
}