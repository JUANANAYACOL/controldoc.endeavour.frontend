using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class SeriesPage
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private PaginationComponent<SeriesDtoResponse, SeriesFilterDtoRequest> PaginationComponet = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private SeriesModal modalSeries = new();

        #endregion Modals

        #region Models

        private MetaModel seriesMeta = new();
        private SeriesDtoResponse recordToDelete = new();
        private SeriesFilterDtoRequest seriesFilterDtoRequest = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string NameproOffice = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private int IdproOffice { get; set; }

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool isEnabled = true;
        private bool dataChargue;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<ProductionOfficesDtoResponse> productionOfficesList = new();
        private List<SeriesDtoResponse> seriesList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetProductionOffices();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleChangedData(bool changed)
        {
            await OnDropDownValueChanged(IdproOffice);
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.SeriesId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/Series/DeleteSeries", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await OnDropDownValueChanged(IdproOffice);
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        private void HandlePaginationGrid(List<SeriesDtoResponse> newDataList)
        {
            seriesList = newDataList;
        }

        #region Language

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion Language

        #endregion HandleMethods

        #region OthersMethods

        #region Modals

        private async Task ShowModalCreate(bool newvalue)
        {
            modalSeries.UpdateModalStatus(true);
        }

        private void ShowModalEdit(SeriesDtoResponse record)
        {
            modalSeries.UpdateModalStatus(true);
            modalSeries.UpdateSelectedRecord(record);
        }

        private void ShowModalDelete(SeriesDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar la serie documental?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        #endregion Modals

        #region GetDataMethods

        private async Task GetProductionOffices()
        {
            try
            {
                ProductionOfficeFilterDtoRequest officeFilterDtoRequest = new();
                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/ProductionOffice/ByFilter", officeFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    productionOfficesList = deserializeResponse.Data;
                }
                else
                {
                    productionOfficesList = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las versiones documentales, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        private async Task OnDropDownValueChanged(int newValue)
        {
            //PageLoadService.MostrarSpinnerReadLoad(Js);
            seriesList = new();
            IdproOffice = newValue;
            isEnabled = ( IdproOffice <= 0 ) ? true : false;
            seriesFilterDtoRequest.ProductionOfficeId = IdproOffice;
            NameproOffice = productionOfficesList.Where(x => x.ProductionOfficeId == IdproOffice).Select(x => x.Name).FirstOrDefault();

            var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/Series/ByFilter", seriesFilterDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>();
            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                seriesList = deserializeResponse.Data;
                seriesMeta = deserializeResponse.Meta;
                PaginationComponet.ResetPagination(seriesMeta);

                dataChargue = true;
            }
            else
            {
                seriesList = new();
                seriesMeta = new();
                notificationModal.UpdateModal(ModalType.Error, "¡No hay registros, por favor seleccione otro valor!", true);
            }
        }

        #endregion GetDataMethods

        #endregion OthersMethods

        #endregion Methods
    }
}