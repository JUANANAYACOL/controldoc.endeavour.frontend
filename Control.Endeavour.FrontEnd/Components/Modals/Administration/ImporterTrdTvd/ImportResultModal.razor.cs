using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ImporterTrdTvd
{
    public partial class ImportResultModal : ComponentBase
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

        #endregion

        #region Parameters
        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> CloseModals { get; set; }

        [Parameter]
        public bool modalStatus { get; set; } = false;
        #endregion

        #region Models
        private ImporterDtoResponse importerResponse { get; set; } = new();
        #endregion

        #region Environments

        #region Environments(Numeric)
        private int total = 0;
        #endregion

        #region Environments(Bool)
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

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

        #region TotalRecords

        public void TotalRecords()
        {
            total = importerResponse.contUnity + importerResponse.contOffice + importerResponse.contSeries +
               importerResponse.contSubseries + importerResponse.contTypologies + importerResponse.contRetentions +
               importerResponse.contTRD + importerResponse.contTRDC;
        }

        #endregion TotalRecords

        #region UpdateModalStatus
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion UpdateModalStatus

        #region HandleModalClosed
        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;
            await CloseModals.InvokeAsync(false);
            ResetFormAsync();
            StateHasChanged();
        }
        #endregion HandleModalClosed

        #region ResetFormAsync
        private void ResetFormAsync()
        {
            total = 0;
            importerResponse = new();
        }
        #endregion ResetFormAsync

        #region GetData
        public void GetData(ImporterDtoResponse data)
        {
            importerResponse = data;
            StateHasChanged();
        }
        #endregion GetData

        #endregion

        #endregion

    }
}
