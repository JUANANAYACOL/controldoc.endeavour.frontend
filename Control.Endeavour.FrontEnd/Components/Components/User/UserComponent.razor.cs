using Microsoft.AspNetCore.Components;

namespace Control.Endeavour.FrontEnd.Components.Components.User
{
    public partial class UserComponent
    {
        #region Parameters
        [Parameter] public string? UserName { get; set; }
        [Parameter] public string? UserImage { get; set; }
        #endregion
    }
}
