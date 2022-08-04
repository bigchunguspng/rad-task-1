namespace RadencyTaskETL
{
    public static class Program
    {
        private static void Main()
        {
            var tracker = new FilesTracker();
            tracker.Run();
        }
    }
}