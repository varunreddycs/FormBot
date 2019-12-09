using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBot.Extensions
{
    public static class ListExtensions
    {

        // List Extensions 
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            // Divides the list into List of Lists based on provided chunk size(Size of List)
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static bool TryParseJson<T>(this string @this, out T result)
        {
            // Checks the if The Json is Valid as per the Json Model
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }

        public static int MatchCountBetweenLists(List<string> listOne, List<string> listTwo)
        {
            int matchCount = 0;
            foreach (string item in listOne)
            {
                if (listTwo.Contains(item))
                {
                    matchCount += 1;
                }
            }
            return matchCount;
        }
    }

}
