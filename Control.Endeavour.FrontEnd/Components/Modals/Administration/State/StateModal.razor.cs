
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using System;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.State
{
    public partial class StateModal : ComponentBase
    {
        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private InputModalComponent inputId;
        private InputModalComponent inputCountryId;
        private InputModalComponent inputCodeNum;
        private InputModalComponent inputCodeTex;
        private InputModalComponent inputName;
        private NotificationsComponentModal notificationModal;


        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        [Parameter] public int Country { get; set; }

        #endregion

        #region Models
        private List<CountryDtoResponse> PaisesList = new();
        private StateDtoResponse _selectedRecord = new();
        private StateDtoRequest stateRequest = new();
        private StateDtoRequest stateRequestEdit = new();
        #endregion

        #region Environments

        private bool isEnable = false;
        private int CountryId { get; set; }
        private bool IsEditForm = false;
        private bool modalStatus = false;
        private bool IsDisabledCode = false;
        private string IdState;

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetCountry();

            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

        }


        #endregion

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region GetCountry
        private async Task GetCountry()
        {
            try
            {
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilter");
                PaisesList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<CountryDtoResponse>();

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el país: {ex.Message}");
            }
        }
        #endregion

        #region HandleValidSubmit
        private async Task HandleValidSubmit()
        {
            // Lógica de envío del formulario
            if (IsEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }

            StateHasChanged();

        }
        #endregion

        #region HandleFormCreate
        private async Task HandleFormCreate()
        {
            if (inputCodeNum.IsInputValid && inputName.IsInputValid && CountryId > 0)
            {
                stateRequest.CodeNum = inputCodeNum.InputValue;
                stateRequest.CodeTxt = inputCodeTex.InputValue ?? "";
                stateRequest.Name = inputName.InputValue;
                stateRequest.CountryId = CountryId;
                stateRequest.User = "admin";


                var responseApi = await HttpClient.PostAsJsonAsync("location/State/AddState", stateRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<StateDtoResponse>>();
                
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                }

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
            }


        }
        #endregion

        #region HandleFormUpdate
        private async Task HandleFormUpdate()
        {
            if (inputCodeNum.IsInputValid && inputName.IsInputValid && CountryId > 0)
            {
                stateRequest.StateId = _selectedRecord.StateId;
                stateRequest.CountryId = CountryId;
                stateRequest.CodeNum = inputCodeNum.InputValue;
                stateRequest.CodeTxt = String.IsNullOrEmpty(inputCodeTex.InputValue) ? _selectedRecord.CodeTxt : inputCodeTex.InputValue;
                stateRequest.Name = inputName.InputValue;
                stateRequest.User = "admin";


                var responseApi = await HttpClient.PostAsJsonAsync("location/State/UpdateState", stateRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<StateDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                    IsDisabledCode = false;
                }

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
            }

        }
        #endregion

        #region ResetFormAsync
        // Método para restablecer el formulario.
        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                stateRequest = new StateDtoRequest();
            }
            else
            {
                stateRequest = stateRequestEdit;
            }

        }
        #endregion

        #region PrepareData

        public void PreparedModal()
        {
            StateHasChanged();
            IsEditForm = false;
            stateRequest = new();
            CountryId = Country;
        }

        #region UpdateRecord
        // Método para actualizar el registro seleccionado.
        public void UpdateRecord(StateDtoResponse response)
        {
            _selectedRecord = response;
            stateRequestEdit.CodeNum = _selectedRecord.CodeNum;
            stateRequestEdit.Name = _selectedRecord.Name;
            stateRequestEdit.CountryId = _selectedRecord.CountryId;
            stateRequestEdit.CodeTxt = _selectedRecord.CodeTxt;
            stateRequest.CodeNum = _selectedRecord.CodeNum;
            stateRequest.Name = _selectedRecord.Name;
            stateRequest.CountryId = _selectedRecord.CountryId;
            stateRequest.CodeTxt = _selectedRecord.CodeTxt;
            IdState = _selectedRecord.StateId.ToString();
            CountryId = _selectedRecord.CountryId;
            IsDisabledCode = true;
            IsEditForm = true;
        }
        #endregion

        #endregion

        #region UpdateModalStatus
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #region HandleModalClosed
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;
            stateRequest = new StateDtoRequest();
            IsDisabledCode = false;
            IdState = "";
            IsEditForm = false;
            StateHasChanged();

        }
        #endregion

        #region HandleModalNotiClose
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }


        }
        #endregion

        #endregion

    }
}
