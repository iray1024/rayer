using Microsoft.Extensions.DependencyInjection;
using Rayer.Installer.Abstractions;
using Rayer.Installer.Services;
using Rayer.Installer.ViewModels;
using System.IO;
using System.Windows;

namespace Rayer.Installer;

public partial class App : System.Windows.Application
{
    private readonly IServiceProvider _serviceProvider;

    public static IServiceProvider Services
    {
        get
        {
            return ((App)Current)._serviceProvider ?? throw new InvalidOperationException("尚未初始化服务提供程序");
        }
    }

    public App()
    {
        _serviceProvider = ConfigureServices();
    }

    private static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<IResourceExtractor, ResourceExtractor>()
            .AddSingleton<MainViewModel>()
            .BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!Directory.Exists(Constants.InstallerTempDir))
        {
            Directory.CreateDirectory(Constants.InstallerTempDir);
        }

        Initialize();

        base.OnStartup(e);
    }

    private static void Initialize()
    {
        var extractor = Services.GetRequiredService<IResourceExtractor>();

        using var copyright = extractor.GetResource("Rayer.Installer.assets.copyright.jpg");
        //using var partner = extractor.GetResource("Rayer.Installer.assets.partner.jpg");

        if (copyright is not null/* && partner is not null*/)
        {
            FileOperator.Save(copyright, Path.Combine(Constants.InstallerTempDir, "copyright.jpg"));
            //FileOperator.Save(partner, Path.Combine(Constants.InstallerTempDir, "partner.jpg"));
        }
    }
}