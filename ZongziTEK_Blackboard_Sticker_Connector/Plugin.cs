using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
using Iced.Intel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZongziTEK_Blackboard_Sticker_Connector.Helpers;
using ZongziTEK_Blackboard_Sticker_Connector.Models;
using ZongziTEK_Blackboard_Sticker_Connector.Services;
using ZongziTEK_Blackboard_Sticker_Connector.Views.Pages;

namespace ZongziTEK_Blackboard_Sticker_Connector
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        public Settings Settings { get; set; } = new();

        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            ConsoleHelper.WriteZongziTEK();

            // Load and save config
            ConsoleHelper.WriteLog("开始加载配置文件", "info");
            Settings = ConfigureFileHelper.LoadConfig<Settings>(Path.Combine(PluginConfigFolder, "Settings.json"));  // 加载配置文件
            ConsoleHelper.WriteLog("成功加载配置文件", "info");
            Settings.PropertyChanged += (sender, args) =>
            {
                ConfigureFileHelper.SaveConfig(Path.Combine(PluginConfigFolder, "Settings.json"), Settings);  // 保存配置文件
                ConsoleHelper.WriteLog("保存配置文件", "info");
            };

            // Add settings pages
            services.AddSettingsPage<SettingsPage>();

            // Add services
            services.AddHostedService<TimetableSyncService>();
            ConsoleHelper.WriteLog("注册课程表同步服务", "info");
        }
    }
}