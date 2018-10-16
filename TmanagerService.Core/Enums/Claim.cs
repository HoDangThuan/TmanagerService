using System.ComponentModel;

namespace TmanagerService.Core.Enums
{
    public enum Claim
    {
        [Description("Create Account")]
        CreateAccount = 1,
        [Description("Insert Request")]
        InsertRequest = 2,
        [Description("Receive Request")]
        ReceiveRequest = 3
    }
}
