using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class SubSeriesPage
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
        private PaginationComponent<SubSeriesDtoResponse, SubSeriesFilterDtoRequest> PaginationComponet = new();

        #endregion

        #region Modals

        private SubSeriesModal modalSubseries = new();
        private NotificationsComponentModal notificationModal = new();
        #endregion



        #region Models
        private MetaModel meta = new();
        private MetaModel metasubSeries = new();
        private SubSeriesFilterDtoRequest SubSeriesFilterDtoRequest = new();
        private SubSeriesDtoResponse recordToDelete = new();
        #endregion

        #region Environments

        #region Environments(String)
        private string NameSerie = string.Empty;
        #endregion

        #region Environments(Numeric)
        private int IdSerie { get; set; }
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool isEnabled = true;
        private bool dataChargue = false;
        #endregion

        #region Environments(List & Dictionary)
        private List<SubSeriesDtoResponse> subSeriesList = new();
        private List<SeriesDtoResponse> seriesList = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetSeries();
        }


        #endregion

        #region Methods

        #region HandleMethods
        private async Task HandleChangedData(bool changed)
        {
            await OnDropDownValueChanged(IdSerie);
            
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.SubSeriesId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/SubSeries/DeleteSubSerie", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await OnDropDownValueChanged(IdSerie);
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
        private void HandlePaginationGrid(List<SubSeriesDtoResponse> newDataList)
        {
            subSeriesList = newDataList;
            
        }

        #region HandleLanguageChanged
        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion
        #endregion

        #region OthersMethods

        #region GetDataMethods

        private async Task GetSeries()
        {
            try
            {
                SeriesFilterDtoRequest filterSeries = new();
                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/Series/ByFilter", filterSeries);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    seriesList = deserializeResponse.Data;
                }
                else
                {
                    seriesList = new();
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
            try
            {
                //PageLoadService.MostrarSpinnerReadLoad(Js);
                
                IdSerie = newValue;
                isEnabled = (IdSerie <= 0) ? true : false;
                SubSeriesFilterDtoRequest.ProductionOfficeId = IdSerie;
                NameSerie = seriesList.Where(x => x.ProductionOfficeId == IdSerie).Select(x => x.Name).FirstOrDefault();

                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/SubSeries/ByFilter", SubSeriesFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SubSeriesDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    subSeriesList = deserializeResponse.Data;
                    metasubSeries = deserializeResponse.Meta;
                    PaginationComponet.ResetPagination(metasubSeries);

                    dataChargue = true;
                }
                else
                {
                    subSeriesList = new();
                    metasubSeries = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡No hay registros, por favor seleccione otro valor!", true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }


        }
        #endregion

        #region ModalMethods
        private async Task ShowModalCreate()
        {
            
            modalSubseries.UpdateModalStatus(true);
        }

        private void ShowModalEdit(SubSeriesDtoResponse record)
        {
            modalSubseries.UpdateModalStatus(true);
            modalSubseries.UpdateSelectedRecord(record);
        }

        private void ShowModalDelete(SubSeriesDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el registro?", true, "Si", "No", modalOrigin: "DeleteModal");
        }
        #endregion

        #endregion

        #endregion

    }
}
