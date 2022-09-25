using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLauncher.Launcher
{
    internal static class LogUtil
    {
        public static string LogTag { get; private set; } = "Game Hash Validation";

        public static void SetTag(string tag) => LogTag = tag;

        public static void D(object msg) => D(LogTag, msg);
        public static void W(object msg) => W(LogTag, msg);
        public static void E(object msg) => E(LogTag, msg);

        public static void D(string tag, object msg) => Debug.Log(string.Format("{0}: {1}", tag, msg.ToString()));
        public static void W(string tag, object msg) => Debug.LogWarning(string.Format("{0}: {1}", tag, msg.ToString()));
        public static void E(string tag, object msg) => Debug.LogError(string.Format("{0}: {1}", tag, msg.ToString()));
    }
}
