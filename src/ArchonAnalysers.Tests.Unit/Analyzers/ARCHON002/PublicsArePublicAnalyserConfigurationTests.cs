using ArchonAnalysers.Analyzers.ARCHON002;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace ArchonAnalysers.Tests.Unit.Analyzers.ARCHON002;

public class PublicsArePublicAnalyserConfigurationTests
{
	[Fact]
	public async Task CustomSingleSlug_TriggersOnConfiguredNamespace()
	{
		const string testCode = $$"""
		                          namespace TestApp.Api;
		                          {|{{PublicsArePublicAnalyser.DiagnosticId}}:internal|} class MyClass;
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs = Api
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyser, DefaultVerifier> test = new()
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

		CSharpAnalyzerTest<PublicsArePublicAnalyser, DefaultVerifier> test = new()
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
		                              {|{{PublicsArePublicAnalyser.DiagnosticId}}:internal|} class ApiClass;
		                          }

		                          namespace TestApp.Exposed
		                          {
		                              {|{{PublicsArePublicAnalyser.DiagnosticId}}:internal|} class ExposedClass;
		                          }
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs = Api, Exposed, Public
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyser, DefaultVerifier> test = new()
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

		CSharpAnalyzerTest<PublicsArePublicAnalyser, DefaultVerifier> test = new()
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
		                          {|{{PublicsArePublicAnalyser.DiagnosticId}}:internal|} class MyClass;
		                          """;

		// No editorconfig file added - should use default "Public"
		CSharpAnalyzerTest<PublicsArePublicAnalyser, DefaultVerifier> test = new()
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
		                          {|{{PublicsArePublicAnalyser.DiagnosticId}}:internal|} class MyClass;
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs =
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyser, DefaultVerifier> test = new()
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
		                          {|{{PublicsArePublicAnalyser.DiagnosticId}}:internal|} class MyClass;
		                          """;

		const string editorConfig = """
		                            [*.cs]
		                            archon_002.public_namespace_slugs =  Api  ,  Public
		                            """;

		CSharpAnalyzerTest<PublicsArePublicAnalyser, DefaultVerifier> test = new()
		{
			TestCode = testCode
		};

		test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
		await test.RunAsync(TestContext.Current.CancellationToken);
	}
}
