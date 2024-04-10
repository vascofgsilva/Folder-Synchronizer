namespace FolderSynchronization
{
    internal class Logger
    {
        public string Path { get; set; }

        public Logger(string path)
        {
            if (!File.Exists(path)) File.Create(path);
            Path = path;
        }

        public void Log(string message) 
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            using (var streamWriter = new StreamWriter(Path, true))
            {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
        }
    }
}
