using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.UsersAdministration
{
    public partial class UserPermissionModal
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

        [Parameter] public EventCallback<CreatePermissionDtoRequest> OnSendPermission { get; set; }

        #endregion

        #region Models
        private CreatePermissionDtoRequest _permissionDtoRequest = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string Funcionalityname = string.Empty;
        #endregion

        #region Environments(Numeric)

        private int FuncionalityIdRecord = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool EnableSwitches = false;
        private bool SelectAccessF { get; set; }
        private bool SelectCreateF { get; set; }
        private bool SelectModifyF { get; set; }
        private bool SelectDeleteF { get; set; }
        private bool SelectConsultF { get; set; }
        private bool SelectPrintF { get; set; }
        private bool SelectActiveState { get; set; }

        private bool modalStatus = false;
        #endregion

        #region Environments(List & Dictionary)

        private List<FuncionalityDtoResponse> lstFuncionalities = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			
            try
            {
                EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetFunconalities();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            //PageLoadService.OcultarSpinnerReadLoad(Js);

        }


        #endregion

        #region Methods

        #region HandleMethods

        private void HandleModalClosed(bool status)
        {

            modalStatus = status;

            StateHasChanged();


        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnSendPermission.InvokeAsync(_permissionDtoRequest);
                UpdateModalStatus(args.ModalStatus);
                _permissionDtoRequest = new();
                FuncionalityIdRecord = 0;
                EnableSwitches = false;
                SelectAccessF = false;
                SelectCreateF = false;
                SelectModifyF = false;
                SelectDeleteF = false;
                SelectConsultF = false;
                SelectPrintF = false;
                SelectActiveState = false;
            }


        }

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region Get Data Methods

        private async Task GetFunconalities()
        {
            try
            {
                FuncionalityFilterDtoResponse funcionalityFilterDto = new();
                var responseApi = await HttpClient.PostAsJsonAsync("permission/Functionality/ByFilter", funcionalityFilterDto);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<FuncionalityDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstFuncionalities = deserializeResponse.Data;
                }
                else
                {
                    lstFuncionalities = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las funcionalidades, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obtaining data: {ex.Message}");
            }
        }
        #endregion

        #region DropDown Methods

        private void FuncionalityChanged(int id)
        {

            FuncionalityDtoResponse selectedFuncionality = lstFuncionalities.FirstOrDefault(f => f.FunctionalityId == id);
            Funcionalityname = selectedFuncionality.Functionality1;
            FuncionalityIdRecord = id;
            EnableSwitches = true;
        }

        #endregion

        #region ModalMethods

        private async Task SavePermission()
        {
            if (EnableSwitches)
            {
                _permissionDtoRequest.AccessF = SelectAccessF;
                _permissionDtoRequest.CreateF = SelectCreateF;
                _permissionDtoRequest.ModifyF = SelectModifyF;
                _permissionDtoRequest.PrintF = SelectPrintF;
                _permissionDtoRequest.DeleteF = SelectDeleteF;
                _permissionDtoRequest.ConsultF = SelectConsultF;
                _permissionDtoRequest.ActiveState = SelectActiveState;
                _permissionDtoRequest.FunctionalityId = FuncionalityIdRecord;
                _permissionDtoRequest.FunctionalityName = Funcionalityname;
                
                notificationModal.UpdateModal(ModalType.Success, "¡Permiso agregado correctamente!", true);


            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Seleccione una funcionalidad!", true);
            }


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
