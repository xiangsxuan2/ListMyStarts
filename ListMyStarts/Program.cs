using Octokit;
using System.Text;

// 传送了名字,否则默认
var username = "xiangsxuan2025";
if (args is not null && args.Length != 0)
{
    username = args[0];
}

// 获取用户star列表
var github = new GitHubClient(new ProductHeaderValue("ListMyStarts"));
var user = await github.User.Get(username);
var starred = await github.Activity.Starring.GetAllForUser(username);

// 生成md文件
StringBuilder mdStr = new StringBuilder();
int idx = 1;
var title = $"{username}'s Starts";
mdStr.AppendLine().Append("# ").AppendLine(title);
foreach (var item in starred)
{
    mdStr.Append($"{idx}. [").Append(item.Name).Append("](").Append(item.HtmlUrl).Append(")\t[").Append(item.Description);
    mdStr.Append(item.Description ?? "No description available.");
    mdStr.Append("]\n");
    idx++;
}
File.WriteAllText($"{title}.md", mdStr.ToString()
                , new UTF8Encoding(false)); // UTF8默认包含Bom, 会在开头多个/FEFF标志,
                                            // 影响md解析, 构造传false取消这个标志

Console.WriteLine(mdStr);