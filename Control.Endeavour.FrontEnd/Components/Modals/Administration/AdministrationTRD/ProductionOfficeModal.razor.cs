using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class ProductionOfficeModal : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private InputModalComponent inputCode { get; set; } = new();
        private InputModalComponent inputName { get; set; } = new();
        private ButtonGroupComponent inputBoss { get; set; } = new();
        private InputModalComponent inputDescription { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter]
        public string idModalIdentifier { get; set; } = null!;

        [Parameter]
        public bool modalStatus { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChangedUpdate { get; set; }

        #endregion Parameters

        #region Models

        private VUserDtoResponse bossSelected { get; set; } = new();
        private ProductionOfficeCreateDtoRequest productionOfficeRequest { get; set; } = new();
        private ProductionOfficeUpdateDtoRequest productionOfficeUpdateRequest { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitList { get; set; } = new();

        #endregion Models

        #region Enviroment

        private decimal CharacterCounter { get; set; } = 0;

        public bool userSearchModalStatus { get; set; } = new();
        public string bossNamme { get; set; } = null!;
        private int userId { get; set; }

        private string text { get; set; } = "";
        private bool isEditForm { get; set; } = false;
        private bool activeState { get; set; } = true;

        private int selectAdministriveUnitId { get; set; } = 0;

        #endregion Enviroment

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await FillAdministrativeUnitDdl();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region updateBossSelection

        public void updateBossSelection(VUserDtoResponse boosToSelect)
        {
            bossSelected = boosToSelect;
            bossNamme = bossSelected.FullName;
            userId = bossSelected.UserId;
            StateHasChanged();
        }

        #endregion updateBossSelection

        #region OpenNewModal

        private async Task OpenNewModal()
        {
            await OnStatusChanged.InvokeAsync(true);
        }

        #endregion OpenNewModal

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            bossSelected = new();
            bossNamme = "";
            productionOfficeRequest.Code = "";
            productionOfficeRequest.Name = "";
            productionOfficeRequest.AdministrativeUnitId = 0;
            productionOfficeRequest.Description = "";
            selectAdministriveUnitId = 0;
        }

        #endregion ResetFormAsync

        #region ResetFormAsync

        public void UpdateAdministrativeUnitSelected(int idSelected, string textSelected)
        {
            text = textSelected;
            productionOfficeRequest.AdministrativeUnitId = idSelected;
        }

        #endregion ResetFormAsync

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            isEditForm = false;
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region FillAdministrativeUnitDdl

        private async Task FillAdministrativeUnitDdl()
        {
            HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
            HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", "0");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
            HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

            if (deserializeResponse!.Succeeded)
            {
                administrativeUnitList = deserializeResponse.Data!;
            }
        }

        #endregion FillAdministrativeUnitDdl

        #region OnChangeUA

        public void OnChangeUA(int value)
        {
            selectAdministriveUnitId = value;
        }

        #endregion OnChangeUA

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            ResetFormAsync();
            modalStatus = status;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        public void GetAdminitativeUnitId(int code)
        {
            try
            {
                productionOfficeRequest.AdministrativeUnitId = code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Officinas Productoras: {ex.Message}");
            }
        }

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            if (isEditForm)
            {
                await HandleFormUpdate();
                isEditForm = false;
            }
            else
            {
                await HandleFormCreate();
            }

            ResetForm();
        }

        #endregion HandleValidSubmit

        #region ResetForm

        private void ResetForm()
        {
            productionOfficeRequest = new();
        }

        #endregion ResetForm

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            try
            {
                if (inputName.IsInputValid && inputBoss.IsInputValid)
                {
                    productionOfficeRequest.BossId = userId;
                    productionOfficeRequest.Name = inputName.InputValue;

                    productionOfficeRequest.Code = inputCode.InputValue;
                    productionOfficeRequest.ActiveState = activeState;
                    productionOfficeRequest.Description = inputDescription.InputValue;
                    productionOfficeRequest.User = "Val";

                    var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/ProductionOffice/CreateProductionOffice", productionOfficeRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProductionOfficesDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, deserializeResponse.Message!, true, "Aceptar");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, deserializeResponse.Message!, true, "Aceptar");
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "Cannot set up", true, "Aceptar");
                }
                productionOfficeRequest = new();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "Error in the process", true, "Aceptar");
            }
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            StateHasChanged();

            productionOfficeUpdateRequest.Name = inputName.InputValue;
            productionOfficeUpdateRequest.AdministrativeUnitId = productionOfficeRequest.AdministrativeUnitId;
            productionOfficeUpdateRequest.Code = productionOfficeRequest.Code;
            productionOfficeUpdateRequest.BossId = productionOfficeRequest.BossId;
            productionOfficeUpdateRequest.Description = productionOfficeRequest.Description;
            productionOfficeUpdateRequest.ActiveState = productionOfficeRequest.ActiveState;
            productionOfficeUpdateRequest.User = "Val";

            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ProductionOffice/UpdateProductionOffice", productionOfficeUpdateRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProductionOfficesDtoResponse>>();
            if (deserializeResponse!.Succeeded)
            { notificationModal.UpdateModal(ModalType.Success, deserializeResponse.Message!, true, "Aceptar"); }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "Cannot set update", true, "Aceptar");
            }
        }

        #endregion HandleFormUpdate

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose()
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnStatusChangedUpdate.InvokeAsync(false);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region UpdateSelectedRecord

        public async Task UpdateSelectedRecord(ProductionOfficesDtoResponse record)
        {
            await FillAdministrativeUnitDdl();
            isEditForm = true;

            text = record.AdministrativeUnitName;

            productionOfficeRequest.BossId = record.BossId;
            productionOfficeRequest.AdministrativeUnitId = record.AdministrativeUnitId;
            productionOfficeRequest.ActiveState = record.ActiveState;
            productionOfficeRequest.Description = string.IsNullOrEmpty(record.Description) ? "" : record.Description;
            productionOfficeRequest.Name = string.IsNullOrEmpty(record.Name) ? "" : record.Name;
            productionOfficeRequest.Code = record.Code;
            productionOfficeUpdateRequest.ProductionOfficeId = record.ProductionOfficeId;

            bossNamme = !record.BossId.HasValue ? "" : ( await GetUserName((int)record!.BossId!) ).ToString();
        }

        #endregion UpdateSelectedRecord

        #region GetUserName

        private async Task<string> GetUserName(int userIdToSearch)
        {
            HttpClient?.DefaultRequestHeaders.Remove("userId");
            HttpClient?.DefaultRequestHeaders.Add("userId", $"{userIdToSearch}");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<UserDtoResponse>>("security/User/ByFilterId");
            HttpClient?.DefaultRequestHeaders.Remove("userId");

            if (deserializeResponse!.Succeeded)
            {
                var resultUser = deserializeResponse.Data;
                return resultUser?.FullName!;
            }
            else
            {
                return "";
            }
        }

        #endregion GetUserName

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

        #endregion Methods
    }
}