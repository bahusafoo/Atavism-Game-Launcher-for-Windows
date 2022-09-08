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

#Sample variable that provides the location of the script
[string]$ScriptDirectory = Get-ScriptDirectory

[string]$global:ScriptDirectory = Get-ScriptDirectory
[string]$global:LauncherInstallerVersion = [System.Windows.Forms.Application]::ProductVersion.ToString()
$global:LauncherInstalledDirectory = ""
$global:BaseConfigPath = "$($global:ScriptDirectory)\Config\BaseConfig.glconfig"
[string[]]$Global:LauncherResources = "Payload\Launcher.exe", "Payload\Config\BaseConfig.glconfig", "Payload\Resources\Launcher.ico", "Payload\Resources\Background_Main.png", "Payload\Resources\Background_TextBox.png", "Payload\Resources\Button_Account.png", "Payload\Resources\Button_Account_Hover.png", "Payload\Resources\Button_Close.png", "Payload\Resources\Button_Close_Hover.png", "Payload\Resources\Button_Discord.png", "Payload\Resources\Button_Discord_Hover.png", "Payload\Resources\Button_Forums.png", "Payload\Resources\Button_Forums_Hover.png", "Payload\Resources\Button_Minimize.png", "Payload\Resources\Button_Minimize_Hover.png", "Payload\Resources\Button_Play_Play_Hover.png", "Payload\Resources\Button_Play_Play_Idle.png", "Payload\Resources\Button_Play_Unreachable_Hover.png", "Payload\Resources\Button_Play_Unreachable_Idle.png", "Payload\Resources\Button_Play_Update_Hover.png", "Payload\Resources\Button_Register.png", "Payload\Resources\Button_Register_Hover.png", "Payload\Resources\Button_Repair.png", "Payload\Resources\Button_Repair_Hover.png", "Payload\Resources\Button_Settings.png", "Payload\Resources\Button_Settings_Hover.png", "Payload\Resources\Frame_Main.png", "Payload\Resources\Launcher.ico", "Payload\Resources\Logo_Game.png"
$global:LogPath = ""
$global:MainConfigDir = ""
$global:MainWorkingDir = ""
$global:LauncherInstallPath = ""
$Global:PublisherName = ""
$Global:GameName = ""
$global:LauncherDir = ""

# Language Overrides
$global:LangTextWindowTitleText = "!GAMENAME! UnInstaller"
$global:LangTextUninstallConfirmButtonText = "Yes"
$global:LangTextUninstallCancelButtonText = "No"
$global:LangTextUnInstallerWelcomeMessageText = "Would you like to uninstall !GAMENAME!?"
