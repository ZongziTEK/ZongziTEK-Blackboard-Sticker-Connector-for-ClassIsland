using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Profile;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using ZongziTEK_Blackboard_Sticker_Connector.Helpers;
using ZongziTEK_Blackboard_Sticker_Connector.IPC;
using ZongziTEK_Blackboard_Sticker_Connector.Models;

namespace ZongziTEK_Blackboard_Sticker_Connector.Services;

public class ConnectService : IHostedService, IConnectService
{
    #region Methods
    public List<Timetable.Lesson> GetCurrentTimetable()
    {
        return _currentTimetable;
    }

    public bool GetIsTimetableSyncEnabled()
    {
        return _settings.IsTimetableSyncEnabled;
    }
    #endregion

    private ILessonsService _lessonsService;
    private ClassPlan? _currentMonitoredClassPlan;
    private List<Timetable.Lesson> _currentTimetable = new();
    private readonly Settings _settings;

    #region Events
    private void OnLessonsServicePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ILessonsService.CurrentClassPlan))
        {
            OnCurrentClassPlanChanged();
        }
    }

    private void OnCurrentClassPlanChanged()
    {
        ConsoleHelper.WriteLog("发现当前课表变化", "info");

        _currentMonitoredClassPlan.ClassesChanged -= OnClassesChanged;
        ConsoleHelper.WriteLog($"取消订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

        _currentMonitoredClassPlan = _lessonsService.CurrentClassPlan;

        _currentMonitoredClassPlan.ClassesChanged += OnClassesChanged;
        ConsoleHelper.WriteLog($"订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

        UpdateCurrentTimetableToMyBaby();
    }

    private void OnClassesChanged(object? sender, EventArgs e)
    {
        ConsoleHelper.WriteLog($"发现课表内有课程变化，课表名称：{((ClassPlan)sender).Name}", "info");
        UpdateCurrentTimetableToMyBaby();
    }

    private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.IsTimetableSyncEnabled))
        {
            NotifyIsTimetableSyncEnabledChanged();
        }
    }
    #endregion

    #region Send to 宝宝
    private void UpdateCurrentTimetableToMyBaby()
    {
        UpdateTimetableToMyBaby(TimetableHelper.GetCurrentTimetable());
    }

    private void UpdateTimetableToMyBaby(List<Timetable.Lesson> timetable)
    {
        _currentTimetable = timetable;

        var ipcService = IAppHost.GetService<IIpcService>();
        ipcService.BroadcastNotificationAsync("ZongziTEK_Blackboard_Sticker_Connector.TimetableUpdated");

        ConsoleHelper.WriteLog("更新黑板贴课表", "info");
    }

    private void NotifyIsTimetableSyncEnabledChanged()
    {
        var ipcService = IAppHost.GetService<IIpcService>();
        ipcService.BroadcastNotificationAsync("ZongziTEK_Blackboard_Sticker_Connector.IsTimetableSyncEnabledChanged");

        ConsoleHelper.WriteLog("通知黑板贴 IsTimetableSyncEnabledChanged 已改变", "info");
    }
    #endregion

    #region Start & End
    public ConnectService(Settings settings)
    {
        _settings = settings;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lessonsService = IAppHost.GetService<ILessonsService>();

        _lessonsService.PropertyChanged += OnLessonsServicePropertyChanged;
        ConsoleHelper.WriteLog("订阅课程服务属性变化事件", "info");

        _settings.PropertyChanged += OnSettingsPropertyChanged;

        _currentMonitoredClassPlan = _lessonsService.CurrentClassPlan;
        _currentMonitoredClassPlan.ClassesChanged += OnClassesChanged;
        ConsoleHelper.WriteLog($"订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

        UpdateCurrentTimetableToMyBaby();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _currentMonitoredClassPlan.ClassesChanged -= OnClassesChanged;
        ConsoleHelper.WriteLog($"取消订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

        _settings.PropertyChanged -= OnSettingsPropertyChanged;

        _lessonsService.PropertyChanged -= OnLessonsServicePropertyChanged;
        ConsoleHelper.WriteLog("取消订阅课程服务属性变化事件", "info");

        return Task.CompletedTask;
    }
    #endregion
}

