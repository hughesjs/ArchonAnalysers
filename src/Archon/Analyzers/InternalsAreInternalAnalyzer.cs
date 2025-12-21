using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
namespace Archon.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InternalsAreInternalAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "ARCHON001";
    private const string Category = "Architecture";

    private static readonly LocalizableString Title = "Types in internal namespaces should be internal or private";
    private static readonly LocalizableString MessageFormat = "Type {0} should be internal or private due to being in namespace {1} but is {2}";

    private static readonly LocalizableString Description =
        "This rule validates that all types in defined internal namespaces have either internal or private access modifiers, excluding compiler generated types";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    // TODO - Grab this from config eventually
    private const string InternalNamespaceSlug = ".Internal";


    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext obj)
    {
        INamespaceSymbol? symbolNamespace = obj.Symbol.ContainingNamespace;

        if (SymbolIsInIrrelevantNamespace(symbolNamespace))
        {
            return;
        }

        ISymbol symbol = obj.Symbol;

        if (SymbolIsMaskedByContainingType(symbol))
        {
            return;
        }

        if (!SymbolHasProblematicAccessibility(symbol))
        {
            return;
        }

        CreateDiagnosticForProblematicSymbol(symbol, obj);
    }

    private static void CreateDiagnosticForProblematicSymbol(ISymbol symbol, SymbolAnalysisContext obj)
    {
        SyntaxNode? syntaxNode = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

        // BaseTypeDeclarationSyntax gets everything including enums (TypeDeclarationSyntax misses enums)
        if (syntaxNode is not BaseTypeDeclarationSyntax baseTypeDeclarationNode)
        {
            return; // Something's gone wrong
        }

        SyntaxToken? problematicModifier =
            baseTypeDeclarationNode.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.PublicKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword));

        if (problematicModifier is null)
        {
            return;
        }

        Location location = problematicModifier.Value.GetLocation();

        Diagnostic diagnostic = Diagnostic.Create(Rule, location, obj.Symbol.Name, obj.Symbol.ContainingNamespace.ToDisplayString(),
            obj.Symbol.DeclaredAccessibility.ToString());
        obj.ReportDiagnostic(diagnostic);
    }

    private static bool SymbolHasProblematicAccessibility(ISymbol symbol) => symbol.DeclaredAccessibility is  (Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal);

    private static bool SymbolIsMaskedByContainingType(ISymbol symbol)
    {
        INamedTypeSymbol? parent = symbol.ContainingType;

        // Top level, unmasked
        if (parent is null)
        {
            return false;
        }

        // Masked
        if (parent.DeclaredAccessibility is Accessibility.Internal or Accessibility.Private or Accessibility.ProtectedAndInternal)
        {
            return true;
        }

        return SymbolIsMaskedByContainingType(parent);
    }


    private static bool SymbolIsInIrrelevantNamespace(INamespaceSymbol? symbolNamespace) => symbolNamespace is null ||
                                                                                            symbolNamespace.IsGlobalNamespace ||
                                                                                            !symbolNamespace.ToDisplayString().Contains(InternalNamespaceSlug);
}
