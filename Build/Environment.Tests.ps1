Describe -Tag "Environment" "Making sure the environment is ready to run the build scripts" {
	It @"
Is Powershell Version 3.0?
	This project requires Powershell 3.0.
	
	You can download the installer here: 
	(http://www.microsoft.com/en-us/download/details.aspx?id=34595)
"@	{
		$PSVersionTable["PSVersion"].Major | should be 3
	}
	It @"
Is MSBuild available?
	If this test fails, it is because the msbuild.exe has not been included in this machine's
	'Path' environment variable.  
	
	Add the location of msbuild.exe to the machine's 'Path' environment variable and run
	this test again.
"@	{
		@($env:Path.Split(";") | where {
			Test-Path (Join-Path $_ "MSBuild.Exe")
		}).Length -gt 0 | should be $true
	}
	It @"
Is MSBuild version 4.0?
	If this test fails, it is because the msbuild.exe that is available to the command line
	is not version 4.0.  This project needs access to the 4.0 version of msbuild.exe.
"@	{
		(msbuild /nologo /version) | out-file "TestDrive:\msbuildversion.txt"
		$version = get-content "TestDrive:\msbuildversion.txt"
		
		$version.StartsWith("4.0") | should be $true
	}
	It @"
Is nuget available?
	If this test fails, it is because the nuget.exe has not been included in this machine's
	'Path' environment variable.  
	
	Add the location of nuget.exe to the machine's 'Path' environment variable and run
	this test again.
"@	{
		@($env:Path.Split(";") | where {
			Test-Path (Join-Path $_ "nuget.exe")
		}).Length -gt 0 | should be $true
	}
}