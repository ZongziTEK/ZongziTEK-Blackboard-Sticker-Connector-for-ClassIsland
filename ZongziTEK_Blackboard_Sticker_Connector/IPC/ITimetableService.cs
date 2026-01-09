using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnetCampus.Ipc.CompilerServices.Attributes;
using ZongziTEK_Blackboard_Sticker_Connector.Models;

namespace ZongziTEK_Blackboard_Sticker_Connector.IPC;

[IpcPublic(IgnoresIpcException = true)]
public interface ITimetableService
{
    List<Timetable.Lesson> GetCurrentTimetable();
}