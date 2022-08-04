using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace RadencyTaskETL
{
    public class FilesTracker
    {
        private readonly string _path;
        private readonly FileSystemWatcher _watcher;
        public State State;

        public FilesTracker()
        {
            _path = ConfigurationManager.AppSettings["path_a"]!;
            if (_path is null) throw new NullReferenceException();

            _watcher = new FileSystemWatcher(_path);
            _watcher.Created += OnCreated;
        }
        
        public void Start()
        {
            FileProcessor.LoadMetaData();
            ProcessExistingFiles();
            _watcher.EnableRaisingEvents = true;

            State = State.Working;
        }

        public void Stop()
        {
            FileProcessor.RenderMetaLog();
            _watcher.EnableRaisingEvents = false;

            State = State.Stopped;
        }

        public void Reset()
        {
            Stop();
            Start();
        }

        private void ProcessExistingFiles()
        {
            var dir = new DirectoryInfo(_path);
            foreach (string path in dir.GetFiles().Select(x => x.FullName)) ProcessFile(path);
        }

        private void OnCreated(object source, FileSystemEventArgs e) => ProcessFile(e.FullPath);

        private void ProcessFile(string path)
        {
            string extension = path.Substring(path.LastIndexOf('.'));
            switch (extension)
            {
                case ".txt":
                case ".csv":
                    new FileProcessor(path).Run();
                    break;
                default:
                    return;
            }
        }
    }

    public enum State
    {
        Working,
        Stopped
    }
}