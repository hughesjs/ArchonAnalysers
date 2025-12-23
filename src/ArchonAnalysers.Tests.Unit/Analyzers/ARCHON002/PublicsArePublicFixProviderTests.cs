using ArchonAnalysers.Analyzers.ARCHON002;
using ArchonAnalysers.FixProviders.ARCHON002;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace ArchonAnalysers.Tests.Unit.Analyzers.ARCHON002;

public class PublicsArePublicFixProviderTests
{
    [Theory]
    [InlineData("struct")]
    [InlineData("interface")]
    [InlineData("record")]
    [InlineData("enum")]
    [InlineData("class")]
    public async Task FixChangesInternalTypeDeclarationsToPublic(string typeKeyword)
    {
        string testCode = $$"""
                            namespace TestApp.Public;
                            {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} {{typeKeyword}} MyType;
                            """;

        string fixedCode = $$"""
                             namespace TestApp.Public;
                             public {{typeKeyword}} MyType;
                             """;

        CSharpCodeFixTest<PublicsArePublicAnalyzer, PublicsArePublicFixProvider, DefaultVerifier> test = new()
        {
            TestCode = testCode,
            FixedCode = fixedCode
        };

        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task FixChangesInternalDelegateToPublic()
    {
        const string testCode = $$"""
                                  namespace TestApp.Public;
                                  {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} delegate void MyDelegate();
                                  """;

        const string fixedCode = """
                                 namespace TestApp.Public;
                                 public delegate void MyDelegate();
                                 """;

        CSharpCodeFixTest<PublicsArePublicAnalyzer, PublicsArePublicFixProvider, DefaultVerifier> test = new()
        {
            TestCode = testCode,
            FixedCode = fixedCode
        };

        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("abstract", "class")]
    [InlineData("sealed", "class")]
    [InlineData("static", "class")]
    [InlineData("readonly", "struct")]
    public async Task FixPreservesNonAccessibilityModifiers(string additionalModifiers, string typeDeclaration)
    {
        string testCode = $$"""
                            namespace TestApp.Public;
                            {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} {{additionalModifiers}} {{typeDeclaration}} MyType;
                            """;

        string fixedCode = $$"""
                             namespace TestApp.Public;
                             public {{additionalModifiers}} {{typeDeclaration}} MyType;
                             """;

        CSharpCodeFixTest<PublicsArePublicAnalyzer, PublicsArePublicFixProvider, DefaultVerifier> test = new()
        {
            TestCode = testCode,
            FixedCode = fixedCode
        };

        await test.RunAsync(TestContext.Current.CancellationToken);
    }
}
