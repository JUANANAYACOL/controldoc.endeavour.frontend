using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using DevExpress.XtraPrinting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Net.ConnectCode.Barcode;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.Label
{
    public partial class LabelModal
    {

        #region Variables

        #region Inject 
        [Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private FilingStateContainer? FilingSC { get; set; }
        [Inject] private IJSRuntime? Js { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals


        #endregion

        #region Parameters
        [Parameter] public string Width { get; set; } = "20%";
        [Parameter] public bool ModalStatus { get; set; } = false;
        [Parameter] public EventCallback<bool> OnModalClosed { get; set; }
        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)
        private string RadicadoTitle = "";
        private string CodigoBarras = "";

        private string FilingNumber = "";
        private string DocumentId = "";
        private string Recipients = "";
        private string Folios = "";
        private string Annexes = "";
        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(List & Dictionary)

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

        private void HandleModalClosed(bool status)
        {
            ModalStatus = status;
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region ModalActionsMethods

        public void UpdateModalStatus(bool newValue)
        {
            ModalStatus = newValue;
            if (!string.IsNullOrEmpty(FilingSC?.FilingNumber) && ModalStatus)
            {
                FilingNumber = FilingSC.FilingNumber;
                DocumentId = FilingSC.DocumentId;
                Recipients = FilingSC.Recipients;
                Folios = FilingSC.Folios;
                Annexes = FilingSC.Annexes;
                RadicadoTitle = "Rótulo del radicado: " + FilingNumber;
                GenerateBarcode();
            }
            StateHasChanged();
        }
        private async Task CloseModal()
        {
            await OnModalClosed.InvokeAsync(false);
        }
        #endregion

        #region GenerateBarcode

        private void GenerateBarcode()
        {
            BarcodeFonts bcf = new BarcodeFonts();
            bcf.BarcodeType = BarcodeFonts.BarcodeEnum.Code39;
            bcf.CheckDigit = BarcodeFonts.YesNoEnum.Yes;
            bcf.Data = FilingNumber;
            bcf.encode();
            //CodigoBarras = bcf.EncodedData;
            CodigoBarras = bcf.Data;


        }

        #endregion

        private async Task PrintDiv()
        {
            await Js.InvokeVoidAsync("printDiv", "contentToPrint");
        }

        #endregion

        #endregion

    }
}
