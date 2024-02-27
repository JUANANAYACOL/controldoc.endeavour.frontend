using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;

public partial class GeneralInformationModal
{
    #region Variables

    #region Injects

    [Inject]
    private IJSRuntime Js { get; set; }
    [Inject]
    private HttpClient? HttpClient { get; set; }
    [Inject]
    private EventAggregatorService? EventAggregator { get; set; }
    #endregion

    #region Components
    private NotificationsComponentModal notificationModalSucces { get; set; } = new();
    private NotificationsComponentModal notificationModal { get; set; } = new();
    private WorKFlowModal worKFlowModal { get; set; } = new();
    #endregion Components       

    #region Models
    public TelerikPdfViewer PdfViewerRef { get; set; }
    private Dictionary<string, string> dataInfoDocument = new();
    private Dictionary<string, string> dataInfoTrd = new();
    private GeneralInformationDtoResponse? document = new();
    #endregion Models

    #region Enviroment

    private string DisplayPdfViewer = "d-none";
    private string ColTableData = "col-md-12";
    private bool modalStatus = false;
    public byte[] FileData { get; set; }
    public string FileBase64Data;
    public int controlId = 0;
    private bool isDropdownOpen = false;
    private bool isDropdownLoans = false;
    private string DropdownMenuRecords = "dropdown-menu show";
    private string DropdownMenuLoans = "dropdown-menu show";

    #endregion Enviroment

    #region Parameters


    #endregion Parameters

    #endregion Variables

    #region OnInitializedAsync
    protected override async Task OnInitializedAsync()
    {
        try
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            modalStatus = false;          
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
        }
    }
    #endregion OnInitializedAsync

    #region Methods

    #region HandleMethods

    async Task HandleLanguageChanged()
    {
        StateHasChanged();
    }
    public void HandlePdfViewer()
    {
        DisplayPdfViewer = "d-none";
        ColTableData = "col-md-12";
    }
    async Task HandleModalRecors()
    {
        //modalAssociatedResourcesSearch.UpdateModalStatus(true);
    }
    private void HandleModalClosed(bool status)
    {
        modalStatus = status;
        StateHasChanged();
    }

    private void HandleModalNotiClose(ModalClosedEventArgs args)
    {
        UpdateModalStatus(args.ModalStatus);
    }

    #endregion HandleMethods

    #region MethodsAsync
    async Task GetGeneralInfo()
    {
        try
        {
            HidePdfViewer();
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

                await GetImagePdf();
            }

            InsertData();
        }
        catch (Exception ex)
        {
            notificationModalSucces.UpdateModal(ModalType.Warning, $"Error al procesar la solicitud: {ex.Message}", true, "Aceptar", "Cancelar");
            Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
        }
    }   
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
    public async Task GeneralInformation(int Idcontrol)
    {
        controlId = Idcontrol;
        await GetGeneralInfo();
        StateHasChanged();
    }
    async Task ShowWorKFlow()
    {
        if (controlId > 0)
        {
            worKFlowModal.UpdateModalStatus(true);
            await worKFlowModal.WorKFlowAsync(controlId);
        }
    }
    #endregion MethodsAsync

    #region MethodsGenerales
    public void UpdateModalStatus(bool newValue)
    {
        modalStatus = newValue;
        StateHasChanged();
    }
    private void InsertData()
    {
        dataInfoTrd = new Dictionary<string, string>
        {
            {"Clasificación del Documento", document?.DocumentClasification?.DocumentalVersionName ?? ""},
            {"Unidad Administrativa", document?.DocumentClasification?.AdministrativeUnitName ?? ""},
            {"Oficina Productora", document?.DocumentClasification?.ProductionOfficeName ?? ""},
            {"Serie", document?.DocumentClasification?.SeriesName ?? ""},
            {"Subserie", document?.DocumentClasification?.SubseriesName ?? ""},
            {"Tipología Documental", document?.DocumentClasification?.DocumentaryTypologiesName ?? ""},
            {"Tipo de correspondencia", document?.DocumentClasification?.CorrespondenceType ?? ""}
            
        };
        dataInfoDocument = new Dictionary<string, string>
        {
            {"Clase", $"{document?.DocumentInformation?.ClassCode ?? ""} {document?.DocumentInformation?.ClassCode  ?? ""}"},
            {"ID Control", $"{document?.DocumentInformation?.ControlId ?? 0}"},
            {"Radicado", document?.DocumentInformation?.ExternalFiling ?? ""},
            {"Radicado externo", $"{document?.DocumentInformation?.DocumentId ?? 0}"},
            {"Año", document?.DocumentInformation?.Year ?? ""},
            {"Prioridad", document?.DocumentInformation?.Priority ?? ""},
            {"N° de Guía / Código postal", document?.DocumentInformation?.NRoGuia ?? "N/A"},
            {"Medios de Recepción / Envío", document?.DocumentInformation?.ReceptionCode?? ""},
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
            {"Sucursal Usuario radicación", document?.DocumentInformation?.BrachOfficeUser ?? "N/A"},
            {"Justificación", "-"},
            {"Usuario Digitalizo", "N/A"},
            {"Fecha digitalizo", ""},
            {"Comentario cierre", document?.DocumentInformation?.CommentaryUserClosedProcess ?? "N/A"},
            {"Usuario Cerró", document?.DocumentInformation?.UserClosedProcess ?? "N/A"},
            {"Justificación de reactivación", "N/A"},
        };
    }
    void ToggleDropdown(ref bool dropDownOpenRef, ref string classdropdown)
    {
        if (dropDownOpenRef)
        {
            classdropdown = "dropdown-menu show";
            dropDownOpenRef = false;
        }
        else
        {
            classdropdown = "dropdown-menu";
            dropDownOpenRef = true;
        }
    }

    async Task ShowPdfViewer()
    {
        DisplayPdfViewer = "";
        ColTableData = "col-md-6";
        if(string.IsNullOrWhiteSpace(FileBase64Data))
        {
            await GetImagePdf();
        }
       
    }
    private void HidePdfViewer()
    {
        DisplayPdfViewer = "d-none";
        ColTableData = "col-md-12";
    }

    #endregion MethodsAsync 

    #endregion Methods
}
