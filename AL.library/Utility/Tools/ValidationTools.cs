using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AL.library.Utility.Tools
{
    public static class ValidationTools
    {
        #region Inscrição Estadual
        /// <summary>
        ///     Valida Inscrição Estadual
        /// </summary>
        public static bool IsIe(string uf, string number)
        {
            //Regras IE: http://www.sintegra.gov.br/insc_est.html
            //Tocantins: http://www2.sefaz.to.gov.br/Servicos/Sintegra/calinse.htm

            uf = (!string.IsNullOrEmpty(uf) ? uf.Trim().ToUpper() : string.Empty);
            number = (!string.IsNullOrEmpty(number) ? number.Trim().ToUpper() : string.Empty);

            string strOrigem;
            string strBase;
            string strBase2;
            string digito1;
            string digito2;
            int valor;
            int soma;
            int resto;
            int peso;
            var arrayPesos = new int[0];
            int p, d;

            if (IsIeEmpty(number))
                return false;

            if (IsIeIsento(number))
                return true;

            strOrigem = ClearIe(number);

            if (string.IsNullOrEmpty(strOrigem))
                return false;

            switch (uf)
            {
                case "AC":
                    strBase = strOrigem = strOrigem.PadLeft(13, '0').Substring(strOrigem.Length < 13 ? 0 : strOrigem.Length - 13);
                    soma = 0;
                    arrayPesos = new[] { 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 11; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    digito1 = ((11 - resto) < 10 ? (11 - resto).ToString() : "0");

                    strBase = (strBase.Substring(0, 11) + digito1);
                    soma = 0;
                    arrayPesos = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 12; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    digito2 = ((11 - resto) < 10 ? (11 - resto).ToString() : "0");

                    strBase2 = (strBase.Substring(0, 11) + digito1 + digito2);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "AL":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    if ((strBase.Substring(0, 2) == "24"))
                    {
                        soma = 0;
                        arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        soma = (soma * 10);
                        resto = (soma % 11);
                        digito1 = ((resto > 9) ? "0" : resto.ToString());
                        strBase2 = (strBase.Substring(0, 8) + digito1);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                case "AM":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    soma = 0;

                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    if ((soma < 11))
                    {
                        digito1 = (11 - soma).ToString();
                    }
                    else
                    {
                        resto = (soma % 11);
                        digito1 = (resto <= 1 ? "0" : (11 - resto).ToString());
                    }

                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "AP":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    p = 0;
                    d = 0;
                    if ((strBase.Substring(0, 2) == "03"))
                    {
                        valor = int.Parse(strBase.Substring(0, 8));
                        if (((valor >= 3000001) && (valor <= 3017000)))
                        {
                            p = 5;
                            d = 0;
                        }
                        else if (((valor >= 3017001) && (valor <= 3019022)))
                        {
                            p = 9;
                            d = 1;
                        }
                        else if ((valor >= 3019023))
                        {
                            p = 0;
                            d = 0;
                        }

                        arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };
                        soma = 0;

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        soma = soma + p;
                        resto = (soma % 11);
                        valor = (11 - resto);

                        if (valor < 10)
                            digito1 = valor.ToString();
                        else if (valor == 10)
                            digito1 = "0";
                        else
                            digito1 = d.ToString();

                        strBase2 = (strBase.Substring(0, 8) + digito1);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                case "BA":
                    if (strOrigem.Length > 9)
                        return false;

                    // VALIDA COM 8 DIGITOS
                    strBase = strOrigem.PadLeft(8, '0').Substring(strOrigem.Length < 8 ? 0 : strOrigem.Length - 8);

                    if (strBase.Substring(0, 1) == "0" || strBase.Substring(0, 1) == "1" || strBase.Substring(0, 1) == "2" || strBase.Substring(0, 1) == "3" || strBase.Substring(0, 1) == "4" || strBase.Substring(0, 1) == "5" || strBase.Substring(0, 1) == "8")
                    {
                        soma = 0;
                        arrayPesos = new[] { 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 6; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 10);
                        digito2 = (resto == 0 ? "0" : (10 - resto).ToString());
                        strBase2 = (strBase.Substring(0, 6) + digito2);

                        soma = 0;
                        arrayPesos = new[] { 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 7; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase2.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 10);
                        digito1 = ((resto == 0) ? "0" : (10 - resto).ToString());

                        strBase2 = (strBase.Substring(0, 6) + (digito1 + digito2));
                    }
                    else
                    {
                        soma = 0;

                        arrayPesos = new[] { 7, 6, 5, 4, 3, 2 };
                        for (var i = 0; i < 6; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        digito2 = ((resto < 2) ? "0" : (11 - resto).ToString());
                        strBase2 = (strBase.Substring(0, 6) + digito2);

                        soma = 0;
                        arrayPesos = new[] { 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 7; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase2.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        digito1 = ((resto < 2) ? "0" : (11 - resto).ToString());
                        strBase2 = (strBase.Substring(0, 6) + (digito1 + digito2));
                    }

                    if ((strBase2 == strOrigem.PadLeft(8, '0').Substring(strOrigem.Length < 8 ? 0 : strOrigem.Length - 8)))
                        return true;

                    // VALIDA COM 9 DIGITOS
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);

                    soma = 0;
                    arrayPesos = new[] { 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 7; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    if (strBase.Substring(0, 1) == "0" || strBase.Substring(0, 1) == "1" || strBase.Substring(0, 1) == "2" || strBase.Substring(0, 1) == "3" || strBase.Substring(0, 1) == "4" || strBase.Substring(0, 1) == "5" || strBase.Substring(0, 1) == "8")
                    {
                        resto = (soma % 10);
                        digito2 = (resto == 0 ? "0" : (10 - resto).ToString());
                    }
                    else
                    {
                        resto = (soma % 11);
                        digito2 = (resto < 2 ? "0" : (11 - resto).ToString());
                    }


                    strBase2 = (strBase.Substring(0, 7) + digito2);

                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    if (strBase.Substring(0, 1) == "0" || strBase.Substring(0, 1) == "1" || strBase.Substring(0, 1) == "2" || strBase.Substring(0, 1) == "3" || strBase.Substring(0, 1) == "4" || strBase.Substring(0, 1) == "5" || strBase.Substring(0, 1) == "8")
                    {
                        resto = (soma % 10);
                        digito1 = ((resto == 0) ? "0" : (10 - resto).ToString());
                    }
                    else
                    {
                        resto = (soma % 11);
                        digito1 = ((resto < 2) ? "0" : (11 - resto).ToString());
                    }

                    strBase2 = (strBase.Substring(0, 7) + (digito1 + digito2));

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "CE":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);

                    if ((valor > 9))
                        valor = 0;

                    digito1 = valor.ToString();
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "DF":
                    strBase = strOrigem = strOrigem.PadLeft(13, '0').Substring(strOrigem.Length < 13 ? 0 : strOrigem.Length - 13);
                    if ((strBase.Substring(0, 2) == "07"))
                    {
                        soma = 0;

                        arrayPesos = new[] { 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                        for (var i = 0; i < 11; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        valor = (11 - resto);
                        digito1 = (valor > 9 ? "0" : valor.ToString());
                        strBase2 = (strBase.Substring(0, 11) + digito1);

                        soma = 0;
                        arrayPesos = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                        for (var i = 0; i < 12; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase2.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        valor = (11 - resto);
                        digito2 = (valor > 9 ? "0" : valor.ToString());

                        strBase2 = (strBase.Substring(0, 11) + digito1 + digito2);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                case "ES":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    digito1 = ((resto < 2) ? "0" : (11 - resto).ToString());
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "GO":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };
                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);

                    if ((resto == 0))
                    {
                        digito1 = "0";
                    }
                    else if ((resto == 1))
                    {
                        valor = int.Parse(strBase.Substring(0, 8));
                        if (valor >= 10103105 && valor <= 10119997)
                            digito1 = "1";
                        else
                            digito1 = "0";
                    }
                    else
                    {
                        digito1 = (11 - resto).ToString();
                    }

                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "MA":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    if ((strBase.Substring(0, 2) == "12"))
                    {
                        soma = 0;
                        arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };
                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        digito1 = ((resto < 2) ? "0" : (11 - resto).ToString());
                        strBase2 = (strBase.Substring(0, 8) + digito1);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                case "MT":
                    strBase = strOrigem.Substring(0, strOrigem.Length - 1).PadLeft(10, '0').Substring(strOrigem.Length < 11 ? 0 : strOrigem.Length - 11);
                    strOrigem = strOrigem.PadLeft(11, '0').Substring(strOrigem.Length < 11 ? 0 : strOrigem.Length - 11);
                    soma = 0;
                    arrayPesos = new[] { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 10; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    digito1 = ((resto < 2) ? "0" : (11 - resto).ToString());
                    strBase2 = (strBase.Substring(0, 10) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "MS":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    if ((strBase.Substring(0, 2) == "28"))
                    {
                        soma = 0;
                        arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        if (resto < 2)
                        {
                            digito1 = "0";
                        }
                        else
                        {
                            valor = 11 - resto;
                            digito1 = (valor > 9 ? "0" : valor.ToString());
                        }

                        strBase2 = (strBase.Substring(0, 8) + digito1);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                case "MG":
                    strBase = strOrigem = strOrigem.PadLeft(13, '0').Substring(strOrigem.Length < 13 ? 0 : strOrigem.Length - 13);

                    strBase2 = strBase.Substring(0, 3) + "0" + strBase.Substring(3, 8);
                    soma = 0;
                    arrayPesos = new[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };

                    for (var i = 0; i < 12; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);

                        if (valor < 10)
                            soma += valor;
                        else
                            soma += int.Parse(valor.ToString("00").Substring(0, 1)) + int.Parse(valor.ToString("00").Substring(1, 1));
                    }

                    //Subtrai-se o resultado da soma do item anterior, da primeira dezena exata imediatamente superior:
                    //Por exemplo, se a soma for 32, fazer: "40"-32= 8
                    if (soma < 10) //próxima dezena é o 10
                    {
                        valor = 10;
                    }
                    else if (soma % 10 == 0) //já é uma dezena
                    {
                        valor = soma;
                    }
                    else
                    {
                        valor = soma + (10 - (int.Parse(soma.ToString().Substring(soma.ToString().Length - 1, 1))));
                    }

                    digito1 = (valor - soma).ToString();
                    strBase2 = strBase.Substring(0, 11) + digito1;

                    soma = 0;
                    arrayPesos = new[] { 3, 2, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                    for (var i = 0; i < 12; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);
                        soma = soma + valor;
                    }

                    resto = soma % 11;
                    valor = 11 - resto;
                    digito2 = (resto < 2 ? "0" : valor.ToString());

                    strBase2 = strBase.Substring(0, 11) + digito1 + digito2;

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "PA":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    if ((strBase.Substring(0, 2) == "15"))
                    {
                        soma = 0;
                        arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        digito1 = ((resto < 2) ? "0" : (11 - resto).ToString());
                        strBase2 = (strBase.Substring(0, 8) + digito1);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                case "PB":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    digito1 = (valor > 9 ? "0" : valor.ToString());
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "PE":

                    // Validando com 9 dígitos
                    strBase = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);

                    soma = 0;
                    arrayPesos = new[] { 8, 7, 6, 5, 4, 3, 2 };
                    for (var i = 0; i < 7; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    digito1 = (resto < 2 ? "0" : valor.ToString());
                    strBase2 = (strBase.Substring(0, 7) + digito1);

                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };
                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    digito2 = (resto < 2 ? "0" : valor.ToString());

                    strBase2 = (strBase.Substring(0, 7) + digito1 + digito2);
                    if ((strBase2 == strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9)))
                        return true;

                    // Validando com 14 dígitos
                    strBase = strOrigem = strOrigem.PadLeft(14, '0').Substring(strOrigem.Length < 14 ? 0 : strOrigem.Length - 14);

                    soma = 0;
                    arrayPesos = new[] { 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2 };
                    for (var i = 0; i < 13; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    if (valor > 9)
                        valor = valor - 10;
                    digito1 = valor.ToString();

                    strBase2 = (strBase.Substring(0, 13) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "PI":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };
                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }
                    resto = (soma % 11);
                    valor = 11 - resto;
                    digito1 = (valor > 9 ? "0" : valor.ToString());
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "PR":
                    strBase = strOrigem = strOrigem.PadLeft(10, '0').Substring(strOrigem.Length < 10 ? 0 : strOrigem.Length - 10);
                    soma = 0;
                    arrayPesos = new[] { 3, 2, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    digito1 = (valor > 9 ? "0" : valor.ToString());

                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    soma = 0;
                    arrayPesos = new[] { 4, 3, 2, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 9; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    digito2 = (valor > 9 ? "0" : valor.ToString());

                    strBase2 = (strBase.Substring(0, 8) + digito1 + digito2);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "RJ":
                    strBase = strOrigem = strOrigem.PadLeft(8, '0').Substring(strOrigem.Length < 8 ? 0 : strOrigem.Length - 8);
                    soma = 0;
                    arrayPesos = new[] { 2, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 7; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    digito1 = ((resto < 2) ? "0" : valor.ToString());

                    strBase2 = (strBase.Substring(0, 7) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "RN":

                    // Validando com 9 dígitos
                    strBase = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    if ((strBase.Substring(0, 2) == "20"))
                    {
                        soma = 0;
                        arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        soma = (soma * 10);
                        resto = (soma % 11);
                        digito1 = ((resto > 9) ? "0" : resto.ToString());

                        strBase2 = (strBase.Substring(0, 8) + digito1);

                        if ((strBase2 == strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9)))
                            return true;
                    }

                    // Validando com 10 dígitos
                    strBase = strOrigem = strOrigem.PadLeft(10, '0').Substring(strOrigem.Length < 10 ? 0 : strOrigem.Length - 10);
                    if ((strBase.Substring(0, 2) == "20"))
                    {
                        soma = 0;
                        arrayPesos = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 9; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        soma = (soma * 10);
                        resto = (soma % 11);
                        digito1 = ((resto > 9) ? "0" : resto.ToString());

                        strBase2 = (strBase.Substring(0, 9) + digito1);

                        if ((strBase2 == strOrigem.Substring(strOrigem.Length < 10 ? 0 : strOrigem.Length - 10)))
                            return true;
                    }
                    break;

                case "RO":

                    // Validando com 9 dígitos
                    strBase = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);

                    strBase2 = strBase.Substring(3, 5);
                    soma = 0;
                    arrayPesos = new[] { 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 5; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);

                    if ((valor > 9))
                        valor = (valor - 10);

                    digito1 = Convert.ToString(valor);
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9)))
                        return true;

                    // Validando com 13 dígitos 
                    strBase = strOrigem = strOrigem.PadLeft(14, '0').Substring(strOrigem.Length < 14 ? 0 : strOrigem.Length - 14);

                    strBase2 = strBase.Substring(0, 13);
                    soma = 0;
                    arrayPesos = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 13; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);

                    if ((valor > 9))
                        valor = (valor - 10);

                    digito1 = Convert.ToString(valor);
                    strBase2 = (strBase.Substring(0, 13) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "RR":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    if ((strBase.Substring(0, 2) == "24"))
                    {
                        soma = 0;
                        arrayPesos = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 9);
                        digito1 = resto.ToString();

                        strBase2 = (strBase.Substring(0, 8) + digito1);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                case "RS":
                    strBase = strOrigem = strOrigem.PadLeft(10, '0').Substring(strOrigem.Length < 10 ? 0 : strOrigem.Length - 10);
                    arrayPesos = new[] { 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    soma = 0;
                    for (var i = 0; i < 9; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);
                    if ((valor > 9))
                        valor = 0;

                    digito1 = valor.ToString();
                    strBase2 = (strBase.Substring(0, 9) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "SC":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };
                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = 11 - resto;
                    digito1 = ((resto < 2) ? "0" : valor.ToString());
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "SE":
                    strBase = strOrigem = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = (11 - resto);

                    if ((valor > 9))
                        valor = 0;

                    digito1 = valor.ToString();
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "SP":
                    if ((strOrigem.Substring(0, 1) == "P"))
                    {
                        strBase = strOrigem = "P" + strOrigem.PadLeft(12, '0').Substring(strOrigem.Length < 12 ? 0 : strOrigem.Length - 12);
                        strBase2 = strBase.Substring(1, 9);
                        soma = 0;

                        arrayPesos = new[] { 1, 3, 4, 5, 6, 7, 8, 10 };

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];

                            valor = int.Parse(strBase2.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        digito1 = Convert.ToString(resto).Substring((Convert.ToString(resto).Length - 1));
                        strBase2 = (strBase.Substring(0, 9) + (digito1 + strBase.Substring(10, 3)));
                    }
                    else
                    {
                        strBase = strOrigem = strOrigem.PadLeft(12, '0').Substring(strOrigem.Length < 12 ? 0 : strOrigem.Length - 12);
                        soma = 0;
                        arrayPesos = new[] { 1, 3, 4, 5, 6, 7, 8, 10 };

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        digito1 = Convert.ToString(resto).Substring((Convert.ToString(resto).Length - 1));

                        strBase2 = (strBase.Substring(0, 8) + (digito1 + strBase.Substring(9, 2)));
                        soma = 0;
                        arrayPesos = new[] { 3, 2, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 11; i++)
                        {
                            peso = arrayPesos[i];

                            valor = int.Parse(strBase2.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        digito2 = Convert.ToString(resto).Substring((Convert.ToString(resto).Length - 1));
                        strBase2 = (strBase2 + digito2);
                    }

                    if ((strBase2 == strOrigem))
                        return true;
                    break;

                case "TO":

                    // Validando com 9 dígitos
                    strBase = strOrigem.PadLeft(9, '0').Substring(strOrigem.Length < 9 ? 0 : strOrigem.Length - 9);

                    strBase2 = strBase.Substring(0, 8);
                    soma = 0;
                    arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                    for (var i = 0; i < 8; i++)
                    {
                        peso = arrayPesos[i];
                        valor = int.Parse(strBase2.Substring(i, 1));
                        valor = (valor * peso);
                        soma = (soma + valor);
                    }

                    resto = (soma % 11);
                    valor = 11 - resto;
                    digito1 = ((resto < 2) ? "0" : valor.ToString());
                    strBase2 = (strBase.Substring(0, 8) + digito1);

                    if ((strBase2 == strOrigem.PadLeft(9, '0')))
                        return true;

                    // Validando com 11 dígitos
                    strBase = strOrigem = strOrigem.PadLeft(11, '0').Substring(strOrigem.Length < 11 ? 0 : strOrigem.Length - 11);

                    if (strBase.Substring(2, 2) == "01" || strBase.Substring(2, 2) == "02" || strBase.Substring(2, 2) == "03" || strBase.Substring(2, 2) == "99")
                    {
                        strBase2 = (strBase.Substring(0, 2) + strBase.Substring(4, 6));
                        soma = 0;
                        arrayPesos = new[] { 9, 8, 7, 6, 5, 4, 3, 2 };

                        for (var i = 0; i < 8; i++)
                        {
                            peso = arrayPesos[i];
                            valor = int.Parse(strBase2.Substring(i, 1));
                            valor = (valor * peso);
                            soma = (soma + valor);
                        }

                        resto = (soma % 11);
                        valor = 11 - resto;
                        digito1 = ((resto < 2) ? "0" : valor.ToString());
                        strBase2 = (strBase.Substring(0, 10) + digito1);

                        if ((strBase2 == strOrigem))
                            return true;
                    }
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unidade de UF inválida \"{0}\"", uf));
            }
            return false;
        }

        /// <summary>
        ///     Valida se o IE está vazio
        /// </summary>
        public static bool IsIeEmpty(string number)
        {
            if (string.IsNullOrEmpty(number))
                return true;

            if (IsIeIsento(number))
                return false;

            var strOrigem = ClearIe(number);

            return string.IsNullOrEmpty(strOrigem);
        }

        /// <summary>
        ///     Valida se é isento
        /// </summary>
        public static bool IsIeIsento(string number)
        {
            if (string.IsNullOrEmpty(number) || number.Trim().Length == 0)
                return false;

            var variacoes = "ISENTO,INSENTO,IZENTO,INZENTO,ISENTA,INSENTA,IZENTA,INZENTA".Split(',');
            return variacoes.Contains(number.ToUpper().Trim());
        }

        /// <summary>
        ///     Formata Inscrição Estadual
        /// </summary>
        public static string FormatIe(string uf, string number, bool mask = true)
        {
            uf = (!string.IsNullOrEmpty(uf) ? uf.Trim().ToUpper() : string.Empty);
            number = (!string.IsNullOrEmpty(number) ? number.Trim().ToUpper().Replace(".", "").Replace("/", "").Replace("-", "") : string.Empty);

            if (IsIeEmpty(number))
                return string.Empty;

            if (IsIeIsento(number))
                return "ISENTO";

            var strOrigem = ClearIe(number);

            if (!IsIe(uf, strOrigem))
                return number;

            string formatted;

            var value = ParserTools.ToInt64(strOrigem.Replace("P", string.Empty));

            switch (uf)
            {
                case "AC":
                    //ex: 01.004.823/001-12 (11 + 2 dígitos)
                    formatted = value.ToString(@"00\.000\.000/000\-00");
                    break;

                case "AL":
                    //ex: 240000048 ("24" + 1 Tipo empresa + 5 número empresa + digito)
                    formatted = value.ToString(@"000000000");
                    break;

                case "AP":
                    //ex: 030123459 ("03" + 6 dígitos (empresa) + 1 dígito verificador)
                    formatted = value.ToString(@"000000000");
                    break;

                case "AM":
                    //ex: 52.718.546-9 (99.999.999-9)
                    formatted = value.ToString(@"00\.000\.000\-0");
                    break;

                case "BA":
                    //ex: 123456-63 (8números) ou 1000003-06 (9 números)
                    if (value.ToString().Length <= 8)
                        formatted = value.ToString(@"000000\-00");
                    else
                        formatted = value.ToString(@"0000000\-00");
                    break;

                case "CE":
                    //ex: 06000001-5 (8 números + 1 dígito)
                    formatted = value.ToString(@"00000000\-0");
                    break;

                case "DF":
                    //ex: 07300001001-09 ("07" + 6 sequencial + 3 matriz/filial + 2 digitos)
                    formatted = value.ToString(@"00000000000\-00");
                    break;

                case "ES":
                    //ex: 99999999-0 (8 números + digito)
                    formatted = value.ToString(@"00000000\-0");
                    break;

                case "GO":
                    //ex: 10.987.654-7 (AB.CDE.FGH-I = 8 dígitos (ABCDEFGH) + 1 dígito verificador (I); onde AB pode ser igual a 10 ou 11 ou 15)
                    formatted = value.ToString(@"00\.000\.000\-0");
                    break;

                case "MA":
                    //ex: 120000385 (9 números = "12" código estado + 6 números cadastro + 1 digito)
                    formatted = value.ToString(@"000000000");
                    break;

                case "MT":
                    //ex: 0013000001-9 (NNNNNNNNNN-D = 10 numeros + 1 digito)
                    formatted = value.ToString(@"0000000000\-0");
                    break;

                case "MS":
                    //ex: 283352868 (8 números + dígito verificador)
                    formatted = value.ToString(@"000000000");
                    break;

                case "MG":
                    //ex: 062.307.904/0081 (AAA BBB BBB CC DD = Onde: A=Código do Município, B=Número da inscrição, C=Número de ordem do estabelecimento, D=Dígitos de controle)
                    formatted = value.ToString(@"000\.000\.000\/0000");
                    break;

                case "PA":
                    //ex: 15999999-5 (8 números + 1 dígito)
                    formatted = value.ToString(@"00000000\-0");
                    break;

                case "PB":
                    //ex: 06000001-5 (8 números + 1 dígito)
                    formatted = value.ToString(@"00000000\-0");
                    break;

                case "PR":
                    //ex: 123.45678-50 (NNNNNNNN-DD)
                    formatted = value.ToString(@"000\.00000\-00");
                    break;

                case "PE":
                    //ex: 0321418-40 (7 números + 2 dígitos)
                    // ou 18.1.001.0000004-9 (13 números + 1 dígito)
                    if (value.ToString().Length <= 9)
                        formatted = value.ToString(@"0000000\-00");
                    else
                        formatted = value.ToString(@"00\.0\.000\.0000000\-0");

                    break;

                case "PI":
                    //ex: 012345679 (8 números + verificador)
                    formatted = value.ToString(@"000000000");
                    break;

                case "RJ":
                    //ex: 99.999.99-3
                    formatted = value.ToString(@"00\.000\.00\-0");
                    break;

                case "RN":
                    //ex: 20.040.040-1(9 dígitos) 
                    // ou 20.0.040.040-0(10 dígitos)
                    if (value.ToString().Length <= 9)
                        formatted = value.ToString(@"00\.000\.000\-0");
                    else
                        formatted = value.ToString(@"00\.0\.000\.000\-0");
                    break;

                case "RS":
                    //ex: 224/3658792 (3 dígitos (município) + 6 dígitos (empresa) + 1 dígito verificador)
                    formatted = value.ToString(@"000\/0000000");
                    break;

                case "RO":
                    //ex: 101.62521-3 ("101" + 5 números + dígito) 
                    // ou 0000000062521-3 (13 dígitos (empresa) + 1 dígito verificador)
                    //if (value.ToString().Length <= 9)
                    //	formatted = value.ToString(@"000\.00000\-0");
                    //else
                    formatted = value.ToString(@"0000000000000\-0");
                    break;

                case "RR":
                    //ex: 24006628-1, 24001755-6, 24003429-0 (8 números + dígito)
                    formatted = value.ToString(@"00000000\-0");
                    break;

                case "SC":
                    //ex: 251.040.852 (7 números + 1 dígito)
                    formatted = value.ToString(@"000\.000\.000");
                    break;

                case "SP":
                    //ex: 110.042.490.114  (Industriais e comerciantes -> 10 números + 2 dígitos)
                    // ou P-01100424.3/002  (Inscrição estadual de Produtor Rural -> P0MMMSSSSD000 = "P" + 0MMMSSSS (algarismos que serão utilizados no cálculo do dígito verificador) + Dígito + 000 = (três) dígitos que compõem o nº de inscrição mas não utilizados no cálculo do dígito verificador)
                    if (number.StartsWith("P", StringComparison.CurrentCultureIgnoreCase))
                        formatted = string.Format("P-{0}", value.ToString(@"00000000\.0\/000"));
                    else
                        formatted = value.ToString(@"000\.000\.000\.000");
                    break;

                case "SE":
                    //ex: 27123456-3 (8 números + 1 dígito)
                    formatted = value.ToString(@"00000000\-0");
                    break;

                case "TO":
                    // ex: 29 022783 6
                    // ou 29 01 022783 6 = (2 números + "01" ou "02" ou "03" ou "99" + 6 números + dígito) 
                    if (value.ToString().Length <= 9)
                        formatted = value.ToString(@"000000000");
                    else
                        formatted = value.ToString(@"00000000000");
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unidade de UF inválida \"{0}\"", uf));
            }

            if (!mask)
                return ClearIe(formatted);

            return formatted;
        }

        /// <summary>
        ///     Remove todos os caracteres não aceitos no IE
        /// </summary>
        public static string ClearIe(string number)
        {
            if (number == null)
                return null;

            if (IsIeIsento(number))
                return "ISENTO";

            var ret = (new Regex(@"([^0-9])")).Replace(number, string.Empty);

            if (number.StartsWith("P", StringComparison.CurrentCultureIgnoreCase))
                ret = string.Format("P{0}", number);

            return ret;
        }
        #endregion

        #region CPF

        public static bool IsCpf(long cpf)
        {
            return IsCpf(cpf.ToString().PadLeft(11, '0'));
        }

        public static bool IsCpf(string cpf)
        {
            var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf == "00000000000" ||
                cpf == "11111111111" ||
                cpf == "22222222222" ||
                cpf == "33333333333" ||
                cpf == "44444444444" ||
                cpf == "55555555555" ||
                cpf == "66666666666" ||
                cpf == "77777777777" ||
                cpf == "88888888888" ||
                cpf == "99999999999")
                return false;

            if (cpf.Length != 11)
                return false;
            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            var resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            var digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto;
            return cpf.EndsWith(digito);
        }

        public static string FormatCpf(string number)
        {
            if (string.IsNullOrEmpty(number) || !IsCpf(number))
                return number;

            number = number.Trim().Replace(".", "").Replace("-", "");

            return Convert.ToUInt64(number).ToString(@"000\.000\.000\-00");
        }

        #endregion

        #region CNPJ

        public static bool IsCnpj(long cnpj)
        {
            return IsCnpj(cnpj.ToString().PadLeft(14, '0'));
        }

        public static bool IsCnpj(string cnpj)
        {
            var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj == "00000000000000" ||
                cnpj == "11111111111111" ||
                cnpj == "22222222222222" ||
                cnpj == "33333333333333" ||
                cnpj == "44444444444444" ||
                cnpj == "55555555555555" ||
                cnpj == "66666666666666" ||
                cnpj == "77777777777777" ||
                cnpj == "88888888888888" ||
                cnpj == "99999999999999")
                return false;

            if (cnpj.Length != 14)
                return false;
            var tempCnpj = cnpj.Substring(0, 12);
            var soma = 0;
            for (var i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            var resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            var digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto;
            return cnpj.EndsWith(digito);
        }

        public static string FormatCnpj(string number)
        {
            if (string.IsNullOrEmpty(number) || !IsCnpj(number))
                return number;

            number = number.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

            return Convert.ToUInt64(number).ToString(@"00\.000\.000\/0000\-00");
        }

        #endregion

        #region Email

        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*");
            Match match = regex.Match(email);
            return match.Success;
        }

        #endregion

        #region Zipcode

        public static bool IsValidZipcode(string zipcode)
        {
            if (string.IsNullOrWhiteSpace(zipcode))
                return false;

            zipcode = zipcode.Trim().Replace(".", "").Replace("-", "");
            return (zipcode.Length == 8 && TextTools.GetNumericCharacters(zipcode).Equals(zipcode));
        }

        public static string FormatZipcode(string zipcode)
        {
            if (!IsValidZipcode(zipcode))
                return zipcode;

            zipcode = TextTools.GetNumericCharacters(zipcode).PadLeft(8, '0');
            zipcode = string.Concat(zipcode.Substring(0, 5), "-", zipcode.Substring(5, 3));

            return zipcode;
        }

        #endregion

        #region AddressUf

        public static bool IsValidAddressUf(string uf)
        {
            if (string.IsNullOrWhiteSpace(uf))
                return false;

            uf = uf.Trim().ToUpper();

            var states = new string[] { "AC", "AL", "AM", "AP", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RO", "RS", "RR", "SC", "SE", "SP", "TO" };
            return states.Contains(uf);
        }

        #endregion

        #region Phone

        public static string FormatPhone(string phone)
        {
            string text = phone;

            if (string.IsNullOrWhiteSpace(text))
                return text;

            //Ignora telefones internacionais
            if (text.StartsWith("+"))
                return text;

            text = TextTools.GetNumericCharacters(text);

            //Remove os zeros a esquerda
            while (text.StartsWith("0"))
                text = text.Substring(1);

            if (text.Length == 8)
                text = string.Format("{0}-{1}", text.Substring(0, 4), text.Substring(4, 4));
            else if (text.Length == 9)
                text = string.Format("{0}-{1}", text.Substring(0, 5), text.Substring(5, 4));
            else if (text.Length == 10)
                text = string.Format("({0}) {1}-{2}", text.Substring(0, 2), text.Substring(2, 4), text.Substring(6, 4));
            else if (text.Length == 11)
                text = string.Format("({0}) {1}-{2}", text.Substring(0, 2), text.Substring(2, 5), text.Substring(7, 4));

            return text;
        }

        #endregion


        public static bool IsValidGtin(string code)
        {
            if (code != (new Regex("[^0-9]")).Replace(code, ""))
            {
                // is not numeric
                return false;
            }
            // pad with zeros to lengthen to 14 digits
            switch (code.Length)
            {
                case 8:
                    code = "000000" + code;
                    break;
                case 12:
                    code = "00" + code;
                    break;
                case 13:
                    code = "0" + code;
                    break;
                case 14:
                    break;
                default:
                    // wrong number of digits
                    return false;
            }
            // calculate check digit
            int[] a = new int[13];
            a[0] = int.Parse(code[0].ToString()) * 3;
            a[1] = int.Parse(code[1].ToString());
            a[2] = int.Parse(code[2].ToString()) * 3;
            a[3] = int.Parse(code[3].ToString());
            a[4] = int.Parse(code[4].ToString()) * 3;
            a[5] = int.Parse(code[5].ToString());
            a[6] = int.Parse(code[6].ToString()) * 3;
            a[7] = int.Parse(code[7].ToString());
            a[8] = int.Parse(code[8].ToString()) * 3;
            a[9] = int.Parse(code[9].ToString());
            a[10] = int.Parse(code[10].ToString()) * 3;
            a[11] = int.Parse(code[11].ToString());
            a[12] = int.Parse(code[12].ToString()) * 3;
            int sum = a[0] + a[1] + a[2] + a[3] + a[4] + a[5] + a[6] + a[7] + a[8] + a[9] + a[10] + a[11] + a[12];
            int check = (10 - (sum % 10)) % 10;
            // evaluate check digit
            int last = int.Parse(code[13].ToString());
            return check == last;
        }
    }
}
