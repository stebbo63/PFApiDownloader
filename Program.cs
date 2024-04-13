namespace pfAPIDownloader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var downloader = new pfAPIDownloader();
            downloader.Start();
        }
    }
}