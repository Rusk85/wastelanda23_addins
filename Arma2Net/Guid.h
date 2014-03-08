/*
* Copyright 2013 Arma2NET Developers
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

#include "Addin.h"

using namespace System;
using namespace System::Runtime::InteropServices;



namespace Arma2Net
{
	[DllImport("rpcrt4.dll", SetLastError = true)]
	extern "C" int UuidCreateSequential(System::Guid guid);

	static System::Guid CreateSequentialUuid()
	{
		const int RPC_S_OK = 0;
		System::Guid g;
		int hr = UuidCreateSequential(g);
		if (hr != RPC_S_OK)
			throw gcnew ApplicationException("UuidCreateSequential failed: " + hr);
		return g;
	}

	static System::Guid NewGuid()
	{
		return CreateSequentialUuid();
	}
}