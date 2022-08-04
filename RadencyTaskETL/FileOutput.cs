using System.IO;
using Newtonsoft.Json;

namespace RadencyTaskETL
{
    public class FileOutput<T> where T : new()
    {
        private readonly string _path;
        private readonly JsonSerializerSettings _settings;

        public FileOutput(string path)
        {
            _path = path;
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        public void SaveData(T db)
        {
            using var writer = File.CreateText(_path);
            writer.Write(JsonConvert.SerializeObject(db, _settings));
        }
    }
}