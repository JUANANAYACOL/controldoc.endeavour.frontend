using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Country
{
    public partial class CountryModal : ComponentBase
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
        private InputModalComponent inputCodeNum;
        private InputModalComponent inputCodeTex;
        private InputModalComponent inputName;
        private InputModalComponent inputCodeLanguage;

        private NotificationsComponentModal notificationModal;

        #endregion

        #region Modals


        #endregion

        #region Parameters

        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        private bool IsDisabledCode = false;
        private bool identification = false;
        private string IdCountry;


        private bool IsEditForm = false;
        private bool modalStatus = false;
        #endregion

        #region Models

        #endregion

        #region Environments

        private CountryDtoResponse _selectedRecord;

        private CountryDtoRequest countryRequest = new();
        private CountryDtoRequest countryRequestEdit = new();
        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
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
            if (inputCodeNum.IsInputValid && inputName.IsInputValid && inputCodeLanguage.IsInputValid)
            {
                countryRequest.CodeNum = inputCodeNum.InputValue;
                countryRequest.CodeTxt = inputCodeTex.InputValue ?? "";
                countryRequest.Name = inputName.InputValue;
                countryRequest.CodeLanguage = inputCodeLanguage.InputValue;
                countryRequest.User = "admin";


                var responseApi = await HttpClient.PostAsJsonAsync("location/Country/AddCountry", countryRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CountryDtoRequest>>();
                if (deserializeResponse.Succeeded)
                {
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
            if (inputCodeNum.IsInputValid && inputName.IsInputValid && inputCodeLanguage.IsInputValid)
            {
                

                countryRequest.CodeNum = inputCodeNum.InputValue;
                countryRequest.Name = inputName.InputValue;
                countryRequest.CodeLanguage = inputCodeLanguage.InputValue;
                countryRequest.CodeTxt = String.IsNullOrEmpty(inputCodeTex.InputValue) ? _selectedRecord.CodeTxt : inputCodeTex.InputValue;
                countryRequest.User = "admin";
                countryRequest.CountryId= _selectedRecord.CountryId;


                var responseApi = await HttpClient.PostAsJsonAsync("location/Country/UpdateCountry", countryRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CountryDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                    IsEditForm = false;
                    await ResetFormAsync();
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
            }


            // Aca iria el consumo del micro servicio para actualizar

        }
        #endregion

        #region ResetFormAsync
        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                countryRequest = new CountryDtoRequest();
            }
            else
            {
                countryRequest = countryRequestEdit;
            }

        }
        #endregion

        #region UpdateRecord
        public void UpdateRecord(CountryDtoResponse response)
        {
            _selectedRecord = response;
            countryRequestEdit.CodeNum = _selectedRecord.CodeNum;
            countryRequestEdit.Name = _selectedRecord.Name;
            countryRequestEdit.CodeLanguage = _selectedRecord.CodeLanguage;
            countryRequestEdit.CodeTxt = _selectedRecord.CodeTxt;
            countryRequest.CodeNum = _selectedRecord.CodeNum;
            countryRequest.Name = _selectedRecord.Name;
            countryRequest.CodeLanguage = _selectedRecord.CodeLanguage;
            countryRequest.CodeTxt = _selectedRecord.CodeTxt;
            IdCountry = _selectedRecord.CountryId.ToString();
            IsDisabledCode = true;
            IsEditForm = true;
        }
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
            countryRequest = new CountryDtoRequest();
            IsDisabledCode = false;
            IdCountry = "";
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
