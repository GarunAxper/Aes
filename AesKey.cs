using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES_KeySchedule
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[,] matrix = new string[4, 4];
                Console.WriteLine("Enter coords of elements in matrix: ");
                int left = Console.CursorLeft;
                int top = Console.CursorTop;
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (j != 0)
                        {
                            top = Console.CursorTop;
                        }
                        else
                        {
                            Console.SetCursorPosition(left, top);
                        }
                        matrix[i, j] = Console.ReadLine();
                        Console.SetCursorPosition(left + 8, top);
                        top = Console.CursorTop;
                        left = Console.CursorLeft;
                    }
                    left = 0;
                    top++;
                }
                Console.WriteLine();
                Console.WriteLine("---Rounds' results---");
                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine("Round " + i.ToString());
                    matrix = GenerateRoundKeyMatrix(matrix, i);
                    DisplayMatrix(matrix);
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong, please try again...");
                Console.ReadLine();
            }
        }

        static readonly string[,] sBox = new string[16, 16]
        {
            {"63","7c","77","7b","f2","6b","6f","c5","30","01","67","2b","fe","d7","ab","76"},
            {"ca","82","c9","7d","fa","59","47","f0","ad","d4","a2","af","9c","a4","72","c0"},
            {"b7","fd","93","26","36","3f","f7","cc","34","a5","e5","f1","71","d8","31","15"},
            {"04","c7","23","c3","18","96","05","9a","07","12","80","e2","eb","27","b2","75"},
            {"09","83","2c","1a","1b","6e","5a","a0","52","3b","d6","b3","29","e3","2f","84"},
            {"53","d1","00","ed","20","fc","b1","5b","6a","cb","be","39","4a","4c","58","cf"},
            {"d0","ef","aa","fb","43","4d","33","85","45","f9","02","7f","50","3c","9f","a8"},
            {"51","a3","40","8f","92","9d","38","f5","bc","b6","da","21","10","ff","f3","d2"},
            {"cd","0c","13","ec","5f","97","44","17","c4","a7","7e","3d","64","5d","19","73"},
            {"60","81","4f","dc","22","2a","90","88","46","ee","b8","14","de","5e","0b","db"},
            {"e0","32","3a","0a","49","06","24","5c","c2","d3","ac","62","91","95","e4","79"},
            {"e7","c8","37","6d","8d","d5","4e","a9","6c","56","f4","ea","65","7a","ae","08"},
            {"ba","78","25","2e","1c","a6","b4","c6","e8","dd","74","1f","4b","bd","8b","8a"},
            {"70","3e","b5","66","48","03","f6","0e","61","35","57","b9","86","c1","1d","9e"},
            {"e1","f8","98","11","69","d9","8e","94","9b","1e","87","e9","ce","55","28","df"},
            {"8c","a1","89","0d","bf","e6","42","68","41","99","2d","0f","b0","54","bb","16"}
        };

        static readonly string[,] rCons = new string[4, 10]
        {
            { "01","02","04","08","10","20","40","80","1b","36", },
            { "00","00","00","00","00","00","00","00","00","00", },
            { "00","00","00","00","00","00","00","00","00","00", },
            { "00","00","00","00","00","00","00","00","00","00", }
        };

        public static string[,] GenerateRoundKeyMatrix(string[,] matrix, int round)
        {
            var roundKeyMatrix = new string[4, 4];
            var lastColumn = Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, matrix.GetLength(1) - 1]).ToArray();
            lastColumn = lastColumn.Skip(1).Concat(lastColumn.Take(1)).ToArray();
            lastColumn = SubBytes(lastColumn);
            var firstColumn = Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, 0]).ToArray();
            var rConsColumn = Enumerable.Range(0, rCons.GetLength(0)).Select(x => rCons[x, round - 1]).ToArray();
            firstColumn = XorColumns(firstColumn, lastColumn);
            firstColumn = XorColumns(firstColumn, rConsColumn);
            for (int i = 0; i < roundKeyMatrix.GetLength(0); i++)
            {
                roundKeyMatrix[i, 0] = firstColumn[i];
            }
            var secondColumn = Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, 1]).ToArray();
            secondColumn = XorColumns(secondColumn, firstColumn);
            for (int i = 0; i < roundKeyMatrix.GetLength(0); i++)
            {
                roundKeyMatrix[i, 1] = secondColumn[i];
            }
            var thirdColumn = Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, 2]).ToArray();
            thirdColumn = XorColumns(thirdColumn, secondColumn);
            for (int i = 0; i < roundKeyMatrix.GetLength(0); i++)
            {
                roundKeyMatrix[i, 2] = thirdColumn[i];
            }
            var finalColumn = Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, matrix.GetLength(1) - 1]).ToArray();
            finalColumn = XorColumns(finalColumn, thirdColumn);
            for (int i = 0; i < roundKeyMatrix.GetLength(0); i++)
            {
                roundKeyMatrix[i, 3] = finalColumn[i];
            }
            return roundKeyMatrix;
        }
        public static string[] SubBytes(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                var sBoxRow = int.Parse(item[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                var sBoxColumn = int.Parse(item[item.Length - 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                var subByte = sBox[sBoxRow, sBoxColumn];
                arr[i] = subByte;
            }
            return arr;
        }
        public static string[] XorColumns(string[] arr1, string[] arr2)
        {
            var result = new string[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                string s1 = FromHexToBinary(arr1[i]);
                string s2 = FromHexToBinary(arr2[i]);
                string xorResult = Xor(s1, s2);
                result[i] = FromBinaryToHex(xorResult);
            }
            return result;
        }
        public static string Xor(string s1, string s2)
        {
            var result = string.Empty;
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] == s2[i])
                {
                    result += "0";
                }
                else
                {
                    result += "1";
                }
            }
            return result;
        }
        public static string FromHexToBinary(string hex)
        {
            var binar = Convert.ToString(Convert.ToInt64(hex, 16), 2);
            if (binar.Length < 8)
            {
                var result = string.Empty;
                for (int i = 0; i < 8 - binar.Length; i++)
                {
                    result += "0";
                }
                result += binar;
                return result;
            }
            else
            {
                return binar;
            }
        }
        public static string FromBinaryToHex(string binary)
        {
            var hex = Convert.ToInt32(binary, 2).ToString("X");
            if (hex.Length < 2)
            {
                var result = string.Empty;
                result += "0";
                result += hex;
                return result;
            }
            else
            {
                return hex;
            }
        }
        public static void DisplayMatrix(string[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
