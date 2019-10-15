# gmod_csModuleLoader
A Small cpp file that will start up the dotnet core runtime, load a cs dll, parse small data, and allow you to write a c# gmod module cross-platform



# how to set up
First create a dotnet core 3.0 project include the Module.cs and ModuleExportInterface.cs files in said project then, start writting code... do keep in mind a large amount of the files are for up comming things and have been ported directly from a work in progress repo called GSharp https://github.com/DuckyC/gm_dotnet/tree/master/GSharp.


Open up the Module.cpp file and fill in the top Directives:
  LoaderFriendlyName, Module_ASMFilename, Module_Namespace_Class, Module_Path
the other directives can be edited for more customization on where to find the dotnet core runtimes on different builds.
Compile the Module.cs file on windos for both 32bit & 64bit then on/for linux 64 bit (osx is unsupported because i dont have a mac).
rename these files accordingly.
```
Win64bit  gmsv_module_win64.dll
Win32bit  gmsv_module_win32.dll
Lin64bit  gmsv_module_linux64.dll
```
now create a folder to house the dotnet core runtime files and your module.
   you can customize how the core runtimes are found but the defult layout is such:
```
./Module/
      Module.dll
      dnc/
        win64/
          * 64 bit windows runtime files
        win32/
          * 32 bit windows runtime files
        lin64/
          * 64 bit windows runtime files
        lin32/
          * 32 bit windows runtime files (when they come out)
        osx64/
          * 64 bit Mac OSx runtime files (when it gets implemented)
        osx32/
          * 32 bit Mac OSx runtime files (when they come out)
```
once you have set that up your good to go, make sure that your module path is relative to the srcds_run binarys or the hl2.exe binary and that the Module_ASMFilename is the same case as your modules name (*it is recomended to use all lowercase*).

## Known Issues/Bugs/Changes-Needed
Cleaner Way to Import SourceSDK Librarys or other Known Librarys with different paths/names.
Better Lua Intergration, I Will be making it easier and clearer to wrap lua objects into c#
Wiki And function summary's, will happen but not now
Gmod Crashes when the module does, I will make an error reporting thing once i'v cleaned up the lua base/intergration
Need to fill in Object Wrappers.
Need to fix pre-mature referance disposure, will make smart refrance handles for lua objects.
