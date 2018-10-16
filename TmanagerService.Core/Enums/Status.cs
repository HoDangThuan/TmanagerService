using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TmanagerService.Core.Enums
{
    public enum Status
    {
        [Description("Đang chờ")]
        Waiting = 1,
        [Description("Đang xử lý")]
        Processing = 2,
        [Description("Đã xong")]
        Done = 2
    }
}
