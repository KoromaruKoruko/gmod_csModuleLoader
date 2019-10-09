// CommandLineParser.cs

using System;
using System.Collections;

namespace SevenZip.CommandLineParser
{
    public enum SwitchType
    {
        Simple,
        PostMinus,
        LimitedPostString,
        UnLimitedPostString,
        PostChar
    }

    internal class CommandSubCharsSet
    {
        public String Chars = "";
        public Boolean EmptyAllowed = false;
    }

    public class CommandForm
    {
        public String IDString = "";
        public Boolean PostStringMode = false;

        public CommandForm(String idString, Boolean postStringMode)
        {
            this.IDString = idString;
            this.PostStringMode = postStringMode;
        }
    }

    public class Parser
    {
        private const String kStopSwitchParsing = "--";
        private const Char kSwitchID1 = '-';
        private const Char kSwitchID2 = '/';
        private const Char kSwitchMinus = '-';
        private readonly SwitchResult[] _switches;
        public ArrayList NonSwitchStrings = new ArrayList();

        public Parser(Int32 numSwitches)
        {
            this._switches = new SwitchResult[numSwitches];
            for (Int32 i = 0; i < numSwitches; i++)
                this._switches[i] = new SwitchResult();
        }

        public SwitchResult this[Int32 index] => this._switches[index];

        private static Boolean IsItSwitchChar(Char c) => c == kSwitchID1 || c == kSwitchID2;

        private static Boolean ParseSubCharsCommand(Int32 numForms, CommandSubCharsSet[] forms,
            String commandString, ArrayList indices)
        {
            indices.Clear();
            Int32 numUsedChars = 0;
            for (Int32 i = 0; i < numForms; i++)
            {
                CommandSubCharsSet charsSet = forms[i];
                Int32 currentIndex = -1;
                Int32 len = charsSet.Chars.Length;
                for (Int32 j = 0; j < len; j++)
                {
                    Char c = charsSet.Chars[j];
                    Int32 newIndex = commandString.IndexOf(c);
                    if (newIndex >= 0)
                    {
                        if (currentIndex >= 0)
                            return false;
                        if (commandString.IndexOf(c, newIndex + 1) >= 0)
                            return false;
                        currentIndex = j;
                        numUsedChars++;
                    }
                }
                if (currentIndex == -1 && !charsSet.EmptyAllowed)
                    return false;
                indices.Add(currentIndex);
            }
            return numUsedChars == commandString.Length;
        }

        private Boolean ParseString(String srcString, SwitchForm[] switchForms)
        {
            Int32 len = srcString.Length;
            if (len == 0)
                return false;
            Int32 pos = 0;
            if (!IsItSwitchChar(srcString[pos]))
                return false;
            while (pos < len)
            {
                if (IsItSwitchChar(srcString[pos]))
                    pos++;
                const Int32 kNoLen = -1;
                Int32 matchedSwitchIndex = 0;
                Int32 maxLen = kNoLen;
                for (Int32 switchIndex = 0; switchIndex < this._switches.Length; switchIndex++)
                {
                    Int32 switchLen = switchForms[switchIndex].IDString.Length;
                    if (switchLen <= maxLen || pos + switchLen > len)
                        continue;
                    if (String.Compare(switchForms[switchIndex].IDString, 0,
                            srcString, pos, switchLen, true) == 0)
                    {
                        matchedSwitchIndex = switchIndex;
                        maxLen = switchLen;
                    }
                }
                if (maxLen == kNoLen)
                    throw new Exception("maxLen == kNoLen");
                SwitchResult matchedSwitch = this._switches[matchedSwitchIndex];
                SwitchForm switchForm = switchForms[matchedSwitchIndex];
                if ((!switchForm.Multi) && matchedSwitch.ThereIs)
                    throw new Exception("switch must be single");
                matchedSwitch.ThereIs = true;
                pos += maxLen;
                Int32 tailSize = len - pos;
                SwitchType type = switchForm.Type;
                switch (type)
                {
                    case SwitchType.PostMinus:
                    {
                        if (tailSize == 0)
                            matchedSwitch.WithMinus = false;
                        else
                        {
                            matchedSwitch.WithMinus = srcString[pos] == kSwitchMinus;
                            if (matchedSwitch.WithMinus)
                                pos++;
                        }
                        break;
                    }
                    case SwitchType.PostChar:
                    {
                        if (tailSize < switchForm.MinLen)
                            throw new Exception("switch is not full");
                        String charSet = switchForm.PostCharSet;
                        const Int32 kEmptyCharValue = -1;
                        if (tailSize == 0)
                            matchedSwitch.PostCharIndex = kEmptyCharValue;
                        else
                        {
                            Int32 index = charSet.IndexOf(srcString[pos]);
                            if (index < 0)
                                matchedSwitch.PostCharIndex = kEmptyCharValue;
                            else
                            {
                                matchedSwitch.PostCharIndex = index;
                                pos++;
                            }
                        }
                        break;
                    }
                    case SwitchType.LimitedPostString:
                    case SwitchType.UnLimitedPostString:
                    {
                        Int32 minLen = switchForm.MinLen;
                        if (tailSize < minLen)
                            throw new Exception("switch is not full");
                        if (type == SwitchType.UnLimitedPostString)
                        {
                            matchedSwitch.PostStrings.Add(srcString.Substring(pos));
                            return true;
                        }
                        String stringSwitch = srcString.Substring(pos, minLen);
                        pos += minLen;
                        for (Int32 i = minLen; i < switchForm.MaxLen && pos < len; i++, pos++)
                        {
                            Char c = srcString[pos];
                            if (IsItSwitchChar(c))
                                break;
                            stringSwitch += c;
                        }
                        matchedSwitch.PostStrings.Add(stringSwitch);
                        break;
                    }
                }
            }
            return true;
        }

        public static Int32 ParseCommand(CommandForm[] commandForms, String commandString,
            out String postString)
        {
            for (Int32 i = 0; i < commandForms.Length; i++)
            {
                String id = commandForms[i].IDString;
                if (commandForms[i].PostStringMode)
                {
                    if (commandString.IndexOf(id) == 0)
                    {
                        postString = commandString.Substring(id.Length);
                        return i;
                    }
                }
                else
                    if (commandString == id)
                {
                    postString = "";
                    return i;
                }
            }
            postString = "";
            return -1;
        }

        public void ParseStrings(SwitchForm[] switchForms, String[] commandStrings)
        {
            Int32 numCommandStrings = commandStrings.Length;
            Boolean stopSwitch = false;
            for (Int32 i = 0; i < numCommandStrings; i++)
            {
                String s = commandStrings[i];
                if (stopSwitch)
                    this.NonSwitchStrings.Add(s);
                else
                    if (s == kStopSwitchParsing)
                    stopSwitch = true;
                else
                    if (!this.ParseString(s, switchForms))
                    this.NonSwitchStrings.Add(s);
            }
        }
    }

    public class SwitchForm
    {
        public String IDString;
        public Int32 MaxLen;
        public Int32 MinLen;
        public Boolean Multi;
        public String PostCharSet;
        public SwitchType Type;

        public SwitchForm(String idString, SwitchType type, Boolean multi,
            Int32 minLen, Int32 maxLen, String postCharSet)
        {
            this.IDString = idString;
            this.Type = type;
            this.Multi = multi;
            this.MinLen = minLen;
            this.MaxLen = maxLen;
            this.PostCharSet = postCharSet;
        }

        public SwitchForm(String idString, SwitchType type, Boolean multi, Int32 minLen) :
            this(idString, type, multi, minLen, 0, "")
        {
        }

        public SwitchForm(String idString, SwitchType type, Boolean multi) :
            this(idString, type, multi, 0)
        {
        }
    }

    public class SwitchResult
    {
        public Int32 PostCharIndex;
        public ArrayList PostStrings = new ArrayList();
        public Boolean ThereIs;
        public Boolean WithMinus;

        public SwitchResult() => this.ThereIs = false;
    }
}
