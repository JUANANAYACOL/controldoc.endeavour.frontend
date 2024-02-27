using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.City
{
    public partial class CityModal : ComponentBase
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

        #region Modals


        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        [Parameter] public int Country { get; set; } = 0;

        [Parameter] public int State { get; set; } = 0;

        #endregion

        #region Models
        private CityDtoResponse _selectedRecord = new();
        private CityDtoRequest cityRequest = new();
        private CityDtoRequest cityRequestEdit = new();
        private StateDtoResponse Departamento = new();
        private CountryDtoResponse Pais = new();
        #endregion

        #region Environments

        private bool IsDisabledCode = false;
        private string IdCity;

        private int StateId { get; set; }
        private int CountryId { get; set; }

        private bool IsEditForm = false;
        private bool modalStatus = false;

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

        #region GetCountry
        private async Task GetCountry()
        {
            try
            {
                if(CountryId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("IdCountry");
                    HttpClient?.DefaultRequestHeaders.Add("IdCountry", $"{CountryId}");
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<CountryDtoResponse>>("location/Country/ByFilterId");
                    HttpClient?.DefaultRequestHeaders.Remove("IdCountry");

                    Pais = deserializeResponse.Data != null ? deserializeResponse.Data : new();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el país: {ex.Message}");
            }
        }
        #endregion

        #region GetState
        private async Task GetState()
        {
            try
            {
                if (StateId > 0)
                {

                    HttpClient?.DefaultRequestHeaders.Remove("IdState");
                    HttpClient?.DefaultRequestHeaders.Add("IdState", $"{StateId}");
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<StateDtoResponse>>("location/State/ByFilterId");
                    HttpClient?.DefaultRequestHeaders.Remove("IdState");

                    Departamento = deserializeResponse.Data != null ? deserializeResponse.Data : new();               
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el departamento: {ex.Message}");
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
            if (inputCodeNum.IsInputValid && inputName.IsInputValid && StateId > 0)
            {
                cityRequest.CodeNum = inputCodeNum.InputValue;
                cityRequest.CodeTxt = inputCodeTex.InputValue ?? "";
                cityRequest.Name = inputName.InputValue;
                cityRequest.StateId = StateId;
                cityRequest.User = "admin";

                var responseApi = await HttpClient.PostAsJsonAsync("location/City/AddCity", cityRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CityDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear la ciudad, por favor intente de nuevo!", true);
                }
            }
            else { notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear la ciudad, por favor intente de nuevo!", true); }
        }
        #endregion

        #region HandleFormUpdate
        private async Task HandleFormUpdate()
        {
            if (inputCodeNum.IsInputValid && inputName.IsInputValid && StateId > 0)
            {
                cityRequest.CodeNum = inputCodeNum.InputValue;
                cityRequest.Name = inputName.InputValue;
                cityRequest.StateId = StateId;
                cityRequest.CityId = Convert.ToInt16( IdCity);
                cityRequest.CodeTxt = String.IsNullOrEmpty(inputCodeTex.InputValue) ? _selectedRecord.CodeTxt : inputCodeTex.InputValue;
                cityRequest.User = "admin";


                var responseApi = await HttpClient.PostAsJsonAsync("location/City/UpdateCity", cityRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CityDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    notificationModal.UpdateModal(ModalType.Success, "se actualizó el registro exitosamente", true, "aceptar");
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    //Logica no Exitosa
                    notificationModal.UpdateModal(ModalType.Error, "No se pudo actualizar el registro", true, "aceptar");
                }
            }
            else { notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar la ciudad, por favor intente de nuevo!", true); }

        }
        #endregion

        #region ResetFormAsync
        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                cityRequest = new CityDtoRequest();
            }
            else
            {
                cityRequest = cityRequestEdit;
            }

        }
        #endregion

        #region PrepareData

        public async Task PreparedModal()
        {
            CountryId = Country;
            StateId = State;
            await GetState();
            await GetCountry();
            IsEditForm = false;
            cityRequest = new();
            StateHasChanged();
        }

        #region UpdateRecord
        public async Task UpdateRecord(CityDtoResponse response)
        {
            _selectedRecord = response;
            CountryId = Country;
            StateId = _selectedRecord.StateId;
            await GetState();
            await GetCountry();
            cityRequestEdit.CodeNum = _selectedRecord.CodeNum;
            cityRequestEdit.Name = _selectedRecord.Name;
            cityRequestEdit.StateId = _selectedRecord.StateId;
            cityRequestEdit.CodeTxt = _selectedRecord.CodeTxt;

            cityRequest.CodeNum = _selectedRecord.CodeNum;
            cityRequest.Name = _selectedRecord.Name;
            cityRequest.StateId = _selectedRecord.StateId;
            cityRequest.CodeTxt = _selectedRecord.CodeTxt;
            cityRequest.CityId = _selectedRecord.CityId;
            IdCity = _selectedRecord.CityId.ToString();
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
            cityRequest = new CityDtoRequest();
            IsDisabledCode = false;
            IdCity = "";
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
