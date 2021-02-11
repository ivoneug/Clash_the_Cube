using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using MJ = MoPubInternal.ThirdParty.MiniJSON;

public class MoPubUtils {


    /// <summary>
    /// Compares two versions to see which is greater.
    /// </summary>
    /// <param name="a">Version to compare against second param</param>
    /// <param name="b">Version to compare against first param</param>
    /// <returns>-1 if the first version is smaller, 1 if the first version is greater, 0 if they are equal</returns>
    public static int CompareVersions(string a, string b)
    {
        var versionA = VersionStringToInts(a);
        var versionB = VersionStringToInts(b);
        for (var i = 0; i < Mathf.Max(versionA.Length, versionB.Length); i++) {
            if (VersionPiece(versionA, i) < VersionPiece(versionB, i))
                return -1;
            if (VersionPiece(versionA, i) > VersionPiece(versionB, i))
                return 1;
        }

        return 0;
    }


    private static int VersionPiece(IList<int> versionInts, int pieceIndex)
    {
        return pieceIndex < versionInts.Count ? versionInts[pieceIndex] : 0;
    }


    private static int[] VersionStringToInts(string version)
    {
        if (string.IsNullOrEmpty(version)) {
            return new[] { 0 };
        }

        int piece;
        return version.Split('.')
            .Select(v => int.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out piece) ? piece : 0)
            .ToArray();
    }


    public static Uri UrlFromString(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        try {
            return new Uri(url);
        } catch {
            Debug.LogError("Invalid URL: " + url);
            return null;
        }
    }

    public static string EncodeArgs(params string[] args)
    {
        return MJ.Json.Serialize(args);
    }

    // Will return a non-null array of strings with at least 'min' non-null string values at the front.
    public static string[] DecodeArgs(string argsJson, int min)
    {
        var err = false;
        var args = MJ.Json.Deserialize(argsJson) as List<object>;
        if (args == null) {
            Debug.LogError("Invalid JSON data: " + argsJson);
            args = new List<object>();
            err = true;
        }
        if (args.Count < min) {
            if (!err)  // Don't double up the error messages for invalid JSON
                Debug.LogError("Missing one or more values: " + argsJson + " (expected " + min + ")");
            while (args.Count < min)
                args.Add("");
        }
        return args.Select(v => v.ToString()).ToArray();
    }

    public static string InvariantCultureToString(object obj)
    {
        return string.Format(CultureInfo.InvariantCulture, "{0}", obj);
    }
}
