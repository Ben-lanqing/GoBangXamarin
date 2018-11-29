using System;
using System.Collections.Generic;
using System.Text;

namespace GoBangXamarin
{
    public static class StaticClass
    {
        public static string LogException(string sender, Exception ex)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{sender} Exception! Message:{ex.Message}");
            stringBuilder.AppendLine($"{sender} Exception! StackTrace:{ex.StackTrace}");
            var inEx = ex.InnerException;
            while (inEx != null)
            {
                stringBuilder.AppendLine($"InnerException! Message:{inEx.Message}");
                stringBuilder.AppendLine($"InnerException! StackTrace:{inEx.StackTrace}");
                inEx = ex.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}
