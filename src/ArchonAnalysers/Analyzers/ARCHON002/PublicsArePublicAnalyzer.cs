using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ArchonAnalysers.Analyzers.ARCHON002;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PublicsArePublicAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "ARCHON002";
	private const string Category = "Architecture";

	private static readonly LocalizableString Title = "Types in public namespaces should be public or protected";
	private static readonly LocalizableString MessageFormat = "Type {0} should be public or protected due to being in namespace {1} but is {2}";
	private static readonly LocalizableString Description =
		"This rule validates that all types in defined public namespaces have public, protected, or protected internal access modifiers";

	private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

	// TODO - Grab this from config eventually
	private const string PublicNamespaceSlug = "Public";


	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		INamespaceSymbol? symbolNamespace = context.Symbol.ContainingNamespace;

		if (SymbolIsInIrrelevantNamespace(symbolNamespace))
		{
			return;
		}

		ISymbol symbol = context.Symbol;

		// Only check top-level types. Nested types are implementation details and can have any accessibility.
		// Unlike ARCHON001 (which prevents leakage), this rule ensures discoverability of the public API surface,
		// and nested types aren't part of that surface regardless of their declared accessibility.
		if (symbol.ContainingType is not null)
		{
			return;
		}

		if (SymbolIsAppropriatelyAccessible(symbol))
		{
			return;
		}

		CreateDiagnosticForProblematicSymbol(symbol, context);
	}

	private void CreateDiagnosticForProblematicSymbol(ISymbol symbol, SymbolAnalysisContext context)
	{
		SyntaxNode? syntaxNode = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

		SyntaxToken? problematicModifier = syntaxNode switch
		{
			BaseTypeDeclarationSyntax typeNode =>
				typeNode.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.InternalKeyword) || m.IsKind(SyntaxKind.PrivateKeyword)),
			DelegateDeclarationSyntax delegateNode =>
				delegateNode.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.InternalKeyword) || m.IsKind(SyntaxKind.PrivateKeyword)),
			_ => null
		};

		if (problematicModifier is null)
		{
			return;
		}

		Location location = problematicModifier.Value.GetLocation();

		Diagnostic diagnostic = Diagnostic.Create(Rule, location, context.Symbol.Name, context.Symbol.ContainingNamespace.ToDisplayString(),
			context.Symbol.DeclaredAccessibility.ToString());
		context.ReportDiagnostic(diagnostic);
	}

	private static bool SymbolIsAppropriatelyAccessible(ISymbol symbol) =>
		symbol.DeclaredAccessibility is Accessibility.Public or
		                                 Accessibility.Protected or
		                                 Accessibility.ProtectedOrInternal;

	private static bool SymbolIsInIrrelevantNamespace(INamespaceSymbol? symbolNamespace) =>
		symbolNamespace is null ||
		symbolNamespace.IsGlobalNamespace ||
        !Regex.IsMatch(symbolNamespace.ToDisplayString(),
            @$"^(?:\w+\.)*(?<Slug>{PublicNamespaceSlug})(?:\.\w+)*$");
}
