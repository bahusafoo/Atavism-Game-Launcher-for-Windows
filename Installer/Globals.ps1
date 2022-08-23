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

[string]$global:ScriptDirectory = Get-ScriptDirectory
[string]$global:LauncherInstallerVersion = [System.Windows.Forms.Application]::ProductVersion.ToString()
$global:LauncherInstalledDirectory = ""
$global:BaseConfigPath = "$($global:ScriptDirectory)\Payload\Config\BaseConfig.config"
[string[]]$Global:LauncherResources = "Payload\Launcher.exe", "Payload\Config\BaseConfig.config", "Payload\Resources\Launcher.ico", "Payload\Resources\Background_Main.png", "Payload\Resources\Background_TextBox.png", "Payload\Resources\Button_Account.png", "Payload\Resources\Button_Account_Hover.png", "Payload\Resources\Button_Close.png", "Payload\Resources\Button_Close_Hover.png", "Payload\Resources\Button_Discord.png", "Payload\Resources\Button_Discord_Hover.png", "Payload\Resources\Button_Forums.png", "Payload\Resources\Button_Forums_Hover.png", "Payload\Resources\Button_Minimize.png", "Payload\Resources\Button_Minimize_Hover.png", "Payload\Resources\Button_Play_Play_Hover.png", "Payload\Resources\Button_Play_Play_Idle.png", "Payload\Resources\Button_Play_Unreachable_Hover.png", "Payload\Resources\Button_Play_Unreachable_Idle.png", "Payload\Resources\Button_Play_Update_Hover.png", "Payload\Resources\Button_Register.png", "Payload\Resources\Button_Register_Hover.png", "Payload\Resources\Button_Repair.png", "Payload\Resources\Button_Repair_Hover.png", "Payload\Resources\Button_Settings.png", "Payload\Resources\Button_Settings_Hover.png", "Payload\Resources\Frame_Main.png", "Payload\Resources\Launcher.ico", "Payload\Resources\Logo_Game.png"
$global:LogPath = ""
$global:MainConfigDir = ""
$global:MainWorkingDir = ""
$global:LauncherInstallPath = ""
$Global:PublisherName = ""
$Global:GameName = ""
$global:EULAText = ""

# Language Overrides
$global:LangTextInstallerWindowTitleText = "!GAMENAME! Launcher Installer"
$global:LangTextInstallerWelcomeMessageText = "Welcome to the !GAMENAME! installation program.  This installer will install the game launcher, which will allow you to choose where to install the game and handle updating the game client from time to time."
$global:LangTextInstallationDirectoryBrowseButtonText = "Browse"
$global:LangTextAgreeAndInstallButtonText = "Agree and Install"
$global:LangTextChooseInstallerLocationInfoText = "Choose a location to install the launcher:"
$global:LangTextEULAInfoText = "End User License Agreement:"
$global:LangTextErrorCreatingInstallationDirectoryMessageBoxMessage = "Error encountered creating installation directory.  Please check that the path (""!LAUNCHERINSTALLPATH!"") exists and that you have permissions to modify it."
$global:LangTextErrorCreatingInstallationDirectoryMessageBoxCaption = "Failed to install launcher files."
$global:LangTextErrorInstallingInChosenDirectoryMessageBoxMessage = "Failed to install launcher files.  Please check that the path (""!LAUNCHERINSTALLPATH!"") exists and that you have permissions to modify it."
$global:LangTextChosenDirectoryDoesNotExistMessageBoxMessage = "The selected path does not exist"
$global:LangTextChosenDirectoryDoesNotExistMessageBoxCaption = "Location is missing"
$global:LangTextProvidedSettingHasNoValueMessageBoxMessage = "A setting provided has either no name or no value!"
$global:LangTextProvidedSettingHasNoValueMessageBoxCaption = "BaseConfig.config error"
$global:LangTextBaseConfigFileInvalidMessageBoxMessage = "There was a problem loading BaseConfig.config file!  Please make sure the file exists and is valid!"
$global:LangTextBaseConfigLoadErrorMessageBoxCaption = "Base Configuration Error"
$global:LangTextLauncherConfigFilesMissingMessageBoxMessage = "One or more required launcher configuration files is missing.  Please contact !PUBLISHER! for assistance."
$global:LangTextLauncherConfigFilesMissingMessageBoxCaption = "Installation Package Error"
$global:LangTextEULADownloadFailedMessageBoxMessage = "End User License Agreement download has failed.  Without the ability to view and accept this, the launcher will not allow play.  Please try again later, or contact !PUBLISHER! for assistance."
$global:LangTextEULADownloadFailedMessageBoxCaption = "EULA Download Error"
$global:LangTextErrorRetrievingPreviousLauncherLocationMessageBoxMessage = "Error retrieving previous launcher installation location.  Please reinstall the launcher to fix this."
$global:LangTextErrorRetrievingPreviousLauncherLocationMessageBoxCaption = "Launcher Install Location Error"
$global:LangTextMissingPreviousLauncherInstallationMessageBoxMessage = "There was an error with a previously defined installation path which no longer exists.  Please reinstall the launcher to fix this."
