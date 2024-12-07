using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class CommandRepositoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var provider = initContext.AdditionalTextsProvider.Where(static file => Path.GetFileName(file.Path) == "raizin_command.md").Collect();

        initContext.RegisterSourceOutput(provider, (context, texts) =>
        {
            foreach (var text in texts)
            {
                try
                {
                    var commandList = ReadCommandFile(text, context);
                    var sourceCode = GenerateSourceCode(commandList);
                    context.AddSource("raizin_command.g.cs", sourceCode);
                }
                catch (Exception e)
                {
                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN00", "Failed to generate command repository", "Failed to generate command repository: {0}", "Raizin", DiagnosticSeverity.Error, true), Location.None, e.Message));
                    throw;
                }
            }
        });
    }

    private const char SignatureLineMark = '#';
    private const char ParameterSeparator = ',';
    private const string TypeMark = ":";
    private const char ParameterSubTypeMark = '@';
    private const char OptionalParameterMark = '?';
    private const char OutputLineMark = '-';
    private const char ChoiceSeparator = '|';
    private const string ChoiceDescriptionSeparator = ":";

    private static List<Command> ReadCommandFile(AdditionalText additionalText, SourceProductionContext context)
    {
        var list = new List<Command>();

        Command? currentCommand = null;
        var descBuilder = new StringBuilder();
        foreach (var (line, lineNumber) in additionalText.GetText()!.Lines.Select(static (x, i) => (x.ToString().TrimEnd(), i)))
        {
            try
            {
                if (line.Length > 0 && line[0] is SignatureLineMark)
                {
                    if (currentCommand is not null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN05", "Signature line duplicated", "Signature line duplicated({0})", "Raizin", DiagnosticSeverity.Error, true), Location.None, lineNumber));
                        throw new InvalidDataException($"Signature line duplicated({lineNumber})");
                    }

                    currentCommand = new Command();
                    var splitStr = line.Substring(1).Split(ParameterSeparator).Select(static x => x.Trim()).ToArray();
                    currentCommand.Name = splitStr[0];
                    for (var i = 1; i < splitStr.Length; ++i)
                    {
                        var remaining = splitStr[i].AsSpan();
                        var paramNameLength = splitStr[i].IndexOf(TypeMark, StringComparison.Ordinal);
                        if (paramNameLength == -1)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN08", "Parameter type is not specified", "Parameter type is not specified: {0}({1})", "Raizin", DiagnosticSeverity.Error, true), Location.None, line, lineNumber));
                            throw new InvalidDataException($"Parameter type is not specified: {line}({lineNumber})");
                        }

                        // get parameter name
                        var paramName = remaining.Slice(0, paramNameLength);
                        remaining = remaining.Slice(paramNameLength + 1);

                        // get parameter types
                        ReadOnlySpan<char> paramType, paramSubType;
                        var paramTypeLength = remaining.IndexOf(ParameterSubTypeMark);
                        if (paramTypeLength >= 0)
                        {
                            paramType = remaining.Slice(0, paramTypeLength);
                            remaining = remaining.Slice(paramTypeLength + 1);

                            paramTypeLength = remaining.IndexOf(ChoiceSeparator);
                            if (paramTypeLength >= 0)
                            {
                                paramSubType = remaining.Slice(0, paramTypeLength);
                                remaining = remaining.Slice(paramTypeLength + 1);
                            }
                            else
                            {
                                paramSubType = remaining;
                                remaining = ReadOnlySpan<char>.Empty;
                            }
                        }
                        else
                        {
                            paramTypeLength = remaining.IndexOf(ChoiceSeparator);
                            if (paramTypeLength >= 0)
                            {
                                paramType = remaining.Slice(0, paramTypeLength);
                                remaining = remaining.Slice(paramTypeLength + 1);
                            }
                            else
                            {
                                paramType = remaining;
                                remaining = ReadOnlySpan<char>.Empty;
                            }

                            paramSubType = ReadOnlySpan<char>.Empty;
                        }

                        // get isRequired
                        var isOptional = paramType[paramType.Length - 1] == OptionalParameterMark;
                        if (isOptional)
                        {
                            paramType = paramType.Slice(0, paramType.Length - 1);
                        }

                        currentCommand.Parameters.Add(new()
                        {
                            Name = paramName.Trim().ToString(),
                            Type = paramType.Trim().ToString(),
                            SubType = paramSubType.Trim().ToString(),
                            IsRequired = !isOptional,
                            Choices = remaining.ToString().Split([ChoiceSeparator], StringSplitOptions.RemoveEmptyEntries).Select(static x =>
                            {
                                var index = x.IndexOf(ChoiceDescriptionSeparator, StringComparison.Ordinal);
                                if (index < 0) return (x.Trim(), "");
                                return (x.Substring(0, index).Trim(), x.Substring(index + 1).Trim());
                            }).ToList(),
                        });
                    }

                    descBuilder.Clear();
                }

                else if (line.Length > 0 && line[0] is OutputLineMark)
                {
                    if (currentCommand is null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN02", "Invalid output line", "Invalid output line({0})", "Raizin", DiagnosticSeverity.Error, true), Location.None, lineNumber));
                        throw new InvalidDataException($"Invalid output line({lineNumber})");
                    }

                    foreach (var keyword in line.Substring(1).Split(ParameterSeparator).Select(static x => x.Trim()))
                    {
                        switch (keyword)
                        {
                            case "rai7":
                                currentCommand.IsRai7 = true;
                                break;
                            case "rai8":
                                currentCommand.IsRai8 = true;
                                break;
                            case "control":
                                currentCommand.IsControl = true;
                                break;
                            case "command":
                                currentCommand.IsCommand = true;
                                break;
                            case "if":
                                currentCommand.IsIf = true;
                                break;
                            case "deprecated":
                                currentCommand.IsDeprecated = true;
                                break;
                            case "regex":
                                currentCommand.IsRegex = true;
                                break;
                            default:
                                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN03", "Unknown keyword", "Unknown keyword: {0}({1})", "Raizin", DiagnosticSeverity.Error, true), Location.None, keyword, lineNumber));
                                throw new InvalidDataException($"Unknown keyword: {keyword}({lineNumber})");
                        }
                    }

                    if (!currentCommand.IsCommand && !currentCommand.IsIf)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN06", "Invalid output line", "command or if is required({0})", "Raizin", DiagnosticSeverity.Error, true), Location.None, lineNumber));
                        throw new InvalidDataException($"command or if is required({lineNumber})");
                    }

                    if (!currentCommand.IsRai7 && !currentCommand.IsRai8)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN07", "Invalid output line", "rai7 or rai8 is required({0})", "Raizin", DiagnosticSeverity.Error, true), Location.None, lineNumber));
                        throw new InvalidDataException($"rai7 or rai8 is required({lineNumber})");
                    }

                    currentCommand.Description = descBuilder.ToString().Trim();
                    list.Add(currentCommand);

                    currentCommand = null;
                }
                else
                {
                    if (line.Length > 0)
                    {
                        descBuilder.Append(line
                            .Replace("\\", "\\\\")
                            .Replace("#", "\\#")
                            .Replace("+", "\\+")
                            .Replace("-", "\\-")
                            .Replace("*", "\\*")
                            .Replace("_", "\\_")
                            .Replace("`", "\\`")
                            .Replace(".", "\\.")
                            .Replace("!", "\\!")
                            .Replace("|", "\\|")
                            .Replace("{", "\\{")
                            .Replace("}", "\\}")
                            .Replace("[", "\\[")
                            .Replace("]", "\\]")
                            .Replace("(", "\\(")
                            .Replace(")", "\\)")
                            .Replace("|", "\\|")
                            .Replace(" ", "&ensp;")
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;")
                            .Replace("=", "\\=")
                        );
                        descBuilder.AppendLine("  ");
                    }
                    else
                    {
                        descBuilder.AppendLine();
                    }
                }
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("RAIZIN01", "Failed to read command file", "Failed to read command file at {0}: {1}", "Raizin", DiagnosticSeverity.Error, true), Location.None, lineNumber, e.Message));
                throw;
            }
        }

        return list;
    }

    private static string GenerateSourceCode(List<Command> commandList)
    {
        var lines = new List<string>();
        foreach (var command in commandList)
        {
            lines.Add($"new(\"{command.Name}\", {command.IsControl.ToString().ToLower()}, {command.IsRai7.ToString().ToLower()}, {command.IsRai8.ToString().ToLower()})");
            lines.Add("{");
            if (command.IsDeprecated)
            {
                lines.Add("\tIsDeprecated = true,");
            }

            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                lines.Add("\tDescription = \"\"\"\n" + command.Description + "\n\"\"\",");
            }

            if (command.IsRegex)
            {
                lines.Add("\tCommandNameRegex = new Regex(@\"^" + command.Name + "$\"),");
            }

            if (command.Parameters.Count > 0)
            {
                lines.Add("\tParameters = [");
                foreach (var param in command.Parameters)
                {
                    if (param.Type is "RulerId" or "FactionId" or "PirateId") param.Type = "PersonId";
                    if (param.SubType is "RulerId" or "FactionId" or "PirateId") param.SubType = "PersonId";

                    lines.Add($"\t\tnew(\"{param.Name}\", ParameterDefinitionType.{param.Type}, ParameterDefinitionType.{(param.SubType != "" ? param.SubType : "Any")}, {(param.IsRequired ? "true" : "false")}, \"{GetParameterInlayHint(param.Name)}\")");
                    if (param.Choices.Count == 0)
                    {
                        lines[lines.Count - 1] += ",";
                    }
                    else
                    {
                        lines.Add("\t\t{");
                        lines.Add("\t\t\tChoices = [");
                        foreach (var choice in param.Choices)
                        {
                            lines.Add($"\t\t\t\tnew(\"{choice.Value}\", \"{choice.ShortDescription}\"),");
                        }

                        lines.Add("\t\t\t],");
                        lines.Add("\t\t},");
                    }
                }

                lines.Add("\t],");
            }

            lines.Add("},");
        }

        var code = $$"""
                     using System;
                     using System.Collections.Generic;
                     using System.Diagnostics.CodeAnalysis;
                     using System.Text.RegularExpressions;
                     using RaizinLanguageServer.Models.Commands;

                     namespace RaizinLanguageServer.Models.Commands;

                     public partial class CommandRepository
                     {
                         public IReadOnlyList<CommandDefinition> CommandList { get; } = 
                         [
                             {{lines.Aggregate((x, y) => x + "\n\t\t" + y)}}
                         ];
                     }
                     """;

        return code;
    }

    private static string GetParameterInlayHint(string name) => name + ":";

    private sealed record Command
    {
        public string Name { get; set; } = "";
        public List<Parameter> Parameters { get; } = new();
        public string Description { get; set; } = "";
        public bool IsRai7 { get; set; }
        public bool IsRai8 { get; set; }
        public bool IsControl { get; set; }
        public bool IsIf { get; set; }
        public bool IsCommand { get; set; }
        public bool IsDeprecated { get; set; }
        public bool IsRegex { get; set; }
    }

    private sealed record Parameter
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string SubType { get; set; } = "";

        public bool IsRequired { get; set; }

        public List<(string Value, string ShortDescription)> Choices { get; set; } = new();
    }
}