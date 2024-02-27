using Microsoft.AspNetCore.Components;

namespace Control.Endeavour.FrontEnd.Components.Components.User
{
    public partial class ThirdCardComponent
    {
        #region Parameters

        [Parameter]
        public string IdentificationNumber { get; set; } = "NIT no encontrado";
        [Parameter]
        public string Names { get; set; } = "Compañia no encontrada";
        [Parameter]
        public string CompanyName { get; set; } = "Nombre no encontrado";
        [Parameter]
        public string Email { get; set; } = "Correo electronico no encontrado";
        [Parameter]
        public string Positionname { get; set; } = "Cargo no encontrado";

        #endregion
    }
}
