using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Document.DocumentaryTask;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using DevExpress.Blazor.Office;
using DevExpress.Blazor.RichEdit;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Documents.DocumentaryTask
{
    public partial class TaskManagementPage
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private DocumentsStateContainer? documentsStateContainer { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals

        private SecondPasswordModal secondPasswordModal = new();
        private DocumentClasificationModal documentClasificationModal = new();
        private SendDocumentModal sendDocumentsModal = new();
        private DocumentRelationModal docRelationModal = new();
        private AttachmentsModal attachmentsModal = new();
        private AttachmentTrayModal attachmentTrayModal = new();
        private GenericSearchModal genericSearchModal1 = new();
        private GenericSearchModal genericSearchModal2 = new();
        private GenericSearchModal genericSearchModal3 = new();
        private CopiesModal copyModal = new();
        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModal2 = new();
        private GenericDocTypologySearchModal genericDocTypologySearchModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        public TelerikPdfViewer? PdfViewerRef { get; set; }
        private DocumentClasificationDtoResponse? ProcedureDocClasContainer { get; set; }
        private CopyDtoResponse ProcedureCopiesContainer { get; set; } = new();
        private SendDocumentDtoResponse ProceduresendDocContainer { get; set; } = new();
        private int ProcedureNRadicado { get; set; }

        private DxRichEdit richEdit = new();

        public byte[] FileData = Array.Empty<byte>();

        private MetaModel meta = new();
        private FileDtoResponse fileInfo = new();
        private VDocumentaryTypologyDtoResponse TRDSelected = new();

        #endregion

        #region Environments

        #region Environments(String)

        public string FileBase64Data { get; set; } = "";

        private string panelTitle = "Enviar";
        private string panelButton = "Enviar";
        private string ProcessCode = "";
        private string TypeSign = "";
        private string modalTitle = "Buscador de Usuarios";
        private string destinationsNames = "";
        private string DisplayDocument = "col-md-6";
        private string DisplayTable = "col-md-6";

        #endregion

        #region Environments(Numeric)
        private int idTask { get; set; }

        private int UserId = 4055;
        private int changeModal = 1;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        public bool SeenTask { get; set; } = false;
        public bool fileWord { get; set; } = false;

        private bool panel_3 = false;
        private bool panel_4 = false;
        private bool cancelTask = false;
        private bool validDisplayTable = false;
        private bool validDisplayDocument = false;
        private bool a = false;
        private bool b = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> systemFieldsTAINSList = new();
        private List<DocumentWorkFlowDtoResponse> documentWorkFlows = new();
        private List<DestinationsDtoRequest> ProcedureCopiesList = new();
        private List<SignatureDtoRequest> ProcedureSignatures = new();
        private List<AttachmentsDtoRequest> ProcedureAttachments = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            if (documentsStateContainer.Identification != 0)
            {
                idTask = documentsStateContainer.Identification;
                await ValidUser(documentsStateContainer.Identification, documentsStateContainer.Id);
                await GetWorkFlow(documentsStateContainer.Identification);

                if (SeenTask)
                {
                    await GetAction();
                    await ShowWord(documentWorkFlows[documentWorkFlows.Count - 1].FileId);
                }
                else
                {
                    await ShowPdf(documentWorkFlows[documentWorkFlows.Count - 1].FilePdfId);
                }
            }

            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleStatusChangedReciver(bool status)
        {
            genericSearchModal1.UpdateModalStatus(status);
        }

        private void HandleStatusChangedUser(bool status)
        {
            genericSearchModal2.UpdateModalStatus(status);
        }

        private void HandleStatusChangedUserCopys(bool status)
        {
            genericSearchModal3.UpdateModalStatus(status);
        }

        private void HandleStatusChangedTRD(bool status)
        {
            genericDocTypologySearchModal.UpdateModalStatus(status);
        }

        private void HandleStatusChangedAttachement(bool status)
        {
            attachmentsModal.UpdateModalStatus(status);
        }

        private void HandleSelectedModal(int changemodal)
        {
            try
            {
                changeModal = changemodal;

                if (changemodal == 2)
                {
                    modalTitle = "Buscador de personas - Natural / Jurídica";
                }
                else
                {
                    modalTitle = "Buscador de Usuarios";
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
        }

        private async Task HandleSecondNotiCloseModal(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && cancelTask)
            {
                navigation.NavigateTo("/DocumentaryTaskTray");
            }
        }

        private async Task HandleNotiCloseModal(ModalClosedEventArgs args)
        {
            await CreateTaskManagement();
        }

        #region ModalConfiguration Methods

        private void ShowSendDocModal()
        {
            sendDocumentsModal.UpdateModalStatus(true);
        }

        private async Task ShowDocClasificationModal()
        {
            await documentClasificationModal.UpdateDocClasification(documentsStateContainer.Identification, SeenTask, ProcedureDocClasContainer);
            documentClasificationModal.UpdateModalStatus(true);
        }

        public async Task ShowCopiesModal()
        {
            await copyModal.UpdateCopys(documentsStateContainer.Identification, SeenTask, ProcedureCopiesContainer);
            copyModal.UpdateModalStatus(true);
        }

        public async Task ShowAttachmenTrayModal()
        {
            await attachmentTrayModal.UpdateAttachment(documentsStateContainer.Identification, SeenTask, ProcedureAttachments);
            attachmentTrayModal.UpdateModalStatus(true);
        }

        public async Task ShowDocRelationModal()
        {
            await docRelationModal.UpdateDocumentRelation(documentsStateContainer.Identification, SeenTask);
            docRelationModal.UpdateModalStatus(true);
        }

        #endregion

        #region ModalLoadData Methods

        private async Task HandleCopys(MyEventArgs<CopyDtoResponse> data)
        {
            if (data.Data != null)
            {
                copyModal.UpdateModalStatus(data.ModalStatus);
                ProcedureCopiesContainer = data.Data;
                ProcedureCopiesList.AddRange((data.Data.DestinationsUser != null) ? data.Data.DestinationsUser.Select(x => new DestinationsDtoRequest { DestinyId = x.UserId, DestinyType = "TDF,U", ItsCopy = true }).ToList() : new());
                ProcedureCopiesList.AddRange((data.Data.DestinationsAdministration != null) ? data.Data.DestinationsAdministration.Select(x => new DestinationsDtoRequest { DestinyId = (int)(x.ThirdPartyId ?? x.ThirdUserId), DestinyType = x.type, ItsCopy = true }).ToList() : new());
            }
            else
            {
                copyModal.UpdateModalStatus(data.ModalStatus);
                ProcedureCopiesContainer = new();
            }
        }

        private async void HandleDocumentClasification(DocumentClasificationDtoResponse data)
        {
            documentClasificationModal.UpdateModalStatus(false);
            ProcedureDocClasContainer = data;

            destinationsNames = string.Join(",", ProcedureDocClasContainer.DestinationsUser.Select(x => x.FullName));
            destinationsNames += string.Join(",", ProcedureDocClasContainer.DestinationsAdministration.Select(x => x.Names));
        }

        private async void HandleSendDocuments(MyEventArgs<SendDocumentDtoResponse> data)
        {
            sendDocumentsModal.UpdateModalStatus(data.ModalStatus);
            ProceduresendDocContainer = data.Data;

            if (ProceduresendDocContainer != null)
            {
                notificationModal.UpdateModal(ModalType.Information, "Confirmar acción", true);
            }
            else
            {
                Console.WriteLine("no se pudo crear el tramite de la tarea");
                notificationModal.UpdateModal(ModalType.Error, "No se pudo crear el tramite de la tarea", true);
            }
        }

        private async Task HandleDocumentRelation(MyEventArgs<int> data)
        {
            docRelationModal.UpdateModalStatus(data.ModalStatus);
            ProcedureNRadicado = data.Data;
        }

        #endregion

        #endregion

        #region OthersMethods

        #region GetActions
        public async Task GetAction()
        {
            try
            {

                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "TAINS");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");

                if (deserializeResponse.Data != null)
                {
                    systemFieldsTAINSList = deserializeResponse.Data.Where(x => x.FieldCode.Equals("PR") || x.FieldCode.Equals("FR")).ToList();
                    meta = deserializeResponse.Meta;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las acciones: {ex.Message}");
            }
        }

        #endregion

        #region CallActions

        public void CallAction(string value)
        {
            ProcessCode = value;

            if (value.Equals("PR"))
            {
                panelTitle = "Enviar";
                panelButton = "Enviar";
                panel_4 = true;
            }
            else if (value.Equals("FR"))
            {
                panelTitle = "Radicar";
                panelButton = "Radicar";
                panel_4 = true;
            }
        }

        #endregion

        #region GetWorkFlow

        private async Task GetWorkFlow(int idTask)
        {
            try
            {
                var workflow = new
                {
                    TaskId = idTask,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/GetWorkFlow", workflow);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentWorkFlowDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    documentWorkFlows = deserializeResponse.Data;
                    meta = deserializeResponse.Meta;
                }
                else
                {
                    Console.WriteLine("no se encontraro un flujo de trabajo para esa tarea documental");
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el flujo de trabajo: {ex.Message}");
            }
        }

        #endregion

        #region ValidUser

        private async Task ValidUser(int taskId, int id)
        {
            try
            {

                FilterManagementDtoRequest filtro = new()
                {
                    TaskId = taskId,
                    UserForwardId = id,
                    Indicted = false
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/ByFilter", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VDocumentaryTaskDtoResponse>>>();

                if (deserializeResponse.Succeeded)
                {
                    SeenTask = true;
                    panel_3 = true;
                    StateHasChanged();
                }
                else
                {
                    SeenTask = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al identificar usuario: {ex.Message}");
            }
        }

        #endregion

        #region ShowWord

        private async Task ShowWord(int id)
        {
            try
            {
                fileWord = true;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileBase64Data = fileInfo.DataFile.ToString();
                    FileData = Convert.FromBase64String(FileBase64Data);
                    await richEdit.LoadDocumentAsync(FileData, DocumentFormat.OpenXml);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion

        #region ShowNewWord

        private async Task ShowNewWord(DocumentWorkFlowDtoResponse docflow)
        {
            try
            {
                SeenTask = true;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{docflow.FileId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileBase64Data = fileInfo.DataFile.ToString();
                    FileData = Convert.FromBase64String(FileBase64Data);
                    await richEdit.LoadDocumentAsync(FileData, DocumentFormat.OpenXml);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion

        #region ShowPdf

        private async Task ShowPdf(int id)
        {
            try
            {
                fileWord = false;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileBase64Data = fileInfo.DataFile;
                    FileData = Convert.FromBase64String(FileBase64Data);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion

        #region ShowNewPdf

        private async Task ShowNewPdf(DocumentWorkFlowDtoResponse docflow)
        {
            try
            {
                SeenTask = false;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{docflow.FilePdfId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileBase64Data = fileInfo.DataFile;
                    FileData = Convert.FromBase64String(FileBase64Data);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion

        #region EditorComponent

        #region Customización de las Firmas
        public void OnCustomizeRibbon(IRibbon ribbon)
        {
            IRibbonTab signTab = ribbon.Tabs.AddCustomTab("Firmas y Otros");

            IBarGroup firstGroup = signTab.Groups.AddCustomGroup();

            IBarButton LoadSignMec = firstGroup.Items.AddCustomButton("Firma Mecánica", () => LoadSign("ME"));
            LoadSignMec.IconCssClass = "bi bi-vector-pen";
            LoadSignMec.Tooltip = "Firma Mecánica";

            IBarButton LoadSignRub = firstGroup.Items.AddCustomButton("Firma Rúbrica", () => LoadSign("RU"));
            LoadSignRub.IconCssClass = "bi bi-pencil-square";
            LoadSignRub.Tooltip = "Firma Rúbrica";
        }
        #endregion

        #region Cargue - Validación de Firmas
        private async Task LoadSign(string type)
        {
            SignatureDtoRequest signRegister = new();
            signRegister.SignUserId = 4055;
            signRegister.SignatureDate = DateTime.Now;

            if (type.Equals("ME", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIME";
                secondPasswordModal.UpdateModalStatus(true);
                signRegister.SignatureType = TypeSign;


            }
            else if (type.Equals("TU", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIRU";
                secondPasswordModal.UpdateModalStatus(true);

                signRegister.SignatureType = TypeSign;
            }
        }

        private async Task HandleValidatePasswordAsync(MyEventArgs<bool> validate)
        {
            if (validate.Data)
            {
                secondPasswordModal.UpdateModalStatus(validate.ModalStatus);

                HttpClient?.DefaultRequestHeaders.Remove("userId");
                HttpClient?.DefaultRequestHeaders.Remove("signatureType");
                HttpClient?.DefaultRequestHeaders.Add("userId", $"{UserId}");
                HttpClient?.DefaultRequestHeaders.Add("signatureType", $"{TypeSign}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<UserSignatureDtoResponse>>>("security/Signature/ByFilterSignatures");
                HttpClient?.DefaultRequestHeaders.Remove("userId");
                HttpClient?.DefaultRequestHeaders.Remove("signatureType");

                if (deserializeResponse.Data != null)
                {
                    var identifyFile = deserializeResponse.Data.FirstOrDefault();

                    string FileSign = await GetSign(identifyFile.FileId.Value);

                    if (string.IsNullOrEmpty(FileSign)) { notificationModal.UpdateModal(ModalType.Error, "No se pudo carga la firma. Intente Nuevamente", true); }

                    Document documentAPI = richEdit.DocumentAPI;

                    byte[] bytes = Convert.FromBase64String(FileSign);
                    Stream file = new MemoryStream(bytes);

                    DocumentImageSource documentImageSource = DocumentImageSource.LoadFromStream(file, 5000, 5000);

                    await documentAPI.Images.CreateAsync(richEdit.Selection.CaretPosition, documentImageSource);
                }
            }
        }

        private async Task<string> GetSign(int FileId)
        {

            HttpClient?.DefaultRequestHeaders.Remove("FileId");
            HttpClient?.DefaultRequestHeaders.Add("FileId", $"{FileId}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
            HttpClient?.DefaultRequestHeaders.Remove("FileId");

            if (deserializeResponse != null) { return deserializeResponse.Data.DataFile; }

            return "";
        }

        private async Task OnDocumentContentChanged(byte[] content)
        {
            FileData = content;
        }

        #endregion

        #endregion

        #region send data to other modals

        private void HandleUserChanged(MyEventArgs<VUserDtoResponse> user)
        {
            sendDocumentsModal.GetReceiverUserData(user.Data);
            genericSearchModal2.UpdateModalStatus(user.ModalStatus);
        }

        #region HandleModalGenericSearch3 - Copies

        private void HandleUsersChanged(MyEventArgs<List<object>> users)
        {
            copyModal.GetReceiverUsersData((List<VUserDtoResponse>)users.Data[0]);
            genericSearchModal3.UpdateModalStatus(users.ModalStatus);
        }

        private void HandleThirdPartyChanged(MyEventArgs<List<object>> users)
        {
            copyModal.GetReceiverThirdData((List<ThirdPartyDtoResponse>)users.Data[0], (List<ThirdUserDtoResponse>)users.Data[2]);
            genericSearchModal3.UpdateModalStatus(users.ModalStatus);
        }

        #endregion

        #region HandleModalGenericSearch1 - Destinations

        private void HandleReciversUserChanged(MyEventArgs<List<object>> listUsers)
        {
            documentClasificationModal.GetReceiverUsersData((List<VUserDtoResponse>)listUsers.Data[0]);
            genericSearchModal1.UpdateModalStatus(listUsers.ModalStatus);
        }

        private void HandleReciversThirdParyChanged(MyEventArgs<List<object>> listUsers)
        {
            documentClasificationModal.GetReceiverThirdData((List<ThirdPartyDtoResponse>)listUsers.Data[0], (List<ThirdUserDtoResponse>)listUsers.Data[2]);
            genericSearchModal1.UpdateModalStatus(listUsers.ModalStatus);
        }
        #endregion

        private void HandleTRDSelectedChanged(MyEventArgs<VDocumentaryTypologyDtoResponse> trd)
        {
            genericDocTypologySearchModal.resetModal();
            genericDocTypologySearchModal.UpdateModalStatus(trd.ModalStatus);
            TRDSelected = trd.Data;

            if (TRDSelected != null)
            {
                documentClasificationModal.GetTRDSelectedData(TRDSelected);
            }
        }

        private void HandleAttachmentChanged(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            attachmentTrayModal.GetAttachmentData(data.Data);
            ProcedureAttachments = data.Data;
            attachmentsModal.UpdateModalStatus(data.ModalStatus);
        }

        #endregion

        #region CreateTaskManagement

        private async Task CreateTaskManagement()
        {
            await richEdit.SaveDocumentAsync();

            var taskManagement = new DocumentaryTaskManagementDtoRequest();

            taskManagement.Archive = FileData;
            taskManagement.ProcessCode = "TAINS," + ProcessCode;
            taskManagement.CreateUserId = UserId;

            if (ProcessCode.Equals("PR"))
            {
                taskManagement.UserTaskId = UserId;
                taskManagement.UserForwardId = ProceduresendDocContainer.Recivers.UserId;
                taskManagement.InstructionCode = ProceduresendDocContainer.Instruction;
                taskManagement.Comentary = ProceduresendDocContainer.Description ?? string.Empty;
            }

            if (ProcedureNRadicado > 0)
            {
                taskManagement.DocumentRelationId = ProcedureNRadicado;
            }

            taskManagement.TaskId = documentsStateContainer.Identification;


            List<DestinationsDtoRequest> Destinations = new();

            //Esto Aplica siempre y cuando alla objeto
            if (ProcedureDocClasContainer != null)
            {
                taskManagement.Destinations = new();
                taskManagement.ClassCode = ProcedureDocClasContainer.ClassCode;
                taskManagement.DocDescription = ProcedureDocClasContainer.Description ?? string.Empty;
                taskManagement.ShipingMethod = string.IsNullOrEmpty(ProcedureDocClasContainer.ShipingMethod) ? null : ProcedureDocClasContainer.ShipingMethod;
                taskManagement.DocumentaryTypologyBehaviorId = ProcedureDocClasContainer.IdTypology;



                if (ProcedureDocClasContainer.DestinationsUser.Count > 0)
                {
                    foreach (var userDes in ProcedureDocClasContainer.DestinationsUser)
                    {
                        var tempUser = new DestinationsDtoRequest { DestinyId = userDes.UserId, DestinyType = userDes.type, ItsCopy = false };
                        Destinations.Add(tempUser);
                    }
                }
                else
                {
                    foreach (var userDes in ProcedureDocClasContainer.DestinationsAdministration)
                    {
                        var tempUser = new DestinationsDtoRequest { DestinyId = (int)(userDes.ThirdPartyId ?? userDes.ThirdUserId), DestinyType = userDes.type, ItsCopy = false };
                        Destinations.Add(tempUser);
                    }
                }

                taskManagement.Destinations.AddRange(Destinations);

            }

            if (ProcedureSignatures.Count > 0) { taskManagement.Signatures = ProcedureSignatures; }


            if (ProcedureCopiesContainer != null)
            {
                if (taskManagement.Destinations == null) { taskManagement.Destinations = new(); }

                if (ProcedureCopiesContainer.DestinationsUser != null && ProcedureCopiesContainer.DestinationsUser.Count > 0)
                {
                    taskManagement.Destinations.AddRange(ProcedureCopiesContainer.DestinationsUser.Select(x => new DestinationsDtoRequest { DestinyId = x.UserId, DestinyType = "TDF,U", ItsCopy = true }).ToList());

                }

                if (ProcedureCopiesContainer.DestinationsAdministration != null && ProcedureCopiesContainer.DestinationsAdministration.Count > 0)
                {
                    taskManagement.Destinations.AddRange(ProcedureCopiesContainer.DestinationsAdministration.Select(x => new DestinationsDtoRequest { DestinyId = (int)(x.ThirdPartyId ?? x.ThirdUserId), DestinyType = x.type, ItsCopy = true }).ToList());
                }
            }

            if (ProcedureAttachments.Any())
            {
                taskManagement.Attachments = ProcedureAttachments;
            }

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/TasksManagement", taskManagement);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<TaskManagementDtoResponse>>();

            if (deserializeResponse.Succeeded)
            {
                notificationModal2.UpdateModal(ModalType.Success, $"El tramite de la tarea documental ha sido creada correctamente", true);
                cancelTask = true;
            }
            else
            {
                notificationModal2.UpdateModal(ModalType.Error, $"Ocurrio un error al intentar crear el tramite de la tarea documental", true);
                cancelTask = false;
            }
        }

        #endregion

        #region Display Table or Document

        private void ShowTable()
        {
            validDisplayTable = !validDisplayTable;

            if(validDisplayTable)
            {
                DisplayTable = "col-md-12";
                DisplayDocument = "d-none";
                a = true;
            }
            else 
            {
                DisplayTable = "col-md-6";
                DisplayDocument = "col-md-6";
                a = false;
            }
        }

        private void ShowDocument()
        {
            validDisplayDocument = !validDisplayDocument;

            if (validDisplayDocument)
            {
                DisplayTable = "d-none";
                DisplayDocument = "col-md-12";
                b = true;
            }
            else
            {
                DisplayTable = "col-md-6";
                DisplayDocument = "col-md-6";
                b = false;
            }
        }

        #endregion

        #endregion

        #endregion

    }
}
