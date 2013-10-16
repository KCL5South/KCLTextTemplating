using TechTalk.SpecFlow;
using NUnit.Framework;

namespace AcceptanceTests.StepDefinitions
{
	[Binding]
	public class TextTransform_Assemblies_StepDefinitions
	{
		public string TemplateFile { get; set; }
		public string TemplateFileName { get; set; }
		public string OutputFileName { get; set; }
		public FileSystem.TempDirectory TestDirectory { get; set; }

		[Before]
		public void RunBeforeScenario()
		{
			TestDirectory = new FileSystem.TempDirectory();
			System.IO.Directory.CreateDirectory(TestDirectory.Append("Testing"));
			System.IO.Directory.CreateDirectory(TestDirectory.Append("Reference"));
			foreach(var file in System.IO.Directory.GetFiles(TestContext.CurrentContext.TestDirectory))
			{
				System.IO.File.Copy(file, TestDirectory.Append(string.Format("Testing\\{0}", System.IO.Path.GetFileName(file))));
			}

			System.IO.File.Move(TestDirectory.Append("Testing\\Mono.TextTemplating.Utility.dll"), TestDirectory.Append("Reference\\Mono.TextTemplating.Utility.dll"));

			TemplateFileName = TestDirectory.Append(string.Format("Testing\\{0}.tt", System.Guid.NewGuid().ToString().Replace("-", "")));
			OutputFileName = TestDirectory.Append(string.Format("Testing\\{0}.cs", System.IO.Path.GetFileNameWithoutExtension(TemplateFileName)));
		}

		[After]
		public void RunAfterScenario()
		{
			TestDirectory.Dispose();
		}

		[When("I have a template with content of:")]
		public void TemplateContent(string content)
		{
			System.IO.File.WriteAllText(TemplateFileName, content);
			Assert.IsTrue(System.IO.File.Exists(TemplateFileName), "Template file was not created correctly.");
		}	

		[Then("TextTemplate.exe should produce output of:")]
		public void Output(string expectedOutput)
		{
			int exitCode = 0;
			string result = Utility.ExecuteCmd(	string.Format("TextTransform.exe -o \"{0}\" \"{1}\"", 
												System.IO.Path.GetFileName(OutputFileName), 
												TemplateFileName),
												TestDirectory.Append("Testing"), 
												out exitCode);

			Assert.AreEqual(0, exitCode, "We got a non-zero exit code: {0}; {1}", exitCode, result);
			Assert.IsTrue(System.IO.File.Exists(OutputFileName), "The file {0} does not exist.", OutputFileName);
			Assert.AreEqual(expectedOutput, System.IO.File.ReadAllText(OutputFileName), "The Content of the output file was not correct.");
		}
	}
}