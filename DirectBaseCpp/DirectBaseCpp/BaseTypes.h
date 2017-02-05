#pragma once

#include <windows.h>
#include <string>
#include <fstream>
#include <cwchar>
#include <vector>

using namespace std;

class Exception { };

class Size
{

public:
	const int Width;
	const int Height;

	Size(int width, int height)
		: Width(width), Height(height) { }
};

class FrameCounter
{
	double _timerFreq;
	size_t _currnetFrameIndex = 0;
	double _lastFrameStart;
	double* _times;

	double ClockGetTime()
	{
		static unsigned __int64 tmp;
		QueryPerformanceCounter((LARGE_INTEGER*)&tmp);
		return ((double)tmp) * _timerFreq;
	}

public:
	static const int TimesCount = 1000;

	FrameCounter()
	{
		_times = new double[TimesCount];
		unsigned __int64 tmp;
		QueryPerformanceFrequency((LARGE_INTEGER*)&tmp);
		_timerFreq = 1.0 / (double)tmp;
	}

	~FrameCounter()
	{
		delete[] _times;
	}

	void StartFrame()
	{
		_currnetFrameIndex = (++_currnetFrameIndex) % TimesCount;
		_lastFrameStart = ClockGetTime();
	}

	void StopFrame()
	{
		_times[_currnetFrameIndex] = ClockGetTime() - _lastFrameStart;
	}

	std::string GetInfo()
	{
		double res = 0;
		for (size_t i = 0; i < TimesCount; i++)
		{
			res += _times[i];
		}
		res = res / TimesCount;
		std::string text = std::string("AVG=") + std::to_string(res) + std::string(" FPS=") + std::to_string(1.0 / res);
		return text;
	}
};

class Helper
{
public:
	static vector<char>* ReadAllFile(const wchar_t* path)
	{
		ifstream file(path, ios::binary | ios::ate);
		streamsize size = file.tellg();
		if (size < 0)
			throw new Exception();
		file.seekg(0, ios::beg);
		vector<char>* buffer = new vector<char>(size);
		if (file.read(buffer->data(), size))
		{
			return buffer;
		}
		else
			throw new Exception();
	}
};

void CheckResult(HRESULT result);