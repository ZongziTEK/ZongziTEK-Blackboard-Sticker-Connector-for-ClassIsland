using ClassIsland.Core;

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
    }
}
