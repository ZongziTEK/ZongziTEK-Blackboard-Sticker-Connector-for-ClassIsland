using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.IconControl;
using ClassIsland.Core.Enums.SettingsWindow;

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
    public Plugin Plugin { get; }

    public SettingsPage(Plugin plugin)
    {
        InitializeComponent();

        Plugin = plugin;
    }
}