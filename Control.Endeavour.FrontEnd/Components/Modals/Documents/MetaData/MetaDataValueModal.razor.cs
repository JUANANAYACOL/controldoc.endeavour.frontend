using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.MetaData
{
    public partial class MetaDataValueModal
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Parameters

        [Parameter]
        public bool modalStatus { get; set; }

        [Parameter]
        public EventCallback<MyEventArgs<MetaDataRelationDtoRequest>> OnStatusChanged { get; set; } = new();

        [Parameter]
        public EventCallback<MyEventArgs<SearchConfigurationArgs>> ConfigurationToUse { get; set; } = new();

        public List<VUserDtoResponse> usersList { get; set; } = new();

        #endregion Parameters

        #region Models

        private MetaDataRelationDtoRequest metaDataSelected { get; set; } = new();

        private List<MetaValuesDtoResponse> metaValues { get; set; } = new();

        private ThirdPartyDtoResponse thirdPartySelected { get; set; } = new();

        private List<VUserDtoResponse> vUsersSelected { get; set; } = new();

        #endregion Models

        #region Modals

        public NotificationsComponentModal notificationModal { get; set; } = new();
        public GenericSearchModal serachModal { get; set; } = new();

        private ButtonGroupComponent inputModal { get; set; } = new();

        #endregion Modals

        #region Environments

        #region Environments(Numeric)

        private decimal CharacterCounter { get; set; } = 0;
        private int configurationInUse { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(String)

        private string selectedDropDown { get; set; } = "";

        private string color { get; set; } = "";
        private string MetaDataValue { get; set; } = "";
        private string showPanelNumber = "d-none";
        private string showPanelAlphaNumeric = "d-none";
        private string showPanelDate = "d-none";
        private string showPanelBool = "d-none";
        private string showPanelUser = "d-none";
        private string showPanelThirdParty = "d-none";
        private string showPanelList = "d-none";

        #endregion Environments(String)

        #region Environments(DateTime)

        private DateTime date { get; set; } = DateTime.Now;
        private DateTime minValueTo { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool multipleSelection { get; set; } = false;

        private bool isActive { get; set; } = false;

        private bool complete { get; set; } = false;
        private bool absent { get; set; } = false;
        private bool incomplete { get; set; } = false;
        private bool none { get; set; } = false;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }

        private void HandleValidSubmit()
        {
            switch (metaDataSelected.FieldType)
            {
                case "FTY,13":

                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;

                    break;

                case "FTY,14":
                    metaDataSelected.DataText = date.ToString();

                    break;

                case "FTY,15":

                    metaDataSelected.DataText = isActive.ToString();

                    break;

                case "FTY,16":

                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;
                    break;

                case "FTY,17":
                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;
                    break;

                case "FTY,18":
                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;
                    break;

                case "FTY,19":

                    metaDataSelected.DataText = selectedDropDown;
                    break;
            }
            metaDataSelected.ColorData = color;
            notificationModal.UpdateModal(ModalType.Warning, "¿Seguro que quieres guardar los cambios realizados?", true, "Si", "No");
        }

        private void HandleCheckBoxes(bool newValue, int checkBoxCase)
        {
            switch (checkBoxCase)
            {
                case 1:
                    color = "MDC,V";
                    complete = newValue;
                    absent = false;
                    incomplete = false;
                    none = false;
                    break;

                case 2:
                    color = "MDC,AZ";
                    absent = newValue;
                    complete = false;
                    incomplete = false;
                    none = false;
                    break;

                case 3:
                    metaDataSelected.ColorData = "MDC,A";
                    incomplete = newValue;
                    absent = false;
                    complete = false;
                    none = false;
                    break;

                case 4:
                    color = "MDC,NE";
                    none = newValue;
                    absent = false;
                    complete = false;
                    incomplete = false;

                    break;
            }
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Warning)
            {
                var result = new MyEventArgs<MetaDataRelationDtoRequest>()
                {
                    Data = metaDataSelected,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(result);

                notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
            }

            StateHasChanged();
        }

        private void HandleUsersSelected(MyEventArgs<List<object>> request)
        {
            vUsersSelected = (List<VUserDtoResponse>)request.Data[0];
            serachModal.UpdateModalStatus(request.ModalStatus);
        }

        private void HandleThirdPartySelected(MyEventArgs<ThirdPartyDtoResponse> request)
        {
            thirdPartySelected = request.Data;
            serachModal.UpdateModalStatus(request.ModalStatus);
        }

        #endregion HandleMethods

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region MetaFieldSelected

        public void MetaFieldSelected(MetaDataRelationDtoRequest newValue)
        {
            metaDataSelected = newValue;
            showPanelNumber = "d-none";
            showPanelAlphaNumeric = "d-none";
            showPanelDate = "d-none";
            showPanelBool = "d-none";
            showPanelUser = "d-none";
            showPanelThirdParty = "d-none";
            showPanelList = "d-none";

            switch (metaDataSelected.FieldType)
            {
                case "FTY,13":
                    showPanelNumber = "";
                    MetaDataValue = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    CharacterCounter = string.IsNullOrEmpty(metaDataSelected.DataText) ? 0 : metaDataSelected.DataText.Length;
                    break;

                case "FTY,14":
                    showPanelDate = "";
                    date = string.IsNullOrEmpty(metaDataSelected.DataText) ? DateTime.Now : DateTime.Parse(metaDataSelected.DataText);
                    break;

                case "FTY,15":
                    showPanelBool = "";
                    isActive = string.IsNullOrEmpty(metaDataSelected.DataText) ? false : bool.Parse(metaDataSelected.DataText);
                    break;

                case "FTY,16":
                    showPanelAlphaNumeric = "";
                    MetaDataValue = metaDataSelected.DataText;
                    break;

                case "FTY,17":
                    showPanelThirdParty = "";
                    MetaDataValue = metaDataSelected.DataText;
                    break;

                case "FTY,18":
                    showPanelUser = "";
                    MetaDataValue = metaDataSelected.DataText;
                    break;

                case "FTY,19":
                    showPanelList = "";
                    metaValues = (List<MetaValuesDtoResponse>)metaDataSelected.MetaValues.OrderBy(x => x.ValueOrder).ToList();
                    selectedDropDown = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    break;
            }

            switch (metaDataSelected.ColorData)
            {
                case "MDC,V":
                    HandleCheckBoxes(true, 1);
                    break;

                case "MDC,AZ":
                    HandleCheckBoxes(true, 2);
                    break;

                case "MDC,A":
                    HandleCheckBoxes(true, 3);
                    break;

                case "MDC,NE":
                    HandleCheckBoxes(true, 4);
                    break;
            }

            StateHasChanged();
        }

        #endregion MetaFieldSelected

        #region CountCharacters

        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                CharacterCounter = value.Length;
            }
            else
            {
                CharacterCounter = 0;
            }
        }

        #endregion CountCharacters

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            isActive = false;

            complete = false;
            absent = false;
            incomplete = false;
            none = false;
            date = DateTime.Now;
            CharacterCounter = 0;
            MetaDataValue = string.Empty;
            usersList = new();

            StateHasChanged();
        }

        #endregion ResetFormAsync

        private async Task openSerachModal(int configuration)
        {
            SearchConfigurationArgs valuesToReturn = new();

            switch (configuration)
            {
                case 1:
                    //Funcionarios
                    valuesToReturn.Configuration = configuration;
                    valuesToReturn.MultipleSelection = true;
                    valuesToReturn.Title = "Usuario - MetaDato";

                    break;

                case 2:
                    //Terceros
                    valuesToReturn.Configuration = configuration;
                    valuesToReturn.MultipleSelection = false;
                    valuesToReturn.Title = "Tercero - MetaDato";
                    break;
            }

            MyEventArgs<SearchConfigurationArgs> args = new()
            {
                Data = valuesToReturn,
                ModalStatus = true,
            };

            await ConfigurationToUse.InvokeAsync(args);
        }

        private void OnDropDownValueChanged(string newValue)
        {
            try
            {
                selectedDropDown = newValue;
            }
            catch (Exception ex)
            {
                // Manejar excepciones, logearlas o tomar medidas apropiadas según tu aplicación
                notificationModal.UpdateModal(ModalType.Error, $"Error en OnDropDownValueChanged: {ex.Message}", true, "Aceptar");
            }
        }

        public void UserSelectionMetaData(List<VUserDtoResponse> request)
        {
            usersList = request;
            foreach (var user in usersList)
            {
                MetaDataValue += string.IsNullOrEmpty(user.FullName) ? "" : $"{user.FullName},";
            }
            StateHasChanged();
        }

        public void ThirdPartySelectionMetaData(ThirdPartyDtoResponse request)
        {
            MetaDataValue = request.Names;
        }

        private void DeleteToList(VUserDtoResponse request)
        {
            try
            {
                usersList.Remove(request);
                MetaDataValue = "";

                foreach (var user in usersList)
                {
                    MetaDataValue += string.IsNullOrEmpty(user.FullName) ? "" : $"{user.FullName},";
                }

                StateHasChanged();
            }
            catch { notificationModal.UpdateModal(ModalType.Error, "Error al remover el meta valor", true, "Aceptar"); }
        }

        #endregion Methods
    }
}