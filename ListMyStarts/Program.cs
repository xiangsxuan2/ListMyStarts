using Octokit;
using System.Runtime.CompilerServices;
using System.Text;


try
{
    // 主程序逻辑
    await MainAsync(args);
}
catch (Exception ex)
{
    // 记录异常信息
    var logMessage = $"[{DateTime.Now}] 程序异常: {ex}";

    // 写入文件
    File.AppendAllText("crash.log", logMessage + "\n\n");

    // 用户友好的错误信息
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("程序运行出错！");
    Console.WriteLine($"错误: {ex.Message}");
    Console.ResetColor();

    Environment.ExitCode = 1; // 设置非零退出码
}
finally
{
    // 确保日志文件被刷新
    await Task.Delay(100);
}

async Task MainAsync(string[] args)
{

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

    // 获取用户收藏的仓库列表
    var starTask = Task.Run((async () =>
    {

        var starred = await github.Activity.Starring.GetAllForUser(username);
        var starsFile = Path.Combine(projectDir, "My Stars.md");
        GenetResporityMarkdownFile(username, starred, starsFile);
    }));

    // 获取用户创建的仓库列表
    var respoTask = Task.Run((async () =>
    {

        var resp = await github.Repository.GetAllForUser(username);
        var RepositoriesFile = Path.Combine(projectDir, "My Repositories.md");
        GenetResporityMarkdownFile(username, resp, RepositoriesFile);

    }));

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

}