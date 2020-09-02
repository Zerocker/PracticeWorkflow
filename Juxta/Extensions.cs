using System;
using System.Linq;
using Juxta.Models;

namespace Juxta
{
    public static class Extensions
    {
        public static string[][] Matches { get; } =
        {
            new string[] { "соблюд", "самоизол", "находи", "прожив" },
            new string[] { "двер" },
            new string[] { "госпитал" },
            new string[] { "местонахож", "устанавл", "не прожив" }
        };

        public static ResultEnum Parse(this string result)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Matches[i].Any(c => result.ToLower().Contains(c)))
                    return (ResultEnum)i;
            }
            return ResultEnum.None;
        }
    }
}
