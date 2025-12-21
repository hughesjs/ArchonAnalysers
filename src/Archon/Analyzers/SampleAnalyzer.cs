using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Archon.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SampleAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "ARCHON001";
	private const string Category = "Architecture";

	private static readonly LocalizableString Title = "Sample Architecture Rule";
	private static readonly LocalizableString MessageFormat = "Sample architecture violation: '{0}'";
	private static readonly LocalizableString Description = "This is a sample architecture rule.";

	private static readonly DiagnosticDescriptor Rule = new(
		DiagnosticId,
		Title,
		MessageFormat,
		Category,
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		// Register analysis actions here
		// Example: context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}
}
