#TextTransform.Assemblies.feature

Feature: We should be able to reference two different assemblies with similar names
	This spawned from needing to reference both the Mono.TextTemplating.dll
	and the Mono.TextTemplating.Utility.dll in my templates.

	The code that is loading the assemblies was simply 
	checking that an assembly name starts with another assembly name 
	without the extension.

	Therefore System.Data could be loaded up when System.Data.Utility is needed.

Scenario: I should be able to instantiate an object from Mono.TextTemplating.Utility.dll
	When I have a template with content of:
	"""
<#@ template language="C#" debug="false" hostspecific="true"#>
<#@ output extension=".cs"#>
<#@ assembly name="..\Reference\Mono.TextTemplating.Utility.dll" #>
<#@ import namespace="Mono.TextTemplating.Utility.EntityFramework" #>
<#= new NullHost().ToString() #>
	"""
	Then TextTemplate.exe should produce output of:
	"""
Mono.TextTemplating.Utility.EntityFramework.NullHost
	"""

Scenario: I should still be able to load assemblies from the GAC
	When I have a template with content of:
	"""
<#@ template language="C#" debug="false" hostspecific="true"#>
<#@ output extension=".cs"#>
<#@ assembly name="System.Data.dll" #>
<#= System.Data.CommandType.StoredProcedure.ToString() #>
	"""
	Then TextTemplate.exe should produce output of:
	"""
StoredProcedure
	"""