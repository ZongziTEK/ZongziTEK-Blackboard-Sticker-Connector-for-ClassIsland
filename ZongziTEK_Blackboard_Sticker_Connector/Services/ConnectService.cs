using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Profile;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using dotnetCampus.Ipc.Context;
using ZongziTEK_Blackboard_Sticker;
using ZongziTEK_Blackboard_Sticker.Shared.IPC;
using ZongziTEK_Blackboard_Sticker_Connector.Helpers;
using ZongziTEK_Blackboard_Sticker_Connector.Models;

namespace ZongziTEK_Blackboard_Sticker_Connector.Services;

public class ConnectService : IHostedService, IConnectService
{
    #region Methods
    public Task<List<Lesson>> GetCurrentTimetable()
    {
        return Task.FromResult(_currentTimetable);
    }

    public Task<bool> GetIsTimetableSyncEnabled()
    {
        return Task.FromResult(_settings.IsTimetableSyncEnabled);
    }
    #endregion

    #region Public Values
    public bool IsFirstConnectionSucceed => _isFirstConnectionSucceed;
    #endregion

    #region Public Events
    public event ConnectedHandler Connected;
    #endregion

    private ILessonsService _lessonsService;
    private ClassPlan? _currentMonitoredClassPlan;
    private List<Lesson> _currentTimetable = new();
    private IpcProvider _ipcProvider;
    private JsonIpcDirectRoutedProvider _ipcDirectRoutedProvider;
    private JsonIpcDirectRoutedClientProxy _jsonIpcClient;
    private bool _isFirstConnectionSucceed;

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

        if (_currentMonitoredClassPlan == null)
        {
            ConsoleHelper.WriteLog("上次无课表，不取消订阅课表内课程变化事件", "warn");
        }
        else
        {
            _currentMonitoredClassPlan.ClassesChanged -= OnClassesChanged;
            ConsoleHelper.WriteLog($"取消订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");
        }

        _currentMonitoredClassPlan = _lessonsService.CurrentClassPlan;

        if (_currentMonitoredClassPlan == null)
        {
            ConsoleHelper.WriteLog("当前无课表，不订阅课表内课程变化事件", "warn");
        }
        else
        {
            _currentMonitoredClassPlan.ClassesChanged += OnClassesChanged;
            ConsoleHelper.WriteLog($"订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");
        }

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

    private void UpdateTimetableToMyBaby(List<Lesson> timetable)
    {
        _currentTimetable = timetable;

        if (!_settings.IsTimetableSyncEnabled)
        {
            ConsoleHelper.WriteLog("TimetableSync 未启用，不更新黑板贴课表", "info");
            return;
        }

        _jsonIpcClient.NotifyAsync("ZongziTEK_Blackboard_Sticker_Connector.TimetableUpdated");

        ConsoleHelper.WriteLog("更新黑板贴课表", "info");
    }

    private void NotifyIsTimetableSyncEnabledChanged()
    {
        _jsonIpcClient.NotifyAsync("ZongziTEK_Blackboard_Sticker_Connector.IsTimetableSyncEnabledChanged");
        ConsoleHelper.WriteLog("通知黑板贴 IsTimetableSyncEnabledChanged 已改变", "info");
    }

    private void NotifyServiceStarted()
    {
        _jsonIpcClient.NotifyAsync("ZongziTEK_Blackboard_Sticker_Connector.ServiceStarted");
        ConsoleHelper.WriteLog("通知黑板贴 ConnectService 启动完毕", "info");
    }

    private void NotifyServiceStopped()
    {
        _jsonIpcClient.NotifyAsync("ZongziTEK_Blackboard_Sticker_Connector.ServiceStopped");
        ConsoleHelper.WriteLog("通知黑板贴 ConnectService 停止", "info");
    }
    #endregion

    #region IPC Connect
    private async Task ConnectIpc()
    {
        var ipcProvider = new IpcProvider("ZongziTEK_Blackboard_Sticker_Connector", new IpcConfiguration { AutoReconnectPeers = true });
        var ipcDirectRoutedProvider = new JsonIpcDirectRoutedProvider(ipcProvider);

        ipcProvider.CreateIpcJoint<IConnectService>(this);
        ipcDirectRoutedProvider.StartServer();
        ConsoleHelper.WriteLog("启动 IPC 服务器", "info");

        _ipcProvider = ipcProvider;
        _ipcDirectRoutedProvider = ipcDirectRoutedProvider;

        ConsoleHelper.WriteLog("开始连接黑板贴", "info");
        _jsonIpcClient = await _ipcDirectRoutedProvider.GetAndConnectClientAsync("ZongziTEK_Blackboard_Sticker");
        _isFirstConnectionSucceed = true;
        Connected?.Invoke();
        ConsoleHelper.WriteLog("已连接到黑板贴", "info");
    }
    #endregion

    #region Start & End
    public ConnectService(Settings settings)
    {
        _settings = settings;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectIpc();

        _lessonsService = IAppHost.GetService<ILessonsService>();

        _lessonsService.PropertyChanged += OnLessonsServicePropertyChanged;
        ConsoleHelper.WriteLog("订阅课程服务属性变化事件", "info");

        _settings.PropertyChanged += OnSettingsPropertyChanged;

        _currentMonitoredClassPlan = _lessonsService.CurrentClassPlan;
        _currentMonitoredClassPlan.ClassesChanged += OnClassesChanged;
        ConsoleHelper.WriteLog($"订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

        UpdateCurrentTimetableToMyBaby();

        NotifyServiceStarted(); // 通知黑板贴服务本启动完毕，以实现黑板贴自动避让等功能
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _currentMonitoredClassPlan.ClassesChanged -= OnClassesChanged;
        ConsoleHelper.WriteLog($"取消订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

        _settings.PropertyChanged -= OnSettingsPropertyChanged;

        _lessonsService.PropertyChanged -= OnLessonsServicePropertyChanged;
        ConsoleHelper.WriteLog("取消订阅课程服务属性变化事件", "info");

        NotifyServiceStopped(); // 通知黑板贴本服务停止，以取消黑板贴的自动避让，或实现其它功能

        return Task.CompletedTask;
    }
    #endregion
}

public delegate void ConnectedHandler();
