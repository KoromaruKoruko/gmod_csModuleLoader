#include "GarrysMod/Lua/Interface.h"
#include <stdio.h>
#include <iostream>
#include <string>

// NOTICE:
//	this has not been tested on:
//		osx		64 bit
//  linux 32bit & osx 32bit are unsupported

#define LoaderFriendlyName			"cpploader"						// The domain Friendly name, should be different if loading multiple modules from
#define Module_ASMFilename			"ExampleModule"						// the files name of your assembly (do not include the file extension)
#define Module_Namespace_Class		"ExampleModule.ModuleExportInterface"	// FullName of the Loader path
#define Module_Path					"csModules/"					// where the c# Module folder is relative to hl2.exe or srcds_run
#define ClrTreePath					"csModules/dnc/"							// where the CoreClr is (Relative to your loading program)

// these are the paths to the CoreCLR Runtime files (edit if you use a different structure)
#define WinClr ClrTreePath "win"
#define LinClr ClrTreePath "lin"
#define OSXClr ClrTreePath "osx"

#define Win64Clr WinClr "64/"
#define Win32Clr WinClr "32/"
#define Lin64Clr LinClr "64/"
#define Lin32Clr LinClr "32/" // As of right now (09.10.2019) dotnet Core does not support Linux32 but it will be added later on
#define OSX64Clr OSXClr "64/"
#define OSX32Clr OSXClr "32/" // As of right now (09.10.2019) dotnet Core does not support Linux32 but it will be added later on

#define MODULEPATH Module_Path Module_ASMFilename ".dll"

// Generalize Definitions
#if defined( _WIN32 ) // Windows
#include <Windows.h>
#define FS_SEPERATOR "\\"
#define PATH_DELIMITER ";"
#ifndef MAX_PATH
#define MAX_PATH 260
#endif // !MAX_PATH

#define OS 1
#if defined (_M_X64)
#define X64 true
#define CoreCLRPath Win64Clr
#else
#define CoreCLRPath Win32Clr
#endif
#define CoreCLRDLL CoreCLRPath "/coreclr.dll"

typedef HMODULE LibPtr;

#define GetFullPath(Path, Output) GetFullPathNameA(Path, MAX_PATH, Output, NULL)
#define ImportDLL(Path) LoadLibraryExA(Path, NULL, 0)
#define GetInterface(CoreCLR, Name) GetProcAddress(CoreCLR, Name)
#define CloseDLL(Ptr) FreeLibrary(Ptr)
// search
void BuildTpaList(const char* directory, const char* extension, std::string& tpaList)
{
	std::string searchPath(directory);
	searchPath.append(FS_SEPERATOR);
	searchPath.append("*");
	searchPath.append(extension);

	WIN32_FIND_DATAA findData;
	HANDLE fileHandle = FindFirstFileA(searchPath.c_str(), &findData);

	if (fileHandle != (HANDLE)-1)
	{
		do
		{
			tpaList.append(directory);
			tpaList.append(FS_SEPERATOR);
			tpaList.append(findData.cFileName);
			tpaList.append(PATH_DELIMITER);
		} while (FindNextFileA(fileHandle, &findData));
		FindClose(fileHandle);
	}
}
#elif defined( __linux__ )
#define OS 0
#if defined (__x86_64__)
#define X64 true
#define CoreCLRPath Lin64Clr
#else
#define CoreCLRPath Lin32Clr
#endif
#define CoreCLRDLL CoreCLRPath "/libcoreclr.so"

typedef void* LibPtr;

// theses defintions have been added from the coreclrhost.h file (Edit being thay have been moved around)
// lines 21-26
#include <dirent.h>
#include <dlfcn.h>
#include <limits.h>
#define FS_SEPERATOR "/"
#define PATH_DELIMITER ":"
#define MAX_PATH PATH_MAX

#include <sys/types.h>
#include "GarrysMod/Lua/LuaBase.h"
#define GetFullPath(Path, Output) realpath(Path, Output);
#define ImportDLL(Path) dlopen(Path, RTLD_NOW | RTLD_LOCAL);
#define GetInterface(CoreCLR, Name) dlsym(CoreCLR, Name)
#define CloseDLL(Ptr) dlclose(Ptr)

//#include <cstring>

// search for dll files withing Path
void BuildTpaList(const char* directory, const char* extension, std::string& tpaList)
{
	DIR* dir = opendir(directory);
	struct dirent* entry;
	int extLength = std::strlen(extension);

	while ((entry = readdir(dir)) != NULL)
	{
		std::string filename(entry->d_name);
		int extPos = filename.length() - extLength;
		if (extPos <= 0 || filename.compare(extPos, extLength, extension) != 0)
			continue;
		tpaList.append(directory);
		tpaList.append(FS_SEPERATOR);
		tpaList.append(filename);
		tpaList.append(PATH_DELIMITER);
	}
}
#elif defined ( __APPLE__ )
#error "Not fully implemented, missing functions"
#define OS 2
#if defined (__x86_64__)
#define X64 true
#define CoreCLRPath OSX64Clr
#else
#define CoreCLRPath OSX32Clr
#endif
#define CoreCLRDLL CoreCLRPath "/libcoreclr.dylib"
#else
#error "Unknown and unsupported Target"
#endif

#if defined (X64)
typedef long long csptr;
#define Open "Open64"
#define Close "Close64"
#else
typedef long csptr;
#define Open "Open32"
#define Close "Close32"
#endif

#pragma region CORECLR API DEFINITIONS
#define CORECLR_HOSTING_API(function, ...) \
    extern "C" int function(__VA_ARGS__); \
    typedef int (*function##_ptr)(__VA_ARGS__)

CORECLR_HOSTING_API(coreclr_initialize,
	const char* exePath,
	const char* appDomainFriendlyName,
	int propertyCount,
	const char** propertyKeys,
	const char** propertyValues,
	void** hostHandle,
	unsigned int* domainId);

CORECLR_HOSTING_API(coreclr_create_delegate,
	void* hostHandle,
	unsigned int domainId,
	const char* entryPointAssemblyName,
	const char* entryPointTypeName,
	const char* entryPointMethodName,
	void** delegate);

CORECLR_HOSTING_API(coreclr_shutdown,
	void* hostHandle,
	unsigned int domainId);

#undef CORECLR_HOSTING_API

#pragma endregion

typedef int (*d_Define)(bool IsX64, unsigned char SystemID);
typedef int (*d_Open)(csptr L);
typedef int (*d_Close)(csptr L);

static d_Open Invoke_Open = NULL;
static d_Close Invoke_Close = NULL;

static LibPtr _coreCLR = NULL;
static void* _hostHandle;
static unsigned int _domainId;
static coreclr_create_delegate_ptr CreateDelegate = NULL;
static coreclr_initialize_ptr initfptr = NULL;
static coreclr_shutdown_ptr shutdownptr = NULL;
void LoadDelegates()
{
	CreateDelegate(_hostHandle, _domainId,
		Module_ASMFilename,
		Module_Namespace_Class,
		Open,
		(void**)&Invoke_Open);
	CreateDelegate(_hostHandle, _domainId,
		Module_ASMFilename,
		Module_Namespace_Class,
		Close,
		(void**)&Invoke_Close);
}

int LoadModule(lua_State* L)
{
	if (_coreCLR == NULL)
	{
		char ModuleFPath[MAX_PATH + 1];
		char CoreClrPath[MAX_PATH + 1];
		char CoreClrDll[MAX_PATH + 1];
		GetFullPath(MODULEPATH, ModuleFPath);
		GetFullPath(CoreCLRPath, CoreClrPath);
		GetFullPath(CoreCLRDLL, CoreClrDll);

		_coreCLR = ImportDLL(CoreClrDll);

		if (_coreCLR == NULL)
		{
			L->luabase->GetField(-10002, "print");
			L->luabase->PushString("failed to import CoreClrDLL");
			L->luabase->Call(1, 0);
			perror("failed to import CoreClrDLL");
			return -1;
		}
		initfptr = (coreclr_initialize_ptr)GetInterface(_coreCLR, "coreclr_initialize");
		if (initfptr == NULL)
		{
			if (L != NULL)
			{
				L->luabase->GetField(-10002, "print");
				L->luabase->PushString("failed to generate init delegate ptr");
				L->luabase->Call(1, 0);
			}
			perror("failed to generate init delegate ptr");
			return -1;
		}

		std::string TpaList; // Add Trusted Assemblies by Path searching
		{
			BuildTpaList(CoreClrPath, ".dll", TpaList);
			char Buffer[MAX_PATH + 1];
			GetFullPath(Module_Path, Buffer);
			BuildTpaList(Buffer, ".dll", TpaList);

			// Add Gmod dll paths
#if _WIN32
			GetFullPath("bin/win64/", Buffer);
			BuildTpaList(Buffer, ".dll", TpaList);
			GetFullPath("bin/tools/", Buffer);
			BuildTpaList(Buffer, ".dll", TpaList);
#elif __linux__
			GetFullPath("bin/linux32/", Buffer);
			BuildTpaList(Buffer, ".dll", TpaList);
			GetFullPath("bin/linux64/", Buffer);
			BuildTpaList(Buffer, ".dll", TpaList);
#endif
		}

		const char* propertyKeys[] = { "TRUSTED_PLATFORM_ASSEMBLIES" };
		const char* propertyValues[] = { TpaList.c_str() };

		if (initfptr(CoreClrPath,            		// runtime path
			LoaderFriendlyName,						// AppDomain friendly name
			sizeof(propertyKeys) / sizeof(char*),   // Property count
			propertyKeys,                           // Property names
			propertyValues,                         // Property values
			&_hostHandle,                           // Host handle  (out)
			&_domainId)                             // AppDomain ID (out)
			!= 0)
		{
			if (L != NULL)
			{
				L->luabase->GetField(-10002, "print");
				L->luabase->PushString("Failed to create runtime enviroment");
				L->luabase->Call(1, 0);
			}
			perror("Failed to create runtime enviroment");
			return -1;
		}

		CreateDelegate = (coreclr_create_delegate_ptr)GetInterface(_coreCLR, "coreclr_create_delegate");
		shutdownptr = (coreclr_shutdown_ptr)GetInterface(_coreCLR, "coreclr_shutdown");
		if (CreateDelegate == NULL)
		{
			if (L != NULL)
			{
				L->luabase->GetField(-10002, "print");
				L->luabase->PushString("Failed to Get CreateDelegate Delegate Pointer");
				L->luabase->Call(1, 0);
			}
			perror("Failed to Get CreateDelegate Delegate Pointer");
			return -1;
		}

		// we dont need a 32 and 64 bit version of this, because it works with single bits only
		d_Define Define;
		CreateDelegate(_hostHandle, _domainId,
			Module_ASMFilename,
			Module_Namespace_Class,
			"Define",
			(void**)&Define);
#if defined (X64)
		Define(true, OS);
#else
		Define(false, OS);
#endif

		LoadDelegates();
	}
	return 0;
}

DLL_EXPORT int gmod13_open(lua_State* L)
{
	if (LoadModule(L) != -1)
		return Invoke_Open((csptr)L);
	else
		return -1;
}

DLL_EXPORT int gmod13_close(lua_State* L)
{
	if (Invoke_Close != NULL)
		return Invoke_Close((csptr)L);
	else
		return -1;

	shutdownptr(_hostHandle, _domainId); // should work
}
