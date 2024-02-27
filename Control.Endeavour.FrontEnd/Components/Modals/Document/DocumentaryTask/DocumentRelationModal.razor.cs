using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Document.DocumentaryTask
{
    public partial class DocumentRelationModal
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

        private NotificationsComponentModal notificationModal;

        #endregion

        #region Parameters

        [Parameter]
        public EventCallback<MyEventArgs<int>> OnStatusChanged { get; set; }

        #endregion

        #region Models


        #endregion

        #region Environments

        #region Environments(String)

        private string nRadicado { get; set; } = string.Empty;
        private string placeHolder = "Seleccione N° de radicado del documento que desea relacionar a la tarea";

        #endregion

        #region Environments(Numeric)

        private int controlId { get; set; } = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool disAbleInput = false;
        private bool isEnableActionButton = true;
        private bool SeenDocRelation = false;

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
            modalStatus = status;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {

                var eventArgs = new MyEventArgs<int>
                {
                    Data = controlId,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(eventArgs);

            }
            else
            {
                var eventArgs = new MyEventArgs<int>();
                await OnStatusChanged.InvokeAsync(eventArgs);
            }

        }

        #endregion

        #region OthersMethods

        #region SendDocumentRelation

        private async Task SelectDocRelationAsync()
        {

            if (nRadicado != null)
            {
                SearchDtoRequest filtro = new SearchDtoRequest()
                {
                    FilingCode = nRadicado,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documents/Document/SearchEngineDocument", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SearchDtoResponse>>>();

                if (deserializeResponse.Data != null)
                {
                    controlId = deserializeResponse.Data[0].ControlId;
                    notificationModal.UpdateModal(ModalType.Information, "Confirmar acción \n ¿Desea Continuar?", true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "El documento asociado no fue encontrado", true);
                }

            }
        }

        #endregion

        #region ResetModal

        public void resetModal()
        {
            nRadicado = "";
        }

        #endregion

        #region UpdateDocumentRelation

        public async Task UpdateDocumentRelation(int id, bool value)
        {
            SeenDocRelation = value;
            disAbleInput = !SeenDocRelation;

            HttpClient?.DefaultRequestHeaders.Remove("TaskId");
            HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{id}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FilingDtoResponse>>("documentarytasks/DocumentaryTask/GetDocumentRelation");
            HttpClient?.DefaultRequestHeaders.Remove("TaskId");

            if (deserializeResponse.Data != null)
            {
                placeHolder = deserializeResponse.Data.ExternalFiling;
            }
            else { Console.WriteLine("no se encontraron documentos relacionados"); }

        }

        #endregion

        #endregion

        #endregion
    }
}
