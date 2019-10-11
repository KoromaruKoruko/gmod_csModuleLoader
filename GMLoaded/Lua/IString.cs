using System.Runtime.InteropServices;
using System;
namespace GMLoaded.Lua
{
    /// <summary>
    ///
    /// <list type='bullet'>
    /// <listheader><description>The String ''type'' is a sequence of characters.The String ''library'' is a standard Lua library which provides functions for the manipulation of strings. In Garry's Mod there are several extra useful functions added to this library.Every string.</description></listheader>
    /// <item><description> function that has a String as first argument is also available as a method for the String type. That means that with string.Right("somestring", 4) for example you can also call ("somestring"):Right(4) which will do the same thing.</description></item>
    /// </list>
    ///
    /// This category lists functions available in the String ''library''.
    /// </summary>
    public interface IString
    {
        /// <summary>
        /// Returns the given string's characters in their numeric ASCII representation.
        /// </summary>
        /// <returns>Type: System.Object - Numerical bytes</returns>
        /// <param name='string'>The String to get the chars from.</param>
        /// <param name='startPos'>The first character of the String to get the byte of.</param>
        /// <param name='endPos'>The last character of the String to get the byte of.</param>
        Object @byte(String @string, [Optional()] Double startPos, [Optional()] Double endPos);

        /// <summary>
        /// Takes the given numerical bytes and converts them to a string.
        /// </summary>
        /// <returns>Type: System.String - String built from given bytes</returns>
        /// <param name='bytes'>The bytes to create the String from.</param>
        String @char(Object bytes);

        /// <summary>
        /// Inserts commas for every third digit.
        /// </summary>
        /// <returns>Type: System.String -  Prettystring</returns>
        /// <param name='InputNumber'>The input number to commafy</param>
        String Comma(Double InputNumber);

        /// <summary>
        /// Returns whether or not the second passed String matches the end of the first.
        /// </summary>
        /// <returns>Type: System.Boolean - ''true'' if the first String ends with the second, or the second is empty, otherwise ''false''.</returns>
        /// <param name='str'>The String whose end is to be checked.</param>
        /// <param name='end'>The String to be matched with the end of the first.</param>
        Boolean EndsWith(String str, String end);

        /// <summary>
        /// Splits a String up wherever it finds the given separator.This is the reverse of LibraryFunctionstringImplode.
        /// </summary>
        /// <returns>Type: System.Object[] - Exploded String as a numerical sequential table.</returns>
        /// <param name='separator'>The String will be separated wherever this sequence is found.</param>
        /// <param name='str'>The String to split up.</param>
        /// <param name='use_patterns'>Set this to true if your separator is a [[Patterns|pattern]].</param>
        Object[] Explode(String separator, String str, Boolean use_patterns);

        /// <summary>
        /// Attempts to find the specified subString in a string, uses [[Patterns]] by default.
        /// </summary>
        /// <param name='haystack'>The String to search in.</param>
        /// <param name='needle'>The String to find, can contain patterns if enabled.</param>
        /// <param name='startPos'>The position to start the search from, can be negative start position will be relative to the end position.</param>
        /// <param name='noPatterns'>Disable patterns.</param>
        void find(String haystack, String needle, [Optional()] Double startPos, [Optional()] Boolean noPatterns);

        /// <summary>
        /// Returns the time as a formatted String or as a table if no format is given.
        /// </summary>
        /// <returns>Type: System.String - Returns the time as a formatted String only if a format was specified.Returns a table only if no format was specified. The table will contain these fields:<br/>FuncArgnumbermsmilliseconds<br/>FuncArgnumbersseconds<br/>FuncArgnumbermminutes<br/>FuncArgnumberhhours<br/><br/></returns>
        /// <param name='float'>The time in seconds to format.</param>
        /// <param name='format'>An optional formatting to use. If no format it specified, a table will be returned instead.</param>
        String FormattedTime(Double @float, [Optional()] String format);

        /// <summary>
        /// Formats the specified values into the String given.
        /// </summary>
        /// <returns>Type: System.String - The formatted string</returns>
        /// <param name='format'>The String to be formatted.<br>Follows this format: http://www.cplusplus.com/reference/cstdio/printf/</param>
        /// <param name='formatParameters'>Values to be formatted into the string.</param>
        String format(String format, Object formatParameters);

        /// <summary>
        /// Creates a String from a Color variable.
        /// </summary>
        /// <returns>Type: System.String - Output</returns>
        /// <param name='color'>The color to put in the string.</param>
        String FromColor(Object[] color);

        /// <summary>
        /// DeprecatedUse either string.sub(str, index, index) or str[index]. Returns char value from the specified index in the supplied string.
        /// </summary>
        /// <returns>Type: System.String -  str</returns>
        /// <param name='str'>The String that you will be searching with the supplied index.</param>
        /// <param name='index'>The index's value of the String to be returned.</param>
        String GetChar(String str, Double index);

        /// <summary>
        /// Returns extension of the file.
        /// </summary>
        /// <returns>Type: System.String -  fileExtension</returns>
        /// <param name='file'>String eg. file-path to get the file extensions from.</param>
        String GetExtensionFromFilename(String file);

        /// <summary>
        /// Returns file name and extension.
        /// </summary>
        /// <returns>Type: System.String - The file name</returns>
        /// <param name='pathString'>The String eg. file-path to get the file-name from.</param>
        String GetFileFromFilename(String pathString);

        /// <summary>
        /// Returns the path only from a file's path.
        /// </summary>
        /// <returns>Type: System.String - Path</returns>
        /// <param name='Inputstring'>String to get path from.</param>
        String GetPathFromFilename(String Inputstring);

        /// <summary>
        /// Using [[Patterns]], returns an iterator which will return either one value if no capture groups are defined, or any capture group matches.
        /// </summary>
        /// <returns>Type: System.Object - The iterator function that can be used in a for-in loop</returns>
        /// <param name='data'>The String to search in</param>
        /// <param name='pattern'>The pattern to search for</param>
        Object gmatch(String data, String pattern);

        /// <summary>
        /// This functions main purpose is to replace certain character sequences in a String using [[Patterns]].
        /// </summary>
        /// <param name='string'>String which should be modified.</param>
        /// <param name='pattern'>The pattern that defines what should be matched and eventually be replaced.</param>
        /// <param name='replacement'>In case of a String the matches sequence will be replaced with it.In case of a table, the matched sequence will be used as key and the table will tested for the key, if a value exists it will be used as replacement.In case of a function all matches will be passed as parameters to the function, the return value(s) of the function will then be used as replacement.</param>
        /// <param name='maxReplaces'>Maximum number of replacements to be made.</param>
        void gsub(String @string, String pattern, String replacement, [Optional()] Double maxReplaces);

        /// <summary>
        /// Joins the values of a table together to form a string.This is the reverse of LibraryFunctionstringExplode and is functionally identical to LibraryFunctiontableconcat, but with less features.
        /// </summary>
        /// <returns>Type: System.String - Imploded pieces</returns>
        /// <param name='separator'>The separator to insert between each piece.</param>
        /// <param name='pieces'>The table of pieces to concatenate. The keys for these must be numeric and sequential.</param>
        String Implode([Optional()] String separator, Object[] pieces);

        /// <summary>
        /// Escapes special characters for JavaScript in a string, making the String safe for inclusion in to JavaScript strings.
        /// </summary>
        /// <returns>Type: System.String - The escaped string.</returns>
        /// <param name='str'>The String that should be escaped.</param>
        String JavascriptSafe(String str);

        /// <summary>
        /// Returns everything left of supplied place of that string.
        /// </summary>
        /// <returns>Type: System.String - Returns a String containing a specified number of characters from the left side of a string.</returns>
        /// <param name='str'>The String to extract from.</param>
        /// <param name='num'>Amount of chars relative to the beginning (starting from 1).</param>
        String Left(String str, Double num);

        /// <summary>
        /// Counts the number of characters in the String (length). This is equivalent to using the length operator (#).
        /// </summary>
        /// <returns>Type: System.Double - Length of the string</returns>
        /// <param name='str'>The String to find the length of.</param>
        Double len(String str);

        /// <summary>
        /// Changes any upper-case letters in a String to lower-case letters.
        /// </summary>
        /// <returns>Type: System.String - The original string, with all uppercase letters replaced with their lowercase variants.</returns>
        /// <param name='str'>The String to convert.</param>
        String lower(String str);

        /// <summary>
        /// Finds a [[Patterns|Pattern]] in a string.
        /// </summary>
        /// <returns>Type: System.Object - Matched text(s)</returns>
        /// <param name='string'>String which should be searched in for matches.</param>
        /// <param name='pattern'>The pattern that defines what should be matched.</param>
        /// <param name='startPosition'>The start index to start the matching from, can be negative to start the match from a position relative to the end.</param>
        Object match(String @string, String pattern, [Optional()] Double startPosition);

        /// <summary>
        /// Converts a digital filesize to human-readable text.
        /// </summary>
        /// <returns>Type: System.String - The human-readable filesize, in Bytes/KB/MB/GB (whichever is appropriate).</returns>
        /// <param name='bytes'>The filesize in bytes.</param>
        String NiceSize(Double bytes);

        /// <summary>
        /// Formats the supplied number (in seconds) to the highest possible time unit.
        /// </summary>
        /// <returns>Type: System.String - A nicely formatted time string.</returns>
        /// <param name='num'>The number to format, in seconds.</param>
        String NiceTime(Double num);

        /// <summary>
        /// Replaces all occurrences of the supplied second string.
        /// </summary>
        /// <returns>Type: System.String - string</returns>
        /// <param name='str'>The String we are seeking to replace an occurrence(s).</param>
        /// <param name='find'>What we are seeking to replace.</param>
        /// <param name='replace'>What to replace find with.</param>
        String Replace(String str, String find, String replace);

        /// <summary>
        /// Repeats a String by the provided number, with an optional separator.
        /// </summary>
        /// <returns>Type: System.String - Repeated string.</returns>
        /// <param name='str'>The String to convert.</param>
        /// <param name='repetitions'>Timer to repeat, this values gets rounded internally.</param>
        /// <param name='separator'>String that will separate the repeated piece. Notice that it doesn't add this String to the start or the end of the result, only between the repeated parts.</param>
        String rep(String str, Double repetitions, [Optional()] String separator);

        /// <summary>
        /// Reverses a string.
        /// </summary>
        /// <returns>Type: System.String - reversed string</returns>
        /// <param name='str'>The String to be reversed.</param>
        String reverse(String str);

        /// <summary>
        /// Returns the last n-th characters of the string.
        /// </summary>
        /// <returns>Type: System.String - Returns a String containing a specified number of characters from the right side of a string.</returns>
        /// <param name='str'>The String to extract from.</param>
        /// <param name='num'>Amount of chars relative to the end (starting from 1).</param>
        String Right(String str, Double num);

        /// <summary>
        /// Sets the character at the specific index of the string.
        /// </summary>
        /// <returns>Type: System.String -  ModifiedString</returns>
        /// <param name='InputString'>The input string</param>
        /// <param name='Index'>The character index, 1 is the first from left.</param>
        /// <param name='ReplacementChar'>String to replace with.</param>
        String SetChar(String InputString, Double Index, String ReplacementChar);

        /// <summary>
        /// Splits the String into a table of strings, separated by the second argument.
        /// </summary>
        /// <returns>Type: System.Object[] - Split table</returns>
        /// <param name='Inputstring'>String to split</param>
        /// <param name='Separator'>Character(s) to split with.</param>
        Object[] Split(String Inputstring, String Separator);

        /// <summary>
        /// Returns whether or not the first String starts with the second.
        /// </summary>
        /// <returns>Type: System.Boolean - Whether the first String starts with the second.</returns>
        /// <param name='inputStr'>String to check.</param>
        /// <param name='start'>String to check with.</param>
        Boolean StartWith(String inputStr, String start);

        /// <summary>
        /// Removes the extension of a path.
        /// </summary>
        /// <returns>Type: System.String - Modifiedstring</returns>
        /// <param name='Inputstring'>The path to change.</param>
        String StripExtension(String Inputstring);

        /// <summary>
        /// Returns a sub-string, starting from the character at position ''StartPos'' of the String (inclusive), and optionally ending at the character at position ''EndPos'' of the String (also inclusive). If EndPos is not given, the rest of the String is returned.
        /// </summary>
        /// <returns>Type: System.String - The substring.</returns>
        /// <param name='string'>The String you'll take a sub-String out of.</param>
        /// <param name='StartPos'>The position of the first character that will be included in the sub-string.</param>
        /// <param name='EndPos'>The position of the last character to be included in the sub-string. It can be negative to count from the end.</param>
        String sub(String @string, Double StartPos, [Optional()] Double EndPos);

        /// <summary>
        /// Fetches a Color type from a string.
        /// </summary>
        /// <returns>Type: System.Object[] - The output StructColor</returns>
        /// <param name='Inputstring'>The String to convert from.</param>
        Object[] ToColor(String Inputstring);

        /// <summary>
        /// Returns given time in "MM:SS:MS" format.
        /// </summary>
        /// <returns>Type: System.String - Formatted time</returns>
        /// <param name='time'>Time in seconds</param>
        String ToMinutesSecondsMilliseconds(Double time);

        /// <summary>
        /// Returns given time in "MM:SS" format.
        /// </summary>
        /// <returns>Type: System.String - Formatted time</returns>
        /// <param name='time'>Time in seconds</param>
        String ToMinutesSeconds(Double time);

        /// <summary>
        /// Splits the String into characters and creates a sequential table.
        /// </summary>
        /// <param name='string'>The String you'll turn into a table.</param>
        void ToTable(String @string);

        /// <summary>
        /// Removes leading spaces/characters from a string.
        /// </summary>
        /// <returns>Type: System.String - Trimmed string</returns>
        /// <param name='str'>String to trim</param>
        /// <param name='char'>Custom character to remove</param>
        String TrimLeft(String str, [Optional()] String @char);

        /// <summary>
        /// Removes trailing spaces/passed character from a string.
        /// </summary>
        /// <returns>Type: System.String - Trimmed string</returns>
        /// <param name='str'>String to remove from</param>
        /// <param name='char'>Custom character to remove, default is a space</param>
        String TrimRight(String str, [Optional()] String @char);

        /// <summary>
        /// Removes leading and trailing matches of a string.
        /// </summary>
        /// <returns>Type: System.String - Modified string</returns>
        /// <param name='Inputstring'>The String to trim.</param>
        /// <param name='Char'>String to match.</param>
        String Trim(String Inputstring, [Optional()] String Char);

        /// <summary>
        /// Changes any lower-case letters in a String to upper-case letters.
        /// </summary>
        /// <returns>Type: System.String - A String representing the value of a String converted to upper-case.</returns>
        /// <param name='str'>The String to convert.</param>
        String upper(String str);

        /// <summary>
        /// DeprecatedThis function is removed in Lua versions later than what GMod is currently using. Use LibraryFunctionstringgmatch instead.Returns an iterator function that is called for every complete match of the pattern, all sub matches will be passed as to the loop.
        /// </summary>
        /// <returns>Type: System.Object - The iterator function that can be used in a for-in loop</returns>
        /// <param name='data'>The String to search in</param>
        /// <param name='pattern'>The pattern to search for</param>
        Object gfind(String data, String pattern);

        /// <summary>
        /// Returns the binary bytecode of the given function.NoteThis does not work with functions created in C/C++. An error will be thrown if it is
        /// </summary>
        /// <returns>Type: System.String - Bytecode</returns>
        /// <param name='func'>The function to get the bytecode of</param>
        /// <param name='stripDebugInfo'>True to strip the debug data, false to keep it</param>
        String dump(Object func, [Optional()] Boolean stripDebugInfo);

        /// <summary>
        /// Escapes all special characters within a string, making the String safe for inclusion in a Lua pattern.
        /// </summary>
        /// <returns>Type: System.String - The String that has been sanitized for inclusion in Lua patterns</returns>
        /// <param name='str'>The String to be sanitized</param>
        String PatternSafe(String str);
    }
}
