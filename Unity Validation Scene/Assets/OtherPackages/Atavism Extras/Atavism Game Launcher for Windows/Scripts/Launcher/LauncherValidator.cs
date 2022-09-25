using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using GameLauncher.Threading;

namespace GameLauncher.Launcher
{
    public class LauncherValidator : MonoBehaviour
    {
        /*
        * NOTE:
        *   The Sort() and Reverse() on the list may cause a performance impact
        *   consider using a Dictionary<> instead
        */

        private const string VALIDATION_RESOURCES_PATH = "ValidationResources";

        private const string TOKEN = "FileHashesList.launchervallst";
        private const string STATUS_NONE = "NONE";
        private const string STATUS_SUCCESS = "SUCCESS";
        private const string STATUS_ERROR = "ERROR";
        private const string STATUS_FAILED = "FAILED";

        [SerializeField] private string launcherValidationListURL = "https://someplace.com/FileHashesList.launchervallst";
        [SerializeField] private GameObject notificationMessageWindow;
        [SerializeField] private GameObject WorkingStatusObject;
        [SerializeField] private TMPro.TMP_Text errorMessageText;
        [SerializeField] private bool validationRequired = true;

        private string currentApplicationPath;

        private void Start()
        {
            LogUtil.SetTag("Game File Hash Validation");

            currentApplicationPath = Directory.GetParent(Path.GetFullPath(Application.dataPath)).FullName;
            LogUtil.D("Game Installation Path: " + currentApplicationPath);
            
            if (Application.isEditor)
            {
                LogUtil.W("Running in editor, skipping launcher + game file validation.");
                WorkingStatusObject.GetComponent<LoadingIcon>().working = false;
                notificationMessageWindow.SetActive(false);
                // Load Next Scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                if (!validationRequired)
                {
                    LogUtil.D("Validation is not set to required, playing will be allowed regardless of validation outcome!");
                }
                
                BeginValidation(status => {
                    WorkingStatusObject.GetComponent<LoadingIcon>().working = false;
                    switch (status)
                    {
                        case STATUS_SUCCESS:
                            LogUtil.D("Game file validation passed, loading next scene.");
                            notificationMessageWindow.SetActive(false);

                            // Load Next Scene
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                            break;
                        case STATUS_NONE:
                            // Display Error
                            if (validationRequired)
                            {
                                LogUtil.E("Game was not launched from the launcher, presenting closure dialog with exit action.");
                                errorMessageText.text = "The game must be launched from the launcher.";
                            }
                            else
                            {
                                errorMessageText.text = "Game was not launched from the launcher.  You will need to update or repair from the launcher to receive the latest update.";
                            }
                            notificationMessageWindow.SetActive(true);
                            break;
                        default:
                            if (validationRequired)
                            {
                                LogUtil.E("Game file validation failed, presenting closure dialog with exit action.");
                                errorMessageText.text = "Error with game validation.  Please update or repair the game from the launcher.";
                            } else
                            {
                                errorMessageText.text = "Game validation has failed.  You will need to update or repair from the launcher to receive the latest update.";
                            }
                            notificationMessageWindow.SetActive(true);
                            break;
                    }
                });
            }
        }

        private void BeginValidation(System.Action<string> callback)
        {
            IEnumerator exec()
            {
                string launchVal = GetCommandLineArgs();
                if (string.IsNullOrEmpty(launchVal))
                {
                    callback?.Invoke(STATUS_NONE);
                    yield break;
                }

                string masterCheckVal = string.Empty;
                string launcherCheckVal = string.Empty;

                List<string> masterMD5List = new List<string>();
                List<string> appMD5List = new List<string>();

                ThreadWorker masterMD5ValWorker = new ThreadWorker(() => {
                    masterCheckVal = ComputeMasterCheckVal(masterMD5List);
                    LogUtil.D("Finished fetching the master MD5 hashes.");
                });

                ThreadWorker launcherMD5ValWorker = new ThreadWorker(() => {
                    launcherCheckVal = ComputeLauncherCheckVal(appMD5List);
                    LogUtil.D("Finished collecting local file hashes.");
                });

                new Thread(masterMD5ValWorker.Run).Start();
                new Thread(launcherMD5ValWorker.Run).Start();

                yield return masterMD5ValWorker.WaitForCompletion();
                yield return launcherMD5ValWorker.WaitForCompletion();

                // LogUtil.D(masterCheckVal);
                // LogUtil.D(launcherCheckVal);

                bool matched = false;
                ThreadWorker comparatorWorker = new ThreadWorker(() => {
                    matched = CompareMD5Lists(masterMD5List, appMD5List);
                });

                new Thread(comparatorWorker.Run).Start();
                yield return comparatorWorker.WaitForCompletion();

                if (launchVal == launcherCheckVal && launcherCheckVal == masterCheckVal && matched)
                {
                    callback?.Invoke(STATUS_SUCCESS);
                }
                else
                {
                    callback?.Invoke(STATUS_FAILED);
                }
            }

            StartCoroutine(exec());
        }

        private string GetCommandLineArgs()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-LaunchVal")
                {
                    return args[i + 1];
                }
            }

            return string.Empty;
        }

        private string ComputeMasterCheckVal(List<string> MD5List)
        {
            // Make sure value in LauncherValidationListURL does include the file name
            string url = EnsureURLWellFormed(launcherValidationListURL);
            
            string MD5DirPath = Path.Combine(Application.persistentDataPath, VALIDATION_RESOURCES_PATH);

            // Replace forward slash with backslash
            MD5DirPath = MD5DirPath.Replace("/", "\\");
            string MD5ListPath = Path.Combine(MD5DirPath, TOKEN);
            
            if (!Directory.Exists(MD5DirPath))
                Directory.CreateDirectory(MD5DirPath);

            UnityWebRequest dlRequest = UnityWebRequest.Get(url);
            dlRequest.downloadHandler = new DownloadHandlerBuffer();
            dlRequest.SendWebRequest();

            LogUtil.D("Fetching the master MD5 hashes from the server.");
            do {
                // Wait for download to complete
            } while(dlRequest.result == UnityWebRequest.Result.InProgress);

            string masterCheckVal = string.Empty;
            switch (dlRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    File.WriteAllText(MD5ListPath, dlRequest.downloadHandler.text);
                    LogUtil.D("MD5List Download Success");

                    string[] masterMd5List = File.ReadAllLines(MD5ListPath);
                    foreach (string fileINfo in masterMd5List)
                    {
                        if (!fileINfo.Contains("####"))
                            MD5List.Add(fileINfo.Split(',')[0].ToLower());
                    }
                    // Correctly Order them
                    MD5List.Sort();
                    MD5List.Reverse();
                    // Grab the last 6 characters of each
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (string md5Value in MD5List)
                    {
                        sb.Append(md5Value.Substring(md5Value.Length - 6));
                    }
                    //Output final validationcheck value
                    masterCheckVal = sb.ToString();

                    break;
                default:
                    LogUtil.E("MD5List Download Error");
                    break;
            }

            return masterCheckVal;
        }

        private string ComputeLauncherCheckVal(List<string> MD5List)
        {
            List<string> files = new List<string>(); 
            SearchFilesRecurse(currentApplicationPath, files);

            foreach (string appFile in files)
            {
                string relPath = appFile.Replace(currentApplicationPath, string.Empty);
                if (!relPath.Contains(TOKEN))
                {
                    using (FileStream fileStream = File.OpenRead(appFile))
                    {
                        using (MD5 md5 = MD5.Create())
                        {
                            string hash = System.BitConverter.ToString(md5.ComputeHash(fileStream)).Replace("-", string.Empty).ToLower();
                            MD5List.Add(hash);
                            // MD5PathList.Add(string.Format("{0},{1}", hash, relPath));
                        }
                    }
                }
            }

            MD5List.Sort();
            MD5List.Reverse();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string md5 in MD5List)
            {
                sb.Append(md5.Substring(md5.Length - 6));
            }
            
            return sb.ToString();
        }

        private bool CompareMD5Lists(List<string> masterList, List<string> localList)
        {
            bool matched = true;
            int index = 0;
            foreach (string md5 in masterList)
            {
                LogUtil.D(string.Format("Comparing {0} with {1}", md5, localList[index]));
                if (!md5.Equals(localList[index], System.StringComparison.OrdinalIgnoreCase))
                {
                    // If one MD5 is not match, break the loops
                    LogUtil.D("Mismatched MD5 hash");
                    matched = false;
                    break;
                }

                index++;
            }

            LogUtil.D("All MD5 hashes are match");
            return matched;
        }

        private void SearchFilesRecurse(string path, IList<string> collection)
        {
            foreach (string file in Directory.GetFiles(path))
                collection.Add(file);
            
            foreach (string dir in Directory.GetDirectories(path))
                SearchFilesRecurse(dir, collection);
        }
    
        private string EnsureURLWellFormed(string url)
        {
            if (!url.EndsWith(string.Format("/{0}", TOKEN)))
            {
                url.TrimEnd(new []{',', '.', '/', '\\'});
                return string.Format("{0}/{1}", url, TOKEN);
            }

            return url;
        }

        private static void RunOnUIThread(System.Action action)
        {
            action?.Invoke();
        }

        public void ValidationFailed()
        {
            if (validationRequired == true)
            {
                Application.Quit();
            } 
            else
            {
                // Load Next Scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}

