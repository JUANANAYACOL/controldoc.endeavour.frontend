using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using DevExpress.Blazor.Primitives.Internal;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http.Json;
using static Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response.SystemFieldsDtoResponse;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayModal : ComponentBase
    {

        #region Variables
        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private FilingStateContainer? FilingSC { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals


        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }
        [Parameter] public EventCallback<bool> OnChangeDataValidation { get; set; }
        [Parameter] public EventCallback<int> ControlId { get; set; }





        #endregion

        #region Models

        #endregion

        #region Environments
        private bool habilitarTypeCode = false;
        private bool habilitarReason = false;

        public bool modalStatus = false;
        private bool seenControlid = false;
        private bool BtnVerificarDisabled = true;


        public string[] ext { get; set; } = { ".xlsx", ".pdf", ".docx", ".doc", ".xls", ".png", ".jpeg", ".gif" };
        private string typecode;
        private string reasonCode;
        private string TypeRequestCode;
        private string CancelationState = "TEA,PE";
        private string cancelationReasonId;
        private int Reason;
        private string controlId;

        private string DTTypeCode = "Selecciona un tipo...";
        private string DTTReasonCode = "Selecciona un usuario...";
        private string DTTReason = "Selecciona un motivo...";

        private string txtAInformation = "Funcionario asignado para validar solicitud";
        private string PHInput = "Ingrese el Documento";
        private NotificationsComponentModal notificationModal;
        private OverrideTrayValidationModal _ModalOverrideTrayValidation;

        private bool IsDisabledCode = false;
        private List<FileInfoData> fileInfoDatas = new List<FileInfoData>();
        private string archivo;
        private string fileExt;
        private string fileName;
        private string email;
        private int ManagerId;
        private string ShowTable = "d-none";
        private OverrideTrayManagerDtoRequest Request;


        private List<VSystemParamDtoResponse> TypeCodeList;
        private List<VSystemParamDtoResponse> ReasonCodeList;
        private List<OverrideTrayReasonDtoResponse> overrideTrayReasons;
        private OverrideTrayRequestDtoResponse _selectedRecord;
        private List<OverrideTrayManagerDtoResponse> UserManager;
        private List<VUserDtoResponse> userList;
        private List<int> ListId = new();
        private int intControlId = 0;




        private bool IsEditForm = false;

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetReasonCode();

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




        #region HandleFilesList

        private async Task HandleFilesList(List<FileInfoData> newList)
        {
            fileInfoDatas = newList;
            archivo = fileInfoDatas[0].Base64Data.ToString();
            fileExt = fileInfoDatas[0].Extension;
            fileName = fileInfoDatas[0].Name;


        }
        #endregion

        #region UpdateModalStatus
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #region UpdateSelectedRecord
        public void UpdateSelectedRecord(OverrideTrayRequestDtoResponse? record)
        {
            _selectedRecord = record;


        }
        #endregion

        #region HandleModalClosed
        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
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
            if (notificationModal.Type == ModalType.Error)
            {
                UpdateModalStatus(args.ModalStatus);
            }

        }
        #endregion

        #region GetAdmin
        private async Task GetAdmin()
        {
            OverrideTrayManagerDtoRequest manager = new();
            manager.TypeCode = typecode;
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/ByFilter", manager);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayManagerDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    UserManager = deserializeResponse.Data;
                    ManagerId = UserManager[0].UserId;
                    txtAInformation = "Funcionario asignado para validar solicitud \n ID: " + UserManager[0].UserId + "\n Gestor: " + UserManager[0].FullName + "\n Unidad: " + UserManager[0].ChargeName + "\n Oficina: " + UserManager[0].ChargeName;
                    await GetInfoUser();
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el usuario administrador" + ex.Message.ToString());
            }

        }
        #endregion

        #region GetInfoUser
        private async Task GetInfoUser()
        {
            VUserDtoRequest user = new();
            user.UserId = ManagerId;
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("generalviews/VUser/ByFilter", user);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    userList = deserializeResponse.Data;
                    email = userList[0].Email;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el usuario administrador" + ex.Message.ToString());
            }

        }
        #endregion

        #region GetTypeCode
        private async Task GetTypeCode()
        {

            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {

                    TypeCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                    habilitarReason = true;
                    await GetAdmin();

                }




            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el tipo: {ex.Message}");
            }
        }
        #endregion

        #region GetReasonCode
        private async Task GetReasonCode()
        {

            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {

                    ReasonCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                    habilitarTypeCode = true;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }
        #endregion

        #region GetReason

        private async Task GetReason()
        {
            try
            {
                OverrideTrayReasonDtoResponse reason = new();
                reason.TypeCode = string.IsNullOrEmpty(typecode) ? "" : typecode;
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/ByFilter", reason);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayReasonDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {

                    overrideTrayReasons = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayReasonDtoResponse>();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }
        #endregion

        #region reset
        public void reset()
        {
            DTTypeCode = "Selecciona un tipo...";
            DTTReasonCode = "Selecciona un usuario...";
            DTTReason = "Selecciona un motivo...";
            txtAInformation = "Funcionario asignado para validar solicitud";
            ShowTable = "d-none";
            PHInput = "Ingrese el Documento";
            ListId.Clear();
            StateHasChanged();

        }
        #endregion

        #region NewModalValidation
        public async void NewModalValidation()
        {
            int _controlId = !String.IsNullOrEmpty(controlId) ? Convert.ToInt32(controlId) : 0;

            if (_controlId > 0)
            {
                FilingSC.DocumentId = controlId;
                _ModalOverrideTrayValidation.UpdateModalStatus(true);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, $"¡Es necesario ingresar un id documento, por favor verifica!", true, "Aceptar", "Cancelar");

            }



        }
        #endregion

        #region CaptureIdControl
        public void CaptureIdControl()
        {
            if (!String.IsNullOrEmpty(controlId))
            {
                BtnVerificarDisabled = false;
            }
            else
            {
                BtnVerificarDisabled = true;
            }
        }
        #endregion



        #region PostRequest

        private async Task PostRequest()
        {
            try
            {
                if (ListId.Count > 1)
                {
                    List<OverrideTrayRequestDtoRequest> Requests = new();
                    OverrideTrayRequestDtoRequest RequestMasivo = new();
                    foreach (var item in ListId)
                    {
                        RequestMasivo.ControlId = item;
                        RequestMasivo.CancelationReasonId = int.Parse(cancelationReasonId);
                        RequestMasivo.TypeRequestCode = TypeRequestCode;
                        RequestMasivo.cancelationState = "TEA,PE";
                        RequestMasivo.TypeCode = typecode;
                        RequestMasivo.UserRequestId = 4055;
                        RequestMasivo.RequestComment = "";
                        RequestMasivo.Email = email;
                        RequestMasivo.User = "Prueba";


                        Requests.Add(RequestMasivo);
                    }

                    var responseApiMasivo = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/CreateCancelationRequests", Requests);
                    var deserializeResponseMasivo = await responseApiMasivo.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayReasonDtoRequest>>();
                    if (deserializeResponseMasivo.Succeeded)
                    {
                        //Logica Exitosa
                        notificationModal.UpdateModal(ModalType.Success, "¡Se creó la razón de forma exitosa!", true, "Aceptar");
                        await OnChangeData.InvokeAsync(true);
                        reset();
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear la razón, por favor intente de nuevo!", true, "Aceptar");
                        reset();

                    }
                }

                else
                {
                    OverrideTrayRequestDtoRequest Request = new();
                    Request.ControlId = int.Parse(controlId);
                    Request.CancelationReasonId = int.Parse(cancelationReasonId);
                    Request.TypeRequestCode = TypeRequestCode;
                    Request.cancelationState = "TEA,PE";
                    Request.TypeCode = typecode;
                    Request.UserRequestId = 4055;
                    Request.RequestComment = "";
                    Request.Email = email;
                    Request.User = "Prueba";

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/CreateCancelationRequest", Request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayReasonDtoRequest>>();
                    if (deserializeResponse.Succeeded)
                    {
                        //Logica Exitosa
                        notificationModal.UpdateModal(ModalType.Success, "¡Se creó la razón de forma exitosa!", true, "Aceptar");
                        await OnChangeData.InvokeAsync(true);
                        reset();
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear la razón, por favor intente de nuevo!", true, "Aceptar");
                        reset();

                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al guardar cancelationRequest: {ex.Message}");

            }



        }
        #endregion

        #region DeleteControlID
        private async Task DeleteControlID(int ControlId)
        {
            if (Convert.ToDecimal(ControlId) != 0)
            {


                ListId.Remove(ControlId);

            }
            if (ListId.Count == 0)
            {
                ShowTable = "d-none";
            }
        }
        #endregion

        #region AddListControlId
        private async Task AddListControlId()
        {
            ShowTable = "";
            if (Convert.ToDecimal(controlId) != 0)
            {
                if (ListId.Contains(Convert.ToInt32(controlId)))
                {
                    notificationModal.UpdateModal(ModalType.Warning, "¡El id de documento ya se encuentra asignado!", true, "Aceptar");

                }
                else
                {
                    ListId.Add(Convert.ToInt32(controlId));
                }
            }
        }
        #endregion
        #endregion

    }
}
