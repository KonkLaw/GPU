#pragma once

#include <windows.h>
#include "Renderer.h"
#include "BaseTypes.h"

class CustomWindow
{
	const LPCWSTR windowClassName = L"MyWindowClass";
	FrameCounter* _frameCounter;
	Renderer* _renderer;
	HWND _windowHandle;

	void RegisteWindow(HINSTANCE hInstance);
	static LRESULT CALLBACK EventProcessor(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);
	void CreateCustomWindow(HINSTANCE hInstance, int nCmdShow);
	Size GetSize()
	{
		RECT rc;
		GetClientRect(_windowHandle, &rc);
		return Size(rc.right - rc.left, rc.bottom - rc.top);
	}
public:
	CustomWindow(HINSTANCE hInstance, int nCmdShow);
	~CustomWindow()
	{
		delete _frameCounter;
		delete _renderer;
	}

	void RunLoop();
};

