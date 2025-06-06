﻿using System.Text.RegularExpressions;
using System.Xml;

namespace Rayer.SearchEngine.Lyric.Decrypter.Qrc;

public static partial class XmlUtils
{
    private static readonly Regex AmpRegex = GetAmpRegex();

    private static readonly Regex QuotRegex =
        GetQuotRegex();

    public static XmlDocument Create(string content)
    {
        content = RemoveIllegalContent(content);

        content = ReplaceAmp(content);

        var _content = ReplaceQuot(content);

        var doc = new XmlDocument();

        try
        {
            doc.LoadXml(_content);
        }
        catch
        {
            doc.LoadXml(content);
        }

        return doc;
    }

    private static string ReplaceAmp(string content)
    {
        // replace & symbol
        return AmpRegex.Replace(content, "&amp;");
    }

    private static string ReplaceQuot(string content)
    {
        var sb = new StringBuilder();

        var currentPos = 0;
        foreach (Match match in QuotRegex.Matches(content))
        {
            sb.Append(content[currentPos..match.Index]);

            var f = match.Result(match.Groups[1].Value + match.Groups[2].Value.Replace("\"", "&quot;")) + "\"";

            sb.Append(f);

            currentPos = match.Index + match.Length;
        }

        sb.Append(content[currentPos..]);

        return sb.ToString();
    }

    /// <summary>
    /// 移除 XML 内容中无效的部分
    /// </summary>
    /// <param name="content">原始 XML 内容</param>
    /// <returns>移除后的内容</returns>
    private static string RemoveIllegalContent(string content)
    {
        int left = 0, i = 0;
        while (i < content.Length)
        {
            if (content[i] == '<')
            {
                left = i;
            }

            // 闭区间
            if (i > 0 && content[i] == '>' && content[i - 1] == '/')
            {
                var part = content.Substring(left, i - left + 1);

                // 存在有且只有一个等号
                if (part.Contains('=') && part.IndexOf('=') == part.LastIndexOf('='))
                {
                    // 等号和左括号之间没有空格 <a="b" />
                    var part1 = content.Substring(left, part.IndexOf('='));
                    if (!part1.Trim().Contains(' '))
                    {
                        content = content[..left] + content[(i + 1)..];
                        i = 0;
                        continue;
                    }
                }
            }

            i++;
        }

        return content.Trim();
    }

    /// <summary>
    /// 递归查找 XML DOM
    /// </summary>
    /// <param name="xmlNode">根节点</param>
    /// <param name="mappingDict">节点名和结果名的映射</param>
    /// <param name="resDict">结果集</param>
    public static void RecursionFindElement(XmlNode xmlNode, Dictionary<string, string> mappingDict,
        Dictionary<string, XmlNode> resDict)
    {
        if (mappingDict.TryGetValue(xmlNode.Name, out var value))
        {
            resDict[value] = xmlNode;
        }

        if (!xmlNode.HasChildNodes)
        {
            return;
        }

        for (var i = 0; i < xmlNode.ChildNodes.Count; i++)
        {
            if (xmlNode.ChildNodes.Item(i) is XmlNode node)
            {
                RecursionFindElement(node, mappingDict, resDict);
            }
        }
    }

    [GeneratedRegex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)")]
    private static partial Regex GetAmpRegex();

    [GeneratedRegex("(\\s+[\\w:.-]+\\s*=\\s*\")(([^\"]*)((\")((?!\\s+[\\w:.-]+\\s*=\\s*\"|\\s*(?:/?|\\?)>))[^\"]*)*)\"")]
    private static partial Regex GetQuotRegex();
}
