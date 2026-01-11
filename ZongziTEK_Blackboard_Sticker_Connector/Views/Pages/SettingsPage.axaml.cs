using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.IconControl;
using ClassIsland.Core.Enums.SettingsWindow;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZongziTEK_Blackboard_Sticker_Connector.Models;
using ZongziTEK_Blackboard_Sticker_Connector.Services;

namespace ZongziTEK_Blackboard_Sticker_Connector.Views.Pages;

[HidePageTitle]
[SettingsPageInfo(
        "zongzitek.blackboard_sticker_connector.settingspage",
        "黑板贴连接器",
        "\uF083",
        "\uF082",
        SettingsPageCategory.External
        )]

public partial class SettingsPage : SettingsPageBase
{
    public Settings Settings { get; set; }

    private ConnectService connectService;

    public SettingsPage(Settings settings)
    {
        InitializeComponent();

        Settings = settings;

        connectService = IAppHost.Host.Services.GetServices<IHostedService>().OfType<ConnectService>().First();

        if (connectService != null)
        {
            connectService.Connected += CheckConnectionStatus;
            CheckConnectionStatus();
        }
    }

    private void CheckConnectionStatus()
    {
        InfoBarNoConnection.IsVisible = !connectService.IsFirstConnectionSucceed;
    }
}