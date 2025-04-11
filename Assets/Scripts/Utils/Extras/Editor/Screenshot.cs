using System;
using UnityEditor;
using UnityEngine;


namespace Honeylab.Utils.Editor
{
    public class Screenshot
    {
        private void Shot()
        {
            string filename = $"screenshot_{DateTime.Now:yyyy-MM-dd HH-mm-ss}.png";
            ScreenCapture.CaptureScreenshot(filename);
        }


        [MenuItem("Tools/Honeylab/Screenshot")]
        private static void ShotMenuItem()
        {
            Screenshot screenshot = new Screenshot();
            screenshot.Shot();
        }
    }
}
