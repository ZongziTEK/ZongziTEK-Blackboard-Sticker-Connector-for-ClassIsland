using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Profile;
using ZongziTEK_Blackboard_Sticker;

namespace ZongziTEK_Blackboard_Sticker_Connector.Helpers
{
    public class TimetableHelper
    {
        public static List<Lesson> GetCurrentTimetable()
        {
            return GetTimetableFromClassPlan(GetCurrentClassPlan());
        }

        public static List<Lesson> GetTimetableFromClassPlan(ClassPlan? classPlan)
        {
            List<Lesson> timetable = new();

            if (classPlan == null) return timetable;

            var timeLayout = classPlan.TimeLayout;
            if (timeLayout == null) return timetable;

            foreach (var classInfo in classPlan.Classes)
            {
                Lesson lesson = new()
                {
                    Subject = GetSubjectName(classInfo.SubjectId),
                    StartTime = timeLayout.Layouts[classInfo.Index].StartTime,
                    EndTime = timeLayout.Layouts[classInfo.Index].EndTime,
                    IsSplitBelow = GetIsSeparatorBelow(timeLayout, classInfo.CurrentTimeLayoutItem)
                };
                timetable.Add(lesson);
            }

            return timetable;
        }

        public static ClassPlan? GetCurrentClassPlan()
        {
            var lessonsService = IAppHost.GetService<ILessonsService>();
            return lessonsService.CurrentClassPlan;
        }

        public static string GetSubjectName(Guid subjectId)
        {
            var profileService = IAppHost.GetService<IProfileService>();
            var subjects = profileService.Profile.Subjects;

            subjects.TryGetValue(subjectId, out var subject);

            return subject?.Name ?? string.Empty;
        }

        public static bool GetIsSeparatorBelow(TimeLayout timeLayout, TimeLayoutItem currentItem)
        {
            var index = timeLayout.Layouts.IndexOf(currentItem);

            if (index == -1 || index >= timeLayout.Layouts.Count - 1)
            {
                return false;
            }

            if (timeLayout.Layouts[index + 1].TimeType == 1) // 下一项为课间
            {
                if (index >= timeLayout.Layouts.Count - 2) // 下一项课间为最后一项
                {
                    return false;
                }

                return timeLayout.Layouts[index + 2].TimeType == 2;
            }

            return timeLayout.Layouts[index + 1].TimeType == 2; // 下一项不为课间
        }
    }
}
