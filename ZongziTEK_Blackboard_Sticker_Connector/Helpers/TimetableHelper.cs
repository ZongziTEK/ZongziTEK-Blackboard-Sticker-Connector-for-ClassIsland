using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Profile;
using Microsoft.Extensions.DependencyInjection;
using ZongziTEK_Blackboard_Sticker_Connector.Models;

namespace ZongziTEK_Blackboard_Sticker_Connector.Helpers
{
    public class TimetableHelper
    {
        public static List<Timetable.Lesson> GetCurrentTimetable()
        {
            return GetTimetableFromClassPlan(GetCurrentClassPlan());
        }

        public static List<Timetable.Lesson> GetTimetableFromClassPlan(ClassPlan? classPlan)
        {
            List<Timetable.Lesson> timetable = new();

            if (classPlan == null) return timetable;

            var timeLayout = classPlan.TimeLayout;
            if (timeLayout == null) return timetable;

            foreach (var classInfo in classPlan.Classes)
            {
                Timetable.Lesson lesson = new()
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

            return timeLayout.Layouts[index + 1].TimeType == 2;
        }
    }
}
