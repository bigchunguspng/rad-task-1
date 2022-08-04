using System.Configuration;
using System.IO;
using System.Linq;

namespace RadencyTaskETL
{
    public class FilesTracker
    {
        private readonly string _path = ConfigurationManager.AppSettings["path_a"]!;
        private FileSystemWatcher _watcher;

        public void Run()
        {
            DoFiles();
            
            _watcher = new FileSystemWatcher(_path);
            _watcher.Created += OnCreated;
            _watcher.EnableRaisingEvents = true;
        }

        private void DoFiles()
        {
            var dir = new DirectoryInfo(_path);
            foreach (string path in dir.GetFiles().Select(x => x.FullName)) DoFile(path);
        }

        private void OnCreated(object source, FileSystemEventArgs e) => DoFile(e.FullPath);

        private void DoFile(string path)
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
}