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
        private readonly MidnightTimer _timer;
        public State State;

        public FilesTracker()
        {
            _path = ConfigurationManager.AppSettings["path_a"]!;
            if (_path is null) throw new NullReferenceException();

            _watcher = new FileSystemWatcher(_path);
            _watcher.Created += OnCreated;
            
            _timer = new MidnightTimer();
            _timer.DayChanged += OnMidnight;
        }
        
        public void Start()
        {
            FileProcessor.LoadMetaData();
            ProcessExistingFiles();
            _watcher.EnableRaisingEvents = true;
            _timer.Enabled = true;
            
            State = State.Working;
        }

        public void Stop()
        {
            FileProcessor.RenderMetaLog();
            _watcher.EnableRaisingEvents = false;
            _timer.Enabled = false;

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
            var files = dir.GetFiles().Select(x => x.FullName).ToArray();
            foreach (string path in files) ProcessFile(path);
        }

        private void OnMidnight(object source, EventArgs e)
        {
            FileProcessor.RenderMetaLog();
            FileProcessor.ResetMeta();
        }

        private void OnCreated(object source, FileSystemEventArgs e) => ProcessFile(e.FullPath);

        private void ProcessFile(string path)
        {
            string extension = path.Substring(path.LastIndexOf('.'));
            switch (extension)
            {
                case ".txt":
                case ".csv":
                    var processor = new FileProcessor(path);
                    processor.Run();
                    processor.MoveFile();
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