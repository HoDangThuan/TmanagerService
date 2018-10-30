using System.ComponentModel;

namespace TmanagerService.Core.Enums
{
    public enum RequestStatus
    {
        [Description("Waiting")]
        Waiting = 1,
        [Description("To Do")]
        ToDo = 2,
        [Description("Done")]
        Done = 3,
        [Description("Approved")]
        Approved = 4
    }
}