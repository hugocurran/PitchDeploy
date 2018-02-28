using System;
using System.Collections.Generic;
using System.Text;

namespace PitchDeploy
{
    public static class Extensions
    {
        public static string NoHlaRoot(this string s)
        {
            string[] s1 = s.Split('.');
            return s.Substring(s1[0].Length + 1); //trailing .
        }
    }
}
