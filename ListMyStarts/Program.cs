using Octokit;
using System.Runtime.CompilerServices;
using System.Text;

// 传送了名字,否则默认
var username = "xiangsxuan2";
if (args is not null && args.Length != 0)
{
    username = args[0];
}

var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;
if (string.IsNullOrWhiteSpace(projectDir))
    throw new Exception("无法获取文件目录, 项目编译输出目录结构可能进行了调整");

var github = new GitHubClient(new ProductHeaderValue("ListMyStarts"));

var starTask = Task.Run((async () =>
{

    var starred = await github.Activity.Starring.GetAllForUser(username);
    var starsFile = Path.Combine(projectDir, "My Stars.md");
    GenetResporityMarkdownFile(username, starred, starsFile);
}));

var respoTask = Task.Run((async () =>
{

    var resp = await github.Repository.GetAllForUser(username);
    var RepositoriesFile = Path.Combine(projectDir, "My Repositories.md");
    GenetResporityMarkdownFile(username, resp, RepositoriesFile);

}));


// 等待两个任务都完成
await Task.WhenAll(starTask, respoTask);
Console.WriteLine("两个任务都已完成");

static void GenetResporityMarkdownFile(string username, IReadOnlyList<Repository> starred, string fileNameAndTitle)
{
    StringBuilder mdStr = new StringBuilder();
    mdStr.AppendLine().Append("# ").AppendLine(fileNameAndTitle);// 标题

    int idx = 1;
    foreach (var item in starred)
    {
        mdStr.Append($"{idx}. [").Append(item.Name).Append("](").Append(item.HtmlUrl).Append(")\t[").Append(item.Description);
        mdStr.Append(item.Description ?? "No description available.");
        mdStr.Append("]\n");
        idx++;
    }
    Console.WriteLine("文件内容:" + mdStr?.ToString() ?? "");
    File.WriteAllText(fileNameAndTitle, mdStr?.ToString() ?? ""
                    , new UTF8Encoding(false)); // UTF8默认包含Bom, 会在开头多个/FEFF标志,
                                                // 影响md解析, 构造传false取消这个标志
}

