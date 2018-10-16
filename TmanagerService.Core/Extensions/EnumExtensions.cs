using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TmanagerService.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription<T>(this T value) where T : struct
        {
            return value
                       .GetType()
                       .GetMember(value.ToString())
                       .FirstOrDefault()
                       ?.GetCustomAttribute<DescriptionAttribute>()
                       ?.Description;
        }
    }
}
