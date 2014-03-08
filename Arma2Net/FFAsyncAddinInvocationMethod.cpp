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
#include "Guid.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Threading::Tasks;

namespace Arma2Net
{
	typedef Tuple<String^, int> InvokeArgsTuple;
	public ref class FFAsyncAddinInvocationMethod : public IAddinInvocationMethod
	{
	private:
		initonly Addin^ addin;

		void InvokeImpl(Object^ object)
		{
			auto tuple = (InvokeArgsTuple^)object;
			addin->Invoke(tuple->Item1, tuple->Item2);
		}

		
	public:
		FFAsyncAddinInvocationMethod(Addin^ addin)
		{
			this->addin = addin;
		}

		virtual String^ Invoke(String^ args, int maxResultSize)
		{
				auto argsCopy = args;

				if (String::IsNullOrEmpty(args)) {
					argsCopy = "";
				}

				auto tuple = Tuple::Create(argsCopy, maxResultSize);
				Task^ task = Task::Factory->StartNew(gcnew Action<Object^>(this, &FFAsyncAddinInvocationMethod::InvokeImpl), tuple);
				return nullptr;
		}
	};
}