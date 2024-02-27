using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class AdministrativeUnitModal
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public int IDVersion { get; set; }
        [Parameter] public EventCallback<bool> OnStatusUpdate { get; set; }
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }

        #endregion

        #region Models

        private InputModalComponent codeinput = new();
        private InputModalComponent descriptioninput = new();
        private InputModalComponent nameinput = new();
        private InputModalComponent Bossinput = new();
        private VUserDtoResponse BossSelected { get; set; } = new();
        private AdministrativeUnitDtoRequest adminUnitDtoRequest = new();
        private AdministrativeUnitsDtoResponse adminUnitDtoResponse = new();
        private AdministrativeUnitsDtoResponse _selectedRecord = new();
        DocumentalVersionFilterDtoRequest documentalVersionsFilterDtoResponse = new();
        private MetaModel meta = new();


        #endregion

        #region Environments

        #region Environments(String)

        private string Text = "Seleccione una opción";

        #endregion

        #region Environments(Numeric)

        private decimal CharacterCounter = 0;

        #endregion

        #region Environments(Bool)

        private bool IsEditForm = false;
        private bool activeState = true;
        private bool modalStatus = false;
        private bool IsDisabledCode = false;
        private bool UpdateForm = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<DocumentalVersionDtoResponse> docVersionList = new();
        #endregion


        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

        }


        #endregion

        #region Methods

        #region HandleMethods

        #region Forms
        private async Task HandleValidSubmit()
        {
            try
            {
                if (IsEditForm)
                {
                    await HandleFormUpdate();


                }
                else
                {
                    await HandleFormCreate();
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, "Aceptar");
            }



            StateHasChanged();
        }

        private async Task HandleFormCreate()
        {
            if (codeinput.IsInputValid && nameinput.IsInputValid)
            {
                adminUnitDtoRequest.DocumentalVersionId = IDVersion;
                adminUnitDtoRequest.Name = nameinput.InputValue;
                adminUnitDtoRequest.Code = codeinput.InputValue;
                adminUnitDtoRequest.BossId = BossSelected.UserId;
                adminUnitDtoRequest.Description = descriptioninput?.InputValue;
                adminUnitDtoRequest.ActiveState = activeState;
                adminUnitDtoRequest.CreateUser = "Front"; //poner Variable de usuario
                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/AdministrativeUnit/CreateAdministrativeUnit", adminUnitDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AdministrativeUnitsDtoResponse>>();

                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                    await OnStatusUpdate.InvokeAsync(true);
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el registro, por favor intente de nuevo!", true, "Aceptar");
            }


        }

        private async Task HandleFormUpdate()
        {
            StateHasChanged();

            _selectedRecord.BossId = BossSelected.UserId;
            _selectedRecord.Name = nameinput.InputValue;
            _selectedRecord.Description = descriptioninput.InputValue;
            _selectedRecord.ActiveState = activeState;
            _selectedRecord.CreateUser = "Front"; //Front Cambiar por variable de sesion

            AdministrativeUnitUpdateDtoRequest dtoRequest = new();
            dtoRequest.AdministrativeUnitId = _selectedRecord.AdministrativeUnitId; //todo
            dtoRequest.DocumentalVersionId = _selectedRecord.DocumentalVersionId;
            dtoRequest.BossId = _selectedRecord.BossId;
            dtoRequest.Code = _selectedRecord.Code;
            dtoRequest.Name = _selectedRecord.Name;
            dtoRequest.Description = _selectedRecord.Description;
            dtoRequest.ActiveState = activeState;
            dtoRequest.User = "Front"; //Poner la variable de Front 

            var response = await HttpClient.PostAsJsonAsync("paramstrd/AdministrativeUnit/UpdateAdministrativeUnit", dtoRequest);


            var deserializeResponse = await response.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BranchOfficesDtoResponse>>();

            if (deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                await OnStatusUpdate.InvokeAsync(true);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
            }
        }
        #endregion



        #region Modal
        #region Notifications
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }
        #endregion
        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            ResetFormAsync();
        }
        #endregion

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods
        private async Task ResetFormAsync()
        {

            if (!IsEditForm)
            {
                adminUnitDtoResponse = new();
            }
            else
            {
                adminUnitDtoResponse = _selectedRecord;
            }
        }

        #region ModalPrepareData

        public void updateBossSelection(VUserDtoResponse boosToSelect)
        {
            BossSelected = boosToSelect;
            adminUnitDtoResponse.BossName = BossSelected.FullName;
        }

        public async Task PreparedModal()
        {
            StateHasChanged();
            UpdateForm = false;
            adminUnitDtoRequest = new();
            await GetDocumentalVersions();
            Text = docVersionList.Where(x => x.DocumentalVersionId == IDVersion).Select(x => x.Name).FirstOrDefault();
        }
        public void UpdateSelectedRecord(AdministrativeUnitsDtoResponse response)
        {
            _selectedRecord = response;
            activeState = _selectedRecord.ActiveState;
            UpdateForm = false;
            IsEditForm = true;
            Text = _selectedRecord.DocumentalVersionName;
            IsDisabledCode = true;
            adminUnitDtoResponse.Code = _selectedRecord.Code;
            adminUnitDtoResponse.Name = _selectedRecord.Name;
            adminUnitDtoResponse.BossName = _selectedRecord.BossName;
            adminUnitDtoResponse.Description = _selectedRecord.Description;
        }

        private async Task OpenNewModal(bool value)
        {
            await OnStatusChanged.InvokeAsync(true);
        }
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #region GetDataMethods
        private async Task GetDocumentalVersions()
        {
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/AdministrativeUnit/ByFilter", documentalVersionsFilterDtoResponse);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    docVersionList = deserializeResponse.Data;
                    meta = deserializeResponse.Meta;
                }
                else
                {
                    docVersionList = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las versiones documentales, por favor intente de nuevo!", true);
                }


            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        #endregion

        #region ValidationMethods
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

        #endregion

        #endregion

        #endregion

    }
}
