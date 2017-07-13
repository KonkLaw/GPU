#include "CustomWindow.h"

CustomWindow::CustomWindow(HINSTANCE hInstance, int nCmdShow)
{
	_frameCounter = new FrameCounter();
	RegisteWindow(hInstance);
	CreateCustomWindow(hInstance, nCmdShow);

	//_renderer = new Renderer(GetSize(), _windowHandle);
	_renderer = new RendererDebugPixel(GetSize(), _windowHandle);
}

void CustomWindow::RegisteWindow(HINSTANCE hInstance)
{
	// Register class
	WNDCLASSEX wndClassExx;
	wndClassExx.cbSize = sizeof(WNDCLASSEX);
	wndClassExx.style = CS_HREDRAW | CS_VREDRAW;
	wndClassExx.lpfnWndProc = EventProcessor;
	wndClassExx.cbClsExtra = 0;
	wndClassExx.cbWndExtra = 0;
	wndClassExx.hInstance = hInstance;
	wndClassExx.hIcon = nullptr;
	wndClassExx.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wndClassExx.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wndClassExx.lpszMenuName = nullptr;
	wndClassExx.lpszClassName = windowClassName;
	wndClassExx.hIconSm = nullptr;
	if (!RegisterClassEx(&wndClassExx))
		throw new Exception();
}

LRESULT CustomWindow::EventProcessor(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	PAINTSTRUCT ps;
	HDC hdc;

	switch (message)
	{
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);
		EndPaint(hWnd, &ps);
		break;

	case WM_DESTROY:
		PostQuitMessage(0);
		break;

		// Note that this tutorial does not handle resizing (WM_SIZE) requests,
		// so we created the window without the resize border.

	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}

	return 0;
}

void CustomWindow::CreateCustomWindow(HINSTANCE hInstance, int nCmdShow)
{
	RECT rectangle = { 0, 0, 200, 200 };
	auto windowStyle = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX;
	AdjustWindowRect(&rectangle, windowStyle, FALSE);
	_windowHandle = CreateWindow(
		windowClassName,
		L"Test",
		windowStyle,
		CW_USEDEFAULT, CW_USEDEFAULT,
		rectangle.right - rectangle.left,
		rectangle.bottom - rectangle.top, nullptr, nullptr, hInstance,
		nullptr);
	CheckResult(!_windowHandle);

	ShowWindow(_windowHandle, nCmdShow);
}

void CustomWindow::RunLoop()
{
	// Main message loop
	MSG msg = { 0 };
	while (WM_QUIT != msg.message)
	{
		static int i = 0;
		if (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
		else
		{
			i++;
			_frameCounter->StartFrame();
			_renderer->Render();
			_frameCounter->StopFrame();
			if (i == FrameCounter::TimesCount)
			{
				i = 0;
				std::string res = _frameCounter->GetInfo();
				std::wstring unicode(res.begin(), res.end());
				SetWindowText(_windowHandle, unicode.c_str());
			}
		}
	}
}
