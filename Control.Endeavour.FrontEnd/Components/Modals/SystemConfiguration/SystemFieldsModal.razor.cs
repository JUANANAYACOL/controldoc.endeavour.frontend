using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class SystemFieldsModal
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

        private InputModalComponent inputCode = new();
        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }


        #endregion

        #region Models
        private SystemFieldsDtoRequest systemFieldsDtoRequest = new SystemFieldsDtoRequest();
        private SystemFieldsDtoRequest systemFieldsDtoRequestEdit = new SystemFieldsDtoRequest();
        
        #endregion

        #region Environments

        #region Environments(String)
        private string systemFieldId = string.Empty;
        private string systemParamId = string.Empty;
        #endregion

        #region Environments(Numeric)

        private int intSystemParamId;
        int characterCounterValue = 0;
        int characterCounterComment = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool IsEditForm = false;
        private bool modalStatus = false;
        private bool IsDisabledCode = false;
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
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;
            systemFieldsDtoRequest = new SystemFieldsDtoRequest();
            systemFieldId = string.Empty;
            systemParamId = string.Empty;
            IsDisabledCode = false;
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }


        }
        #region FormMethods
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
                if (!string.IsNullOrEmpty(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value) && inputCode.IsInputValid)
                {
                    systemFieldsDtoRequest.SystemParamId = intSystemParamId;
                    var responseApi = await HttpClient.PostAsJsonAsync("params/SystemFields/CreateSystemFields", systemFieldsDtoRequest);

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
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }
        private async Task HandleFormUpdate()
        {
            try
            {
                if (!string.IsNullOrEmpty(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value))
                {
                 
                    SystemFieldUpdateDtoRequest updateDtoRequest = new();
                    updateDtoRequest.SystemFieldId = int.Parse(systemFieldId);
                    updateDtoRequest.SystemParamId = intSystemParamId;
                    updateDtoRequest.Code = systemFieldsDtoRequest.Code;
                    updateDtoRequest.Value = systemFieldsDtoRequest.Value;
                    updateDtoRequest.Coment = systemFieldsDtoRequest.Coment;
                    updateDtoRequest.UpdateUser = "Front"; // Cambiar por la varibale de session del usuario
                    //systemFieldsDtoRequest.SystemParamId = intSystemParamId;
                    var responseApi = await HttpClient.PostAsJsonAsync("params/SystemFields/UpdateSystemFields", updateDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SystemFieldsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                        await OnChangeData.InvokeAsync(true);
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                systemFieldsDtoRequest = new SystemFieldsDtoRequest();
            }
            else
            {
                systemFieldsDtoRequest = systemFieldsDtoRequestEdit;
            }
        }
        #endregion

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region ValidationMethods
        private void CountCharacters(ChangeEventArgs e, ref int charactersCounterVariable)
        {
            String value = e.Value.ToString() ?? String.Empty;


            if (!string.IsNullOrEmpty(value))
            {
                charactersCounterVariable = value.Length;

            }
            else
            {
                charactersCounterVariable = 0;
            }
        }
        private bool IsTextAreaValue(string value)
        {
            return Regex.IsMatch(value, "^[a-zA-Z\\s]+$");
        }
        #endregion

        #region ModalMethods
        public void UpdateParamId(int paramId)
        {
            intSystemParamId = paramId;
        }
        public void ReceiveRecord(SystemFieldsDtoResponse response)
        {
            IsEditForm = true;
            IsDisabledCode = true;
            systemFieldId = response.SystemFieldId.ToString();
            systemParamId = response.SystemParamId.ToString();
            systemFieldsDtoRequest.Value = response.Value;
            systemFieldsDtoRequest.Code = response.Code;
            systemFieldsDtoRequest.Coment = response.Coment;
            systemFieldsDtoRequestEdit = systemFieldsDtoRequest;
        }
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #endregion

        #endregion

    }
}
