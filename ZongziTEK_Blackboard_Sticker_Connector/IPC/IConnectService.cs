using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace ZongziTEK_Blackboard_Sticker.Shared.IPC;

[IpcPublic(IgnoresIpcException = true)]
public interface IConnectService
{
    Task<List<Timetable.Lesson>> GetCurrentTimetable();
    Task<bool> GetIsTimetableSyncEnabled();
}