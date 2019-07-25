using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using ZXing;
using ZXing.Common;

namespace AL.library.Utility.Tools
{
    public static class StringTools
    {
        /// <summary>
        /// Se a string for maior que o Length informado, trunca o texto
        /// </summary>
        public static string FormatMaxLength(this String text, int length, bool trim = false)
        {
            if (text == null)
                return string.Empty;

            if (text.Length > length)
                text = text.Substring(0, length);

            if (trim)
                return text.Trim();

            return text;
        }

        /// <summary>
        /// Formata um numero para padrão de Moeda Brasileiro
        /// </summary>
        public static string FormatCpf(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return cpf;

            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }

        /// <summary>
        /// Formata um numero para padrão de Moeda Brasileiro
        /// </summary>
		public static string FormatCnpj(this string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
                return cnpj;

            return Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
        }

        public static string FormatPhoneNumber(this string phone)
        {
            if (phone.Length == 10)
                return string.Format("({0}) {1}-{2}", phone.Substring(0, 2), phone.Substring(2, 4), phone.Substring(6, 4));

            if (phone.Length == 11)
                return string.Format("({0}) {1}-{2}", phone.Substring(0, 2), phone.Substring(2, 5), phone.Substring(7, 4));

            return phone;
        }

        public static string FormatMoneyFromEN(this string param)
        {
            return ParserTools.ToDouble(param.Replace('.', ',')).FormatMoney();
        }

        public static string RemoveDiacritics(this string param)
        {
            return TextTools.RemoveDiacritics(param);
        }

        public static string RemoveSpecialCharacters(this string param, bool removeSpaces = false, bool removeDiacritics = true, bool removePageBreak = false, bool trim = false, string exceptions = "")
        {
            return TextTools.RemoveSpecialCharacters(param, removeSpaces, removeDiacritics, removePageBreak, trim, exceptions);
        }

        public static string GetNumericCharacters(this string param)
        {
            return TextTools.GetNumericCharacters(param);
        }

        public static string ReplaceRegex(this string param, string regexPattern, string replacement)
        {
            if (string.IsNullOrEmpty(param))
                return param;

            return new System.Text.RegularExpressions.Regex(regexPattern).Replace(param, replacement);
        }

        public static string RemovePageBreak(this string param, string separator = " ")
        {
            return TextTools.RemovePageBreak(param, separator);
        }

        public static string RemoveNonVisibleCharacters(this string param)
        {
            return TextTools.RemoveNonVisibleCharacters(param);
        }

        public static string RemoveSpaces(this string param)
        {
            return TextTools.RemoveSpaces(param);
        }

        public static string RemoveHtmlTags(this string param)
        {
            return TextTools.RemoveHtmlTags(param);
        }

        public static string GetBase64Barcode(this string barCode)
        {
            if (string.IsNullOrEmpty(barCode)) return string.Empty;

            var barcodeWriter = new BarcodeWriterGeneric
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 30,
                    PureBarcode = true
                }
            };

            var matrix = barcodeWriter.Encode(barCode);

            var width = matrix.Width;
            var height = matrix.Height;

            var image = new Image<Rgba32>(width, height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {

                    image[x, y] = matrix[x, y] ? Rgba32.Black : Rgba32.White;
                    //image.SetPixel(x, y, (matrix.get_Renamed(x,y).Equals(0) ? BLACK : WHITE));
                }
            }

            return image.ToBase64String(SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance);
        }

        public static string GetBase64QrCode(this string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode)) return string.Empty;

            var qrcodeWriter = new BarcodeWriterGeneric
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 160,
                    Width = 160,
                    Margin = 0
                }
            };

            var matrix = qrcodeWriter.Encode(qrCode);

            var width = matrix.Width;
            var height = matrix.Height;

            var image = new Image<Rgba32>(width, height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {

                    image[x, y] = matrix[x, y] ? Rgba32.Black : Rgba32.White;
                    //image.SetPixel(x, y, (matrix.get_Renamed(x,y).Equals(0) ? BLACK : WHITE));
                }
            }

            return image.ToBase64String(SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance);
        }
    }
}
