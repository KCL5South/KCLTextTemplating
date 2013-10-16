#TextTransformTargets.feature

Feature: Correctly adding templates to the project file
	When I add a file with a .tt extension to the 'None' ItemGroup
	and mark it as KCLTextTemplating
	then it is ran through the KCLTextTemplating program.

Scenario: Incorrectly Registering Templates
	Given a project file consisting of:
	"""
<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTarget="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
	<ItemGroup>
		<None Include="Template.tt"/>
	</ItemGroup>

	<Target Name="Build">
		<CallTarget Targets="KCLTextTemplating"/>
	</Target>

	<Import Project="KCLTextTemplating.targets"/>
</Project>
	"""
	And a template file called "Template.tt"
	And the template file consists of:
	"""
Some Random Text
	"""
	When the project is built
	Then there should be no output files

Scenario: Incorrectly Registering Templates (Should not trigger target at all...)
	Given a project file consisting of:
	"""
<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTarget="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
	<ItemGroup>
		<None Include="Template2.tt"/>
		<None Include="Template.tt">
			<KCLTextTemplating>True</KCLTextTemplating>
		</None>
	</ItemGroup>

	<Target Name="Build">
		<CallTarget Targets="KCLTextTemplating"/>
	</Target>

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
	When the project is built
	Then the output of the msbuild call should not match this regex:
	"""
Files to process:(.*)Template2.tt
	"""

Scenario: Incorrect Extension On Templates
	Given a project file consisting of:
	"""
<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTarget="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
	<ItemGroup>
		<None Include="Template.someExtension">
			<KCLTextTemplating>True</KCLTextTemplating>
		</None>
	</ItemGroup>

	<Target Name="Build">
		<CallTarget Targets="KCLTextTemplating"/>
	</Target>

	<Import Project="KCLTextTemplating.targets"/>
</Project>
	"""
	And a template file called "Template.someExtension"
	And the template file consists of:
	"""
It Doesn't matter 
	"""
	When the project is built
	Then there should be no output files

Scenario: Correctly Registering Templates Produces Output
	Given a project file consisting of:
	"""
<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTarget="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
	<ItemGroup>
		<None Include="Template.tt">
			<KCLTextTemplating>True</KCLTextTemplating>
		</None>
	</ItemGroup>

	<Target Name="Build">
		<CallTarget Targets="KCLTextTemplating"/>
	</Target>

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
	When the project is built
	Then a file called "Template.cs" should exist
	And it should have content of:
	"""
Some Random Text
	"""

Scenario: Templates with already generated output are skipped
	Given a template file called "Template.tt"
	And the template file consists of:
	"""
<#@ template language="C#" debug="false" hostspecific="true"#>
<#@ output extension=".cs"#>
Some Random Text
	"""
	And a file called "Template.cs" exists 
	And a project file consisting of:
	"""
<?xml version="1.0" encoding="utf-8"?>
<Project DefaultTarget="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">
	<ItemGroup>
		<None Include="Template.tt">
			<KCLTextTemplating>True</KCLTextTemplating>
		</None>
	</ItemGroup>

	<Target Name="Build">
		<CallTarget Targets="KCLTextTemplating"/>
	</Target>

	<Import Project="KCLTextTemplating.targets"/>
</Project>
	"""
	When the project is built
	Then the output of the msbuild call should match this regex:
	"""
Skipping target "KCLTextTemplating"
	"""
