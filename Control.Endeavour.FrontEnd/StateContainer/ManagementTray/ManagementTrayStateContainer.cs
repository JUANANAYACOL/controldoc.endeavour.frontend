using Control.Endeavour.FrontEnd.Models.Enums.Documents;

namespace Control.Endeavour.FrontEnd.StateContainer.ManagementTray
{
    public class ManagementTrayStateContainer
    {
        #region Propiedades

        public DocumentStatusEnum Status { get; set; }

        #endregion

        #region Atributos

        public event Action? ComponentChange;

        #endregion

        #region Metodos

        public void Parametros(DocumentStatusEnum status)
        {
            Status = status;
            ExecuteAction();
        }

        private void ExecuteAction() => ComponentChange?.Invoke();

        #endregion
    }
}
