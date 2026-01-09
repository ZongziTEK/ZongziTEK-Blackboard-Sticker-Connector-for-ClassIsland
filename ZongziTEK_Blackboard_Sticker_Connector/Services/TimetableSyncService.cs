using ClassIsland.Core.Abstractions.Services;
using System.ComponentModel;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Profile;
using Microsoft.Extensions.Hosting;
using ZongziTEK_Blackboard_Sticker_Connector.Helpers;
using ZongziTEK_Blackboard_Sticker_Connector.Models;

namespace ZongziTEK_Blackboard_Sticker_Connector.Services
{
    public class TimetableSyncService : IHostedService
    {
        private ILessonsService _lessonsService;
        private ClassPlan? _currentMonitoredClassPlan;

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

            SendCurrentTimetableToMyBaby();
        }

        private void OnClassesChanged(object? sender, EventArgs e)
        {
            ConsoleHelper.WriteLog($"发现课表内有课程变化，课表名称：{((ClassPlan)sender).Name}", "info");
            SendCurrentTimetableToMyBaby();
        }

        private void SendCurrentTimetableToMyBaby()
        {
            SendTimetableToMyBaby(TimetableHelper.GetCurrentTimetable());
        }

        private void SendTimetableToMyBaby(List<Timetable.Lesson> timetable)
        {
            ConsoleHelper.WriteLog("向黑板贴发送课表", "info");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lessonsService = IAppHost.GetService<ILessonsService>();

            _lessonsService.PropertyChanged += OnLessonsServicePropertyChanged;
            ConsoleHelper.WriteLog($"订阅课程服务属性变化事件", "info");

            _currentMonitoredClassPlan = _lessonsService.CurrentClassPlan;
            _currentMonitoredClassPlan.ClassesChanged += OnClassesChanged;
            ConsoleHelper.WriteLog($"订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

            SendCurrentTimetableToMyBaby();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _currentMonitoredClassPlan.ClassesChanged -= OnClassesChanged;
            ConsoleHelper.WriteLog($"取消订阅课表内课程变化事件，课表名称：{_currentMonitoredClassPlan.Name}", "info");

            _lessonsService.PropertyChanged -= OnLessonsServicePropertyChanged;
            ConsoleHelper.WriteLog($"取消订阅课程服务属性变化事件", "info");

            return Task.CompletedTask;
        }
    }
}
