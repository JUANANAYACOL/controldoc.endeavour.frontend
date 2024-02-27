using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.DataSource;


namespace Control.Endeavour.FrontEnd.Pages.Administration
{
	public partial class ActionsAdministration
	{

		#region Variables

		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components

        

        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal  = new();
        private ActionsModal actionsModal  = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private ActionsDtoResponse recordToDelete = new();
        private ActionsFilterDtoRequest actionsFilterDtoRequest = new();


		#endregion

		#region Environments

		#region Environments(String)

		#endregion

		#region Environments(Numeric)

		#endregion

		#region Environments(DateTime)

		#endregion

		#region Environments(Bool)

		#endregion

		#region Environments(List & Dictionary)
		private List<ActionsDtoResponse> actionsDataList = new();

		#endregion

		#endregion

		#endregion

		#region OnInitializedAsync
		protected override async Task OnInitializedAsync()
		{
			EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetActions();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
                

		}


		#endregion

		#region Methods

		#region HandleMethods

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetActions();
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = (recordToDelete.ActionId.HasValue) ? recordToDelete.ActionId.Value : 0;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Actions/DeleteActions", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetActions();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado la acción!", true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        #endregion

        #region OthersMethods

        #region ModalMethods

        private void ShowModalCreate()
        {
            actionsModal.UpdateModalStatus(true);
        }
        private void ShowModalEdit(ActionsDtoResponse record)
        {
            actionsModal.UpdateModalStatus(true);
            actionsModal.ReceiveRecord(record);
        }
        private void ShowModalDelete(ActionsDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el mensaje?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        #endregion

        #region GetData

        // Método para obtener la lista de sucursales.
        private async Task GetActions()
        {
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/Actions/ByFilter", actionsFilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ActionsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    actionsDataList = deserializeResponse.Data;
                }
                else
                {
                    actionsDataList = new List<ActionsDtoResponse>();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las acciones , por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de fallo al obtener las sucursales.
                Console.WriteLine($"Error al obtener las sucursales: {ex.Message}");
            }
        }

        #endregion 






        #endregion

        #endregion

    }
}
