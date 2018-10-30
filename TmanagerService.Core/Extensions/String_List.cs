
using System.Collections.Generic;

namespace TmanagerService.Core.Extensions
{
    public static class String_List
    {
        private const string strMark = ",NStr~| ";
        public static List<string> ToList(string str)
        {
            if (str == null)
                return null;
            List<string> lstStr = new List<string>();
            foreach (string st in str.Split(strMark))
            {
                lstStr.Add(st);
            }
            return lstStr;
        }

        public static string ToString(List<string> lstStr)
        {
            if (lstStr.Count == 0)
                return null;
            return string.Join(strMark, lstStr);
        }

        public static string AddString(string lstStr, string str)
        {
            if (lstStr == null)
                lstStr = str;
            else
                lstStr = lstStr + strMark + str;
            return lstStr;
        }
    }
}
