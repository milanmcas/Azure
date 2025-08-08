namespace AzureAppAccess
{
    public class MyConfig
    {
        public int PageSize { get; set; }
        public MyHost Host { get; set; }
    }

    public class MyHost
    {
        public Uri BaseUrl { get; set; }
        public string Password { get; set; }
    }
}
