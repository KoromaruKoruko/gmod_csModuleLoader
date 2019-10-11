using System.Runtime.InteropServices;
using System;

namespace GMLoaded.Lua
{
    /// <summary>
    /// The file library provides functions for finding, reading and writing to files. <br/>
    /// <list type='bullet'>
    /// <listheader><description>The following path values are most commonly used:</description></listheader>
    /// <item><description> “LUA” searches the lua files (in /lua/, in your gamemodes, in all the addons).
    ///</description></item>
    /// <item><description> “GAME” searches all the mounted content (main folder, addons, mounted games etc).
    ///</description></item>
    /// <item><description> “MOD” searches only the garrysmod folder.
    ///</description></item>
    /// <item><description> “DATA” searches in the data folder.</description></item>
    /// </list>
    /// For the full list of path values, type “path” in the console.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// Appends a file relative to the ''data'' folder.
        /// </summary>
        /// <param name='name'>The file's name.</param>
        /// <param name='content'>The content which should be appended to the file.</param>
        void Append(String name, String content);

        /// <summary>
        /// Deletes a file or ''empty'' folder that is relative to the '''data''' folder.You can't remove any files outside of '''data''' folder.
        /// </summary>
        /// <param name='name'>The file name</param>
        void Delete(String name);

        /// <summary>
        /// Creates a directory that is relative to the ''data'' folder.
        /// </summary>
        /// <param name='name'>The directory's name.</param>
        void CreateDir(String name);

        /// <summary>
        /// Returns a boolean of whether the file or directory exists or not.
        /// </summary>
        /// <returns>Type: System.Boolean - Returns true if the file exists and false if it is not.</returns>
        /// <param name='name'>The file or directory's name.</param>
        /// <param name='path'>The path of where to look for the file.* "GAME" Structured like base folder (garrysmod/), searches all the mounted content (main folder, addons, mounted games etc)* "LUA" or "lsv" - All Lua folders (lua/) including gamesmodes and addons* "DATA" Data folder (garrysmod/data)* "MOD" Strictly the game folder (garrysmod/), ignores mounting.</param>
        Boolean Exists(String name, String path);

        /// <summary>
        /// Returns a list of files and directories inside a single folder.
        /// </summary>
        /// <param name='name'>The wildcard to search for. "models/*.mdl" will list .mdl files in the models/ folder.</param>
        /// <param name='path'>The path to look for the files and directories in. See [[File_Search_Paths|this list]] for a list of valid paths.</param>
        /// <param name='sorting'>The sorting to be used, optional.* "nameasc" sort the files ascending by name* "namedesc" sort the files descending by name* "dateasc" sort the files ascending by date* "datedesc" sort the files descending by date</param>
        void Find(String name, String path, [Optional()] String sorting);

        /// <summary>
        /// Returns if the given file is a directory.
        /// </summary>
        /// <returns>Type: System.Boolean -  isDir</returns>
        /// <param name='fileName'>The file or directory's name.</param>
        /// <param name='path'>The path type.</param>
        Boolean IsDir(String fileName, String path);

        /// <summary>
        /// Returns the file's size in bytes. If the file is not found, returns -1.
        /// </summary>
        /// <param name='fileName'>The file's name.</param>
        /// <param name='path'>The path type.* "GAME" Structured like base folder (garrysmod/), searches all the mounted content (main folder, addons, mounted games etc)* "LUA" or "lsv" - All Lua folders (lua/) including gamesmodes and addons* "DATA" Data folder (garrysmod/data)* "MOD" Strictly the game folder (garrysmod/), ignores mounting.</param>
        void Size(String fileName, String path);

        /// <summary>
        /// Attempts to open a file with the given mode.
        /// </summary>
        /// <returns>Type: System.Object - The opened file</returns>
        /// <param name='fileName'>The files name. See LibraryFunctionfileWrite for details on filename restrictions when writing to files.</param>
        /// <param name='fileMode'>The mode to open the file in. Possible values are:* '''r''' - read mode* '''w''' - write mode* '''a''' - append mode* '''rb''' - binary read mode* '''wb''' - binary write mode* '''ab''' - binary append mode</param>
        /// <param name='path'>The path type.* "GAME" Structured like base folder (garrysmod/), searches all the mounted content (main folder, addons, mounted games etc)* "LUA" or "lsv" - All Lua folders (lua/) including gamesmodes and addons* "DATA" Data folder (garrysmod/data)* "MOD" Strictly the game folder (garrysmod/), ignores mounting.</param>
        Object Open(String fileName, String fileMode, String path);

        /// <summary>
        /// Returns the content of a file.Beware of casing -- some filesystems are case-sensitive. SRCDS on Linux seems to force file/directory creation to lowercase, but will not modify read operations.
        /// </summary>
        /// <returns>Type: System.String - The data from the file as a string, or nil if the file isn't found</returns>
        /// <param name='fileName'>The name of the file.</param>
        /// <param name='path'>The path used to look up the file.* "GAME" Structured like base folder (garrysmod/), searches all the mounted content (main folder, addons, mounted games etc)* "LUA" or "lsv" - All Lua folders (lua/) including gamesmodes and addons* "DATA" Data folder (garrysmod/data)* "MOD" Strictly the game folder (garrysmod/), ignores mounting.It can also be a boolean:* true = "GAME"* false = "DATA"</param>
        String Read(String fileName, [Optional()] String path);

        /// <summary>
        /// Returns when the file or folder was lasted modified in Unix time.
        /// </summary>
        /// <returns>Type: System.Double - Seconds passed since Unix epoch.</returns>
        /// <param name='path'>The '''file''' or '''folder''' path.</param>
        /// <param name='gamePath'>The game path to be used.* "GAME" Structured like base folder (garrysmod/), searches all the mounted content (main folder, addons, mounted games etc)* "LUA" or "lsv" - All Lua folders (lua/) including gamesmodes and addons* "DATA" Data folder (garrysmod/data)* "MOD" Strictly the game folder (garrysmod/), ignores mounting.</param>
        Double Time(String path, String gamePath);

        /// <summary>
        /// Writes the given string to a file. Erases all previous data in the file. To add data without deleting previous data, use LibraryFunctionfileAppend.It is recommended to write only to lowercase file paths and names because some filesystems are case-sensitive. The Linux build of SRCDS seems to auto-lower directory and file names on write, but not on read.
        /// </summary>
        /// <param name='fileName'>The name of the file being written into.The filename '''must''' end with ".txt", ".jpg", ".png", ".vtf" or ".dat" and the path is relative to the '''data/''' folder.Restricted symbols are: '''" :'''</param>
        /// <param name='content'>The content that will be written into the file.</param>
        void Write(String fileName, String content);
    }
}
