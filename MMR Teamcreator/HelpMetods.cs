using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Win32;
using MMR_Teamcreator.Model;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MMR_Teamcreator
{
    static class HelpMetods
    {
        public static bool ContainsNumber(string args) => args.ToCharArray().Any(x => char.IsNumber(x));
        public static bool IsPath(string path)
        {
            if (!path.Contains('\\') || !path.Contains(':'))
                return false;
            if (path.Split(':')[0].Length != 1)
                return false;
            return true;
        }
        public static string StringRange(string str, int[] range)
        {
            try
            {
                if (range.Count() != 2)
                    throw new Exception("Range must contain start and end index only!");
                if (range[0] > range[1])
                    throw new Exception("Starting index must be lower than ending");
                if (str.Length-1 < range[1])
                    throw new Exception("");

                string _out = string.Empty;

                for (int i = 0; i < range[1]; i++)
                    _out += str[i];

                return _out += "...";                
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}", "ERROR", MessageBoxButton.OK);
                return default;
            }
        }
    }
}