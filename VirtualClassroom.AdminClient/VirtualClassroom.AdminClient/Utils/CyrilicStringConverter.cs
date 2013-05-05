using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VirtualClassroom.AdminClient
{
    public static class CyrilicStringConverter
    {
        /// <summary>
        /// Converts all cyrilic characters in a string to latin characters
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <returns>A string with all cyrilic characters replaced with latin letters</returns>
        public static string ConvertCyrillicToLatinLetters(this string input)
        {
            //log.Debug("Converting to latin letters");
            var bulgarianLetters = new[]
            {
                "а", "б", "в", "г", "д", "е", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п",
                "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ь", "ю", "я"
            };

            var latinRepresentationsOfBulgarianLetters = new[]
            {
                "a", "b", "v", "g", "d", "e", "j", "z", "i", "y", "k",
                "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "h",
                "c", "ch", "sh", "sht", "u", "i", "yu", "ya"
            };

            // replace every letter from the bulgarian aplhabet with a latin symbol that
            // has the same index
            for (var i = 0; i < bulgarianLetters.Length; i++)
            {
                input = input.Replace(bulgarianLetters[i],
                    latinRepresentationsOfBulgarianLetters[i]);

                input = input.Replace(bulgarianLetters[i].ToUpper(),
                    latinRepresentationsOfBulgarianLetters[i].CapitalizeFirstLetter());
            }

            return input;
        }

        /// <summary>
        /// Makes the first letter of a strign in CAPITAL_CASE
        /// </summary>
        /// <param name="input">The string to capitalize</param>
        /// <returns>The same string, but with capitalized first letter</returns>
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // return the string with capitalized first letter
            return input.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) +
                input.Substring(1, input.Length - 1);
        }
    }
}
