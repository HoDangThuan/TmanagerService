using System.Collections.Generic;
using System.Linq;

namespace TmanagerService.Core.Extensions
{
    public static class ConvertListPicture
    {
        public static string ToString(List<string> list_Pic)
        {
            string result = "";
            foreach (var pic in list_Pic)
            {
                result = result +"|"+ pic;
            }
            return result;
        }

        public static List<string> ToList(string string_Pic)
        {
            string[] result = string_Pic.Split(new char[] { '|' });
            return result.ToList();
        }
    }
}
