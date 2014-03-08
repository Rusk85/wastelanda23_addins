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
	typedef Tuple<Guid, String^, int> InvokeArgsTuple;
	public ref class AsyncAddinInvocationMethod : public IAddinInvocationMethod
	{
	private:
		static String^ CID_PREFIX = "___CID";
		initonly Addin^ addin;
		initonly Dictionary<Guid, String^>^ results = gcnew Dictionary<Guid, String^>;
		initonly Dictionary<Guid, Task^>^ tasks = gcnew Dictionary<Guid, Task^>;

		void InvokeImpl(Object^ object)
		{
			auto tuple = (InvokeArgsTuple^)object;
			auto result = addin->Invoke(tuple->Item2, tuple->Item3);
			results[tuple->Item1] = result;
		}

		String^ uuidString(Guid str) 
		{
			return CID_PREFIX + str;
		}

	public:
		AsyncAddinInvocationMethod(Addin^ addin)
		{
			this->addin = addin;
		}

		virtual String^ Invoke(String^ args, int maxResultSize)
		{
			if (!String::IsNullOrEmpty(args) && args->StartsWith(CID_PREFIX))
			{
				if (args->Length <= CID_PREFIX->Length) {
					return "Invalid guid " + args + " for call";
				}

				auto cidPart = args->Substring(CID_PREFIX->Length);
				System::Guid guid = System::Guid::Parse(cidPart);
				Task^ matchingTask;

				if (tasks->TryGetValue(guid, matchingTask))
				{
					matchingTask->Wait();
					return results[guid];
				}
				else
				{
					return "guid " + args + " not found";
				}
			}
			else
			{
				//System::Guid guid = CreateSequentialUuid();
				System::Guid guid = System::Guid::NewGuid();

				auto argsCopy = args;

				if (String::IsNullOrEmpty(args)) {
					argsCopy = "";
				}

				auto tuple = Tuple::Create(guid, argsCopy, maxResultSize);
				Task^ task = Task::Factory->StartNew(gcnew Action<Object^>(this, &AsyncAddinInvocationMethod::InvokeImpl), tuple);
				tasks[guid] = task;

				return uuidString(guid);
			}
			return nullptr;
		}
	};
}