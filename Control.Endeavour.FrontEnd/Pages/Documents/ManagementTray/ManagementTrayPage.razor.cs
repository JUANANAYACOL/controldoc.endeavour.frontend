
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Documents;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Control.Endeavour.FrontEnd.StateContainer.ManagementTray;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Net.Http.Json;
using Telerik.SvgIcons;


namespace Control.Endeavour.FrontEnd.Pages.Documents.ManagementTray;
public partial class ManagementTrayPage
{
    #region Variables

    #region Inject 
    [Inject] private EventAggregatorService? EventAggregator { get; set; }
    [Inject] private HttpClient? HttpClient { get; set; }
    [Inject] private ManagementTrayStateContainer managementTrayStateContainer { get; set; }
    #endregion

    #region Components

    private int mounthValue { get; set; }
    private int yearValue { get; set; }
    private int daysValue { get; set; }
    public bool enabledMes { get; set; } = false;
    public bool enabledDia { get; set; } = false;
    private string idControl { get; set; }
    private string numRadicado { get; set; }
    private string? classCodeValue { get; set; }
    private string? prioridadValue { get; set; }
    private InputModalComponent IdcontrolInput { get; set; }
    private InputModalComponent NumRadicaInput { get; set; }

    #endregion

    #region Input
    private InputModalComponent idControlInput { get; set; }
    private InputModalComponent numRadicaInput { get; set; }
    #endregion

    #region Models
    private MetaModel Meta = new();
    private DataCardDtoResponse DataCards;
    private ManagementTrayDtoResponse InfoGridCard;
    private NotificationsComponentModal notificationModal = new();
    private ManagementOfProceduresModal managementOfProcedures = new();
    private GeneralInformationModal generalInformation = new();
    private AttachmentTrayModal attachmentTrayModal = new();
    private AttachmentsModal attachmentsModal = new();
    private PaginationComponent<ManagementTrayDtoResponse, ManagementTrayFylterDtoRequest> PaginationComponet = new();
    private ManagementTrayFylterDtoRequest FilterDtoRequest = new();
    private List<ManagementTrayDtoResponse>? managementTrayToReturn { get; set; } = new();
    private List<FileInfoData> lstFileInfoData { get; set; } = new();   
    private List<AttachmentsDtoRequest> listAttachment { get; set; } = new();
    private AttachmentsType Filing = AttachmentsType.Filing;
    #endregion

    #region Environments(String)

    private string textYear = "Seleccione un año...";
    private string textMes = "Seleccione un mes...";
    private string textDia = "Seleccione un dia...";
    private string textClase = "Seleccione una clase de comunciación...";
    private string textPrio = "Seleccione la prioridad...";
    private string Gex = "";
    private string Enp = "";
    private string Etr = "";
    private string Cop = "";
    private string GexP = "";
    private string EnpP = "";
    private string EtrP = "";
    private string CopP = "";
    private string Estado = "";
    private string StatusCode = "";
    private string TablaAdjuntos = "d-none";
    #endregion

    #region Environments(Numeric)
    private int userId = 4055;
    private int companyId = 17;
    private int Total = 0;
    #endregion

    #region Environments(Bool)
    private bool activeState = false;
    private bool clickPendiente = false;
    private bool dueDateValue = false;
    private bool activateProcedure = false;
    #endregion

    #region Environments(DateTime)
    private DateTime? SelectedDate { get; set; }
    private DateTime Min = DateTime.Now;
    private DateTime Max = new DateTime(2025, 1, 1, 19, 30, 45);

    #endregion

    #region Environments(List & Dictionary)
    private List<VSystemParamDtoResponse>? FormatBG { get; set; } = new();
    private List<VSystemParamDtoResponse>? FormatCL { get; set; } = new();
    private List<ManagementTrayDtoResponse>? GeneralList { get; set; } = new();
    private List<DateDtoResponse> Year = new();
    private List<DateDtoResponse> Mounth = new();
    private List<DateDtoResponse> Days = new();
    private List<DataCardDtoRequest> Data = new();
    #endregion

    #endregion

    #region OnInitializedAsync
    protected override async Task OnInitializedAsync()
    {
        try
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            managementTrayToReturn = new();
            FillList();
            await GetDataCards();
            await GetPriority();
            await GetNotiComuni();

            if (managementTrayStateContainer.Status > 0)
            {
                await GetData(managementTrayStateContainer.Status);
            }
            else
            {
                await GetData(DocumentStatusEnum.WithoutProcessingWord);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
        }
    }

    #endregion

    #region Methods

    #region  GetFillList
    private void FillList()
    {
        // llenar listado de meses
        var cultura = CultureInfo.CreateSpecificCulture("es-CO");
        var qry = from m in cultura.DateTimeFormat.MonthNames
                  select cultura.TextInfo.ToTitleCase(m);

        var item = 1;
        foreach (var mes in qry)
        {
            Mounth.Add(new DateDtoResponse { nombre = mes, valor = item });
            item++;
        }

        // llenar listado de dias
        for (int i = 0; i < 31; i++)
        {
            Days.Add(new DateDtoResponse { nombre = $"{(i + 1)}", valor = (i + 1) });
        }

        // llenar listado de años
        for (int i = 2021; i < DateTime.Now.Year + 1; i++)
        {
            Year.Add(new DateDtoResponse { nombre = $"{(i + 1)}", valor = (i + 1) });
        }

    }

    #endregion

    #region DataCards
    private async Task GetDataCards()
    {
        try
        {
            Data = new();
            HttpClient?.DefaultRequestHeaders.Remove("AssingUserId");
            HttpClient?.DefaultRequestHeaders.Add("AssingUserId", $"{userId}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DataCardDtoResponse>>("documentmanagement/Document/ByAssingUserId");
            HttpClient?.DefaultRequestHeaders.Remove("AssingUserId");
            DataCards = deserializeResponse.Data;

            if (DataCards != null)
            {
                int total = DataCards.withoutProcessing + DataCards.inProgress + DataCards.successfulManagement;
                double porcent = (Convert.ToDouble(DataCards.withoutProcessing) * 100) / total;
                double porcent2 = (Convert.ToDouble(DataCards.inProgress) * 100) / total;
                double porcent3 = (Convert.ToDouble(DataCards.successfulManagement) * 100) / total;
                double porcent4 = (0 * 100) / total;

                DataCardDtoRequest dato1 = new DataCardDtoRequest()
                {
                    Category = DataCards.InProgressWord,
                    Value = porcent2,
                    color = "#EAD519"
                };

                DataCardDtoRequest dato2 = new DataCardDtoRequest()
                {
                    Category = DataCards.WithoutProcessingWord,
                    Value = porcent,
                    color = "#AB2222"
                };

                DataCardDtoRequest dato3 = new DataCardDtoRequest()
                {
                    Category = DataCards.SuccessfullManagementWord,
                    Value = porcent3,
                    color = "#82A738"
                };

                Data.Add(dato1);
                Data.Add(dato2);
                Data.Add(dato3);

                EnpP = porcent.ToString("N2") + "%";
                EtrP = porcent2.ToString("N2") + "%";
                GexP = porcent3.ToString("N2") + "%";
                CopP = porcent4.ToString("N2") + "%";

                Enp = DataCards.withoutProcessing.ToString();
                Etr = DataCards.inProgress.ToString();
                Gex = DataCards.successfulManagement.ToString();
                Cop = "0";
            }
            else
            {
                Enp = "0";
                Etr = "0";
                Gex = "0";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
        }
    }
    #endregion

    #region Dropdownlist

    private async Task GetPriority()
    {
        try
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "RPRI");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                FormatBG = deserializeResponse.Data;
                Meta = deserializeResponse.Meta ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
        }
    }
    private async Task GetNotiComuni()
    {
        try
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                FormatCL = deserializeResponse.Data;
                Meta = deserializeResponse.Meta ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
        }
    }

    private async Task cascadingMes()
    {
        enabledMes = true;
    }
    private async Task cascadingDia()
    {
        enabledDia = true;
    }

    #endregion

    #region Data Grilla
    async Task GetData(DocumentStatusEnum status)
    {
        try
        {
            GeneralList = new();
            Meta = new();
            activateProcedure = false;
            if (status == DocumentStatusEnum.WithoutProcessingWord)
            {
                Estado = DocumentStatusEnum.WithoutProcessingWord.GetDisplayValue();
                StatusCode = DocumentStatusEnum.WithoutProcessingWord.GetCoreValue();
                activateProcedure = true;
            }
            if (status == DocumentStatusEnum.InProgressWord)
            {
                Estado = DocumentStatusEnum.InProgressWord.GetDisplayValue();
                StatusCode = DocumentStatusEnum.InProgressWord.GetCoreValue();
            }
            if (status == DocumentStatusEnum.SuccessfullManagementWord)
            {
                Estado = DocumentStatusEnum.SuccessfullManagementWord.GetDisplayValue();
                StatusCode = DocumentStatusEnum.SuccessfullManagementWord.GetCoreValue();
            }

            var data = new
            {
                assingUserId = userId,
                flowStateCode = StatusCode,
                classCode = classCodeValue,
                controlId = string.IsNullOrWhiteSpace(idControl) ? 0 : Convert.ToInt32(idControl),
                filingCode = numRadicado,
                priorityFields = !string.IsNullOrEmpty(prioridadValue) ? $"RPRI,{prioridadValue}" : "",
                year = yearValue,
                month = mounthValue,
                days = daysValue,
                dueDate = dueDateValue,
            };

            var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/ByFilter", data);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ManagementTrayDtoResponse>>>();
            if (!deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Error, "¡No se encontraron documentos con los parámetros ingresados, por favor intente de nuevo!", true, "Aceptar");

            }
            else
            {
                GeneralList = deserializeResponse.Data;
                Meta = deserializeResponse.Meta ?? new();
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
        }
    }
    private void HandlePaginationGrid(List<ManagementTrayDtoResponse> newDataList)
    {
        GeneralList = newDataList;
    }
    #endregion

    #region Metodos Generales
    public async Task Search()
    {
        if (Estado.Equals(DocumentStatusEnum.WithoutProcessingWord.GetDisplayValue()))
        {
            await GetData(DocumentStatusEnum.WithoutProcessingWord);
        }
        else if (Estado.Equals(DocumentStatusEnum.InProgressWord.GetDisplayValue()))
        {
            await GetData(DocumentStatusEnum.InProgressWord);
        }
        else if (Estado.Equals(DocumentStatusEnum.SuccessfullManagementWord.GetDisplayValue()))
        {
            await GetData(DocumentStatusEnum.SuccessfullManagementWord);
        }
        else
        {
            notificationModal.UpdateModal(ModalType.Warning, "Debe ingresar un parámetro de búsqueda", true);
        }
    }
    private async Task Refresh()
    {
        idControl = "";
        numRadicado = "";
        textYear = "Seleccione un año...";
        textMes = "Seleccione un mes...";
        textDia = "Seleccione un dia...";
        classCodeValue = "Seleccione una clase...";
        prioridadValue = "Seleccione una prioridad...";
    }
    #endregion

    #region Modals

    async Task ShowModalManagementProcedure(ManagementTrayDtoResponse model)
    {
        managementOfProcedures.UpdateModalStatus(true);
        managementOfProcedures.ResetFormAsync();
        await managementOfProcedures.ManagementOfProcedures(model);
    }
    async Task ShowModalGeneralInformation(ManagementTrayDtoResponse model)
    {
        generalInformation.UpdateModalStatus(true);
        await generalInformation.GeneralInformation(model.controlId);
    }
    async Task mostrarModal()
    {
        //modalUpdateTypologyBG.UpdateModalStatus(true);
    }

    public async Task ShowModalAttachments(ManagementTrayDtoResponse model)
    {
        await attachmentTrayModal.UpdateAttachmentDocument(model.controlId, true);
        attachmentTrayModal.UpdateModalStatus(true);
    }
    private async Task OpenModalManagementProceduresTM()
    {
        if (string.IsNullOrEmpty(classCodeValue))
        {
            notificationModal.UpdateModal(ModalType.Warning, "Debe seleccionar una clase de comunicación", true);
        }
        else
        {
            //managementOfProcedures.UpdateModalStatus(true);
        }

    }

    #endregion

    #region HandleMethods

    private async Task HandleModalNotiClose(ModalClosedEventArgs args)
    {

    }
    private void HandleStatusChangedAttachement(bool status)
    {
        attachmentsModal.UpdateModalStatus(status); 
    }
    public async Task HandleAttachmentChanged(MyEventArgs<List<AttachmentsDtoRequest>> data)
    {
        attachmentTrayModal.GetAttachmentData(data.Data);
        listAttachment = data.Data;
        attachmentsModal.UpdateModalStatus(data.ModalStatus);
    }
    async Task HandleRefreshData(bool refresh)
    {
        await GetDataCards();
        await GetData(DocumentStatusEnum.WithoutProcessingWord);
    }
    private async Task HandleLanguageChanged()
    {
        StateHasChanged();
    }
    
    private async Task HandleAttachmentAdd()
    {      

        if (listAttachment?.Count > 0)
        {
            foreach (var item in listAttachment)
            {
                
            }
        }  
    }

    #endregion HandleMethods       

    #region changeManagementTray
    public async Task ChangeStateManagementTray(ManagementTrayDtoResponse model)
    {
        var allanagementTraySavedInManger = managementTrayToReturn?.Select(x => x.controlId).ToList();
        if (allanagementTraySavedInManger.Contains(model.controlId) && !model.Selected)
        {
            var elementToErrase = managementTrayToReturn.Find(x => x.controlId == model.controlId);
            managementTrayToReturn.Remove(elementToErrase!);
        }
        else if (!allanagementTraySavedInManger.Contains(model.controlId) && model.Selected)
        {
            managementTrayToReturn.Add(model);
        }
    }

    #endregion changeManagementTray

    #endregion Methods
}
