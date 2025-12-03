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
Console.WriteLine("文件内容:"+mdStr?.ToString() ?? "");
// 写入到源码目录的md文件
// 获取当前源码文件路径, 这个方法不确定可靠,依赖于项目编译输出目录结构
var projectDir = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
var mdfile = Path.Combine(projectDir, "My Starts.md");
Console.WriteLine("文件位置:" + mdfile);
File.WriteAllText(mdfile, mdStr?.ToString() ?? ""
                , new UTF8Encoding(false)); // UTF8默认包含Bom, 会在开头多个/FEFF标志,
                                            // 影响md解析, 构造传false取消这个标志

Console.WriteLine(mdStr?.ToString() ?? "");
