using ArchonAnalysers.Analyzers.ARCHON002;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace ArchonAnalysers.Tests.Unit.Analyzers.ARCHON002;

public class PublicsArePublicAnalyzerConfigurationTests
{
	[Fact]
	public async Task CustomSingleSlug_TriggersOnConfiguredNamespace()
	{
		const string testCode = $$"""
		                          namespace TestApp.Api;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass;
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs = Api
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task CustomSingleSlug_DoesNotTriggerOnDefaultNamespace()
	{
		const string testCode = """
		                        namespace TestApp.Public;
		                        internal class MyClass;
		                        """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs = Api
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task MultipleSlugs_TriggersOnAllConfiguredNamespaces()
	{
		const string testCode = $$"""
		                          namespace TestApp.Api
		                          {
		                              {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class ApiClass;
		                          }

		                          namespace TestApp.Exposed
		                          {
		                              {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class ExposedClass;
		                          }
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs = Api, Exposed, Public
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task MultipleSlugs_DoesNotTriggerOnNonConfiguredNamespace()
	{
		const string testCode = """
		                        namespace TestApp.Internal;
		                        internal class MyClass;
		                        """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs = Api, Exposed
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task NoConfiguration_UsesDefaultPublicSlug()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass;
		                          """;

		// No editorconfig file added - should use default "Public"
		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task EmptyConfiguration_UsesDefaultPublicSlug()
	{
		const string testCode = $$"""
		                          namespace TestApp.Public;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass;
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs =
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
		await test.RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task WhitespaceInConfiguration_IsProperlyTrimmed()
	{
		const string testCode = $$"""
		                          namespace TestApp.Api;
		                          {|{{PublicsArePublicAnalyzer.DiagnosticId}}:internal|} class MyClass;
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs =  Api  ,  Public
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyzer, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
		await test.RunAsync(TestContext.Current.CancellationToken);
	}
}
