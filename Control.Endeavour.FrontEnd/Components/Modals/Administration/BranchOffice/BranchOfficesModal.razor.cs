using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.BranchOffice
{
    public partial class BranchOfficesModal
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent inputId { get; set; } = new();
        private InputModalComponent inputCode { get; set; } = new();
        private InputModalComponent inputName { get; set; } = new();
        private InputModalComponent inputRegion { get; set; } = new();
        private InputModalComponent inputTerritory { get; set; } = new();
        private ButtonGroupComponent inputAddress { get; set; } = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Parameters

        [Parameter] public string IdModalIdentifier { get; set; } = "";

        [Parameter] public EventCallback<bool> OnChangeData { get; set; } = new();
        [Parameter] public EventCallback<int> OnIdSaved { get; set; } = new();

        [Parameter] public EventCallback<bool> OnAddressStatus { get; set; } = new();

        #endregion Parameters

        #region Models

        private BranchOfficesDtoResponse _selectedRecord { get; set; } = new();
        private BranchOfficeDtoRequest branchOfficeRequest { get; set; } = new();
        private BranchOfficeDtoRequest branchOfficeRequestEdit { get; set; } = new();
        private BranchOfficeUpdateDtoRequest requestUpdate { get; set; } = new();
        private AddressDtoRequest? addressRequest { get; set; }

        #endregion Models

        #region Environments

        #region Environments(String)

        private string addressString { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Bool)

        private bool IsActive { get; set; } = true;
        private bool IsEditForm { get; set; } = false;
        private bool modalStatus { get; set; } = false;
        private bool IsDisabledCode { get; set; } = false;

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

        #endregion HandleMethods

        #region OthersMethods

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            try
            {
                if (IsEditForm)
                {
                    await HandleFormUpdate();
                    IsEditForm = false;
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
            // Lógica de envío del formulario

            StateHasChanged();
        }

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            if (inputCode.IsInputValid && inputName.IsInputValid)
            {
                branchOfficeRequest.Code = inputCode.InputValue;
                branchOfficeRequest.NameOffice = inputName.InputValue;
                branchOfficeRequest.Region = inputRegion.InputValue;
                branchOfficeRequest.Territory = inputTerritory.InputValue;
                branchOfficeRequest.Address = addressRequest;
                branchOfficeRequest.User = "";

                var responseApi = await HttpClient!.PostAsJsonAsync("params/BranchOffice/CreateBranchOffice", branchOfficeRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BranchOfficesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el registro, por favor intente de nuevo!", true, "Aceptar");
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el registro, por favor intente de nuevo!", true, "Aceptar");
            }
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            if (inputCode.IsInputValid && inputName.IsInputValid)
            {
                _selectedRecord.Code = inputCode.InputValue;
                _selectedRecord.NameOffice = inputName.InputValue;
                _selectedRecord.Region = String.IsNullOrEmpty(inputRegion.InputValue) ? _selectedRecord.Region : inputRegion.InputValue;
                _selectedRecord.Territory = String.IsNullOrEmpty(inputTerritory.InputValue) ? _selectedRecord.Territory : inputTerritory.InputValue;

                requestUpdate.Address = addressRequest;
                requestUpdate.Code = _selectedRecord.Code;
                requestUpdate.NameOffice = _selectedRecord.NameOffice;
                requestUpdate.Region = _selectedRecord.Region;
                requestUpdate.Territory = _selectedRecord.Territory;
                requestUpdate.User = "Front User"; //Falta consumir variables de sesion para poner el usuario

                var response = await HttpClient!.PostAsJsonAsync("params/BranchOffice/UpdateBranchOffice", requestUpdate);

                var deserializeResponse = await response.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BranchOfficesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
            }

            // Aca iria el consumo del micro servicio para actualizar
        }

        #endregion HandleFormUpdate

        #region ResetFormAsync

        // Método para restablecer el formulario.
        public void ResetFormAsync()
        {
            requestUpdate = new();
            branchOfficeRequest = new();
            _selectedRecord = new();
            addressRequest = null;
            addressString = "";
        }

        #endregion ResetFormAsync

        #region recieveBranchOffice

        // Método para actualizar el registro seleccionado.
        public void recieveBranchOffice(BranchOfficesDtoResponse response)
        {
            _selectedRecord = response;
            branchOfficeRequestEdit.Code = _selectedRecord.Code;
            branchOfficeRequestEdit.NameOffice = _selectedRecord.NameOffice;
            branchOfficeRequestEdit.Region = _selectedRecord.Region;
            branchOfficeRequestEdit.Territory = _selectedRecord.Territory;

            branchOfficeRequest.Code = _selectedRecord.Code;
            branchOfficeRequest.NameOffice = _selectedRecord.NameOffice;
            branchOfficeRequest.Region = _selectedRecord.Region;
            branchOfficeRequest.Territory = _selectedRecord.Territory;

            requestUpdate.BranchOfficeId = _selectedRecord.BranchOfficeId;
            addressString = response.AddressString;
            if (response.Address != null)
            {
                addressRequest = new AddressDtoRequest()
                {
                    CityId = response.Address.CityId,
                    CountryId = response.Address.CountryId,
                    CrBis = response.Address.CrBis ?? false,
                    CrCardinality = response.Address.CrCardinality,
                    CrComplement = response.Address.CrComplement,
                    CrLetter = response.Address.CrLetter,
                    CrNumber = response.Address.CrNumber,
                    CrType = response.Address.CrType,
                    HouseClass = response.Address.HouseClass,
                    HouseNumber = response.Address.HouseNumber,
                    HouseType = response.Address.HouseType,
                    StateId = response.Address.StateId,
                    StBis = response.Address.StBis ?? false,
                    StCardinality = response.Address.StCardinality,
                    StComplement = response.Address.StComplement,
                    StLetter = response.Address.StLetter,
                    StNumber = response.Address.StNumber,
                    StType = response.Address.StType
                };
            }
            else
            {
                addressRequest = null;
            }

            IsDisabledCode = true;
            IsEditForm = true;
        }

        #endregion recieveBranchOffice

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        public void OpenCreateModal()
        {
            modalStatus = true;
            ResetFormAsync();
            StateHasChanged();
        }

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            branchOfficeRequest = new BranchOfficeDtoRequest();
            IsDisabledCode = false;

            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        private async Task OpenNewModal()
        {
            await OnAddressStatus.InvokeAsync(true);

            if (_selectedRecord != null && ( _selectedRecord.AddressId != 0 && _selectedRecord.AddressId != null ))
            {
                await OnIdSaved.InvokeAsync((int)_selectedRecord.AddressId);
            }
        }

        #endregion OthersMethods

        #region updateAddressSelection

        public void updateAddressSelection(List<(string, AddressDtoRequest)> address)
        {
            if (address != null && address.Count > 0)
            {
                (addressString, addressRequest) = address[0];
            }
        }

        #endregion updateAddressSelection

        #endregion Methods
    }
}