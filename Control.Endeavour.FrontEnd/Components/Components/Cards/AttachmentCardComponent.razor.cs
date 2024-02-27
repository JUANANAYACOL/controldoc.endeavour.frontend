using Microsoft.AspNetCore.Components;

namespace Control.Endeavour.FrontEnd.Components.Components.Cards
{
    public partial class AttachmentCardComponent
    {
        #region Parameters

        [Parameter]
        public string ArchiveName { get; set; } = "Nombre no encontrado";
        [Parameter]
        public string ExibitCodeName { get; set; } = "Tipo de documento no encontrado";
        [Parameter]
        public string AttDescription { get; set; } = "Descripcion no encontrado";
        [Parameter]
        public string CreateUser { get; set; } = "Usuario no encontrado";
        [Parameter]
        public string CreateDate { get; set; } = "Fecha no encontrado";

        #endregion
    }
}
