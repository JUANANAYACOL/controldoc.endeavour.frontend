using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayManagerModal: ComponentBase
    {

		#region Variables
		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components

        private NotificationsComponentModal notificationModal;

        #endregion

        #region Modals
        private TelerikDropDownList<VUserDtoResponse, int> DLUser;
        private TelerikDropDownList<VSystemParamDtoResponse, string> DLTypeCode;



        #endregion

        #region Parameters
        [Parameter] public bool modalStatus { get; set; } = false;
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        private int UserId;
        private string TypeCode;
        private string DFUser;
        private string DFCode;
        private bool Habilitar=false;
        private bool HabilitarUser = true;

        #endregion

        #region Models
        private OverrideTrayManagerDtoResponse _selectedRecord;



        #endregion

        #region Environments
        private List<VUserDtoResponse> UserList;
        private List<VSystemParamDtoResponse> TypeCodeList;
        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            await GetUsers();
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

        #region UpdateModalStatus
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            UserList.Count();
            StateHasChanged();
        }
        #endregion

        #region GetUsers
        private async Task GetUsers()
        {
            try
            {
                VUserDtoResponse user = new();
                var responseApi = await HttpClient.PostAsJsonAsync("security/User/ByFilter", user);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    UserList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VUserDtoResponse>();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
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
        public void UpdateSelectedRecord(OverrideTrayManagerDtoResponse? record)
        {
            _selectedRecord = record;
            HabilitarUser = false;
            DFUser= _selectedRecord.NameUser;
            DFCode= _selectedRecord.NameTypeCode;
            DLUser.Refresh();
            DLTypeCode.Refresh();

        }
        #endregion

        #region Reset
        private void reset()
        {
            DFUser = "Selecciona un usuario...";
            DFCode = "Selecciona un tipo...";
            DLUser.Refresh();
            DLTypeCode.Refresh();
        }
        #endregion

        #region PostManager
        private async Task PostManager()
        {
            OverrideTrayManagerDtoRequest Manager = new();
            Manager.UserId = UserId;
            Manager.TypeCode = TypeCode;
            Manager.User = "Admin";

            var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/CreateCancelationManager", Manager);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayManagerDtoRequest>>();
            if (deserializeResponse.Succeeded)
            {
                //Logica Exitosa
                notificationModal.UpdateModal(ModalType.Success, "¡Se creó el administrador de forma exitosa!", true, "Aceptar");
                await OnChangeData.InvokeAsync(true);
                reset();
            }
            else
            {
                //Logica no Exitosa
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el administrador, por favor intente de nuevo!", true, "Aceptar");
                reset();

            }


        }

        #endregion

        #region PutManager

        private async Task PutManager()
        {
            OverrideTrayManagerEditDtoRequest ManagerEdit = new();
            ManagerEdit.CancelationManagerId = _selectedRecord.CancelationManagerId;
            ManagerEdit.UserId = _selectedRecord.UserId;
            ManagerEdit.TypeCode = TypeCode;
            ManagerEdit.User = "Admin";


            var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/UpdateCancelationManager", ManagerEdit);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayManagerEditDtoRequest>>();
            if (deserializeResponse.Succeeded)
            {
                //Logica Exitosa
                notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el administrador de forma exitosa!", true, "Aceptar");
                await OnChangeData.InvokeAsync(true);
                reset();

            }
            else
            {
                //Logica no Exitosa
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el administrador, por favor intente de nuevo!", true, "Aceptar");
                reset();


            }

        }

        #endregion

        #region Save
        private async Task Save()
        {
            try
            {
                if (Habilitar == true)
                {
                    PostManager();
                }
                else
                {
                    PutManager();

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
