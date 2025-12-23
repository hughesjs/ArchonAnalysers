using ArchonAnalysers.Analyzers.ARCHON002;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace ArchonAnalysers.Tests.Unit.Analyzers.ARCHON002;

public class PublicsArePublicAnalyzerTests
{
	[Fact]
	public async Task DiagnosticAppearsOnInternalClass()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticDoesntAppearOnPublicClass()
	{
		const string testCode = """
		                        namespace TestApp.Public;
		                        public class MyClass;
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticDoesntAppearOnNestedClassRegardlessOfAccessibility()
	{
		const string testCode = """
		                        namespace TestApp.Public;
		                        public class OuterClass
		                        {
		                            private class PrivateNested;
		                            protected class ProtectedNested;
		                            internal class InternalNested;
		                            protected internal class ProtectedInternalNested;
		                            private protected class PrivateProtectedNested;
		                            public class PublicNested;
		                        }
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticDoesntAppearInNonPublicNamespace()
	{
		const string testCode = """
		                        namespace TestApp.Internal;
		                        internal class MyClass;
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicRecord()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} record MyRecord;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicStruct()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} struct MyStruct;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicInterface()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} interface IMyInterface;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicEnum()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} enum MyEnum;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

    [Fact]
    public async Task NoDiagnosticOnInternalClassInPublicNamespaceContainingSectionWithInternalSlugFragment()
    {
        const string testCode = $$"""
                                  namespace TestApp.Public.InternalStuff;
                                  {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass;
                                  """;
        CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

	[Fact]
	public async Task DiagnosticAppearsOnInternalClassInNestedPublicNamespace()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public.SubNamespace;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticAppearsOnInternalDelegate()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} delegate void MyDelegate();
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticDoesntAppearOnPublicDelegate()
	{
		const string testCode = """
		                        namespace TestApp.Public;
		                        public delegate void MyDelegate();
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticAppearsOnInternalGenericClass()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass<T>;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task DiagnosticDoesntAppearOnPublicGenericClass()
	{
		const string testCode = """
		                        namespace TestApp.Public;
		                        public class MyClass<T>;
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task NoDiagnosticOnInternalClassInInternalNamespaceContainingPublicFragment()
	{
		const string testCode = """
		                        namespace TestApp.Internal.PublicStuff;
		                        internal class MyClass;
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync(TestContext.Current.CancellationToken);
	}
}
