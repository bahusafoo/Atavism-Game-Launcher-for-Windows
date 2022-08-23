###################################################
# Atavism Game Launcher for Windows
# Scratch Script
# Author(s): Bahusafoo
###################################################

$ReleaseBuildPath = "E:\Projects\Gamdev\LauncherDemoGame2\Builds\21.12.13.1\"

function StripEndingSlash ($PathToStrip) {
    if ($PathToStrip -like "*\") {
        $ReleaseBuildPathString = ""
        foreach ($i in 0..$($PathToStrip.Length - 2)) {
            $ReleaseBuildPathString = "$($ReleaseBuildPathString)$($PathToStrip[$i])"
        }
        # Loop through again in case of multiple ending \
        StripEndingSlash -PathToStrip $ReleaseBuildPathString
    } else {
        return $PathToStrip
    }
}

# Make sure the path does not end in \
$ReleaseBuildPath = StripEndingSlash -PathToStrip $ReleaseBuildPath

# Create new arraylist object to store hash infos
$ReleaseHashInfo = New-Object System.Collections.ArrayList
$ReleaseHashList = New-Object System.Collections.ArrayList
# Gather Hash infos
foreach ($ChildFile in $(Get-ChildItem -Recurse -Path $ReleaseBuildPath)) {
    if ($ChildFile.FullName.Replace($ReleaseBuildPath, '').contains("FileHashesList.launchervallst") -ne $true) {
        $HashInfo = Get-FileHash -Path $ChildFile.FullName -Algorithm MD5
        if ($HashInfo.Hash) {
            $ReleaseHashInfo.Add("$($HashInfo.Hash.ToLower()),$($ChildFile.FullName.Replace($ReleaseBuildPath,''))") | Out-Null
            $ReleaseHashList.Add($HashInfo.Hash.ToLower()) | Out-Null
        }
    }
}
$ReleaseHashList = $ReleaseHashList | Sort-Object -Descending

$OutPutRoot = $($(Get-Item -Path $(Get-Item -Path $ReleaseBuildPath).Parent.FullName).Parent.FullName)
if (!(Test-Path -Path "$($OutPutRoot)\Launcher Resources")) {
    New-Item -Path "$($OutPutRoot)\Launcher Resources" -ItemType Directory -Force -erroraction SilentlyContinue
}
$ReleaseHashInfo | Out-File -FilePath "$($OutPutRoot)\Launcher Resources\FileHashesList.launchervallst" -Encoding utf8 -Force

$ValKeyString = ""
foreach ($ReleaseHashListEntry in $ReleaseHashList) {
    $ValKeyString = "$($ValKeyString)$($ReleaseHashListEntry.substring($ReleaseHashListEntry.Length - 6))"
    Write-Host "Added: $($ReleaseHashListEntry.substring($ReleaseHashListEntry.Length - 6))"
}
$ValKeyString | Out-File -FilePath "$($OutPutRoot)\Launcher Resources\ValidationString.txt" -Encoding utf8 -Force

 Start-Process -FilePath "$($ReleaseBuildPath)\LauncherDemoGame2.exe" -ArgumentList "-LaunchVal $($ValKeyString)"