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

#include <Windows.h>
#include <msclr\marshal.h>
#include "Bridge.h"
#include "AddinManager.h"
#include "Utils.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Text;

namespace Arma2Net
{
	Assembly^ Bridge::ResolveAssembly(Object^ sender, ResolveEventArgs^ e)
	{
		auto requestedAssembly = gcnew AssemblyName(e->Name);

		Utils::Log("Resolving assembly {0}", requestedAssembly->Name);

		Assembly^ assembly;
		if (assemblyCache->TryGetValue(requestedAssembly->FullName, assembly))
		{
			Utils::Log("Retrieved assembly {0} from assembly cache", requestedAssembly->Name);
			return assembly;
		}

		auto extensions = gcnew array<String^> { ".dll", ".exe" };

		auto directories = gcnew List<String^>;
		directories->Add(Utils::BaseDirectory); // for our own assemblies
		directories->AddRange(Directory::EnumerateDirectories(Utils::AddinDirectory)); // for a dependency in an addin folder

		for each (auto directory in directories)
		{
			for each (auto extension in extensions)
			{
				try
				{
					auto filename = Path::Combine(directory, requestedAssembly->Name + extension);
					Utils::Log("Loading assembly {0} from {1}", requestedAssembly->Name, filename);
					auto assembly = Assembly::LoadFrom(filename);
					assemblyCache->Add(requestedAssembly->FullName, assembly);
					return assembly;
				}
				catch (...) { }
			}
		}
		return nullptr;
	}

	static void LogUnhandledException(Object^ sender, UnhandledExceptionEventArgs^ e)
	{
		auto ex = e->ExceptionObject;
		Utils::Log("Fatal unhandled exception of type {0}", ex->GetType());
		Utils::Log(ex->ToString());
	}

	Bridge::Bridge(void)
	{
		Utils::Log("Arma2NET initializing on CLR {0}", Environment::Version);
		assemblyCache = gcnew Dictionary<String^, Assembly^>();
		AppDomain::CurrentDomain->AssemblyResolve += gcnew ResolveEventHandler(ResolveAssembly);
		AppDomain::CurrentDomain->UnhandledException += gcnew UnhandledExceptionEventHandler(LogUnhandledException);
		addinManager = gcnew AddinManager();
		Utils::Log("Loading addins");
		try
		{
			addinManager->LoadAddins();
		}
		catch (Exception^ e)
		{
			Utils::Log("Failed to load addins");
			Utils::Log(e->ToString());
		}
	}

	static Tuple<String^, String^>^ GetAddinNameAndArgs(String^ str)
	{
		auto split = str->Split(gcnew array<wchar_t> { ' ' }, 2);
		return Tuple::Create(split[0], split->Length > 1 ? split[1] : nullptr);
	}

	String^ Bridge::InvokeFunction(String^ name, String^ args, int maxResultSize)
	{
		if (String::IsNullOrEmpty(name))
			return nullptr;
		return addinManager->InvokeAddin(name, args, maxResultSize);
	}

	void Bridge::InvokeFunction(char* output, int outputSize, const char* function)
	{
		auto maxResultSize = outputSize - 1; // reserve last byte

		auto functionString = gcnew String(function);

		auto addinNameAndArgs = GetAddinNameAndArgs(functionString);

		String^ result = nullptr;
		try
		{
			result = Bridge::InvokeFunction(addinNameAndArgs->Item1, addinNameAndArgs->Item2, maxResultSize);
			if (result == nullptr)
				return;
		}
		catch (Exception^ e)
		{
			result = "throw \"" + e->GetType() + "\"";
			Utils::Log("Failed to invoke function {0}", functionString);
			Utils::Log(e->ToString());
		}

		auto byteCount = Encoding::ASCII->GetByteCount(result);
		if (byteCount > maxResultSize)
		{
			result = "throw \"ResultTooLong\"";
			Utils::Log("Failed to return the result of {0} because it is too long ({0} > {1})", functionString, byteCount, maxResultSize);
		}

		msclr::interop::marshal_context context;
		strncpy_s(output, outputSize, context.marshal_as<const char*>(result), _TRUNCATE);
	}
}