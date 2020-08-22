using System;
using System.IO;
using UnityEngine;

namespace CameraPlus
{
    public class RootConfig
    {
        public string FilePath { get; }

        public bool ProfileSceneChange = false;
        public string MenuProfile = "";
        public string GameProfile = "";
        public string CurrentProfile = "";
        public int CameraQuadLayer = 0;

        public event Action<RootConfig> ConfigChangedEvent;

        private readonly FileSystemWatcher _configWatcher;
        private bool _saving;

        public RootConfig(string filePath)
        {
            FilePath = filePath;

            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            if (File.Exists(FilePath))
            {
                Load();
                var text = File.ReadAllText(FilePath);
            }
            Save();

            _configWatcher = new FileSystemWatcher(Path.GetDirectoryName(FilePath))
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(FilePath),
                EnableRaisingEvents = true
            };
            _configWatcher.Changed += ConfigWatcherOnChanged;
        }

        ~RootConfig()
        {
            _configWatcher.Changed -= ConfigWatcherOnChanged;
        }

        public void Save()
        {
            _saving = true;
            ConfigSerializer.SaveConfig(this, FilePath);
            _saving = false;
        }

        public void Load()
        {
            ConfigSerializer.LoadConfig(this, FilePath);
        }

        private void ConfigWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (_saving)
            {
                _saving = false;
                return;
            }

            Load();

            if (ConfigChangedEvent != null)
            {
                ConfigChangedEvent(this);
            }
        }
    }
}
