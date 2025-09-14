using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RPGNameGenerator.cs
/// Simple, configurable name generator for Unity (C#).
/// Attach to a GameObject or use its static methods from other scripts.
/// Supports syllable-based generation, optional prefixes/suffixes, capitalization and seeds.
/// </summary>
[AddComponentMenu("Utils/RPG Name Generator")]
public class RpgNameGenerator : MonoBehaviour
{
    [Header("Syllables")]
    [Tooltip("Core syllables used to assemble names")]
    public string[] syllables = new string[] {
        "an","ar","el","en","ir","or","ul","ra","ri","ro","va","vel","mar","dor","thal","gorn","eth","is","on","al"
    };

    [Header("Optional prefixes (titles, clan beginnings)")]
    public string[] prefixes = new string[] {
        "Lord","Lady","Captain","Sir","Dame","Master","Arch","High","Shadow","Blood","Iron"
    };

    [Header("Optional suffixes (lineage, nicknames)")]
    public string[] suffixes = new string[] {
        "son","borne","dottir","blade","walker","heart","bringer","bane","weaver","smith"
    };

    [Header("Generation rules")]
    public int minSyllables = 2;
    public int maxSyllables = 4;
    public bool usePrefix = false;
    public bool useSuffix = false;
    public bool capitalizeEachSyllable = false;
    public char joiner = '\0'; // if set to '\0' we concatenate directly

    [Header("Seed (0 = random)")]
    public int seed = 0;

    private System.Random rng;

    private void Awake()
    {
        InitRng();
    }

    private void InitRng()
    {
        if (seed == 0)
            rng = new System.Random(Guid.NewGuid().GetHashCode());
        else
            rng = new System.Random(seed);
    }

    /// <summary>
    /// Generate a single name using the inspector settings.
    /// </summary>
    public string GenerateName()
    {
        if (rng == null) InitRng();

        int syllCount = rng.Next(Mathf.Max(1, minSyllables), Mathf.Max(1, maxSyllables) + 1);

        List<string> parts = new List<string>();

        if (usePrefix && prefixes != null && prefixes.Length > 0 && rng.NextDouble() < 0.5)
        {
            parts.Add(Choose(prefixes));
        }

        for (int i = 0; i < syllCount; i++)
        {
            string s = Choose(syllables);
            if (capitalizeEachSyllable)
                s = Capitalize(s);
            parts.Add(s);
        }

        if (useSuffix && suffixes != null && suffixes.Length > 0 && rng.NextDouble() < 0.5)
        {
            parts.Add(Choose(suffixes));
        }

        string name = JoinParts(parts);
        name = PostProcess(name);
        return name;
    }

    /// <summary>
    /// Generate N names and return them as an array.
    /// </summary>
    public string[] GenerateNames(int count)
    {
        string[] arr = new string[Mathf.Max(0, count)];
        for (int i = 0; i < arr.Length; i++) arr[i] = GenerateName();
        return arr;
    }

    private string Choose(string[] list)
    {
        if (list == null || list.Length == 0) return string.Empty;
        int idx = rng.Next(0, list.Length);
        return list[idx];
    }

    private string JoinParts(List<string> parts)
    {
        if (parts == null || parts.Count == 0) return string.Empty;
        if (joiner == '\0')
            return string.Concat(parts);
        else
            return string.Join(joiner.ToString(), parts);
    }

    private string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        if (s.Length == 1) return s.ToUpper();
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    private string PostProcess(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return raw;

        // Make first letter uppercase, rest lowercase by default
        string result = char.ToUpper(raw[0]) + (raw.Length > 1 ? raw.Substring(1) : "");

        // Optional: remove duplicate letters where syllables meet ("rr", "aa"), keep a single char
        result = CollapseDuplicateBoundaries(result);

        return result;
    }

    private string CollapseDuplicateBoundaries(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(s[0]);
        for (int i = 1; i < s.Length; i++)
        {
            if (char.ToLower(s[i]) == char.ToLower(s[i - 1])) continue; // skip duplicates
            sb.Append(s[i]);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Public helper: set seed at runtime. Call this to re-seed the generator.
    /// </summary>
    public void SetSeed(int newSeed)
    {
        seed = newSeed;
        InitRng();
    }

    /// <summary>
    /// Editor convenience: generate and log a sample of names.
    /// </summary>
    [ContextMenu("Generate Sample Names (10)")]
    public void GenerateSample()
    {
        string[] sample = GenerateNames(10);
        Debug.Log("--- RPG Name Generator sample ---");
        for (int i = 0; i < sample.Length; i++) Debug.Log(sample[i]);
    }

    // Static convenience method for other scripts to generate a quick name with default rules.
    public static string QuickName(System.Random rng = null)
    {
        System.Random r = rng ?? new System.Random(Guid.NewGuid().GetHashCode());
        string[] syl = new string[] { "an", "el", "or", "ir", "th", "ra", "sa", "na", "la", "vin", "dor", "mir", "gal", "fen" };
        int count = r.Next(2, 4);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < count; i++) sb.Append(CapStatic(ChooseStatic(syl, r)));
        return sb.ToString();
    }

    private static string ChooseStatic(string[] list, System.Random r)
    {
        return list[r.Next(0, list.Length)];
    }

    private static string CapStatic(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        if (s.Length == 1) return s.ToUpper();
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}
