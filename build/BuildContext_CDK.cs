using Cake.Core;
using Cake.Common;
using Cake.Core.IO;
using Cake.Powershell;
using Cake.Common.Diagnostics;
using CodingChallenge.CakeBuild.Models;
using System.ComponentModel;

namespace CodingChallenge.CakeBuild;
public partial class BuildContext
{

    public string GetCdkParamOverrides()
    {
        var retString = "";

        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this.Config.AwsApplication))
        {
            string name = descriptor!.Name!;
            object value = descriptor.GetValue(Config.AwsApplication)!;
            retString += $"-c \"{name}={value}\" ";
        }
        return retString;
    }
    public void DeployStack(ProjectSettings projectSetting, string stackname)
    {
        var stackFullPath = System.IO.Path.Combine(this.Config.StandardFolders.RootFullPath, $"src\\{projectSetting.ProjectName}\\bin\\{Config.DotnetSettings.DotnetConfiguration}\\{Config.DotnetSettings.DotnetFramework}\\{projectSetting.ProjectName}.dll");
        this.Information($"Stack Full Path is {stackFullPath}");
        var cdkAppPath = $"dotnet {stackFullPath}";
        _context.Information($"cdk app path is {cdkAppPath}");

        var parameterOverrides = GetCdkParamOverrides()!;
        _context.Information($"param over {parameterOverrides} ... stackname : {stackname}");

        var arguments = new ProcessArgumentBuilder()
                    .Append("deploy")
                    .Append("--require-approval=never")
                    .Append("--verbose")
                    //.Append($"-o ../.cdk")
                    .Append($"-o {this.Config.StandardFolders.CdkDirFullPath}")
                    .Append($"--app \"{cdkAppPath}\"")
                    .Append($"{stackname}")
                    .Append($"{parameterOverrides}");
        _ =
        _context.StartProcess(
            "cdk",
            new ProcessSettings
            {
                Arguments = arguments,
                RedirectStandardOutput = true
            },
            out _
        );

        _context.Information($"output -- cdk {arguments.Render()}");
    }




    public void DockerXBuild(string tag, string folderPath, string? dockerFilePath = null,string? platform=null)
    {

        var arguments = new ProcessArgumentBuilder()
                    .Append("buildx build")
                    .Append($"-t {tag}")
                    .Append($"-t {tag}:latest")
                    .Append($"-t {tag}:{this.Config.AwsApplication.Version}");

        if (!string.IsNullOrWhiteSpace(dockerFilePath))
        {
            arguments.Append($"-f {dockerFilePath}");
        }
        if (!string.IsNullOrWhiteSpace(platform))
        {
            arguments.Append($"--platform={platform}");
        }
        arguments.Append($"--load");
        arguments.Append($"{folderPath}");
        _context.Information($"arguments -- docker {arguments.Render()}");

        _context.StartProcess(
             "docker",
             new ProcessSettings
             {
                 Arguments = arguments,
                 RedirectStandardOutput = true
             },
             out _
         );


        _context.Information($"output -- docker {arguments.Render()}");
    }
}
