#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace FragmentsOfWysdom.Acceptance
{
    public static class AcceptanceEnvironment
    {
        [MenuItem("Tools/Fragments of Wysdom/Configure Acceptance Profile")]
        public static void Configure()
        {
            RequireSupportedEditor();
            PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Standalone, ApiCompatibilityLevel.NET_Standard);
            AssetDatabase.SaveAssets();
            Debug.Log("Acceptance Standalone API profile set to .NET Standard 2.1. Tests have NOT run.");
        }

        public static void CaptureAndValidate()
        {
            string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string evidence = Environment.GetEnvironmentVariable("FW_ACCEPTANCE_EVIDENCE");
            if (string.IsNullOrWhiteSpace(evidence))
                evidence = Path.Combine(project, "Evidence", "gui-" + DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));
            Directory.CreateDirectory(evidence);
            var profile = PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.Standalone);
            var package = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages()
                .SingleOrDefault(p => p.name == "com.unity.test-framework");
            string report = Path.Combine(evidence, "environment.txt");
            File.WriteAllLines(report, new[] {
                "ExecutionUtc=" + DateTime.UtcNow.ToString("o"), "UnityVersion=" + Application.unityVersion,
                "ApiCompatibilityLevel=" + profile, "ActiveBuildTarget=" + EditorUserBuildSettings.activeBuildTarget,
                "Runtime=" + Environment.Version, "Mono=" + (Type.GetType("Mono.Runtime") != null),
                "TestFramework=" + (package == null ? "UNKNOWN" : package.name + "@" + package.version),
                "IL2CPP/AOT/stripping/Android=UNVERIFIED"
            });
            Debug.Log("Acceptance evidence: " + evidence);
            try
            {
                RequireSupportedEditor();
                if (profile != ApiCompatibilityLevel.NET_Standard || EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone)
                    throw new InvalidOperationException("Select a desktop Standalone target and configure .NET Standard 2.1 before acceptance.");
                if (Type.GetType("Mono.Runtime") == null) throw new InvalidOperationException("Expected real Unity Editor Mono execution.");
                if (package == null) throw new InvalidOperationException("Cannot identify the installed Unity Test Framework.");
                string stage = Path.Combine(project, "Evidence", "staged-artifacts.json");
                File.Copy(stage, Path.Combine(evidence, "staged-artifacts.json"), true);
                using (var manifest = JsonDocument.Parse(File.ReadAllText(stage)))
                {
                    var artifacts = manifest.RootElement.GetProperty("Artifacts").EnumerateArray().ToArray();
                    if (artifacts.Length != 6) throw new InvalidOperationException("Expected exactly six staged DLLs.");
                    foreach (var item in artifacts)
                    {
                        string name = item.GetProperty("File").GetString();
                        if (Path.GetFileName(name) != name) throw new InvalidOperationException("Invalid staged file name.");
                        var loaded = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(name)));
                        string hash = Hash(loaded.Location);
                        File.AppendAllText(report, loaded.FullName + " SHA256=" + hash + Environment.NewLine);
                        if (!string.Equals(hash, item.GetProperty("SHA256").GetString(), StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("Loaded assembly differs from staged candidate: " + name);
                    }
                    foreach (var helper in manifest.RootElement.GetProperty("TestHelpers").EnumerateArray())
                    {
                        string name = helper.GetProperty("File").GetString();
                        if (Path.GetFileName(name) != name || Hash(Path.Combine(Application.dataPath, "Acceptance/Editor/Generated", name))
                            != helper.GetProperty("SHA256").GetString())
                            throw new InvalidOperationException("Staged test helper differs from recorded source: " + name);
                    }
                }
            }
            catch (Exception error)
            {
                File.AppendAllText(report, "ENVIRONMENT GATE FAILED: " + error + Environment.NewLine);
                throw new InvalidOperationException("Unity acceptance environment/dependency gate failed. Preserve environment.txt and Editor.log; do not patch production to bypass it.", error);
            }
        }

        private static void RequireSupportedEditor()
        {
            if (!Application.unityVersion.StartsWith("6000.3.", StringComparison.Ordinal))
                throw new InvalidOperationException("This acceptance fixture requires an explicitly selected Unity 6.3 LTS patch.");
        }

        private static string Hash(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
        }
    }
}
#endif
