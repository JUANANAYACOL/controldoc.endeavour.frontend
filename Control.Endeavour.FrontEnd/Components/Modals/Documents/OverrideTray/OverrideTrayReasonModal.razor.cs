using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayReasonModal
    {

        #region Variables
        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        InputModalComponent InputId;
        InputModalComponent InputNombre;

        #endregion

        #region Modals


        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }


        #endregion

        #region Models
        private OverrideTrayReasonDtoRequest overrideTrayReasonDtoRequest = new();
        #endregion

        #region Environments
        private string NameReason;
        private int id;
        private string ReasonCode;
        private string TypeCode;
        private bool Habilitar;
        private bool modalStatus;
        public bool Temp;
        //DefaultText
        private string placeHolderDefault = "Nombre";
        private string PHID = "";
        private string DTReason = "Selecciona un motivo...";
        private string DTTypeCode = "Selecciona un tipo...";

        private List<VSystemParamDtoResponse> TypeCodeList;
        private List<VSystemParamDtoResponse> ReasonCodeList;
        private List<OverrideTrayReasonDtoResponse> ReasonList;
        private NotificationsComponentModal notificationModal;
        private OverrideTrayReasonDtoResponse _selectedRecord;
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

        #region MyRegion
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
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
                    Habilitar = true;

                    TypeCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
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

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
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

        #region UpdateSelectedRecord
        public void UpdateSelectedRecord(OverrideTrayReasonDtoResponse? record)
        {
            _selectedRecord = record;
            Temp=Habilitar==true?false:true;
            PHID =  _selectedRecord.CancelationReasonId.ToString();
            placeHolderDefault = _selectedRecord.NameReason;
            DTReason = _selectedRecord.NameReasonCode;
            DTTypeCode = _selectedRecord.NameTypeCode;


        }
        #endregion

        #region Reset
        public void reset()
        {
            DTReason = "Selecciona un motivo...";
            DTTypeCode = "Selecciona un tipo...";
            placeHolderDefault = "Nombre";
            PHID = "";

        }
        #endregion

        #region PostReason
        private async Task PostReason()
        {
            OverrideTrayReasonDtoRequest Reason = new();
            Reason.NameReason = NameReason;
            Reason.ReasonCode = ReasonCode;
            Reason.TypeCode = TypeCode;
            Reason.User = "Admin";

            var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/CreateCancelationReason", Reason);
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

        #endregion

        #region PutReason

        private async Task PutReason()
        {
            OverrideTrayReasonEditDtoRequest ReasonEdit = new();
            ReasonEdit.CancelationReasonId = _selectedRecord.CancelationReasonId;
            Console.WriteLine(_selectedRecord.CancelationReasonId);
            ReasonEdit.NameReason = _selectedRecord.NameReason;
            ReasonEdit.ReasonCode = ReasonCode == null?_selectedRecord.ReasonCode:ReasonCode;
            ReasonEdit.TypeCode = TypeCode == null ? _selectedRecord.TypeCode:TypeCode;
            ReasonEdit.User = "Admin";


            var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/UpdateCancelationReason", ReasonEdit);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayReasonEditDtoRequest>>();
            if (deserializeResponse.Succeeded)
            {
                //Logica Exitosa
                notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó la razón de forma exitosa!", true, "Aceptar");
                await OnChangeData.InvokeAsync(true);
                reset();

            }
            else
            {
                //Logica no Exitosa
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar la razón, por favor intente de nuevo!", true, "Aceptar");
                reset();


            }

        }

        #endregion

        #region Save
        private async Task Save()
        {
            try
            {
                if (Temp == true)
                {
                    PostReason();
                }
                else
                {
                    PutReason();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar un administrador {ex.Message}");
            }
        }
        #endregion

        #endregion

    }
}
