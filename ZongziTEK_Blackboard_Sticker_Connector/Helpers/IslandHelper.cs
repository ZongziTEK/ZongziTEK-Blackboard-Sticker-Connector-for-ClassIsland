using ClassIsland.Core;
using System.Reflection;

namespace ZongziTEK_Blackboard_Sticker_Connector.Helpers
{
    public class IslandHelper
    {
        public static void ShowIsland()
        {
            dynamic app = AppBase.Current;
            app.Settings.IsMainWindowVisible = true;
        }

        public static void HideIsland()
        {
            dynamic app = AppBase.Current;
            app.Settings.IsMainWindowVisible = false;
        }

        public static double GetOccupiedHeight()
        {
            if (GetIslandPosition() < 3) // 岛在上面
            {
                return GetIslandTotalHeight() + GetIslandOffsetY();
            }
            else // 岛在下面
            {
                return GetIslandTotalHeight() - GetIslandOffsetY();
            }
        }

        public static double GetIslandOffsetY()
        {
            dynamic app = AppBase.Current;
            return app.Settings.WindowDockingOffsetY;
        }

        public static int GetIslandPosition()
        {
            dynamic app = AppBase.Current;
            return app.Settings.WindowDockingLocation; // 0 1 2 在上，3 4 5 在下
        }

        public static double GetIslandTotalHeight()
        {
            var app = AppBase.Current;
            var mainWindow = app.MainWindow as dynamic;

            ConsoleHelper.WriteLog("尝试使用反射获取 Island 总高度");

            if (mainWindow == null)
            {
                ConsoleHelper.WriteLog("未获取到 Island 总高度，原因：MainWindow == null");
                return 0;
            }

            FieldInfo fieldInfo = mainWindow.GetType().GetField(
                "RootLayoutTransformControl",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public
            );

            if (fieldInfo == null)
            {
                ConsoleHelper.WriteLog("未获取到 Island 总高度，原因：fieldInfo == null", "error");
                return 0;
            }

            object controlObj = fieldInfo.GetValue(mainWindow);

            if (controlObj == null)
            {
                ConsoleHelper.WriteLog("未获取到 Island 总高度，原因：controlObj == null", "error");
                return 0;
            }

            dynamic control = controlObj;

            try
            {
                var height = control.Bounds.Height;
                ConsoleHelper.WriteLog($"获取到 Island 总高度：{height}");
                return height;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLog("未获取到 Island 总高度，原因：", "error");
                Console.WriteLine(ex);
                Console.WriteLine("--- 错误信息末尾 ---");
                return 0;
            }
        }
    }
}
