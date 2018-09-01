#addin nuget:?package=cake.iis&version=0.3.0
#addin nuget:?package=cake.hosts&version=1.1.0
#addin nuget:?package=cake.powershell&version=0.4.5

var target = Argument("target", "default");

var rootPath  = "./";
var srcPath   = rootPath + "1-src/";
var solution  = rootPath + "nuget.server.sln";

var website = new {
    host = "nuget-server.test",
    path = srcPath + "web.nuget.server",
    appPoolName = "nuget-server.test"
};

Task("clean")
    .Description("清理缓存")
    .Does(() =>
{
    CleanDirectories("./src/**/bin");
});


Task("restore")
    .Description("还原依赖")
    .Does(() =>
{
    NuGetRestore(solution);
});


Task("build")
    .Description("构建项目")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
{
    MSBuild(solution, new MSBuildSettings {
	Verbosity = Verbosity.Minimal
    });
});


Task("deploy")
    .Description("部署项目")
    .Does(() =>
{
    AddHostsRecord("127.0.0.1", website.host);
	
    DeleteSite(website.host);

    CreateWebsite(new WebsiteSettings()
    {
        Name = website.host,
        Binding = IISBindings.Http.SetHostName(website.host)
                                  .SetIpAddress("*")
                                  .SetPort(80),
        ServerAutoStart = true,
        PhysicalDirectory = website.path,
        ApplicationPool = new ApplicationPoolSettings()
        {
            Name = website.appPoolName,
            IdentityType = IdentityType.LocalSystem,
            MaxProcesses = 1,
            ManagedRuntimeVersion = "v4.0"
        }
    });
});


Task("open-browser")
    .Description("打开浏览器")
    .Does(() =>
{
    StartPowershellScript("Start-Process", args =>
    {
        args.Append("chrome.exe")
            .Append("'-incognito'")
            .Append(",'http://" + website.host + "/'");
    });
});


Task("default")
.IsDependentOn("build")
.IsDependentOn("deploy")
.IsDependentOn("open-browser");

RunTarget(target);
