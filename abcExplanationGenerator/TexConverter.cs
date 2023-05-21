using System.Diagnostics;
using System.Text.RegularExpressions;

partial class TexConverter
{
    public static void Convert()
    {
        const string inputFilePath = "../../../Inputs/";

        Console.Write("元となるファイル名を記入して下さい : ");
        var source = Console.ReadLine();
        var outputFileName = "output-" + source;

        try
        {
            var inputText = File.ReadAllText(inputFilePath + source);
            var convertedText = ConvertTexStrings(inputText);

            File.WriteAllText(outputFileName, convertedText);

            Console.WriteLine($"{outputFileName}\n" +
                              $"変換が完了しました。");
        }
        catch (Exception ex)
        {
            Console.WriteLine("エラーが発生しました: " + ex.Message);
        }
    }

    /// <summary>
    /// $で囲まれた文字列を[tex: ]で囲まれた文字列に変換する
    /// </summary>
    /// <param name="inputText"></param>
    /// <returns></returns>
    private static string ConvertTexStrings(string inputText)
    {
        var convertedText = inputText;

        // 正規表現を使用して$で囲まれた文字列を検索し、[tex: ]で囲む
        convertedText = MyRegex().Replace(convertedText, "[tex: $1]");

        return convertedText;
    }

    [GeneratedRegex("\\$(.*?)\\$")]
    private static partial Regex MyRegex();
}