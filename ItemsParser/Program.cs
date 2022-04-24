#pragma warning disable CS8600
#pragma warning disable CS8602 

using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Web;

if (!Directory.Exists("cache"))
    Directory.CreateDirectory("cache");

if (args.Contains("--redownload") || args.Contains("-r") || !File.Exists("cache/index.html"))
{
    await Utils.DownloadFile("https://csgostash.com/", "cache/index.html");
}

List<Collection> collections = new();
var doc = new HtmlDocument();
doc.Load("cache/index.html");

var navbar = doc.DocumentNode.SelectSingleNode("//body/div[@class=\"container header\"]/div[@class=\"navbar-wrapper\"]/nav/div/div[@id=\"navbar-expandable\"]/ul");
var casesLinks = navbar.ChildNodes[13].SelectSingleNode("ul").ChildNodes;
var collectionsLinks = navbar.ChildNodes[15].SelectSingleNode("ul").ChildNodes;

foreach (var caseLink in casesLinks)
{
    if (caseLink.FirstChild is null || caseLink.FirstChild.Name != "a")
        continue;
    string href = caseLink.FirstChild.GetAttributeValue("href", "").Trim();
    if (!href.StartsWith("https://csgostash.com/case/"))
        continue;

    collections.Add(await LoadCollection(href));
}

foreach (var collectionLink in collectionsLinks)
{
    if (collectionLink.FirstChild is null || collectionLink.FirstChild.Name != "a")
        continue;
    string href = collectionLink.FirstChild.GetAttributeValue("href", "").Trim();

    collections.Add(await LoadCollection(href));
}

string json = JsonConvert.SerializeObject(collections, Formatting.Indented);
File.WriteAllText("SkinList.json", json);

static async Task<Collection> LoadCollection(string url)
{
    Collection collection = new();
    collection.CanBeStattrak = false;
    string collectionName = Utils.ReplaceInvalidChars(url.Split('/').Last());
    string collectionPath = $"cache/{collectionName}";

    Console.Write($"Loading {collectionName}...");
    Stopwatch sw = Stopwatch.StartNew();
    if (!Directory.Exists(collectionPath))
        Directory.CreateDirectory(collectionPath);

    if (!File.Exists($"{collectionPath}/index.html"))
        await Utils.DownloadFile(url, $"{collectionPath}/index.html");

    var doc = new HtmlDocument();
    doc.Load($"{collectionPath}/index.html");
    var collectionNameFull = doc.DocumentNode.SelectSingleNode("//body/div[2]/div[2]/div/div[2]/h1").InnerText;
    collection.Name = HttpUtility.HtmlDecode(collectionNameFull.Trim());

    var contentNodes = doc.DocumentNode.SelectSingleNode("(//body/div[@class=\"container main-content\"]/div[@class=\"row\"])[4]");
    foreach (var row in contentNodes.ChildNodes)
    {
        if (row.InnerHtml.Contains("Knives"))
            continue;
        else if (row.InnerHtml.Contains("<script>"))
            continue;
        else if (row.InnerHtml.Contains("https://csgostash.com/skin"))
        {
            string name = "";
            string rarity = "";

            foreach (var child in row.ChildNodes[1].ChildNodes)
            {
                if (child.Name == "#text")
                    continue;

                if (child.Name == "h3")
                {
                    name = child.InnerText;
                }
                else if (child.Name == "div" && child.GetAttributeValue("class", "") == "stattrak")
                {
                    collection.CanBeStattrak = true;
                }
                else if (child.Name == "a")
                {
                    string href = child.GetAttributeValue("href", "");
                    if (href.Contains("https://csgostash.com/skin-rarity/"))
                    {
                        rarity = href.Replace("https://csgostash.com/skin-rarity/", "");
                    }
                    else if (href.Contains("https://csgostash.com/skin/"))
                    {
                        collection.Skins.Add(await LoadSkin(href, collectionPath, name, rarity));
                    }
                }
            }
        }
    }

    collection.HighestRarity = collection.Skins.First().Rarity;
    collection.LowestRarity = collection.Skins.Last().Rarity;

    sw.Stop();
    Console.WriteLine($" Done in {sw.Elapsed.TotalSeconds.ToString("0.00", CultureInfo.InvariantCulture)}s");
    return collection;
}

static async Task<Skin> LoadSkin(string url, string path, string name, string rarity)
{
    Skin skin = new()
    {
        Name = name,
        Rarity = rarity.Replace("+Grade", "")
    };

    string skinFileName = Utils.ReplaceInvalidChars(url.Split('/').Last());
    string skinDirPath = $"{path}/items";
    string skinFilePath = $"{skinDirPath}/{skinFileName}.html";

    if (!Directory.Exists(skinDirPath))
        Directory.CreateDirectory(skinDirPath);

    if (!File.Exists(skinFilePath))
        await Utils.DownloadFile(url, skinFilePath);

    var doc = new HtmlDocument();
    doc.Load(skinFilePath);

    var containerNode = doc.DocumentNode.SelectSingleNode(
        "//body/div[@class=\"container main-content\"]/div[@class=\"row text-center\"]" +
        "/div[@class=\"col-md-10\"]/div/div[@class=\"col-md-5 col-widen text-center\"]" +
        "/div[@class=\"well text-left wear-well\"]/div/div/div"
    );

    var minWearNode = containerNode.ChildNodes[1].InnerText.Trim();
    var maxWearNode = containerNode.ChildNodes[3].InnerText.Trim();

    skin.MinWear = float.Parse(minWearNode, CultureInfo.InvariantCulture);
    skin.MaxWear = float.Parse(maxWearNode, CultureInfo.InvariantCulture);

    return skin;
}
