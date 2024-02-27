using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.BranchOffice;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class BranchOfficesPage
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private PaginationComponent<BranchOfficesDtoResponse, BranchOfficeFilterDtoRequest> PaginationComponet { get; set; } = new();

        #endregion Components

        #region Modals

        private AddressModal modalAddress { get; set; } = new();
        private BranchOfficesModal modalbranchOffice { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Models

        private List<BranchOfficesDtoResponse> branchOfficesList { get; set; } = new();

        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        private BranchOfficeFilterDtoRequest? FilterDtoRequest { get; set; } = new();

        private BranchOfficesDtoResponse recordToDelete { get; set; } = new();

        #endregion Models

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetBranchsOffices();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OthersMethods

        #region GetBranchsOffices

        // Método para obtener la lista de sucursales.
        private async Task GetBranchsOffices()
        {
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("params/BranchOffice/ByFilter", FilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<BranchOfficesDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    branchOfficesList = deserializeResponse.Data;
                    meta = deserializeResponse.Meta;
                    PaginationComponet.ResetPagination(meta);
                }
                else
                {
                    branchOfficesList = new List<BranchOfficesDtoResponse>();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las oficinas productoras, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de fallo al obtener las sucursales.
                Console.WriteLine($"Error al obtener las sucursales: {ex.Message}");
            }
        }

        #endregion GetBranchsOffices

        #region HandleId

        private async Task HandleId(int id)
        {
            await modalAddress.UpdateModalIdAsync(id);
        }

        #endregion HandleId

        #region ShowModalEdit

        private void ShowModalEdit(BranchOfficesDtoResponse record)
        {
            modalbranchOffice.UpdateModalStatus(true);
            modalbranchOffice.recieveBranchOffice(record);
        }

        #endregion ShowModalEdit

        #region ShowModalDelete

        private void ShowModalDelete(BranchOfficesDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el mensaje?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        #endregion ShowModalDelete

        #region ShowModalCreate

        private void ShowModalCreate()
        {
            modalbranchOffice.OpenCreateModal();
            modalAddress.ResetFormAsync();
        }

        #endregion ShowModalCreate

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.BranchOfficeId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient!.PostAsJsonAsync("params/BranchOffice/DeleteBranchOffice", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        await GetBranchsOffices();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true);
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

        #endregion HandleModalNotiClose

        #region HandleRefreshGridDataAsync

        private async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetBranchsOffices();
        }

        #endregion HandleRefreshGridDataAsync

        #region HandleAddressModal

        private void HandleAddressModal(bool newValue)
        {
            modalAddress.UpdateModalStatus(newValue);
        }

        #endregion HandleAddressModal

        #region HandleUserSelectedChanged

        private void HandleUserSelectedChanged(MyEventArgs<List<(string, AddressDtoRequest)>> address)
        {
            modalbranchOffice.updateAddressSelection(address.Data!);
            modalAddress.UpdateModalStatus(address.ModalStatus);
        }

        #endregion HandleUserSelectedChanged

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<BranchOfficesDtoResponse> newDataList)
        {
            branchOfficesList = newDataList;
        }

        #endregion HandlePaginationGrid

        #endregion OthersMethods

        #endregion Methods
    }
}