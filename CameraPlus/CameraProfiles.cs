using IPA.Utilities;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CameraPlus
{
    public static class CameraProfiles
    {
        public static string pPath = Path.Combine(UnityGame.UserDataPath, "." + Plugin.Name.ToLower());
        public static string mPath = Path.Combine(UnityGame.UserDataPath, Plugin.Name);
        public static string currentlySelected = "None";

        public static void CreateMainDirectory()
        {
            DirectoryInfo di = Directory.CreateDirectory(pPath);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(Path.Combine(pPath, "Profiles"));
            var a = new DirectoryInfo(Path.Combine(pPath, "Profiles")).GetDirectories();
            if (a.Length > 0)
                currentlySelected = a.First().Name;
        }

        public static void SaveCurrent()
        {
            DirectoryCopy(mPath, Path.Combine(pPath, "Profiles", GetNextProfileName()), true);
        }

        public static void SetNext(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(Path.Combine(pPath, "Profiles")).GetDirectories();
            if (now == null)
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index < dis.Count() - 1)
                    currentlySelected = dis.ElementAtOrDefault(index + 1).Name;
                else
                    currentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
            }
        }

        public static void TrySetLast(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(Path.Combine(pPath, "Profiles")).GetDirectories();
            if (now == null)
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index == 0 && dis.Length >= 2)
                    currentlySelected = dis.ElementAtOrDefault(dis.Count() - 1).Name;
                else if (index < dis.Count() && dis.Length >= 2)
                    currentlySelected = dis.ElementAtOrDefault(index - 1).Name;
                else
                    currentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
            }
        }

        public static void DeleteProfile(string name)
        {
            if (Directory.Exists(Path.Combine(pPath, "Profiles", name)))
                Directory.Delete(Path.Combine(pPath, "Profiles", name), true);
        }

        public static string GetNextProfileName()
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(pPath, "Profiles"));
            DirectoryInfo[] dirs = dir.GetDirectories();
            int index = 1;
            string folName = "CameraPlusProfile";
            foreach (var dire in dirs)
            {
                folName = $"CameraPlusProfile{index.ToString()}";
                index++;
            }
            return folName;
        }

        public static void SetProfile(string name)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(pPath, "Profiles", name));
            if (!dir.Exists)
                return;
            DirectoryInfo di = new DirectoryInfo(mPath);
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            foreach (DirectoryInfo dim in di.GetDirectories())
                dim.Delete(true);

            DirectoryCopy(dir.FullName, mPath, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
                return;

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
    class ProfileChanger : MonoBehaviour
    {
        public void ProfileChange(String ProifileName)
        {
            var cs = Resources.FindObjectsOfTypeAll<CameraPlusBehaviour>();
            foreach (var c in cs)
                CameraUtilities.RemoveCamera(c);
            foreach (var csi in Plugin.Instance.Cameras.Values)
                Destroy(csi.Instance.gameObject);
            Plugin.Instance.Cameras.Clear();
            CameraProfiles.SetProfile(ProifileName);
            CameraUtilities.ReloadCameras();
        }
    }
}
