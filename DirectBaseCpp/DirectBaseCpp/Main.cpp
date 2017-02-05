#include "CustomWindow.h"

#ifdef _DEBUG
	#define _CRTDBG_MAP_ALLOC  
	#include <stdlib.h>  
	#include <crtdbg.h>  
	#define new new(_NORMAL_BLOCK, __FILE__, __LINE__)
#endif


int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow)
{
	CustomWindow* window = new CustomWindow(hInstance, nCmdShow);
	window->RunLoop();
	delete window;

#ifdef _DEBUG
	_CrtDumpMemoryLeaks();
#endif
}