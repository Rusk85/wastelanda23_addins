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
#include "AddinAttribute.h"
#include "AddinInfo.h"
#include "AddinManager.h"
#include "SyncAddinInvocationMethod.h"
#include "Utils.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Reflection;

namespace Arma2Net
{
	List<String^>^ FindAddinFiles()
	{
		auto addinFiles = gcnew List<String^>;
		for each (auto directory in Directory::EnumerateDirectories(Utils::AddinDirectory))
		{
			for each (auto file in Directory::EnumerateFiles(directory, "*.dll"))
				addinFiles->Add(file);
		}
		return addinFiles;
	}

	List<AddinInfo^>^ FindAddins(Assembly^ assembly)
	{
		auto addins = gcnew List<AddinInfo^>;
		for each (auto type in assembly->GetTypes())
		{
			auto attributes = type->GetCustomAttributes(AddinAttribute::typeid, true);
			if (attributes->Length == 0)
				continue;

			auto info = gcnew AddinInfo;
			info->Type = type;
			info->Attribute = (AddinAttribute^)attributes[0];
			addins->Add(info);
		}
		return addins;
	}

	AddinManager::AddinManager(void)
	{
		loadedAddins = gcnew Dictionary<String^, Addin^>(StringComparer::OrdinalIgnoreCase);
	}

	void AddinManager::LoadAddins(void)
	{
		auto files = FindAddinFiles();
		for each (auto file in files)
		{
			Utils::Log("Loading addin assembly from {0}", file);
			auto assembly = Assembly::LoadFile(file);
			auto addins = FindAddins(assembly);

			Utils::Log("Found {0} addins", addins->Count);

			for each (auto addin in addins)
			{
				Utils::Log("Found addin type {0}", addin->Type);

				auto attr = addin->Attribute;
				if (attr->Name == nullptr)
				{
					Utils::Log("The addin name for {0} was not specified; ignoring.", addin->Type);
					continue;
				}

				auto logStr = "Loading addin " + attr->Name;
				if (attr->Version != nullptr)
					logStr += " version " + attr->Version;
				if (attr->Author != nullptr)
					logStr += " by " + attr->Author;
				if (attr->Description != nullptr)
					logStr += " (" + attr->Description + ")";
				Utils::Log(logStr);

				try
				{
					auto obj = (Addin^)Activator::CreateInstance(addin->Type);
					loadedAddins->Add(attr->Name, obj);
				}
				catch (Exception^ e)
				{
					Utils::Log("Failed to load addin {0}", attr->Name);
					Utils::Log(e->ToString());
				}
			}
		}
	}

	String^ AddinManager::InvokeAddin(String^ name, String^ args, int maxResultSize)
	{
		Addin^ addin;
		if (!loadedAddins->TryGetValue(name, addin))
			throw gcnew InvalidOperationException("Cannot locate addin " + name);

		if (addin->InvocationMethod == nullptr)
			addin->InvocationMethod = gcnew SyncAddinInvocationMethod(addin);

		return addin->InvocationMethod->Invoke(args, maxResultSize);
	}
}