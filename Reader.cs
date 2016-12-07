using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBOT.ServerGUI
{
    public static class Reader
    {
        public enum Results
        {
            BARCODE_OK,
            BARCODE_NONSTD,
            BARCODE_ERROR,
            QR_OK,
            QR_ERRORNONSTD,
            QR_ERRORBARCOE,
            QR_ERRORSIZE
        }

        private static string barcode;

        public static string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        private static string serialNo;

        public static string SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        private static DateTime expDate;

        public static DateTime ExpDate
        {
            get { return expDate; }
            set { expDate = value; }
        }

        private static string partyNo;

        public static string PartyNo
        {
            get { return partyNo; }
            set { partyNo = value; }
        }

        private static char[] alfabe ={'A','B','C','D','E','F','G','H','I',
                                        'J','K','L','M','N','O','Q','P','R',
                                        'S','T','U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i',
                                        'j','k','l','m','n','o','q','p','r',
                                        's','t','u','v','w','x','y','z'};



        public static Results CheckInput(string _inputQR)
        {
            if (_inputQR.Length == 8 || _inputQR.Length == 13 || _inputQR.Length == 14)
            {
                if (_inputQR.IndexOfAny(alfabe, 0, _inputQR.Length) == -1)
                {
                    if (Correct_Barcode(_inputQR))
                    {
                        Barcode = _inputQR;
                        return Results.BARCODE_OK;
                    }
                    else
                    {
                        Barcode = string.Empty;
                        return Results.BARCODE_NONSTD;
                    }
                }
                else
                {
                    Barcode = string.Empty;
                    return Results.BARCODE_ERROR;
                }
            }
            else if (_inputQR.Length > 14 && _inputQR.Length <= 69)
            {
                if (_inputQR.IndexOfAny(alfabe, 0, 14) == -1)
                {
                    //Divide QR
                    if (DivideQR(_inputQR))
                        return Results.QR_OK;
                    else
                    {
                        Barcode = string.Empty;
                        SerialNo = string.Empty;
                        PartyNo = string.Empty;
                        ExpDate = DateTime.MinValue;

                        return Results.QR_ERRORNONSTD;
                    }
                }
                else
                {
                    Barcode = string.Empty;
                    SerialNo = string.Empty;
                    PartyNo = string.Empty;
                    ExpDate = DateTime.MinValue;

                    return Results.QR_ERRORBARCOE;
                }
            }
            else
            {
                Barcode = string.Empty;
                SerialNo = string.Empty;
                PartyNo = string.Empty;
                ExpDate = DateTime.MinValue;

                return Results.QR_ERRORSIZE;
            }
        }

        private static bool DivideQR(string code)
        {
            string[] sonuc = new string[4];
            string[] buffer = new string[2];

            char[] read_code;
            char[] read_code_last;

            if (code[0] == '0' && code[1] == '1' && code.IndexOf(".") != -1)
            {
                buffer = code.Split('.');
                read_code = buffer[0].ToCharArray();
                read_code_last = buffer[1].ToCharArray();

                if ((read_code[0] == '0' && read_code[1] == '1') && (read_code[16] == '2' && read_code[17] == '1') && (read_code_last[0] == '1' && read_code_last[1] == '7') && (read_code_last[8] == '1' && read_code_last[9] == '0'))
                {
                    for (int i = 2; i < 16; i++)
                        sonuc[0] += read_code[i];

                    for (int i = 18; i < read_code.Length; i++)
                        sonuc[1] += read_code[i];

                    for (int i = 2; i < 8; i++)
                        sonuc[2] += read_code_last[i];

                    for (int i = 10; i < read_code_last.Length; i++)
                        sonuc[3] += read_code_last[i];

                    if (!Correct_Barcode(sonuc[0]))
                    {
                        sonuc[0] = string.Empty;
                        return false;
                    }
                    else
                    {
                        string remZero = sonuc[0];

                        if (remZero[0] == '0')
                            sonuc[0] = remZero.Remove(0, 1);

                        Barcode = sonuc[0];
                        SerialNo = sonuc[1];

                        int year, month, day;
                        string tempExpDate = sonuc[2];
                        Int32.TryParse(tempExpDate.Substring(0, 2), out year);
                        Int32.TryParse(tempExpDate.Substring(2, 2), out month);
                        Int32.TryParse(tempExpDate.Substring(4, 2), out day);

                        ExpDate = new DateTime(2000+year, month, day == 0 ? 31 : day);                        

                        PartyNo = sonuc[3];
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Local Method to check barcode GTIN standards 
        /// </summary>
        /// <param name="barcode">Barcode to be checked</param>
        /// <returns>Returns true if GTIN standards are applicable</returns>
        private static bool Correct_Barcode(string barcode)
        {
            bool sonuc = false;

            char[] elements = barcode.ToCharArray();
            int total_el = elements.Length;
            int kontrol;

            if (Int32.TryParse(elements[total_el - 1].ToString(), out kontrol))
            {
                int kont_total = 0;
                for (int i = total_el - 2; i >= 0; i = i - 2)
                {
                    kont_total += Int32.Parse(elements[i].ToString()) * 3;
                }
                for (int i = total_el - 3; i >= 0; i = i - 2)
                {
                    kont_total += Int32.Parse(elements[i].ToString());
                }

                if ((10 - (kont_total % 10)) % 10 == kontrol)
                {
                    sonuc = true;
                }
            }

            return sonuc;
        }
    }
}
