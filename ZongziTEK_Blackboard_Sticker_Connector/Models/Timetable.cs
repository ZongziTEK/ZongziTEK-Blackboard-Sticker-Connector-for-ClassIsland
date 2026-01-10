namespace ZongziTEK_Blackboard_Sticker;

public class Lesson
{
    public string Subject { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
    public bool IsSplitBelow { get; set; } = false;
    public bool IsStrongClassOverNotificationEnabled { get; set; } = false;
}