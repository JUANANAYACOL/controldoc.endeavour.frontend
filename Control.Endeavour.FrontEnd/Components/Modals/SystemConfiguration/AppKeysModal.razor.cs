using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class AppKeysModal : ComponentBase
    {

		#region Variables

		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }
        #endregion

        #region Components

        private InputModalComponent inputKeyName = new();
        private InputModalComponent inputValue1 = new();
        private InputModalComponent inputValue2 = new();
        private InputModalComponent inputValue3 = new();
        private InputModalComponent inputValue4 = new();

        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        #endregion

        #region Models
        private AppKeysDtoRequest appKeysDtoRequest { get; set; } = new();
        private AppKeysDtoRequest appKeysDtoRequestEdit { get; set; } = new();

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)
        private int appFunctionId { get; set; }
        private int appKeyId { get; set; }
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool IsEditForm { get; set; }
        private bool modalStatus = false;
        #endregion

        #region Environments(List & Dictionary)

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
        #region Form Methods
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

        private async Task HandleFormCreate()
        {
            try
            {
                appKeysDtoRequest.CompanyId = 17;
                if (!string.IsNullOrEmpty(appKeysDtoRequest.KeyName) && !string.IsNullOrEmpty(appKeysDtoRequest.Value1))
                {
                    if (IsTextAreaValue(appKeysDtoRequest.KeyName) && IsTextAreaValue(appKeysDtoRequest.Value1) && IsTextAreaValue(appKeysDtoRequest.Value2) && IsTextAreaValue(appKeysDtoRequest.Value3) && IsTextAreaValue(appKeysDtoRequest.Value4))
                    {
                        
                        appKeysDtoRequest.AppFunctionId = appFunctionId;
                        var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/CreateAppKeys", appKeysDtoRequest);

                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AppKeysDtoResponse>>();

                        if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                            await OnChangeData.InvokeAsync(true);
                        }
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
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message,  true);
            }
        }

        private async Task HandleFormUpdate()
        {
            try
            {
                if (!string.IsNullOrEmpty(appKeysDtoRequest.KeyName) && !string.IsNullOrEmpty(appKeysDtoRequest.Value1))
                {
                    if (IsTextAreaValue(appKeysDtoRequest.KeyName) && IsTextAreaValue(appKeysDtoRequest.Value1) && IsTextAreaValue(appKeysDtoRequest.Value2) && IsTextAreaValue(appKeysDtoRequest.Value3) && IsTextAreaValue(appKeysDtoRequest.Value4))
                    {
                        Dictionary<string, dynamic> headers = new() { { "AppFunctionId", appFunctionId }, { "AppKeyId", appKeyId } };
                        AppKeysUpdateDtoRequest appKeysUpdate = new();
                        appKeysUpdate.AppFunctionId = appFunctionId;
                        appKeysUpdate.AppKeyId = appKeyId;
                        appKeysUpdate.KeyName = appKeysDtoRequest.KeyName;
                        appKeysUpdate.Value1 = appKeysDtoRequest.Value1;
                        appKeysUpdate.Value2 = appKeysDtoRequest.Value2;
                        appKeysUpdate.Value3 = appKeysDtoRequest.Value3;
                        appKeysUpdate.Value4 = appKeysDtoRequest.Value4;
                        appKeysUpdate.CompanyId = 17;
                        appKeysUpdate.UpdateUser = "Front"; //Cambiar por varibale de usuario
                        
                        var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/UpdateAppKeys", appKeysUpdate);

                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AppKeysDtoResponse>>();

                        if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                            await OnChangeData.InvokeAsync(true);
                        }
                    }

                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message,  true);
            }
        }
        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                appKeysDtoRequest = new AppKeysDtoRequest();
            }
            else
            {
                appKeysDtoRequest = appKeysDtoRequestEdit;
            }
        }
        #endregion
        private async Task HandleModalClosedAsync(bool status)
        {

            modalStatus = status;
            StateHasChanged();
            appKeysDtoRequest = new();


        }
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatusAsync(args.ModalStatus);
            }


        }

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region ModalMethods
        public void ReceiveRecord(AppKeysDtoResponse response)
        {
            appFunctionId = response.AppFunctionId;
            appKeyId = response.AppKeyId;
            appKeysDtoRequest.KeyName = response.KeyName;
            appKeysDtoRequest.Value1 = response.Value1;
            appKeysDtoRequest.Value2 = response.Value2;
            appKeysDtoRequest.Value3 = response.Value3;
            appKeysDtoRequest.Value4 = response.Value4;
            appKeysDtoRequest.CompanyId = 17;
            appKeysDtoRequestEdit = appKeysDtoRequest;
            IsEditForm = true;
        }
        public async Task UpdateModalStatusAsync(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
            
        }
        public void AppFunctionId(int appFunctionIdRecord)
        {
            appFunctionId = appFunctionIdRecord;
        }

        #endregion

        #region ValidationMethods
        private bool IsTextAreaValue(string value)
        {

            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            // Retorna true si la cadena contiene solo letras, números, guiones, punto, barra diagonal, guión bajo y dos puntos
            return Regex.IsMatch(value, "^[a-zA-Z0-9:\\-._/\\s]+$");
        }
        #endregion

        #endregion

        #endregion

    }
}
