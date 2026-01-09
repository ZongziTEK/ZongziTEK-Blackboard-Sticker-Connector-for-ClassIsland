using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZongziTEK_Blackboard_Sticker_Connector.Models
{
    public class Timetable
    {
        public class Lesson
        {
            public string Subject { get; set; } = string.Empty;
            public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
            public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
            public bool IsSplitBelow { get; set; } = false;
            public bool IsStrongClassOverNotificationEnabled { get; set; } = false;
        }
    }
}
