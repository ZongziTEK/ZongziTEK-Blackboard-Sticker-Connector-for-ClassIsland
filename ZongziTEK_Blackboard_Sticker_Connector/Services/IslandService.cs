using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using ClassIsland.Core;
using Microsoft.Extensions.Hosting;
using ZongziTEK_Blackboard_Sticker_Connector.Helpers;

namespace ZongziTEK_Blackboard_Sticker_Connector.Services
{
    public class IslandService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConsoleHelper.WriteLog("岛管服务开始启动");
            _ = Task.Run(() =>
            {
                var app = AppBase.Current;
                while (app.MainWindow == null) ;
                app.MainWindow.Loaded += MainWindow_Loaded;
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            ConsoleHelper.WriteLog("IslandService 发现 ClassIsland 主界面加载完毕");
        }
    }
}
