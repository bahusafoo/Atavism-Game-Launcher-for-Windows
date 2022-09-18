// LaunchedFromLauncherCheck.cs
// Author: Bahusafoo
// Version: 21.12.16.7

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using System;


public class LaunchedFromLauncherCheck : MonoBehaviour
{
    //TODO: Change this, for now use static secret phrase
    static string launcherCheckVal = "";
    static string masterCheckVal = "";
    static string currentApplicationPath = "";
    [SerializeField] string launcherValidationListURL = "https://someplace.com/FileHashesList.launchervallst";
    [SerializeField] GameObject NotificationMessageWindow;
    [SerializeField] GameObject ErrorMessageText;
    [SerializeField] bool ValidationIsRequiredToPlay = true;
    static ArrayList appFilesList = new ArrayList();
    static ArrayList appFileMD5sList = new ArrayList();
    static ArrayList masterFileMD5sList = new ArrayList();
    static ArrayList appFileMD5PathList = new ArrayList();
    // Start is called before the first frame update
    void Start()
    {
        currentApplicationPath = System.IO.Directory.GetParent(Path.GetFullPath(Application.dataPath)).FullName;
        Debug.Log("Game Validation Manager: Game Installation Path: " + currentApplicationPath);
        if (Application.isEditor)
        {
            Debug.LogWarning("Game Validation Manager: Running in editor, skipping launcher + game file validation.");
            this.NotificationMessageWindow.SetActive(false);
            // Load Next Scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } else
        {
            if (ValidationIsRequiredToPlay != true)
            {
                Debug.Log("Game Validation Manager: Validation is not set to required, playing will be allowed regardless of validation outcome!");
            }
            switch (CommandLineArgsCheck(launcherCheckVal))
            {
                case "SUCCESS":
                    Debug.Log("Game Validation Manager: Game file validation passed, loading next scene.");
                    this.NotificationMessageWindow.SetActive(false);
                    // Load Next Scene
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                case "NONE":
                    // Display Error
                    if (ValidationIsRequiredToPlay == true)
                    {
                        Debug.Log("Game Validation Manager: Game was not launched from the launcher, presenting closure dialog with exit action.");
                        ErrorMessageText.GetComponent<TMPro.TextMeshProUGUI>().text = "The game must be launched from the launcher.";
                    }
                    else
                    {
                        ErrorMessageText.GetComponent<TMPro.TextMeshProUGUI>().text = "Game was not launched from the launcher.  You will need to update or repair from the launcher to receive the latest update.";
                    }
                    this.NotificationMessageWindow.SetActive(true);
                    break;
                default:
                    // Display Error
                    if (ValidationIsRequiredToPlay == true)
                    {
                        Debug.Log("Game Validation Manager: Game file validation failed, presenting closure dialog with exit action.");
                        ErrorMessageText.GetComponent<TMPro.TextMeshProUGUI>().text = "Error with game validation.  Please update or repair the game from the launcher.";
                    } else
                    {
                        ErrorMessageText.GetComponent<TMPro.TextMeshProUGUI>().text = "Game validation has failed.  You will need to update or repair from the launcher to receive the latest update.";
                    }
                    this.NotificationMessageWindow.SetActive(true);
                    break;
            }
        }
    }
    public string CommandLineArgsCheck(string launcherCheckVal)
    {

        string[] args = System.Environment.GetCommandLineArgs();
        string launchVal = "";
        try
        {
            for (int i = 0; i < args.Length; i++)
            {
                //Debug.Log("ARG " + i + ": " + args[i]);
                if (args[i] == "-LaunchVal")
                {
                    launchVal = args[i + 1];
                }
            }
        } catch
        {
            return "ERROR";
        }
        if (launchVal != "")
        {
            masterCheckVal = ComputeMasterCheckVal();
            launcherCheckVal = ComputelauncherCheckVal();
            bool CurrentMatchMaster = CompareMD5Lists();
            if (launchVal == launcherCheckVal && launcherCheckVal == masterCheckVal && CurrentMatchMaster == true)
            {
                return "SUCCESS";
            } else
            {
                return "FAILED";
            }
        }
        else
        {
            return "NONE";
        }
    }

    string ComputelauncherCheckVal()
    {
        GatherFiles(currentApplicationPath);
        foreach (string appFile in appFilesList)
        {
            if (appFile.Replace(currentApplicationPath, string.Empty).Contains("FileHashesList.launchervallst") != true)
            {
                // Debug.Log(appFile);
                using (var appFileStream = File.OpenRead(appFile))
                {
                    using (var appFileMD5 = MD5.Create())
                    {

                        string appFileStreamHash = BitConverter.ToString(appFileMD5.ComputeHash(appFileStream)).Replace("-", string.Empty).ToLower();
                        appFileMD5sList.Add(appFileStreamHash);
                        appFileMD5PathList.Add(appFileStreamHash + "," + appFile.Replace(currentApplicationPath, string.Empty));
                    }
                }
            }
        }
        appFileMD5sList.Sort();
        appFileMD5sList.Reverse();
        string launcherCheckValString = "";
        foreach (string appFileMD5 in appFileMD5sList)
        {
            launcherCheckValString = launcherCheckValString + appFileMD5.Substring(appFileMD5.Length - 6);
        }
        return launcherCheckValString;

    }

    string ComputeMasterCheckVal()
    {
        // make sure value in LauncherValidationListURL does not include the file name
        if (launcherValidationListURL.EndsWith("/FileHashesList.launchervallst") != true)
        {
            launcherValidationListURL = launcherValidationListURL + "/FileHashesList.launchervallst";
        }

        string MD5ListDLPath = Path.Combine(Application.persistentDataPath, "ValidationResources");
        MD5ListDLPath = Path.Combine(MD5ListDLPath.Replace("/", "\\"), "FileHashesList.launchervallst");

        // Make sure directory exists
        if (!Directory.Exists(Path.GetDirectoryName(MD5ListDLPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(MD5ListDLPath));
        }

        var dlClient = UnityWebRequest.Get(launcherValidationListURL);
        DownloadHandler dlFile = new DownloadHandlerBuffer();
        dlClient.downloadHandler = dlFile;
        dlClient.SendWebRequest();
        do
        {
            //Nothing
        } while (dlClient.result.ToString() == "InProgress");

        ArrayList MasterMD5ArrayList = new System.Collections.ArrayList();
        String MasterCheckVal = "";
        switch (dlClient.result.ToString())
        {
            case "Success":

                File.WriteAllText(MD5ListDLPath, dlClient.downloadHandler.text);
                Debug.Log("Game Validation Manager: MD5List Download Success");

                string[] MasterMD5List = System.IO.File.ReadAllLines(@MD5ListDLPath);


                foreach (string DLListFileInfo in MasterMD5List)
                {
                    if (DLListFileInfo.Contains("####") == true)
                    {
                        // Ignore Comments
                    } else
                    {
                        MasterMD5ArrayList.Add(DLListFileInfo.Split(',')[0].ToLower());
                    }
                }
                MasterMD5ArrayList.Sort();
                MasterMD5ArrayList.Reverse();
                masterFileMD5sList = MasterMD5ArrayList;
                foreach (string masterFileMD5Value in MasterMD5ArrayList)
                {
                    MasterCheckVal = MasterCheckVal + masterFileMD5Value.Substring(masterFileMD5Value.Length - 6);
                }
                // Debug.Log("master MD5List Count: " + masterFileMD5sList.Count.ToString());
                break;
            default:
                Debug.Log("Game Validation Manager: MD5List Download Error");
                break;
        }
        return MasterCheckVal;
    }

    bool CompareMD5Lists()
    {
        bool AllMD5Match = true;
        try
        {
            int MD5ListIndex = 0;
            foreach (string MD5Item in masterFileMD5sList)
            {
                    // Debug.Log("COMPARE: " + MD5Item + " -> " + appFileMD5sList[MD5ListIndex].ToString());
                    if (MD5Item == appFileMD5sList[MD5ListIndex].ToString())
                    {
                        // Debug.Log(" - MATCH");
                    }
                    else
                    {
                        AllMD5Match = false;
                        // Debug.Log(" - NOT MATCH");
                    }
                MD5ListIndex += 1;
            }
        } catch
        {
            AllMD5Match = false;
        }
        return AllMD5Match;
    }

    static void GatherFiles(string appDir)
    {
        foreach (string checkFile in Directory.GetFiles(appDir))
        {
            appFilesList.Add(checkFile);

        }
        foreach (string checkDirectory in Directory.GetDirectories(appDir))
        {
            GatherFiles(checkDirectory);
        }
    }
 
    public void ValidationFailed()
    {
        if (ValidationIsRequiredToPlay == true)
        {
            Application.Quit();
        } else
        {
            // Load Next Scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}


