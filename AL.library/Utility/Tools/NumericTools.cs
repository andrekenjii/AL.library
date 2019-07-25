using System;

namespace AL.library.Utility.Tools
{
    public static class NumericTools
    {
        /// <summary>
        /// Completa o número com zeros a esquerda até o tamanho especificado
        /// </summary>
        public static string FormatLength(this int number, int length)
        {
            string text = number.ToString();

            if (text.Length < length)
                text = text.PadLeft(length, '0');
            else if (text.Length > length)
                text = text.Substring(0, length);

            return text;
        }

        /// <summary>
        /// Completa o número com zeros a esquerda até o tamanho especificado
        /// </summary>
        public static string FormatLength(this long number, int length)
        {
            string text = number.ToString();

            if (text.Length < length)
                text = text.PadLeft(length, '0');
            else if (text.Length > length)
                text = text.Substring(0, length);

            return text;
        }

        /// <summary>
        /// Formata decimal no padrão americano, ex: 1.20 utilizando o número de casas decimais informado
        /// </summary>
        public static string FormatDecimalEN(this double number, int length, int decimals)
        {
            number = Math.Round(number, decimals);

            string text;
            if (decimals > 0)
            {
                text = number.ToString(string.Format("0.{0}", "".PadLeft(decimals, '0')), new System.Globalization.CultureInfo("en-US"));
                length++;
            }
            else
            {
                text = number.ToString("0", new System.Globalization.CultureInfo("en-US"));
            }

            if (text.Length > length)
                text = text.Substring(0, length);

            return text;
        }

        /// <summary>
        /// Formata um numero para padrão de Moeda Brasileiro
        /// </summary>

        public static string FormatMoneyBr(this string value)
        {
            string text;
            var dec = ParserTools.ToDecimalFormatEnglish(value);

            text = dec.ToString("#,##0.00", new System.Globalization.CultureInfo("pt-BR"));

            return text;
        }

        public static string FormatMoney(this double value)
        {
            string text;

            text = value.ToString("#,##0.00");

            return text;
        }

        public static string FormatMoney(this double? value)
        {
            string text;

            if (value == null)
                text = "0,00";
            else
                text = ((double)value).ToString("#,##0.00");

            return text;
        }
    }
}
