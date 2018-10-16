using System.ComponentModel;

namespace TmanagerService.Core.Enums
{
    public enum RequestStatus
    {
        [Description("Đang chờ")]
        Waiting = 1,
        [Description("Đang xử lý")]
        ToDo = 2,
        [Description("Đã xong")]
        Done = 3,
        [Description("Chấp nhận")]
        Approved = 4
    }
}