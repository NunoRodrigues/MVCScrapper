using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MVCScrapper
{
    public static class Extensions
    {
        public static string CleanString(this string value)
        {
            return value.CleanString(true);
            //return value;
        }

        public static string CleanString(this string value, bool htmlDecode)
        {
            if (htmlDecode) value = HttpUtility.HtmlDecode(value);
            return Regex.Replace(value.Trim(), @"\s+", " ").Replace(":", "").ToString();
        }

        public static string ToKey(this string value)
        {
            return value.CleanString().Replace(" ","");
        }

        /*
        public static int CountOld(this string value, string find)
        {
            string[] slices = value.Split(new string[] { "&nbsp;&nbsp;" }, StringSplitOptions.None);
            return slices.Length;
        }
        

        public static int Count(this string value, string find)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = value.IndexOf(find, i)) != -1)
            {
                i += find.Length;
                count++;
            }
            return count;
        }
        */
        
        public static int StartsWithCounter(this string value, string find)
        {
            int count = 0;
            
            int nextIndex = value.IndexOf(find, 0);
            
            for (int i = 0; i < value.Length; i++)
            {
                if (i == nextIndex)
                {
                    i += find.Length - 1;
                    count++;
                    nextIndex = value.IndexOf(find, i);
                }
                else
                {
                    break;
                }
            }

            return count;
        }
    }
}