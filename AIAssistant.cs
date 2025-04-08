using System;
using System.Collections.Generic;
using System.Text;

namespace  AI_3DGen
{
    public static class AIAssistant
    {
        public static string ExplainResult(string result, string explanation, string? code = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== SONUÇ ===");
            sb.AppendLine(result);
            sb.AppendLine();
            sb.AppendLine("=== AÇIKLAMA ===");
            sb.AppendLine(explanation);
            
            if (!string.IsNullOrEmpty(code))
            {
                sb.AppendLine();
                sb.AppendLine("=== KOD ===");
                sb.AppendLine("```csharp");
                sb.AppendLine(code);
                sb.AppendLine("```");
            }

            return sb.ToString();
        }

        public static string ExplainError(string error, string explanation, string? solution = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== HATA ===");
            sb.AppendLine(error);
            sb.AppendLine();
            sb.AppendLine("=== AÇIKLAMA ===");
            sb.AppendLine(explanation);
            
            if (!string.IsNullOrEmpty(solution))
            {
                sb.AppendLine();
                sb.AppendLine("=== ÇÖZÜM ===");
                sb.AppendLine(solution);
            }

            return sb.ToString();
        }

        public static string ExplainProcess(string title, string description, string? nextSteps = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {title.ToUpper()} ===");
            sb.AppendLine(description);
            
            if (!string.IsNullOrEmpty(nextSteps))
            {
                sb.AppendLine();
                sb.AppendLine("=== SONRAKİ ADIMLAR ===");
                sb.AppendLine(nextSteps);
            }

            return sb.ToString();
        }
    }
}
