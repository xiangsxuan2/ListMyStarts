using Octokit;
using System.Runtime.CompilerServices;
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

// 生成文件内容
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

// 写入到源码目录的md文件
var path = GetFilePath();// 获取当前源码文件路径
var dir = Path.GetDirectoryName(Path.GetDirectoryName(path));
var mdfile = Path.Combine(dir, "My Starts.md");//跳转两层目录找到源代码里的md文件,这方法可能不通用
File.WriteAllText(mdfile, mdStr?.ToString() ?? ""
                , new UTF8Encoding(false)); // UTF8默认包含Bom, 会在开头多个/FEFF标志,
                                            // 影响md解析, 构造传false取消这个标志

Console.WriteLine(mdStr?.ToString() ?? "");

//调用源码路径
static string GetFilePath([CallerFilePath] string path = null)
{
    return path;
}