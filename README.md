# gmod_csModuleLoader
A Small cpp file that will start up the dotnet core runtime, load a cs dll, parse small data, and allow you to write a c# gmod module cross-platform



# how to set up
First create a dotnet core 3.0 project include the Module.cs and ModuleExportInterface.cs files in said project then, start writting code... do keep in mind a large amount of the files are for up comming things and have been ported directly from a work in progress repo called GSharp https://github.com/DuckyC/gm_dotnet/tree/master/GSharp.


Open up the Module.cs file and fill in the top Directives:
  LoaderFriendlyName, Module_ASMFilename, Module_Namespace_Class, Module_Path
the other directives can be edited for more customization on where to find the dotnet core runtimes on different builds.
Compile the Module.cs file on windos for both 32bit & 64bit then on/for linux 64 bit (osx is unsupported because i dont have a mac).
rename these files accordingly.
Win64bit  gmsv_module_win64.dll
Win32bit  gmsv_module_win32.dll
Lin64bit  gmsv_module_linux64.dll

now create a folder to house the dotnet core runtime files and your module.
  you can customize how the core runtimes are found but the defult layout is such:
    ./Module/
      Module.dll
      win/
        64/
          * 64 bit windows runtime files
        32/
          * 32 bit windows runtime files
      lin/
        64/
          * 64 bit windows runtime files
        32/
          * 32 bit windows runtime files (when they come out)
      osx/
        64/
          * 64 bit Mac OSx runtime files (when it gets implemented)
        32/
          * 32 bit Mac OSx runtime files (when they come out)
once you have set that up your good to go, make sure that your module path is relative to the srcds_run binarys or the hl2.exe binary and that the Module_ASMFilename is the same case as your modules name (*it is recomended to use all lowercase*).
