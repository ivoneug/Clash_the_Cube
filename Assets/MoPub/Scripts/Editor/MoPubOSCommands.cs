using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

public class MoPubOSCommands
{
    // Helper function to create dest directory, used in other functions below.  Normal usage is to call
    // MkDir(Path.GetDirectory(destFilePath)), to ensure that the file-to-be-created has a directory in which to be
    // created. Hence no action is taken for an empty path, since that's what you get when the file is going into the
    // current working directory.
    public static void Mkdir(string path)
    {
        if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            Directory.CreateDirectory(path);
    }


    // Simple analog of the Linux/BSD "mv" command.  If source is...
    // > a file (ending in anything BUT / or *), it will be moved to dest.
    // > a directory (ending in /), it will be recursively moved to dest.
    // > a directory with wildcard (ending in /*), the directory contents will be recursively moved to dest, then the
    // remaining empty directory is removed.
    public static bool Mv(string source, string dest)
    {
        if (source.EndsWith("/*")) {
            source = Path.GetDirectoryName(source);
            if (!dest.EndsWith("/")) // Make sure dest is treated as a directory.
                dest += "/";
            Debug.LogFormat("Moving directory contents from '{0}' to '{1}'", source, dest);
            var result = Find(source, recursive: false)
                        .Select(p => Path.Combine(source, p))
                        .All(p => Mv(p, dest))
                     && RmDir(source);
            if (result)
                Rm(source + ".meta");
            return result;
        }

        if (!File.Exists(source) && !Directory.Exists(source)) {
            Debug.LogWarningFormat("Source doesn't exist: {0}", source);
            return false;
        }
        if (string.IsNullOrEmpty(Path.GetFileName(dest)) || File.Exists(source) && Directory.Exists(dest))
            dest = Path.Combine(dest, Path.GetFileName(source));

        if (File.Exists(dest) || Directory.Exists(dest)) {
            Debug.LogWarningFormat("Ignoring, destination already exists: {0}", dest);
            return false;
        }

        Debug.LogFormat("Moving '{0}' to '{1}'", source, dest);
        try {
            Mkdir(Path.GetDirectoryName(dest));  // Parent dirs in dest.
            Directory.Move(source, dest);  // This works if source is file or directory.
            source += ".meta";
            dest += ".meta";
            if (File.Exists(source) && !File.Exists(dest))
                Directory.Move(source, dest);
            return true;
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while moving '{0}' to '{1}': {2}", source, dest, e);
            return false;
        }
    }


    // Simple analog of the Linux/BSD "rm" command.
    internal static bool Rm(string path)
    {
        try {
            Debug.Log("Removing file: " + path);
            File.Delete(path);
            path += ".meta";
            if (File.Exists(path)) File.Delete(path);
            return true;
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while deleting file '{0}': {1}", path, e);
            return false;
        }
    }


    // Simple analog of the Linux/BSD "rmdir" command.
    public static bool RmDir(string path)
    {
        Debug.Log("Removing directory: " + path);
        if (!Directory.Exists(path))
        {
            Debug.LogWarningFormat("Ignoring, directory not found: {0}", path);
            return false;
        }
        try {
            Directory.Delete(path, true);
            if (string.IsNullOrEmpty(Path.GetFileName(path)))
                path = Path.GetDirectoryName(path);
            path += ".meta";
            if (File.Exists(path))
                File.Delete(path);
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while deleting '{0}': {1}", path, e);
            return false;
        }
        return true;
    }


    // MSDN docs say that Directory.GetFileSystemEntries() returns the "names" of the directory's contents,
    // but we're getting back full paths.  This wrapper works around that because we need the simple names.
    // Both overloads of Directory.GetFileSystemEntries() -- with or without a search pattern -- are supported.
    public static IEnumerable<string> GetFileSystemEntries(string dir, string pattern = null)
    {
        try {
            return string.IsNullOrEmpty(pattern)
                ? from name in Directory.GetFileSystemEntries(dir) select Path.GetFileName(name)
                : from name in Directory.GetFileSystemEntries(dir, pattern) select Path.GetFileName(name);
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while finding files in dir {0}: {1}", dir, e);
            return Enumerable.Empty<string>();
        }
    }


    // Simple analog of the Linux/BSD "find" command.  It is equivalent to just "find dir" with flags "-type f"
    // and/or "-type d" added.  However the returned paths are relative to dir, unlike the find command.
    public static IEnumerable<string> Find(string dir, bool files = true, bool dirs = true, bool recursive = true)
    {
        if (!Directory.Exists(dir))
            yield break;
        foreach (var name in GetFileSystemEntries(dir)) {
            var path = Path.Combine(dir, name);
            if (File.Exists(path)) {
                if (files)
                    yield return name;
            } else if (Directory.Exists(path)) {
                if (dirs)
                    yield return name;
                if (!recursive) continue;
                foreach (var subPath in Find(path, files, dirs))
                    yield return Path.Combine(name, subPath);
            }
        }
    }
}
