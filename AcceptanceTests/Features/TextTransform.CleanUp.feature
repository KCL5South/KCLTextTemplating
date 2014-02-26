#TextTransform.CleanUp.feature

Feature: Template output is properly cleaned up.
	When I add a file with a .tt extension to the 'None' ItemGroup
	and mark it as KCLTextTemplating
	Then when I clean the project, there should not be any KCLTextTemplating output.

Scenario: Cleaning up
	Given a project file consisting of:
	"""
<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTarget="RunTemplates" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
	<ItemGroup>
		<None Include="Template.tt">
			<KCLTextTemplating>True</KCLTextTemplating>
		</None>
	</ItemGroup>

	<Target Name="RunTemplates">
		<CallTarget Targets="KCLTextTemplating"/>
	</Target>

    <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
    <Import Project="KCLTextTemplating.targets"/>
</Project>
	"""
	And a template file called "Template.tt"
	And the template file consists of:
	"""
<#@ template language="C#" debug="false" hostspecific="true"#>
<#@ output extension=".cs"#>
Some Random Text
	"""
	When the target RunTemplates is ran
	Then a file called "Template.cs" should exist
    When the project is cleaned
    Then there should be no output files 

Scenario: Cleaning up against other code files
	Given a project file consisting of:
	"""
<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTarget="RunTemplates" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
	<ItemGroup>
		<None Include="Template.tt">
			<KCLTextTemplating>True</KCLTextTemplating>
        </None>
        <Compile Include="*.cs"/>
	</ItemGroup>

	<Target Name="RunTemplates">
		<CallTarget Targets="KCLTextTemplating"/>
	</Target>

    <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
    <Import Project="KCLTextTemplating.targets"/>
</Project>
	"""
    And a template file called "Template.tt"
    And a code file called "Test.cs"
	And the template file consists of:
	"""
<#@ template language="C#" debug="false" hostspecific="true"#>
<#@ output extension=".cs"#>
Some Random Text
    """
    And the code file consists of:
    """
public class TestClass { }
    """
	When the target RunTemplates is ran
	Then a file called "Template.cs" should exist
    When the project is cleaned
    Then there should be no output files
    And the code file should still exist.
