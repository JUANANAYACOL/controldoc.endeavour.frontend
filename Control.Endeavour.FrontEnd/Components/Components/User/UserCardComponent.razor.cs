using Microsoft.AspNetCore.Components;

namespace Control.Endeavour.FrontEnd.Components.Components.User
{
    public partial class UserCardComponent
    {
        #region Variables

        #region Parameters

        [Parameter]
        public string FullName { get; set; } = "Nombre no encontrado";

        [Parameter]
        public string AdministrativeUnitName { get; set; } = "Unidad Administrativa no encontrado";

        [Parameter]
        public string ProductionOfficeName { get; set; } = "Oficina productora no encontrado";

        [Parameter]
        public string Positionname { get; set; } = "Cargo no encontrado";

        #endregion Parameters

        #endregion Variables
    }
}