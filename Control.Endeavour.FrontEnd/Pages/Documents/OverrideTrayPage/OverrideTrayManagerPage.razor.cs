using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Documents.OverrideTrayPage
{
    public partial class OverrideTrayManagerPage
    {

		#region Variables
		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private NotificationsComponentModal notificationModalSucces;
        private NotificationsComponentModal notificationModal;

        #endregion

        #region Modals
        private OverrideTrayManagerModal ModalManager;

        #endregion

        #region Parameters


        #endregion

        #region Models
        private List<OverrideTrayManagerDtoResponse> ManagerList;
        private OverrideTrayManagerDtoResponse recordToDelete;

        #endregion

        #region Environments
        private bool crear = true;
        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            await GetManager();
			EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

		}


        #endregion

        #region Methods

        #region ShowModalEdit
        private async Task ShowModalEdit(OverrideTrayManagerDtoResponse args)
        {
            ModalManager.UpdateModalStatus(true);
            ModalManager.UpdateSelectedRecord(args);

        }
        #endregion

        #region ShowModal
        private async Task ShowModal()
        {
            if (!crear)
            {
                ModalManager.UpdateModalStatus(true);
            }

        }
        #endregion

        #region ShowModalDelete
        private void ShowModalDelete(OverrideTrayManagerDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el permiso?", true, "Si", "No", modalOrigin: "DeleteModal");
        }
        #endregion

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region GetManager
        private async Task GetManager()
        {
            try
            {
                OverrideTrayManagerDtoResponse overrideTrayManagerDtoResponse = new();
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/ByFilter", overrideTrayManagerDtoResponse);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayManagerDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    ManagerList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayManagerDtoResponse>();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las administradores: {ex.Message}");
            }
        }
        #endregion

        #region HandleRefreshGridDataAsync
        public async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetManager();
        }
        #endregion

        #region HandleModalNotiClose
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
            {
                if (recordToDelete != null)
                {
                    DeleteGeneralDtoRequest DeleteManager = new();
                    DeleteManager.Id = recordToDelete.CancelationManagerId;
                    DeleteManager.User = "Admin";

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/DeleteCancelationManager", DeleteManager);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true);
                        }
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el administrador, por favor intente de nuevo!", true, "Aceptar");
                    }
                    await HandleRefreshGridDataAsync(true);

                }
            }
            else
            {
                Console.WriteLine("Registro No eliminado");
            }


        }
        #endregion

        #endregion

    }
}
