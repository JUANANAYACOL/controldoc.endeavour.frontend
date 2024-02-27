using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Document.DocumentaryTask
{
    public partial class FormatMasterListModal
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components

        private NotificationsComponentModal notificationModal;
        #endregion

        #region Modals

        public InputModalComponent inputCode = new();
        public InputModalComponent inputName = new();
        public InputModalComponent inputVersion = new();
        public InputModalComponent inputProcess = new();

        

        #endregion

        #region Parameters
        [Parameter] public EventCallback<MyEventArgs<TemplateModelDtoResponse>> OnStatusChanged { get; set; }
        #endregion

        #region Models

        private TemplateDocumentDtoResponse templateDocumentDto = new();
        private TemplateFilterDtoRequest filter = new() { Type = "TFOR,DTXT" };
        private TemplateDocumentDtoResponse BasicTemplate = new();

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<TemplateDocumentDtoResponse> templateDocumentList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetTemplateDoc();
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
            modalStatus = status;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("LoadFile") && templateDocumentDto != null)
            {

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", templateDocumentDto.FileId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");

                TemplateModelDtoResponse sendModel = new() { TemplateId = templateDocumentDto.TemplateId, Archive = deserializeResponse.Data.DataFile };

                var eventArgs = new MyEventArgs<TemplateModelDtoResponse>
                {
                    Data = sendModel,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(eventArgs);

            }

        }

        #endregion

        #region OthersMethods

        private async Task GetTemplateDoc()
        {
            filter.Code = string.IsNullOrEmpty(inputCode.InputValue) ? "" : inputCode.InputValue;
            filter.NameTemplate = string.IsNullOrEmpty(inputName.InputValue) ? "" : inputName.InputValue;
            filter.Version = string.IsNullOrEmpty(inputVersion.InputValue) ? 0 : int.Parse(inputVersion.InputValue);
            filter.Process = string.IsNullOrEmpty(inputProcess.InputValue) ? "" : inputProcess.InputValue;

            var responseApi = await HttpClient.PostAsJsonAsync("documents/TemplateDocuments/ByFilter", filter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<TemplateDocumentDtoResponse>>>();

            if (deserializeResponse.Succeeded)
            {
                BasicTemplate = deserializeResponse.Data.Where(x => x.TempCode.Equals("pb")).FirstOrDefault();
                templateDocumentList = deserializeResponse.Data.Where(x => !x.TempCode.Equals("pb")).ToList() ?? new();

            }

        }

        private async Task ResetFormAsync()
        {
            filter = new() { Type = "TFOR,DTXT" };
            inputVersion.InputValue = string.Empty;
            inputName.InputValue = string.Empty;
            inputVersion.InputValue = string.Empty;
            inputCode.InputValue = string.Empty;

            await GetTemplateDoc();
        }

        private void LoadFile(TemplateDocumentDtoResponse record)
        {
            templateDocumentDto = record;

            if (templateDocumentDto != null)
            {
                notificationModal.UpdateModal(ModalType.Warning, "Nombre: " + templateDocumentDto.TempName, true, modalOrigin: "LoadFile");
            }

        }

        #endregion

        #endregion

    }
}
