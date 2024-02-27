using System;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.StateContainer.Documents
{
    public class DocumentsStateContainer
    {
        #region Propiedades

        public string? Code { get; set; }
        public string? UserType { get; set; }
        public List<string> Codes { get; set; } = new List<string>();
        public List<bool> Values { get; set; } = new List<bool>();

        public int Identification { get; set; }
        public int Id { get; set; }

        #endregion

        #region Atributos

        public event Action? ComponentChange;

        #endregion

        #region Metodos

        public void Parametros(string code, string userType, List<string> codes, List<bool> values)
        {
            Code = code; UserType = userType; Codes = codes; Values = values;
            ExecuteAction();
        }

        public void ParametrosVisor(int identification, int id)
        {
            Identification = identification; Id = id;
            ExecuteAction();
        }

        private void ExecuteAction() => ComponentChange?.Invoke();

        #endregion
    }
}
