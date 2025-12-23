using ArchonAnalysers.Analyzers.ARCHON001;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace ArchonAnalysers.Tests.Unit.Analyzers.ARCHON001;

public class InternalsAreInternalAnalyzerTests
{
    [Fact]
    public async Task DiagnosticAppearsOnPublicClass()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class MyClass;
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnProtectedClassNestedInsidePublicClass()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                  {
                                        {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:protected|} class MyClass;
                                  }
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnInternalClassNestedInsidePublicClass()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                  {
                                    internal class MyClass;
                                  }
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnProtectedClassNestedInsideInternalClass()
    {
        const string testCode = """
                                namespace TestApp.Internal;
                                internal class OuterClass
                                {
                                      protected class MyClass;
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnProtectedInternalClassNestedInsideInternalClass()
    {
        const string testCode = """
                                namespace TestApp.Internal;
                                internal class OuterClass
                                {
                                    protected internal class MyClass;
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task NoDiagnosticOnPublicClassesNotInInternalNamespace()
    {
        const string testCode = """
                                namespace TestApp.Public;
                                public class MyClass;
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnProtectedInternalClassNestedInsidePublicClass()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                  {
                                      {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:protected|} internal class MyClass;
                                  }
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnInternalClass()
    {
        const string testCode = """
                                namespace TestApp.Internal;
                                internal class MyClass;
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnShieldedNestedPublicClass()
    {
        const string testCode = """
                                namespace TestApp.Internal;
                                internal class OuterClass
                                {
                                    public class MyClass;
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnPrivateNestedClass()
    {
        const string testCode = $$"""
                                namespace TestApp.Internal;
                                {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                {
                                    private class MyClass;
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnPrivateProtectedNestedClass()
    {
        const string testCode = $$"""
                                namespace TestApp.Internal;
                                {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                {
                                    private protected class MyClass;
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicRecord()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} record MyRecord;
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicStruct()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} struct MyStruct;
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicInterface()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} interface IMyInterface;
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicEnum()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} enum MyEnum;
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnPublicClassesNestedInsideInternalClass()
    {
        const string testCode = """
                                namespace TestApp.Internal;
                                internal class OuterClass
                                {
                                    public class MiddleClass
                                    {
                                        public class InnerClass;
                                    }
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnAllPublicClassChain()
    {
        const string testCode = $$"""
                                namespace TestApp.Internal;
                                {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                {
                                    {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class MiddleClass
                                    {
                                        {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class InnerClass;
                                    }
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicClassesInChainUntilMasked()
    {
        const string testCode = $$"""
                                namespace TestApp.Internal;
                                {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OutermostClass
                                {
                                    {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                    {
                                        internal class MiddleClass
                                        {
                                            public class InnerClass;
                                        }
                                    }
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnAllLevelsWhenProtectedInMiddle()
    {
        const string testCode = $$"""
                                namespace TestApp.Internal;
                                {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class OuterClass
                                {
                                    {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:protected|} class MiddleClass
                                    {
                                        {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class InnerClass;
                                    }
                                }
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicClassInNestedInternalNamespace()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal.SubNamespace;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class MyClass;
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicDelegate()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} delegate void MyDelegate();
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnInternalDelegate()
    {
        const string testCode = """
                                namespace TestApp.Internal;
                                internal delegate void MyDelegate();
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticAppearsOnPublicGenericClass()
    {
        const string testCode = $$"""
                                  namespace TestApp.Internal;
                                  {|{{InternalsAreInternalAnalyzer.DiagnosticId}}:public|} class MyClass<T>;
                                  """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DiagnosticDoesntAppearOnInternalGenericClass()
    {
        const string testCode = """
                                namespace TestApp.Internal;
                                internal class MyClass<T>;
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task NoDiagnosticOnPublicClassInPublicNamespaceContainingInternalFragment()
    {
        const string testCode = """
                                namespace TestApp.Public.InternalStuff;
                                public class MyClass;
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }
}
