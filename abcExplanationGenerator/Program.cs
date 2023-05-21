using System.Text;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

Console.Write("書きたいABCの番号を記入してください : ");
var abcNumber = int.Parse(Console.ReadLine() ?? string.Empty);
Console.Write("何番まで書きますか？(a,b,c,d,e,f,g,h) : ");
var problemLimit = Console.ReadLine()[0];

#region Webスクレイピング

// example: https://atcoder.jp/contests/abc302/tasks/abc302_a
var problemSetUrl = $"https://atcoder.jp/contests/abc{abcNumber}/tasks/abc{abcNumber}_";
var allProblems = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };
var problemSetUrls = allProblems.Where((_, index) => index <= problemLimit - 'a').Select(x => problemSetUrl + x)
    .ToList();
Console.WriteLine("以下のUrlから取得します : " + problemSetUrl);

IHtmlDocument doc;
var content = "";
foreach (var url in problemSetUrls)
{
    using (var client = new HttpClient())
    await using (var stream = await client.GetStreamAsync(new Uri(url)))
    {
        var parser = new HtmlParser();
        doc = await parser.ParseDocumentAsync(stream);
        content += $"# [{doc.Title}]({url})\n \n" +
                   $"## 問題概要\n \n";
    }
}

#endregion

#region 記事の生成

var fileName = $"./ABC{abcNumber}の解説・反省.md";
var fullPath = Path.GetFullPath(fileName);
Console.WriteLine(fullPath + "\nファイルを生成します...");
await using (var fs = File.Create(fileName))
{
}


Console.WriteLine("以下の内容のファイルが生成されます : \n" + content);

var encoding = Encoding.GetEncoding("utf-8");
await using (var writer = new StreamWriter(fileName, true, encoding))
{
    writer.WriteLine(content);
}

Console.WriteLine("ファイルの生成が完了しました");

#endregion