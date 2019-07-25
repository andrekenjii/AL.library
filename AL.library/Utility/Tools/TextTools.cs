using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AL.library.Utility.Tools
{
    public static class TextTools
    {
        /// <summary>
        /// Remove caracteres especiais
        /// </summary>
        /// <param name="text"></param>
        /// <param name="removeSpaces"></param>
        /// <param name="removeDiacritics"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string text, bool removeSpaces = false, bool removeDiacritics = true, bool removePageBreak = false, bool trim = false, string exceptions = "")
        {
            string ret = text;

            if (string.IsNullOrEmpty(ret))
                return ret;

            if (removeDiacritics)
                ret = RemoveDiacritics(ret);

            ret = System.Text.RegularExpressions.Regex.Replace(ret, @"\t", " ");

            if (!removeSpaces)
                ret = System.Text.RegularExpressions.Regex.Replace(ret, string.Format(@"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\s{0}]+?", exceptions), string.Empty);
            else
                ret = System.Text.RegularExpressions.Regex.Replace(ret, string.Format(@"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ{0}]+?", exceptions), string.Empty);

            if (removePageBreak)
                RemovePageBreak(ret);

            if (trim)
                ret = ret.Trim();

            // "a - b" = "a  b", isso remove os espaços seguidos
            ret = ret.Replace("  ", " ");

            return ret;
        }

        public static string GetNumericCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string ret = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9]+?", string.Empty);

            return ret != null ? ret.Trim() : ret;
        }

        public static string RemoveNonVisibleCharacters(string text)
        {
            if (text == null)
                return null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] >= 32 && text[i] <= 255 && text[i] != 127 && text[i] != 129 && text[i] != 141 && text[i] != 143 && text[i] != 144 && text[i] != 157)
                    sb.Append(text[i]);
            }

            return sb.ToString();
        }

        public static string RemovePageBreak(string text, string separator = " ")
        {
            if (text == null)
                return null;

            text = text.Replace("\r\n", separator);
            text = text.Replace("\r", string.Empty);
            text = text.Replace("\n", separator);
            return text;
        }

        public static string RemoveSpaces(string text)
        {
            if (text == null)
                return null;

            text = text.Replace(" ", string.Empty);
            return text;
        }

        public static string RemoveHtmlTags(string text)
        {
            if (text == null)
                return null;

            text = System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]*>", String.Empty);
            return text;
        }

        public static string MaxLength(string text, int maxLength, string sufixToIncludeWhenTruncated = null)
        {
            if (text == null)
                return null;

            text = text.Trim();

            if (string.IsNullOrEmpty(sufixToIncludeWhenTruncated))
            {
                if (text.Length > maxLength)
                    text = text.Substring(0, maxLength);
            }
            else
            {
                if (text.Length > maxLength)
                    text = text.Substring(0, maxLength - sufixToIncludeWhenTruncated.Length) + sufixToIncludeWhenTruncated;
            }

            return text;
        }

        public static string FixedLength(string text, int length)
        {
            if (text == null)
                return null;

            if (text.Length > length)
                text = text.Substring(0, length);
            else if (text.Length < length)
                text = string.Concat(text, new string(' ', length - text.Length));

            return text;
        }

        public static string GenerateHash(this string data)
        {
            var sha1 = SHA1.Create();

            var hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            var returnValue = new StringBuilder();

            foreach (var b in hashData)
                returnValue.Append(b.ToString("X2"));

            return returnValue.ToString();
        }

        /// <summary>
        /// Remove acentuação
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] > 255)
                    sb.Append(text[i]);
                else
                    sb.Append(s_Diacritics[text[i]]);
            }

            return sb.ToString();
        }
        private static readonly char[] s_Diacritics = GetDiacritics();
        private static char[] GetDiacritics()
        {
            char[] accents = new char[256];

            for (int i = 0; i < 256; i++)
                accents[i] = (char)i;

            accents[(byte)'á'] = accents[(byte)'à'] = accents[(byte)'ã'] = accents[(byte)'â'] = accents[(byte)'ä'] = 'a';
            accents[(byte)'Á'] = accents[(byte)'À'] = accents[(byte)'Ã'] = accents[(byte)'Â'] = accents[(byte)'Ä'] = 'A';

            accents[(byte)'é'] = accents[(byte)'è'] = accents[(byte)'ê'] = accents[(byte)'ë'] = 'e';
            accents[(byte)'É'] = accents[(byte)'È'] = accents[(byte)'Ê'] = accents[(byte)'Ë'] = 'E';

            accents[(byte)'í'] = accents[(byte)'ì'] = accents[(byte)'î'] = accents[(byte)'ï'] = 'i';
            accents[(byte)'Í'] = accents[(byte)'Ì'] = accents[(byte)'Î'] = accents[(byte)'Ï'] = 'I';

            accents[(byte)'ó'] = accents[(byte)'ò'] = accents[(byte)'ô'] = accents[(byte)'õ'] = accents[(byte)'ö'] = 'o';
            accents[(byte)'Ó'] = accents[(byte)'Ò'] = accents[(byte)'Ô'] = accents[(byte)'Õ'] = accents[(byte)'Ö'] = 'O';

            accents[(byte)'ú'] = accents[(byte)'ù'] = accents[(byte)'û'] = accents[(byte)'ü'] = 'u';
            accents[(byte)'Ú'] = accents[(byte)'Ù'] = accents[(byte)'Û'] = accents[(byte)'Ü'] = 'U';

            accents[(byte)'ç'] = 'c';
            accents[(byte)'Ç'] = 'C';

            accents[(byte)'ñ'] = 'n';
            accents[(byte)'Ñ'] = 'N';

            accents[(byte)'ÿ'] = accents[(byte)'ý'] = 'y';
            accents[(byte)'Ý'] = 'Y';

            return accents;
        }

        public static string FormatTextForUrl(string name, bool toLower = true)
        {
            if (name == null)
                return null;

            name = RemoveSpecialCharacters(name);
            name = name.Replace(" ", "-");
            if (toLower)
                name = name.ToLower();

            return name;
        }

        private static byte[] GetFileByteOrderMark(string filename)
        {
            //Read the BOM (Byte Order Mark) -- ref: http://en.wikipedia.org/wiki/Byte_order_mark
            var bom = new byte[4];
            using (var file = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            return bom;
        }

        public static Encoding GetFileEncoding(string filename)
        {
            //Read the BOM
            var bom = GetFileByteOrderMark(filename);

            Encoding encoding = null;

            //Analyze the BOM (Byte Order Mark) -- ref: http://en.wikipedia.org/wiki/Byte_order_mark
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                encoding = Encoding.UTF7;
            else if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                encoding = Encoding.UTF8;
            else if (bom[0] == 0xff && bom[1] == 0xfe) //UTF-16LE
                encoding = Encoding.Unicode;
            else if (bom[0] == 0xfe && bom[1] == 0xff) //UTF-16BE
                encoding = Encoding.BigEndianUnicode;
            else if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                encoding = Encoding.UTF32;
            else if (bom[0] == 0x49 && bom[1] == 0x46 && bom[2] == 0x20 && bom[3] == 0x45)
                encoding = Encoding.UTF8;
            else
                encoding = Encoding.GetEncoding(1252); //No windows, é usado esse como padrão

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.WriteLine(string.Format("GetFileEncoding() -- {0}\t{1:x} {2:x} {3:x} {4:x}\t{5}", filename, bom[0], bom[1], bom[2], bom[3], encoding.ToString()));

            return encoding;
        }

        public static string EncondeTo64(this string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string InsertValueInterval(this string text, int interval, char value)
        {
            var lstText = text.ToList();
            var sentinel = lstText.Count() / interval;
            var countInterval = interval;
            interval++;

            for (var i = 0; i < sentinel; i++)
            {
                lstText.Insert(countInterval, value);
                countInterval += interval;
            }

            return new string(lstText.ToArray());
        }
    }
}
