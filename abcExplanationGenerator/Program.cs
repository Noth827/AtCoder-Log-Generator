﻿using System.Text;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

Console.Write("1.記事テンプレートの生成 2.$$をtex形式(はてな準拠)に変換 : ");
var select = int.Parse(Console.ReadLine() ?? string.Empty);

switch (select)
{
    case 1:
        await GenerateTemplate();
        break;
    case 2:
        TexConverter.Convert();
        break;
}

async Task GenerateTemplate()
{
    Console.Write("書きたいABCの番号を記入してください : ");
    var abcNumber = int.Parse(Console.ReadLine() ?? string.Empty);
    Console.Write("何番まで書きますか？(a,b,c,d,e,f,g,h) : ");
    var problemLimit = Console.ReadLine()![0];

    #region Webスクレイピング

    // example: https://atcoder.jp/contests/abc302/tasks/abc302_a
    var problemSetUrl = $"https://atcoder.jp/contests/abc{abcNumber}/tasks/abc{abcNumber}_";
    var allProblems = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };
    var problemSetUrls = allProblems.Where((_, index) => index <= problemLimit - 'a').Select(x => problemSetUrl + x)
        .ToList();
    Console.WriteLine("以下のUrlから取得します : " + problemSetUrls[0]);

    IHtmlDocument doc;
    var content = "";
    var client = new HttpClient();

    foreach (var url in problemSetUrls)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            Console.WriteLine("不正なURLです: " + uri);
            continue;
        }

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var parser = new HtmlParser();
                doc = await parser.ParseDocumentAsync(stream);
                content += $"# [{doc.Title}]({url})\n \n" +
                           $"## 問題概要\n \n";
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Httpエラーが発生しました: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("エラーが発生しました: " + ex.Message);
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
    
    Dispose(client);

    #endregion

    void Dispose(IDisposable cli)
    {
        cli.Dispose();
    }
}