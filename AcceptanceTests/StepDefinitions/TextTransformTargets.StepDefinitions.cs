using TechTalk.SpecFlow;
using NUnit.Framework;
using System.Diagnostics;

namespace AcceptanceTests.StepDefinitions
{
	[Binding]
	public class TextTransformTargets_StepDefinitions
	{
		public string ProjectFile { get; set; }
		public string ProjectFileName { get; set; }
		public string TemplateFile { get; set; }
		public string TemplateFileName { get; set; }
        public string CodeFileName { get; set; }
		public string OutputFileName { get; set; }
		public string ExecuteOutput { get; set; }
		public bool DoesOutputExist { get; set; }
		public FileSystem.TempDirectory TestDirectory { get; set; }

		[Before]
		public void RunBeforeScenario()
		{
			TestDirectory = new FileSystem.TempDirectory();
			DoesOutputExist = false;
			foreach(var file in System.IO.Directory.GetFiles(TestContext.CurrentContext.TestDirectory))
			{
				System.IO.File.Copy(file, TestDirectory.Append(System.IO.Path.GetFileName(file)));
			}
		}

		[After]
		public void RunAfterScenario()
		{
			TestDirectory.Dispose();
		}

		[Given(@"a project file consisting of:")]
		public void GetProjectFile(string project)
		{
			ProjectFile = project;
		}

		[Given(@"a template file called ""(.*)""")]
		public void GetTemplateFile(string templateFile)
		{
			TemplateFileName = TestDirectory.Append(templateFile);
		}
        
        [Given(@"a code file called ""(.*)""")]
		public void GetCodeFile(string codeFile)
		{
			CodeFileName = TestDirectory.Append(codeFile);
		}

		[Given(@"the template file consists of:")]
		public void GetTemplateContent(string templateContent)
		{
			TemplateFile = templateContent;
		}

		[Given(@"a file called ""(.*)"" exists")]
		public void FileExists(string filename)
		{
			OutputFileName = TestDirectory.Append(filename);
			DoesOutputExist = true;
		}

		[When("the project is built")]
		public void BuildProject()
		{
			ProjectFileName = TestDirectory.Append("build.proj");
			System.IO.File.WriteAllText(ProjectFileName, ProjectFile);
			System.IO.File.WriteAllText(TemplateFileName, TemplateFile);

			if(DoesOutputExist)
			{
				System.IO.File.WriteAllText(OutputFileName, "");
			}

			int exitCode = 0;
			ExecuteOutput = Utility.ExecuteCmd(string.Format(@"msbuild ""{0}"" /clp:PerformanceSummary", ProjectFileName), out exitCode);

			Assert.AreEqual(0, exitCode, ExecuteOutput);
		}

		[When("the target RunTemplates is ran")]
		public void RunTemplates()
		{
			ProjectFileName = TestDirectory.Append("build.proj");
			System.IO.File.WriteAllText(ProjectFileName, ProjectFile);
			System.IO.File.WriteAllText(TemplateFileName, TemplateFile);

			if(DoesOutputExist)
			{
				System.IO.File.WriteAllText(OutputFileName, "");
			}

			int exitCode = 0;
			ExecuteOutput = Utility.ExecuteCmd(string.Format(@"msbuild ""{0}"" /T:RunTemplates", ProjectFileName), out exitCode);

			Assert.AreEqual(0, exitCode, ExecuteOutput);
		}

		[Then("the project is cleaned")]
		public void CleanProject()
		{
			ProjectFileName = TestDirectory.Append("build.proj");
			System.IO.File.WriteAllText(ProjectFileName, ProjectFile);
			System.IO.File.WriteAllText(TemplateFileName, TemplateFile);

			if(DoesOutputExist)
			{
				System.IO.File.WriteAllText(OutputFileName, "");
			}

			int exitCode = 0;
			ExecuteOutput = Utility.ExecuteCmd(string.Format(@"msbuild ""{0}"" /T:Clean", ProjectFileName), out exitCode);

			Assert.AreEqual(0, exitCode, ExecuteOutput);
		}

		[Then("there should be no output files")]
		public void MakeSureNoOutputFilesExist()
		{
			string expectedOutput = TestDirectory.Append(string.Format("{0}.cs", System.IO.Path.GetFileNameWithoutExtension(TemplateFileName)));

			Assert.IsFalse(System.IO.File.Exists(OutputFileName));
		}

		[Then("the output of the msbuild call should not match this regex:")]
		public void OutputShouldNotMatchRegEx(string regex)
		{
			Assert.IsFalse(System.Text.RegularExpressions.Regex.IsMatch(ExecuteOutput, regex), 
				"The Output ({1}) does match: {0}", ExecuteOutput, regex);
		}

		[Then("the output of the msbuild call should match this regex:")]
		public void OutputShouldMatchRegEx(string regex)
		{
			Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(ExecuteOutput, regex), 
				"The Output ({1}) does match: {0}", ExecuteOutput, regex);
		}

		[Then(@"a file called ""(.*)"" should exist")]
		public void OutputExists(string outputFile)
		{
			OutputFileName = TestDirectory.Append(outputFile);
			Assert.IsTrue(System.IO.File.Exists(OutputFileName), "The Output file {0} doesn't exist", outputFile);

		}

		[Then("it should have content of:")]
		public void OutputContent(string outputContent)
		{
			Assert.AreEqual(outputContent, System.IO.File.ReadAllText(OutputFileName), "The content of the output file is not correct.");
		}

        [Then("the code file should still exist.")]
        public void CodeFileStillExists()
        {
            Assert.IsTrue(System.IO.File.Exists(CodeFileName));
        }
	}
}
