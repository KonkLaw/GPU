#include "BaseTypes.h"

void CheckResult(HRESULT result)
{
	if (result != 0)
		throw new Exception();
}