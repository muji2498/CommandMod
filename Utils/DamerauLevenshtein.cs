using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandMod;

public static class DamerauLevenshtein
{
    // source: https://www.dotnetperls.com/levenshtein
    public static int Compute(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        // Verify arguments.
        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        // Initialize arrays.
        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        // Begin looping.
        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                // Compute cost.
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        // Return cost.
        return d[n, m];
    }

    public static List<string> FindNearestStrings(this IEnumerable<string> sourceList, string compareString,
        int maxMatches = 5)
    {
        if (sourceList == null || !sourceList.Any() || string.IsNullOrWhiteSpace(compareString))
            return null;
        
        var results = new List<(string Word, int Distance, bool IsSubstring)>();

        foreach (var word in sourceList)
        {   
            var distance = Compute(word, compareString);
            var isSubstring = word.Contains(compareString);
            results.Add((word, distance, isSubstring));
        }
        
        return results
            .OrderByDescending(r => r.IsSubstring)
            .ThenBy(r => r.Distance)
            .ThenBy(r => r.Word)
            .Take(maxMatches)
            .Select(r => r.Word)
            .ToList();
    }
}
    