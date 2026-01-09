using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
using Iced.Intel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZongziTEK_Blackboard_Sticker_Connector.Models;
using ZongziTEK_Blackboard_Sticker_Connector.Views.Pages;

namespace ZongziTEK_Blackboard_Sticker_Connector
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        public Settings Settings { get; set; } = new();

        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            Settings = ConfigureFileHelper.LoadConfig<Settings>(Path.Combine(PluginConfigFolder, "Settings.json"));  // 加载配置文件
            Settings.PropertyChanged += (sender, args) =>
            {
                ConfigureFileHelper.SaveConfig(Path.Combine(PluginConfigFolder, "Settings.json"), Settings);  // 保存配置文件
            };

            services.AddSettingsPage<SettingsPage>();
        }
    }
}