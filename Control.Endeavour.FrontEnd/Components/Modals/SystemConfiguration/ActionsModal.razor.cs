using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;


namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class ActionsModal
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

        
        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }


        #endregion

        #region Models
        private ActionsDtoResponse actionsDtoResponse = new();
        private ActionsDtoResponse actionsDtoResponseEdit = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string OriginText = "Seleccione el origen";
        private string ClassNameText = "Seleccione la clase";
        private string FlowStateText = "Seleccione el estado de flujo";

        #endregion

        #region Environments(Numeric)


        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool IsEditForm = false;
        private bool modalStatus = false;
        
        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> classCodeList = new();
        private List<VSystemParamDtoResponse> flowStateList = new();
        private List<VSystemParamDtoResponse> originList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetClasses();
                await GetFlowState();
                await GetOrigin();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la inicialización: {ex.Message}");
            }

        }


        #endregion

        #region Methods

        #region HandleMethods
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;
            actionsDtoResponse = new ();
            OriginText = "Seleccione el origen";
            ClassNameText = "Seleccione la clase";
            FlowStateText = "Seleccione el estado de flujo";
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
                if (ValidateFields())
                {
                    actionsDtoResponse.CreateUser = "Front"; //cambiar por variable de usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Actions/CreateActions", actionsDtoResponse);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ActionsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                        await OnChangeData.InvokeAsync(true);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Success, "¡No se creó el registro de forma exitosa!", true, "Aceptar");
                    }
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
                if (ValidateFields())
                {
                    actionsDtoResponse.UpdateUser = "Front"; //cambiar por variable de usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Actions/UpdateActions", actionsDtoResponse);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ActionsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                        await OnChangeData.InvokeAsync(true);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Success, "¡No se actualizó el registro de forma exitosa!", true, "Aceptar");
                    }
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
                actionsDtoResponse = new();
            }
            else
            {
                actionsDtoResponse = actionsDtoResponseEdit;
            }
        }
        #endregion

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region ValidateFields
        private bool ValidateFields()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(actionsDtoResponse.Name))
            {
                errors.Add("Nombre");
            }

            if (string.IsNullOrEmpty(actionsDtoResponse.ClassCode))
            {
                errors.Add("Clase");
            }

            if (string.IsNullOrEmpty(actionsDtoResponse.FlowStateCode))
            {
                errors.Add("Estado de flujo");
            }

            if (string.IsNullOrEmpty(actionsDtoResponse.Origin))
            {
                errors.Add("Origen");
            }

            if (errors.Count > 0)
            {
                var message = string.Join(", ", errors);
                notificationModal.UpdateModal(ModalType.Error, $"Los siguientes campos son obligatorios: {message}", true);
                return false;
            }

            return true;
        }

        #endregion

        #region GetData

        private async Task GetClasses()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    classCodeList = deserializeResponse.Data;
                }
                else { classCodeList = new(); }

            }
            catch (Exception ex)
            {
               
                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los tipos de clase: {ex.Message}", true);
            }
        }
        private async Task GetOrigin()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "ORI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    originList = deserializeResponse.Data;
                }
                else { originList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los tipos de origen", true);
            }
        }

        private async Task GetFlowState()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "ES");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    flowStateList = deserializeResponse.Data;
                }
                else { flowStateList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los tipos de estado de flujo", true);
            }
        }

        #endregion

        

        #region ModalMethods
        
        public async void ReceiveRecord(ActionsDtoResponse response)
        {
            IsEditForm = true;
            actionsDtoResponse.ActionId = response.ActionId;
            actionsDtoResponse.Name = response.Name;
            actionsDtoResponse.FlowStateCode = response.FlowStateCode;
            actionsDtoResponse.ClassCode = response.ClassCode;
            actionsDtoResponse.Origin = response.Origin;
            OriginText = response.OriginName;
            ClassNameText = response.ClassCodeName;
            FlowStateText = response.FlowStateName;
            
            actionsDtoResponseEdit = actionsDtoResponse;
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
