###################################################
# Atavism Game Launcher for Windows
# Author(s): Bahusafoo
###################################################

function Get-ScriptDirectory
{
<#
	.SYNOPSIS
		Get-ScriptDirectory returns the proper location of the script.

	.OUTPUTS
		System.String
	
	.NOTES
		Returns the correct path within a packaged executable.
#>
	[OutputType([string])]
	param ()
	if ($null -ne $hostinvocation)
	{
		Split-Path $hostinvocation.MyCommand.path
	}
	else
	{
		Split-Path $script:MyInvocation.MyCommand.Path
	}
}

[string]$Global:ScriptDirectory = Get-ScriptDirectory
$Global:AttemptedChecks = 0
[version]$global:VersionAtLaunch = 0.0.0.0
[version]$global:CurrentVersion = 0.0.0.0
