#pragma once
#include "Renderer.h"

class PixelInfo
{
public:
	float x;
	float y;
	unsigned int coverage;
	unsigned int sampleIndex;
	float tx;
	float ty;

	static PixelInfo GetDefault()
	{
		PixelInfo newOne = PixelInfo();
		newOne.x = -999;
		newOne.y = -999;
		newOne.coverage = 999;
		newOne.coverage = 999;
		return newOne;
	}
};

class RendererDebugPixel : public Renderer
{

private:

	ID3D11Buffer* _buffer = nullptr;
	ID3D11UnorderedAccessView* _uav = nullptr;

	UINT counter = 0;
	int pixelsCount = 0;
	int framePassed = 0;

public:
	RendererDebugPixel(Size size, HWND windowHandle) : Renderer(size, windowHandle)
	{
		InitNewPixelShaders();
		InitUAV(size);
	}

	void InitNewPixelShaders()
	{
		// dispose old and use new;
		_pixelShader->Release();

		std::vector<char>* psBytecode = Helper::ReadAllFile(L"CompiledShaders\\PixelShaderWithOutput.cso");
		_device->CreatePixelShader(
			psBytecode->data(),
			psBytecode->size(),
			NULL,
			&_pixelShader);
		delete psBytecode;
	}

	void InitUAV(Size size)
	{
		pixelsCount = Renderer::sampleDescription.Count * (size.Height * size.Width);

		// buffer for data
		D3D11_BUFFER_DESC buffDesc;
		ZeroMemory(&buffDesc, sizeof(buffDesc));
		buffDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_UNORDERED_ACCESS;
		buffDesc.ByteWidth = sizeof(PixelInfo) * pixelsCount;
		buffDesc.MiscFlags = D3D11_RESOURCE_MISC_BUFFER_STRUCTURED;
		buffDesc.StructureByteStride = sizeof(PixelInfo);
		buffDesc.Usage = D3D11_USAGE_DEFAULT;

		// array for buffer
		PixelInfo* emptyArray = new PixelInfo[pixelsCount];
		for (size_t i = 0; i < pixelsCount; i++)
			emptyArray[i] = PixelInfo::GetDefault();
		D3D11_SUBRESOURCE_DATA data;
		ZeroMemory(&data, sizeof(data));
		data.pSysMem = emptyArray;

		auto hr = _device->CreateBuffer(&buffDesc, &data, &_buffer);

		// UAV
		D3D11_UNORDERED_ACCESS_VIEW_DESC uavDesc;
		ZeroMemory(&uavDesc, sizeof(uavDesc));
		uavDesc.Format = DXGI_FORMAT_UNKNOWN;
		uavDesc.ViewDimension = D3D11_UAV_DIMENSION_BUFFER;
		uavDesc.Buffer.FirstElement = 0;
		uavDesc.Buffer.NumElements = pixelsCount;
		uavDesc.Buffer.Flags = D3D11_BUFFER_UAV_FLAG_APPEND;
		hr = _device->CreateUnorderedAccessView(_buffer, &uavDesc, &_uav);

		_immediateContext->OMSetRenderTargetsAndUnorderedAccessViews(1, &_renderTargetView, _depthStencilView, 1, 1, &_uav, &counter);
	}

	void Render()
	{
		if (framePassed == 0)
		{
			framePassed++;
		}
		else if (framePassed == 1)
		{
			framePassed++;

			D3D11_BUFFER_DESC desc;
			_buffer->GetDesc(&desc);

			D3D11_BUFFER_DESC buffReadDesc;
			ZeroMemory(&buffReadDesc, sizeof(buffReadDesc));
			buffReadDesc.ByteWidth = desc.ByteWidth;
			buffReadDesc.MiscFlags = D3D11_RESOURCE_MISC_BUFFER_STRUCTURED;
			buffReadDesc.StructureByteStride = desc.StructureByteStride;
			buffReadDesc.Usage = D3D11_USAGE_STAGING;
			buffReadDesc.CPUAccessFlags = D3D10_CPU_ACCESS_READ;

			ID3D11Buffer* _bufferRead = nullptr;
			auto hr = _device->CreateBuffer(&buffReadDesc, NULL, &_bufferRead);
			_immediateContext->CopyResource(_bufferRead, _buffer);
			D3D11_MAPPED_SUBRESOURCE ms;
			hr = _immediateContext->Map(_bufferRead, 0, D3D11_MAP_READ, 0, &ms);

			PixelInfo* result = (PixelInfo*)ms.pData;

			ofstream myfile;
			myfile.open("result.txt");
			for (int i = 0; i < pixelsCount; i++)
			{
				PixelInfo info = result[i];
				myfile
					<< "x=" << info.x
					<< " y=" << info.y
					<< " cov=" << info.coverage
					<< " si=" << info.sampleIndex
					<< " textX=" << info.tx
					<< " textY=" << info.ty
					<< endl;
				//if (info.coverage != 1 && info.coverage != 999)
				//	int y = 1;
			}

			myfile.close();

			_immediateContext->Unmap(_bufferRead, 0);
		}

		// base class call.
		Renderer::Render();
	}

	~RendererDebugPixel()
	{
		_uav->Release();
		_buffer->Release();
	}
};

