using System.ComponentModel;
using System.Runtime.InteropServices;
using System;
namespace GMLoaded.Source.Interaces
{
    public interface IFileSystem : IAppSystem, IBaseFileSystem
    {
        Boolean IsSteam();

        /// <param name='nExtraAppId'>Default: -1</param>
        IntPtr MountSteamContent([Optional()] [DefaultValue(-1)] Int32 nExtraAppId);

        /// <param name='pPath'></param>
        /// <param name='pathID'></param>
        /// <param name='addType'></param>
        void AddSearchPath(String pPath, String pathID, IntPtr addType);

        /// <param name='pPath'></param>
        /// <param name='pathID'>Default: 0</param>
        Boolean RemoveSearchPath(String pPath, [Optional()] [DefaultValue(0)] String pathID);

        void RemoveAllSearchPaths();

        /// <param name='szPathID'></param>
        void RemoveSearchPaths(String szPathID);

        /// <param name='pPathID'></param>
        /// <param name='bRequestOnly'></param>
        void MarkPathIDByRequestOnly(String pPathID, Boolean bRequestOnly);

        /// <param name='pFileName'></param>
        /// <param name='pPathID'></param>
        /// <param name='pDest'></param>
        /// <param name='maxLenInChars'></param>
        /// <param name='pathFilter'></param>
        /// <param name='pPathType'></param>
        String RelativePathToFullPath(String pFileName, String pPathID, IntPtr pDest, Int32 maxLenInChars, IntPtr pathFilter, IntPtr pPathType);

        /// <param name='pathID'></param>
        /// <param name='bGetPackFiles'></param>
        /// <param name='pDest'></param>
        /// <param name='maxLenInChars'></param>
        Int32 GetSearchPath(String pathID, Boolean bGetPackFiles, IntPtr pDest, Int32 maxLenInChars);

        /// <param name='fullpath'></param>
        /// <param name='pathID'></param>
        Boolean AddPackFile(String fullpath, String pathID);

        /// <param name='pRelativePath'></param>
        /// <param name='pathID'>Default: 0</param>
        void RemoveFile(String pRelativePath, [Optional()] [DefaultValue(0)] String pathID);

        /// <param name='pOldPath'></param>
        /// <param name='pNewPath'></param>
        /// <param name='pathID'>Default: 0</param>
        Boolean RenameFile(String pOldPath, String pNewPath, [Optional()] [DefaultValue(0)] String pathID);

        /// <param name='path'></param>
        /// <param name='pathID'>Default: 0</param>
        void CreateDirHierarchy(String path, [Optional()] [DefaultValue(0)] String pathID);

        /// <param name='pFileName'></param>
        /// <param name='pathID'>Default: 0</param>
        Boolean IsDirectory(String pFileName, [Optional()] [DefaultValue(0)] String pathID);

        /// <param name='pStrip'></param>
        /// <param name='maxCharsIncludingTerminator'></param>
        /// <param name='fileTime'></param>
        void FileTimeToString(IntPtr pStrip, Int32 maxCharsIncludingTerminator, Int32 fileTime);

        /// <param name='file'></param>
        /// <param name='nBytes'></param>
        void SetBufferSize(IntPtr file, UInt32 nBytes);

        /// <param name='file'></param>
        Boolean IsOk(IntPtr file);

        /// <param name='file'></param>
        Boolean EndOfFile(IntPtr file);

        /// <param name='pOutput'></param>
        /// <param name='maxChars'></param>
        /// <param name='file'></param>
        IntPtr ReadLine(IntPtr pOutput, Int32 maxChars, IntPtr file);

        /// <param name='file'></param>
        /// <param name='pFormat'></param>
        Int32 FPrintf(IntPtr file, String pFormat);

        /// <param name='pFileName'></param>
        /// <param name='pPathID'>Default: 0</param>
        /// <param name='bValidatedDllOnly'>Default: true</param>
        IntPtr LoadModule(String pFileName, [Optional()] [DefaultValue(0)] String pPathID, [Optional()] [DefaultValue(true)] Boolean bValidatedDllOnly);

        /// <param name='pModule'></param>
        void UnloadModule(IntPtr pModule);

        /// <param name='pWildCard'></param>
        /// <param name='pHandle'></param>
        String FindFirst(String pWildCard, IntPtr pHandle);

        /// <param name='handle'></param>
        String FindNext(IntPtr handle);

        /// <param name='handle'></param>
        Boolean FindIsDirectory(IntPtr handle);

        /// <param name='handle'></param>
        void FindClose(IntPtr handle);

        /// <param name='pWildCard'></param>
        /// <param name='pPathID'></param>
        /// <param name='pHandle'></param>
        String FindFirstEx(String pWildCard, String pPathID, IntPtr pHandle);

        /// <param name='pFileName'></param>
        /// <param name='pDest'></param>
        /// <param name='maxLenInChars'></param>
        String GetLocalPath(String pFileName, IntPtr pDest, Int32 maxLenInChars);

        /// <param name='pFullpath'></param>
        /// <param name='pDest'></param>
        /// <param name='maxLenInChars'></param>
        Boolean FullPathToRelativePath(String pFullpath, IntPtr pDest, Int32 maxLenInChars);

        /// <param name='pDirectory'></param>
        /// <param name='maxlen'></param>
        Boolean GetCurrentDirectory(IntPtr pDirectory, Int32 maxlen);

        /// <param name='pFileName'></param>
        IntPtr FindOrAddFileName(String pFileName);

        /// <param name='handle'></param>
        /// <param name='buf'></param>
        /// <param name='buflen'></param>
        Boolean String(IntPtr handle, IntPtr buf, Int32 buflen);

        /// <param name='pRequests'></param>
        /// <param name='nRequests'></param>
        /// <param name='phControls'></param>
        IntPtr AsyncReadMultiple(IntPtr pRequests, Int32 nRequests, IntPtr phControls);

        /// <param name='pFileName'></param>
        /// <param name='pSrc'></param>
        /// <param name='nSrcBytes'></param>
        /// <param name='bFreeMemory'></param>
        /// <param name='pControl'></param>
        IntPtr AsyncAppend(String pFileName, IntPtr pSrc, Int32 nSrcBytes, Boolean bFreeMemory, IntPtr pControl);

        /// <param name='pAppendToFileName'></param>
        /// <param name='pAppendFromFileName'></param>
        /// <param name='pControl'></param>
        IntPtr AsyncAppendFile(String pAppendToFileName, String pAppendFromFileName, IntPtr pControl);

        /// <param name='iToPriority'>Default: 0</param>
        void AsyncFinishAll([Optional()] [DefaultValue(0)] Int32 iToPriority);

        void AsyncFinishAllWrites();

        IntPtr AsyncFlush();

        Boolean AsyncSuspend();

        Boolean AsyncResume();

        /// <param name='pFetcher'></param>
        void AsyncAddFetcher(IntPtr pFetcher);

        /// <param name='pFetcher'></param>
        void AsyncRemoveFetcher(IntPtr pFetcher);

        /// <param name='pszFile'></param>
        /// <param name='phFile'></param>
        IntPtr AsyncBeginRead(String pszFile, IntPtr phFile);

        /// <param name='hFile'></param>
        IntPtr AsyncEndRead(IntPtr hFile);

        /// <param name='hControl'></param>
        /// <param name='wait'>Default: true</param>
        IntPtr AsyncFinish(IntPtr hControl, [Optional()] [DefaultValue(true)] Boolean wait);

        /// <param name='hControl'></param>
        /// <param name='ppData'></param>
        /// <param name='pSize'></param>
        IntPtr AsyncGetResult(IntPtr hControl, IntPtr ppData, IntPtr pSize);

        /// <param name='hControl'></param>
        IntPtr AsyncAbort(IntPtr hControl);

        /// <param name='hControl'></param>
        IntPtr AsyncStatus(IntPtr hControl);

        /// <param name='hControl'></param>
        /// <param name='newPriority'></param>
        IntPtr AsyncSetPriority(IntPtr hControl, Int32 newPriority);

        /// <param name='hControl'></param>
        void AsyncAddRef(IntPtr hControl);

        /// <param name='hControl'></param>
        void AsyncRelease(IntPtr hControl);

        /// <param name='resourcelist'></param>
        IntPtr WaitForResources(String resourcelist);

        /// <param name='handle'></param>
        /// <param name='progress'></param>
        /// <param name='complete'></param>
        Boolean GetWaitForResourcesProgress(IntPtr handle, IntPtr progress, IntPtr complete);

        /// <param name='handle'></param>
        void CancelWaitForResources(IntPtr handle);

        /// <param name='hintlist'></param>
        /// <param name='forgetEverything'></param>
        Int32 HintResourceNeed(String hintlist, Int32 forgetEverything);

        /// <param name='pFileName'></param>
        Boolean IsFileImmediatelyAvailable(String pFileName);

        /// <param name='pFileName'></param>
        void GetLocalCopy(String pFileName);

        void PrintOpenedFiles();

        void PrintSearchPaths();

        /// <param name='pfnWarning'></param>
        void SetWarningFunc(IntPtr pfnWarning);

        /// <param name='level'></param>
        void SetWarningLevel(IntPtr level);

        /// <param name='pfnLogFunc'></param>
        void AddLoggingFunc(IntPtr pfnLogFunc);

        /// <param name='logFunc'></param>
        void RemoveLoggingFunc(IntPtr logFunc);

        IntPtr GetFilesystemStatistics();

        /// <param name='pFileName'></param>
        /// <param name='pOptions'></param>
        /// <param name='flags'>Default: 0</param>
        /// <param name='pathID'>Default: 0</param>
        /// <param name='ppszResolvedFilename'></param>
        IntPtr OpenEx(String pFileName, String pOptions, [Optional()] [DefaultValue(0)] UInt32 flags, [Optional()] [DefaultValue(0)] String pathID, IntPtr ppszResolvedFilename);

        /// <param name='pOutput'></param>
        /// <param name='sizeDest'></param>
        /// <param name='size'></param>
        /// <param name='file'></param>
        Int32 ReadEx(IntPtr pOutput, Int32 sizeDest, Int32 size, IntPtr file);

        /// <param name='pFileName'></param>
        /// <param name='pPath'></param>
        /// <param name='ppBuf'></param>
        /// <param name='bNullTerminate'>Default: false</param>
        /// <param name='bOptimalAlloc'>Default: false</param>
        /// <param name='nMaxBytes'>Default: 0</param>
        /// <param name='nStartingByte'>Default: 0</param>
        /// <param name='pfnAlloc'></param>
        Int32 ReadFileEx(String pFileName, String pPath, IntPtr ppBuf, [Optional()] [DefaultValue(false)] Boolean bNullTerminate, [Optional()] [DefaultValue(false)] Boolean bOptimalAlloc, [Optional()] [DefaultValue(0)] Int32 nMaxBytes, [Optional()] [DefaultValue(0)] Int32 nStartingByte, IntPtr pfnAlloc);

        /// <param name='pFileName'></param>
        IntPtr FindFileName(String pFileName);

        void SetupPreloadData();

        void DiscardPreloadData();

        /// <param name='type'></param>
        /// <param name='archiveFile'></param>
        void LoadCompiledKeyValues(IntPtr type, String archiveFile);

        /// <param name='type'></param>
        /// <param name='filename'></param>
        /// <param name='pPathID'>Default: 0</param>
        IntPtr LoadKeyValues(IntPtr type, String filename, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='head'></param>
        /// <param name='type'></param>
        /// <param name='filename'></param>
        /// <param name='pPathID'>Default: 0</param>
        Boolean LoadKeyValues(IntPtr head, IntPtr type, String filename, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='type'></param>
        /// <param name='outbuf'></param>
        /// <param name='bufsize'></param>
        /// <param name='filename'></param>
        /// <param name='pPathID'>Default: 0</param>
        Boolean ExtractRootKeyName(IntPtr type, IntPtr outbuf, IntPtr bufsize, String filename, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='pFileName'></param>
        /// <param name='pSrc'></param>
        /// <param name='nSrcBytes'></param>
        /// <param name='bFreeMemory'></param>
        /// <param name='bAppend'>Default: false</param>
        /// <param name='pControl'></param>
        IntPtr AsyncWrite(String pFileName, IntPtr pSrc, Int32 nSrcBytes, Boolean bFreeMemory, [Optional()] [DefaultValue(false)] Boolean bAppend, IntPtr pControl);

        /// <param name='pFileName'></param>
        /// <param name='pSrc'></param>
        /// <param name='nSrcBytes'></param>
        /// <param name='bFreeMemory'></param>
        /// <param name='bAppend'>Default: false</param>
        /// <param name='pControl'></param>
        IntPtr AsyncWriteFile(String pFileName, IntPtr pSrc, Int32 nSrcBytes, Boolean bFreeMemory, [Optional()] [DefaultValue(false)] Boolean bAppend, IntPtr pControl);

        /// <param name='pRequests'></param>
        /// <param name='nRequests'></param>
        /// <param name='pszFile'></param>
        /// <param name='line'></param>
        /// <param name='phControls'></param>
        IntPtr AsyncReadMultipleCreditAlloc(IntPtr pRequests, Int32 nRequests, String pszFile, Int32 line, IntPtr phControls);

        /// <param name='pFullPath'></param>
        /// <param name='buf'></param>
        /// <param name='bufSizeInBytes'></param>
        Boolean GetFileTypeForFullPath(String pFullPath, IntPtr buf, IntPtr bufSizeInBytes);

        /// <param name='hFile'></param>
        /// <param name='buf'></param>
        /// <param name='nMaxBytes'>Default: 0</param>
        /// <param name='pfnAlloc'></param>
        Boolean ReadToBuffer(IntPtr hFile, IntPtr buf, [Optional()] [DefaultValue(0)] Int32 nMaxBytes, IntPtr pfnAlloc);

        /// <param name='hFile'></param>
        /// <param name='pOffsetAlign'></param>
        /// <param name='pSizeAlign'></param>
        /// <param name='pBufferAlign'></param>
        Boolean GetOptimalIOConstraints(IntPtr hFile, IntPtr pOffsetAlign, IntPtr pSizeAlign, IntPtr pBufferAlign);

        /// <param name='hFile'></param>
        /// <param name='nSize'>Default: 0</param>
        /// <param name='nOffset'>Default: 0</param>
        IntPtr AllocOptimalReadBuffer(IntPtr hFile, [Optional()] [DefaultValue(0)] UInt32 nSize, [Optional()] [DefaultValue(0)] UInt32 nOffset);

        /// <param name='param0'></param>
        void FreeOptimalReadBuffer(IntPtr param0);

        void BeginMapAccess();

        void EndMapAccess();

        /// <param name='pFullpath'></param>
        /// <param name='pPathId'></param>
        /// <param name='pDest'></param>
        /// <param name='maxLenInChars'></param>
        Boolean FullPathToRelativePathEx(String pFullpath, String pPathId, IntPtr pDest, Int32 maxLenInChars);

        /// <param name='handle'></param>
        Int32 GetPathIndex(IntPtr handle);

        /// <param name='pPath'></param>
        /// <param name='pPathID'></param>
        Int32 GetPathTime(String pPath, String pPathID);

        IntPtr GetDVDMode();

        /// <param name='bEnable'></param>
        /// <param name='bCacheAllVPKHashes'></param>
        /// <param name='bRecalculateAndCheckHashes'></param>
        void EnableWhitelistFileTracking(Boolean bEnable, Boolean bCacheAllVPKHashes, Boolean bRecalculateAndCheckHashes);

        /// <param name='pWhiteList'></param>
        /// <param name='pFilesToReload'></param>
        void RegisterFileWhitelist(IntPtr pWhiteList, IntPtr pFilesToReload);

        void MarkAllCRCsUnverified();

        /// <param name='pPathname'></param>
        /// <param name='eType'></param>
        /// <param name='pFilter'></param>
        void CacheFileCRCs(String pPathname, IntPtr eType, IntPtr pFilter);

        /// <param name='pPathID'></param>
        /// <param name='pRelativeFilename'></param>
        /// <param name='nFileFraction'></param>
        /// <param name='pFileHash'></param>
        IntPtr CheckCachedFileHash(String pPathID, String pRelativeFilename, Int32 nFileFraction, IntPtr pFileHash);

        /// <param name='pFiles'></param>
        /// <param name='nMaxFiles'></param>
        Int32 GetUnverifiedFileHashes(IntPtr pFiles, Int32 nMaxFiles);

        Int32 GetWhitelistSpewFlags();

        /// <param name='flags'></param>
        void SetWhitelistSpewFlags(Int32 flags);

        /// <param name='func'></param>
        void InstallDirtyDiskReportFunc(IntPtr func);

        IntPtr CreateFileCache();

        /// <param name='cacheId'></param>
        /// <param name='ppFileNames'></param>
        /// <param name='nFileNames'></param>
        /// <param name='pPathID'></param>
        void AddFilesToFileCache(IntPtr cacheId, IntPtr ppFileNames, Int32 nFileNames, String pPathID);

        /// <param name='cacheId'></param>
        /// <param name='pFileName'></param>
        Boolean IsFileCacheFileLoaded(IntPtr cacheId, String pFileName);

        /// <param name='cacheId'></param>
        Boolean IsFileCacheLoaded(IntPtr cacheId);

        /// <param name='cacheId'></param>
        void DestroyFileCache(IntPtr cacheId);

        /// <param name='pFile'></param>
        /// <param name='ppExistingFileWithRef'></param>
        Boolean RegisterMemoryFile(IntPtr pFile, IntPtr ppExistingFileWithRef);

        /// <param name='pFile'></param>
        void UnregisterMemoryFile(IntPtr pFile);

        /// <param name='bCacheAllVPKHashes'></param>
        /// <param name='bRecalculateAndCheckHashes'></param>
        void CacheAllVPKFileHashes(Boolean bCacheAllVPKHashes, Boolean bRecalculateAndCheckHashes);

        /// <param name='PackFileID'></param>
        /// <param name='nPackFileNumber'></param>
        /// <param name='nFileFraction'></param>
        /// <param name='md5Value'></param>
        Boolean CheckVPKFileHash(Int32 PackFileID, Int32 nPackFileNumber, Int32 nFileFraction, IntPtr md5Value);

        /// <param name='pszFilename'></param>
        /// <param name='pPathId'></param>
        void NotifyFileUnloaded(String pszFilename, String pPathId);
    }
}
