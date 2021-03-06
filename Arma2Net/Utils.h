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

#pragma once

#include "Addin.h"

namespace Arma2Net
{
	public ref class Utils abstract sealed
	{
	private:
		static System::IO::StreamWriter^ logWriter;
		static System::String^ baseDirectory;
		static System::String^ addinDirectory;
		static System::String^ logDirectory;
	internal:
		static System::Collections::Generic::Dictionary<System::String^, Addin^>^ LoadedAddins;
	public:
		static property System::String^ BaseDirectory { System::String^ get(void); private: void set(System::String^ value); }
		static property System::String^ AddinDirectory { System::String^ get(void); private: void set(System::String^ value); }
		static property System::String^ LogDirectory { System::String^ get(void); private: void set(System::String^ value); }
		static Utils();
		static void Log(System::String^ format, ... array<Object^>^ args);
		static void Log(System::String^ message);
	};
}