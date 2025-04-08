using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace AI_3DGen
{
    public static class LanguageHelper
    {
        public static CultureInfo DetectLanguage(string text)
        {
            if (string.IsNullOrEmpty(text))
                return CultureInfo.InvariantCulture;

            // Türkçe karakterler varsa Türkçe olarak algıla
            if (text.Any(c => "şŞğĞüÜöÖıİçÇ".Contains(c)))
                return new CultureInfo("tr-TR");

            // İngilizce karakterler varsa İngilizce olarak algıla
            if (text.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)))
                return new CultureInfo("en-US");

            // Varsayılan olarak İngilizce
            return new CultureInfo("en-US");
        }

        public static string? GetResource(string key, CultureInfo? culture)
        {
            if (string.IsNullOrEmpty(key) || culture == null)
                return null;

            // Dil bazlı kaynaklar
            var resources = new Dictionary<string, Dictionary<string, string>>
            {
                { 
                    "en-US", 
                    new Dictionary<string, string>
                    {
                        { "PromptEmpty", "Prompt cannot be empty" },
                        { "RetryFailed", "All {0} retries failed" },
                        { "RetryMessage", "Attempt {0} failed. Retrying in 2 seconds..." }
                    }
                },
                { 
                    "tr-TR", 
                    new Dictionary<string, string>
                    {
                        { "PromptEmpty", "Model açıklaması boş olamaz" },
                        { "RetryFailed", "{0} deneme başarısız oldu" },
                        { "RetryMessage", "Deneme {0} başarısız. 2 saniye sonra tekrar denenecek..." }
                    }
                }
            };

            if (resources.TryGetValue(culture.Name, out var langResources) &&
                langResources.TryGetValue(key, out var value))
            {
                return value;
            }

            // Varsayılan olarak İngilizce
            if (resources.TryGetValue("en-US", out var defaultResources) &&
                defaultResources.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }
    }
}
