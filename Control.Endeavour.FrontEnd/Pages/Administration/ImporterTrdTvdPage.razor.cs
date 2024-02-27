using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ImporterTrdTvd;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ImporterTrdTvdPage
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
        private NotificationsComponentModal notificationModal = new();
        private ImporterTrdTvdModal modalImporterTrdTvd = new();
        private ImportResultModal modalImportResult = new();
        #endregion

        #region Parameters


        #endregion

        #region Models
        private ImporterHistoryDtoRequest? FilterDtoRequest { get; set; } = new();
        private PaginationComponent<ImporterHistoryDtoResponse, ImporterHistoryDtoRequest> PaginationComponet = new();
        private MetaModel? meta { get; set; } = new() { PageSize = 10 };
        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)
        private bool dataChargue { get; set; } = false;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        private List<ImporterHistoryDtoResponse> importerHistoryList = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetImporterHistory();
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region GetImporterHistory

        private async Task GetImporterHistory()
        {
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ImporterHistory/ByFilter", FilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ImporterHistoryDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    importerHistoryList = deserializeResponse.Data ?? new();
                    meta = deserializeResponse!.Meta;
                    dataChargue = true;
                    PaginationComponet.ResetPagination(meta!);
                }
                else
                {
                    importerHistoryList = new();
                    meta = new() { PageSize = 10 };
                    dataChargue = false;
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar el Historial de Importaciones de TRD y TVD, por favor intente de nuevo!", true);
                }
            }
            catch
            {
                importerHistoryList = new();
                meta = new() { PageSize = 10 };
                dataChargue = false;
                notificationModal.UpdateModal(ModalType.Error, "¡La Version Documental seleccionada no posee importaciones!", true);
            }
        }

        #endregion GetImporterHistory

        #region HandlePagination

        private void HandlePaginationGrid(List<ImporterHistoryDtoResponse> newDataList)
        {
            importerHistoryList = newDataList;
        }

        #endregion HandlePagination

        #region HandleStatusChanged
        private void HandleStatusChanged(bool status)
        {
            modalImporterTrdTvd.UpdateModalStatus(status);
        }
        #endregion HandleStatusChanged

        #region ShowModal
        private void ShowModal()
        {
            modalImporterTrdTvd.UpdateModalStatus(true);
        }

        #endregion ShowModal

        #region OpenNewModal
        private void OpenNewModal(ImporterDtoResponse response)
        {
            modalImportResult.GetData(response);
            modalImportResult.UpdateModalStatus(true);

        }
        #endregion OpenNewModal

        #region HandleRefreshGridData
        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetImporterHistory();
        }
        #endregion HandleRefreshGridData

        #region HandleModalClosed
        private void HandleModalClosed(bool status)
        {
            modalImporterTrdTvd.UpdateModalStatus(status);
            modalImportResult.UpdateModalStatus(status);
            modalImporterTrdTvd.ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #endregion

        #endregion

    }
}
