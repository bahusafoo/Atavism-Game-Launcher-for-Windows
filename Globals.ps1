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

Add-Type -AssemblyName presentationCore
[string]$global:ScriptDirectory = Get-ScriptDirectory
$global:CurrentProcessID = $PID
$Global:VersionRunning = $([System.Windows.Forms.Application]::ProductVersion.ToString().Replace(",", "."))

##### Base Config Globals #####
$global:BaseConfigPath = "$($global:ScriptDirectory)\Config\BaseConfig.glconfig"
[version]$global:BaseConfigPackageVersion = "0.0.0.0"
$Global:GameName = ""
$Global:PublisherName = ""
$global:LogPath = ""
$Global:LauncherDownloadRootURL = ""
$Global:LauncherMainConfigURL = ""
$global:MainConfigDir = ""

##### Main Config Globals #####
[version]$Global:CurrentConfigPackageVersion = "0.0.0.0"
$Global:GameEXEName = ""
$Global:PublisherContactEmail = ""
$Global:GameWebsite = ""
$Global:ClientDownloadRootURL = ""
$Global:MenuButton1URL = ""
$Global:MenuButton2URL = ""
$Global:MenuButton3URL = ""
$Global:MenuButton4URL = ""
$Global:GameFileHashListURL = ""
$global:MainWorkingDir = ""
[array]$global:FileValidationResults = $null
[float]$Global:CurrentDownloadQueue = 0
##### Theme Colors #####
$global:TextColor = "D3D3D3"
$global:ReleaseNotesTextColor = ""
$global:ProgressBarColor = "00a2ed"

##### Others #####
[float]$global:TotalCurrentSize = 0
[float]$global:TotalDownloadSize = 0
[float]$global:TotalEndSize = 0
[int]$global:DownloadLogEntryReductionVar = 4
$Global:GameCheckStatus = "NeedsUpdate"
$Global:StatusLabelDownloadText = ""
$Global:Icon_Launcher = $null
$Global:Image_Button_Close_Idle = $null
$Global:Image_Button_Close_Hover = $null
$Global:Image_Button_Minimize_Idle = $null
$Global:Image_Button_Minimize_Hover = $null
$Global:Image_Background_Main = $null
$Global:Image_BorderFrame_Main = $null
$Global:Image_Logo_Game = $null
$global:FirstRun = $true
$global:ReloadSettings = $false
$global:EULAText = "No EULA Downloaded.  Please exit the launcher and do not launch the game until the EULA becomes available.  By bypassing this warning, you agree to not hold the publisher accountable for any consequence of using this software."

##### Saved User Settings #####
$global:UserPref_GameInstallationDirectory = ""

##### Language Texts #####
$global:LangTextLauncherVersionLabelText = "Launcher v!LAUNCHERVERSION!"
$global:LangTextValidatingGameFiles = "Validating local game files"
$global:LangTextGameClientFilesDownloading = "Downloading !GAMENAME! files"
$global:LangTextGameClientFilesDownloadFailed = "!GAMENAME! client files download failed"
$global:LangTextGameLaunchedSuccess = "!GAMENAME! launched succesfully, launcher closing..."
$global:LangTextGameNeedsUpdateOrInstalled = "!GAMENAME! files need updated or installed"
$global:LangTextGameClientFileValidationFailed = "Validation Result: Local File Validation job returned an unknown state.  Launcher will abort, please try again."
$global:LangTextLaunchGameFail = "Error Launching the game!"
$global:LangTextLaunchGameStarting = "!GAMENAME! is starting..."
$global:LangTextGameUnavailable = "!GAMENAME! is currently unavailable."
$global:LangTextGameReady = "!GAMENAME! is ready to play!"
$global:LangTextFileValidationListDownloadFailed = "File Validation List Download Failed."
$global:LangTextFileValidationListDownloadFailedMessageBoxCaption = "Hash validation file download error"
$global:LangTextFileValidationListDownloadFailedMessageBoxMessage = "There was an error downloading the game file hash comparison list from the server.  Please try again later or contact !PUBLUSHER! for assistance."
$global:LangTextFileValidationListInvalid = "File Validation List Invalid."
$global:LangTextFileMasterHashListInvalidMessageBoxCaption = "Master Hash List from Server is Invalid!"
$global:LangTextFileMasterHashListInvalidMessageBoxMessage = "The Master Hash list returned from the server is not a valid Master Hash list.`n`r`n`rPlease contact !PUBLUSHER! and provide them the file located at ""!WORKINGDIR!\FileHashesList.launchervallst"" as well as the log located at ""!LOGPATH!\Launcher.log"""
$global:LangTextEULADownloadFailedMessageBoxCaption = "EULA Download Error"
$global:LangTextEULADownloadFailedMessageBoxMessage = "End User License Agreement download has failed.  Without the ability to view and accept this, the launcher will not allow play.  Please try again later, or contact !PUBLISHER! for assistance."
$global:LangTextLauncherSettingsNotConfiguredMessageBoxCaption = "User Settings Error"
$global:LangTextLauncherSettingsNotConfiguredMessageBoxMessage = "Launcher settings were not configured.  The launcher will not install or patch the game until these have been set.  Please use the settings button to configure settings."
$global:LangTextMainConfigFileInvalidMessageBoxCaption = "Main Configuration from Server is Invalid!"
$global:LangTextMainConfigFileInvalidMessageBoxMessage = "The Main Configuration file returned from the server is not a valid launcher Configuration file.`n`r`n`rPlease contact !PUBLISHER! and provide them the file located at ""!MAINCONFIGDIR!\MainConfig.glconfig"" as well as the log located at ""!LOGPATH!\Launcher.log"""
$global:LangTextBaseConfigFileInvalidMessageBoxCaption = "BaseConfig.conf error"
$global:LangTextBaseConfigSettingMissingPropertiesMessageBoxMessage = "A setting provided has either no name or no value!"
$global:LangTextBaseConfigUnknownSettingMessageBoxMessage = "A setting provided (!SETTINGNAME!) is unknown and will be ignored!"
$global:LangTextBaseConfigFileInvalidMessageBoxMessage = "There was a problem loading BaseConfig.glconfig file!  Please make sure the file exists and is valid!"
$global:LangTextBaseConfigLoadErrorMessageBoxCaption = "Base Configuration Error"
$global:LangTextBaseConfigLoadErrorMessageBoxMessage = "There was an error loading the base Configuration file for the launcher, please try reinstalling.  The application will exit now."
$global:LangTextMainConfigLoadErrorMessageBoxCaption = "Main Configuration Load Error"
$global:LangTextMainConfigLoadErrorMessageBoxMessage = "There was an error loading the Main Configuration file for the launcher. Please try again later, or contact !PUBLISHER! for assistance.  The application will exit now."
$global:LangTextReinstallConfirmationMessageBoxCaption = "Repair via redownload?"
$global:LangTextReinstallConfirmationMessageBoxMessage = "This action will redownload and reinstall the entire game.  Are you sure you would like to continue?"
$global:LangTextLauncherResourcesErrorMessageBoxCaption = "Launcher Resources Error"
$global:LangTextLauncherResourcesErrorMessageBoxMessage = "There was an error finding launcher resources.  Please reinstall the launcher and try again, or contact the game publisher for assistance."
$global:LangTextSettingsWindowTitle = "Launcher Settings"
$global:LangTextProvideGameInstallLocation = "Please provide the location where you would like to install the game or the existing location if it is already installed."
$global:LangTextGameInstallationDirectoryLabel = "Game Installation Directory:"
$global:LangTextCancelInstall = "Cancel and Exit"
$global:LangTextAgreeAndInstall = "Agree and Save"
$global:LangTextLauncherLogs = "Launcher Logs"
$global:LangTextBrowseInstallationDirectory = "Browse"
$global:LangTextAVExclusionsWindowTitle = "Antivirus Exclusion Recommendations"
$global:LangTextAVExclusionsInfoText = "If you have issues with installation or patching, consider adding the follwing exclusions to your antivirus software (Highlight + Copy is enabled):"
$global:LangTextAVExclusionsButtonText = "Finished"
$global:LangTextClientRepairNeededStatus = "Client Repair is needed."
$global:LangTextOrphanedFilesCleanupStatus = "Searching for and cleaning orphaned game files"