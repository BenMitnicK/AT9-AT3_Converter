using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace at3_at9_Converter
{
    public sealed class LanguageService
    {
        private readonly string languageDirectory;
        private const string VersionKey = "version";

        public LanguageService(string baseDirectory)
        {
            languageDirectory = Path.Combine(baseDirectory, "lang");
        }

        public IReadOnlyList<LanguageItem> GetAvailableLanguages()
        {
            if (!Directory.Exists(languageDirectory))
                Directory.CreateDirectory(languageDirectory);

            List<LanguageItem> languages = new List<LanguageItem>();

            foreach (string file in Directory.GetFiles(languageDirectory, "*.json"))
            {
                if (IsBackupLanguageFile(file))
                    continue;

                string code = Path.GetFileNameWithoutExtension(file);
                Dictionary<string, string> language = LoadFromFile(file);

                if (!language.ContainsKey(VersionKey))
                    continue;

                string name = language.ContainsKey("language") ? language["language"] : code;

                languages.Add(new LanguageItem { Code = code, Name = name });
            }

            return languages;
        }

        public void EnsureEnglishTemplate(Dictionary<string, string> englishLanguage)
        {
            if (!Directory.Exists(languageDirectory))
                Directory.CreateDirectory(languageDirectory);

            string filePath = Path.Combine(languageDirectory, "en.json");

            if (File.Exists(filePath))
            {
                Dictionary<string, string> existing = LoadFromFile(filePath);
                if (!IsOlderVersion(existing, englishLanguage))
                    return;

                BackupLanguageFile(filePath, existing.ContainsKey(VersionKey) ? existing[VersionKey] : "unknown");
            }
            else
            {
                WriteLanguageFile(filePath, englishLanguage);
                return;
            }

            WriteLanguageFile(filePath, englishLanguage);
        }

        public LanguageLoadResult LoadLanguage(string langCode, Dictionary<string, string> fallback)
        {
            string filePath = Path.Combine(languageDirectory, langCode + ".json");

            if (!File.Exists(filePath))
                filePath = Path.Combine(languageDirectory, "en.json");

            if (!File.Exists(filePath))
                return new LanguageLoadResult(new Dictionary<string, string>(fallback), false);

            Dictionary<string, string> loaded = LoadFromFile(filePath);
            bool isOutdated = !IsEnglishLanguage(langCode) && IsOlderVersion(loaded, fallback);

            foreach (KeyValuePair<string, string> pair in fallback)
            {
                if (!loaded.ContainsKey(pair.Key))
                    loaded[pair.Key] = pair.Value;
            }

            return new LanguageLoadResult(loaded, isOutdated);
        }

        private static Dictionary<string, string> LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }

        private static bool IsOlderVersion(Dictionary<string, string> language, Dictionary<string, string> reference)
        {
            int languageVersion = GetVersion(language);
            int referenceVersion = GetVersion(reference);

            return languageVersion < referenceVersion;
        }

        private static int GetVersion(Dictionary<string, string> language)
        {
            string version;
            if (!language.TryGetValue(VersionKey, out version))
                return 0;

            int parsedVersion;
            return int.TryParse(version, out parsedVersion) ? parsedVersion : 0;
        }

        private static bool IsEnglishLanguage(string langCode)
        {
            return string.Equals(langCode, "en", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsBackupLanguageFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            return fileName.EndsWith(".backup.json", StringComparison.OrdinalIgnoreCase);
        }

        private static void WriteLanguageFile(string filePath, Dictionary<string, string> language)
        {
            string json = JsonConvert.SerializeObject(language, Formatting.Indented);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        private static void BackupLanguageFile(string filePath, string version)
        {
            string directory = Path.GetDirectoryName(filePath);
            string name = Path.GetFileNameWithoutExtension(filePath);
            string backupPath = Path.Combine(directory, name + ".v" + version + ".backup.json");

            if (File.Exists(backupPath))
            {
                backupPath = Path.Combine(directory, name + ".v" + version + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".backup.json");
            }

            File.Copy(filePath, backupPath, false);
        }
    }

    public sealed class LanguageLoadResult
    {
        public LanguageLoadResult(Dictionary<string, string> language, bool isOutdated)
        {
            Language = language;
            IsOutdated = isOutdated;
        }

        public Dictionary<string, string> Language { get; private set; }
        public bool IsOutdated { get; private set; }
    }

    public sealed class LanguageItem
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
