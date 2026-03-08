using dotnetCampus.Ipc.CompilerServices.Attributes;
using ZongziTEK.BlackboardSticker;

namespace ZongziTEK.BlackboardSticker.Shared.IPC;

[IpcPublic(IgnoresIpcException = true)]
public interface IConnectService
{
    Task<List<Lesson>> GetCurrentTimetable();
    Task<bool> GetIsTimetableSyncEnabled();
    Task<double> GetIslandTerritoryHeight();
    Task<int> GetIslandDockingLocation();
}