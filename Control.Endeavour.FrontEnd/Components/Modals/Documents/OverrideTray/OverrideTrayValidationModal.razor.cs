using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayValidationModal : ComponentBase
    {

        #region Variables
        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private FilingStateContainer? FilingSC { get; set; }
        #endregion

        #region Components
        private NotificationsComponentModal notificationModalSucces { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion

        #region Modals


        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        [Parameter] public int controlId { get; set; }


        #endregion

        #region Models
        public TelerikPdfViewer PdfViewerRef { get; set; }
        private Dictionary<string, string> dataInfoDocument = new();
        private Dictionary<string, string> dataInfoTrd = new();
        private GeneralInformationDtoResponse? document = new();

        #endregion

        #region Environments
        public bool modalStatus = false;

        private string DisplayPdfViewer = "d-none";

        private string ColTableData = "col-md-12";
        public byte[] FileData { get; set; }
        public string FileBase64Data;

        private bool validDisplayPDF = false;

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            
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

        #region UpdateModalStatus
        public async void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            await GetGeneralInfo();
            StateHasChanged();
        }
        #endregion

        #region ShowModalValidation
        public void ShowModalValidation(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #region HandleModalClosed
        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }
        #endregion

        #region HandleModalNotiClose
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            UpdateModalStatus(args.ModalStatus);
        }
        #endregion

        #region GetGeneralInfo
        private async Task GetGeneralInfo()
        {
            try
            {
                HidePdfViewer();
                controlId= Convert.ToInt32(FilingSC.DocumentId);
                if (controlId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("controlId");
                    HttpClient?.DefaultRequestHeaders.Add("controlId", $"{controlId}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<GeneralInformationDtoResponse>>("documents/Document/GeneralInfo").ConfigureAwait(false);
                    HttpClient?.DefaultRequestHeaders.Remove("controlId");
                    if (deserializeResponse!.Succeeded)
                    {
                        document = deserializeResponse.Data;
                    }
                    else
                    {
                        notificationModalSucces.UpdateModal(ModalType.Warning, $"¡Error el id documento digitado no presenta información!", true, "Aceptar", "Cancelar");

                        HandleModalClosed(false);
                    }

                    //await GetImagePdf();
                }

                InsertData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
            }
        }
        #endregion

        #region GetImagePdf
        async Task GetImagePdf()
        {
            try
            {
                FileBase64Data = "";
                if (controlId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("controlId");
                    HttpClient?.DefaultRequestHeaders.Add("controlId", $"{controlId}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ImagePdfDtoResponse>>("documents/Document/GetImageDocument").ConfigureAwait(false);
                    HttpClient?.DefaultRequestHeaders.Remove("controlId");
                    if (deserializeResponse!.Succeeded)
                    {
                        FileBase64Data = deserializeResponse.Data!.Rrchive;
                        FileData = Convert.FromBase64String(FileBase64Data);
                    }
                }

            }
            catch (Exception ex)
            {
                notificationModalSucces.UpdateModal(ModalType.Warning, $"Error al obtener la imagen pdf del documento: {ex.Message}", true, "Aceptar", "Cancelar");

            }
        }
        #endregion

       

        #region InsertData
        private void InsertData()
        {

            dataInfoTrd = new Dictionary<string, string>
    {
        {"Clasificación del documento", document?.DocumentClasification?.DocumentalVersionName ?? ""},
        {"Unidad administrativa", document?.DocumentClasification?.AdministrativeUnitName ?? ""},
        {"Oficina productora", document?.DocumentClasification?.ProductionOfficeName ?? ""},
        {"Serie", document?.DocumentClasification?.SeriesName ?? ""},
        {"Subserie", document?.DocumentClasification?.SubseriesName ?? ""},
        {"Tipología documental", document?.DocumentClasification?.DocumentaryTypologiesName ?? ""},
        {"Tipo de correspondencia", document?.DocumentClasification?.CorrespondenceType ?? ""}
    };
            dataInfoDocument = new Dictionary<string, string>
    {
        {"Clase", $"{document?.DocumentInformation?.ClassCode ?? ""} {document?.DocumentInformation?.ClassCode  ?? ""}"},
        {"ID control", $"{document?.DocumentInformation?.ControlId ?? 0}"},
        {"Radicado", document?.DocumentInformation?.ExternalFiling ?? ""},
        {"Radicado externo", $"{document?.DocumentInformation?.DocumentId ?? 0}"},
        {"Año", document?.DocumentInformation?.Year ?? ""},
        {"Prioridad", document?.DocumentInformation?.Priority ?? ""},
        {"N° de guía / Código postal", document?.DocumentInformation?.NRoGuia ?? "N/A"},
        {"Medios de recepción / Envío", document?.DocumentInformation?.ReceptionCode?? ""},
        {"Asunto", document?.DocumentInformation?.DocDescription ?? ""},
        {"Notificación", document?.DocumentInformation?.Notificacion ?? ""},
        {"Firmante", document?.DocumentInformation?.Firmantes ?? ""},
        {"Destinatario (s)", document?.DocumentInformation?.Destinatarios ?? ""},
        {"Expediente(s) ID", ""},
        {"Fecha vencimiento", $"{document?.DocumentInformation?.DueDate!:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
        {"Días / Horas plazo vencimiento", document?.DocumentInformation?.DaysHoursDueDate ?? "N/A"},
        {"Activo en el sistema", document?.DocumentInformation?.Active == true ? "SI" : "NO"},
        {"Fecha radicación", $"{document?.DocumentInformation?.DocDate!:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
        {"Fecha documento", $"{document ?.DocumentInformation ?.CreateDate !:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
        {"Usuario radicación", document?.DocumentInformation ?.CreateUser ?? "N/A"},
        {"Sucursal usuario radicación", document?.DocumentInformation?.BrachOfficeUser ?? "N/A"},
        {"Justificación", "-"},
        {"Usuario digitalizo", "N/A"},
        {"Fecha digitalizo", ""},
        {"Comentario cierre", document?.DocumentInformation?.CommentaryUserClosedProcess ?? "N/A"},
        {"Usuario cerró", document?.DocumentInformation?.UserClosedProcess ?? "N/A"},
        {"Justificación de reactivación", "N/A"},
    };
        }
        #endregion

        #region ShowPdfViewer
        async Task ShowPdfViewer()
        {
            DisplayPdfViewer = "";
            ColTableData = "col-md-6";
            if (string.IsNullOrWhiteSpace(FileBase64Data))
            {
                //await GetImagePdf();
            }

        }
        #endregion

        #region HidePdfViewer
        private void HidePdfViewer()
        {
            DisplayPdfViewer = "d-none";
            ColTableData = "col-md-12";
        }
        #endregion

        #region ShowPdfComplete
        private void ShowPdfComplete()
        {
            DisplayPdfViewer = "col-md-12";
            ColTableData = "d-none";
        }
        #endregion

        
        #endregion

    }
}
