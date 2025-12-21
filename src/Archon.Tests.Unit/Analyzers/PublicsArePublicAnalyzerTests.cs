using Archon.Analyzers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Archon.Tests.Unit.Analyzers;

public class PublicsArePublicAnalyzerTests
{
	[Fact]
	public async Task DiagnosticAppearsOnInternalClass()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DIAGNOSTIC_ID}}:internal|} class MyClass;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync();
	}

	[Fact]
	public async Task DiagnosticDoesntAppearOnPublicClass()
	{
		const string testCode = """
		                        namespace TestApp.Public;
		                        public class MyClass;
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync();
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
		await test.RunAsync();
	}

	[Fact]
	public async Task DiagnosticDoesntAppearInNonPublicNamespace()
	{
		const string testCode = """
		                        namespace TestApp.Internal;
		                        internal class MyClass;
		                        """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync();
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicRecord()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DIAGNOSTIC_ID}}:internal|} record MyRecord;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync();
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicStruct()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DIAGNOSTIC_ID}}:internal|} struct MyStruct;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync();
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicInterface()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DIAGNOSTIC_ID}}:internal|} interface IMyInterface;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync();
	}

	[Fact]
	public async Task DiagnosticAppearsOnPublicEnum()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DIAGNOSTIC_ID}}:internal|} enum MyEnum;
		                          """;
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
		await test.RunAsync();
	}

    [Fact]
    public async Task NoDiagnosticOnInternalClassInPublicNamespaceContainingSectionWithInternalSlugFragment()
    {
        const string testCode = """
                                namespace TestApp.Public.InternalStuff;
                                internal class MyClass;
                                """;
        CSharpAnalyzerTest<InternalsAreInternalAnalyzer, DefaultVerifier> test = new() { TestCode = testCode };
        await test.RunAsync();
    }
}
