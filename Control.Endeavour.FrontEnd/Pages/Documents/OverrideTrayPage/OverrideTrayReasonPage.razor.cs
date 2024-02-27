using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Documents.OverrideTrayPage
{
    public partial class OverrideTrayReasonPage
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

        private OverrideTrayReasonModal modalReason;

        #endregion

        #region Parameters


        #endregion

        #region Models
        private OverrideTrayReasonDtoResponse recordToDelete;

        #endregion

        #region Environments
        private List<OverrideTrayReasonDtoResponse> ReasonList;

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            await GetReason();
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
        #region ShowModalEdit
        private async Task ShowModalEdit(OverrideTrayReasonDtoResponse args)
        {
            modalReason.UpdateModalStatus(true);
            modalReason.UpdateSelectedRecord(args);
            modalReason.Temp = false;

        }
        #endregion

        #region ShowModal
        private async Task ShowModal()
        {
            
            
            modalReason.UpdateModalStatus(true);
            modalReason.reset();
            modalReason.Temp = true;
        }
        #endregion

        #region ShowModalDelete
        private void ShowModalDelete(OverrideTrayReasonDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el permiso?", true, "Si", "No", modalOrigin: "DeleteModal");
        }
        #endregion


        #region GetReason
        private async Task GetReason()
        {
            try
            {
                OverrideTrayReasonDtoResponse reason = new();

                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/ByFilter", reason);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayReasonDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    ReasonList= deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayReasonDtoResponse>();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las Perfiles: {ex.Message}");
            }
        }
        #endregion

        #region HandleRefreshGridDataAsync
        public async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetReason();
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
                    DeleteManager.Id = recordToDelete.CancelationReasonId;
                    DeleteManager.User = "Admin";

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/DeleteCancelationReason", DeleteManager);
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
