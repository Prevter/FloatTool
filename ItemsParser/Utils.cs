static class Utils
{
    static readonly HttpClient Client = new();

    public static async Task DownloadFile(string url, string filename)
    {
        using var s = await Client.GetStreamAsync(url);
        using var fs = new FileStream(filename, FileMode.OpenOrCreate);
        await s.CopyToAsync(fs);
    }

    public static string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
}
