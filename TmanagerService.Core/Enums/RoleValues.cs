using System.ComponentModel;

namespace TmanagerService.Core.Enums
{
    public enum RoleValues
    {
        [Description("Admin")]
        Admin = 1,
        [Description("Supervisor")]
        Supervisor = 2,
        [Description("Repair Person")]
        RepairPerson = 3
    }
}
