using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using Microsoft.Extensions.Hosting;
using ZongziTEK_Blackboard_Sticker_Connector.Helpers;

namespace ZongziTEK_Blackboard_Sticker_Connector.Services;

public class IslandService : IHostedService
{
    public event IslandTerritoryChangedHandler IslandTerritoryChanged;

    private IComponentsService _componentsService;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ConsoleHelper.WriteLog("岛管服务开始启动");
        _ = Task.Run(() =>
        {
            var app = AppBase.Current;
            while (app.MainWindow == null) ;
            app.MainWindow.Loaded += MainWindow_Loaded;
        });

        var settings = ((object)AppBase.Current).GetType().GetProperty("Settings")?.GetValue(AppBase.Current);
        if (settings is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += Settings_PropertyChanged;
        }

        _componentsService = IAppHost.GetService<IComponentsService>();
        _componentsService.CurrentComponents.Lines.CollectionChanged += Lines_CollectionChanged;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        AppBase.Current.MainWindow.Loaded -= MainWindow_Loaded;

        var settings = ((object)AppBase.Current).GetType().GetProperty("Settings")?.GetValue(AppBase.Current);
        if (settings is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged -= Settings_PropertyChanged;
        }

        _componentsService.CurrentComponents.Lines.CollectionChanged -= Lines_CollectionChanged;

        return Task.CompletedTask;
    }

    private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        ConsoleHelper.WriteLog("IslandService 发现 ClassIsland 主界面加载完毕");
    }

    private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ConsoleHelper.WriteLog("IslandService 发现 ClassIsland 设置发生变化");

        IslandTerritoryChanged?.Invoke();
    }

    private void Lines_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ConsoleHelper.WriteLog("IslandService 发现 ClassIsland 群岛行数发生变化");

        IslandTerritoryChanged?.Invoke();
    }
}


public delegate void IslandTerritoryChangedHandler();
