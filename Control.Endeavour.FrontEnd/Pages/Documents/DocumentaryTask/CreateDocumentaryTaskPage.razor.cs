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
using DevExpress.Blazor.Office;
using DevExpress.Blazor.RichEdit;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Documents.DocumentaryTask
{
    public partial class CreateDocumentaryTaskPage
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }
        #endregion

        #region Components

        private DxRichEdit richEditControl = new();

        #endregion

        #region Modals

        private FormatMasterListModal formatMasterListModal = new();
        private DocumentClasificationModal documentClasificationModal = new();
        private SendDocumentModal sendDocumentsModal = new();
        private DocumentRelationModal docRelationModal = new();
        private AttachmentsModal attachmentsModal = new();
        private GenericSearchModal genericSearchModal1 = new();
        private GenericSearchModal genericSearchModal2 = new();
        private GenericSearchModal genericSearchModal3 = new();
        private GenericDocTypologySearchModal genericDocTypologySearchModal = new();
        private CopiesModal copyModal = new();
        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModal2 = new();

        #endregion

        #region Parameters


        #endregion

        #region Models
        private DocumentClasificationDtoResponse docClasContainer { get; set; } = new();
        private SendDocumentDtoResponse sendDocumentContainer { get; set; } = new();

        private MetaModel meta = new();
        private VDocumentaryTypologyDtoResponse TRDSelected = new();
        private SecondPasswordModal modalSecondPass = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string ProcessCode = "";
        private string panelTitle = "Enviar";
        private string panelButton = "Enviar";
        private string TypeSign = "";
        private string modalTitle = "Buscador de Usuarios";
        private string destinationsNames = "";
        private string SelectedFile = "";

        #endregion

        #region Environments(Numeric)

        private int UserId = 4055; //Usuario de quien se obtiene el id para poder acceder a las firmas
        private int changeModal = 1;
        private int TemplateId = 0;
        private int docRelationContainer { get; set; }

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool panel_2 = false;
        private bool panel_3 = false;
        private bool panel_4 = false;
        private bool cancelTask = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> systemFieldsTAINSList = new();
        public List<SignatureDtoRequest> signatures = new();
        private byte[] TaskManagementFile = Array.Empty<byte>();
        private List<DestinationsDtoRequest> _Copys = new();
        private List<AttachmentsDtoRequest>? attachmentsContainer = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetAction(); //en el futuro esto ira en otro lado
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region ModalConfiguration Methods

        private void ShowCopiesModal()
        {
            copyModal.UpdateModalStatus(true);
        }

        private void ShowFormatMasterModal()
        {
            formatMasterListModal.UpdateModalStatus(true);
        }

        private void ShowDocClasificationModal()
        {
            documentClasificationModal.UpdateModalStatus(true);
        }

        private void ShowSendDocsModal()
        {
            sendDocumentsModal.UpdateModalStatus(true);
        }

        private void ShowDocRelationModal()
        {
            docRelationModal.UpdateModalStatus(true);
            docRelationModal.resetModal();
        }

        private void ShowAttachmentModal()
        {
            attachmentsModal.UpdateModalStatus(true);
        }

        #endregion

        private void HandleStatusChangedTRD(bool status)
        {
            genericDocTypologySearchModal.UpdateModalStatus(status);
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

        private async Task HandleNotiCloseModal(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await CreateDocumentTask();
            }
        }

        private Task HandleSecondNotiCloseModal(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && cancelTask)
            {
                navigation.NavigateTo("/DocumentaryTaskTray");
            }

            return Task.CompletedTask;
        }

        private async Task HandleValidatePasswordAsync(MyEventArgs<bool> validate)
        {
            if (validate.Data)
            {
                modalSecondPass.UpdateModalStatus(validate.ModalStatus);

                SignatureFilterDtoRequest signature = new() { UserId = UserId, SignatureType = TypeSign };

                var responseApi = await HttpClient.PostAsJsonAsync("security/Signature/ByFilterSignatures", signature);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<UserSignatureDtoResponse>>>();

                if (deserializeResponse != null)
                {
                    var identifyFile = deserializeResponse.Data.FirstOrDefault();

                    if (identifyFile!.FileId == null) { notificationModal.UpdateModal(ModalType.Error, "No se pudo carga la firma. Intente Nuevamente", true); }


                    string FileSign = await GetSign(identifyFile.FileId!.Value);

                   if(string.IsNullOrEmpty(FileSign)) { notificationModal.UpdateModal(ModalType.Error, "Ingrese una firma al sistema.", true); }


                    Document documentAPI = richEditControl.DocumentAPI;

                    byte[] bytes = Convert.FromBase64String(FileSign);
                    Stream file = new MemoryStream(bytes);

                    DocumentImageSource documentImageSource = DocumentImageSource.LoadFromStream(file, 5000, 5000);

                    await documentAPI.Images.CreateAsync(richEditControl.Selection.CaretPosition, documentImageSource);
                }
            }
        }

        #region DataContainer Methods

        private void HandleCopys(MyEventArgs<CopyDtoResponse> data)
        {
            copyModal.UpdateModalStatus(data.ModalStatus);
            _Copys.AddRange((data.Data.DestinationsUser != null) ? data.Data.DestinationsUser.Select(x => new DestinationsDtoRequest { DestinyId = x.UserId, DestinyType = "TDF,U", ItsCopy = true }).ToList() : new());
            _Copys.AddRange((data.Data.DestinationsAdministration != null) ? data.Data.DestinationsAdministration.Select(x => new DestinationsDtoRequest { DestinyId = (int)(x.ThirdPartyId ?? x.ThirdUserId), DestinyType = x.type, ItsCopy = true }).ToList() : new());
        }

        private void HandleDocumentClasification(DocumentClasificationDtoResponse data)
        {
            documentClasificationModal.UpdateModalStatus(false);
            docClasContainer = data;
            panel_3 = true;

            destinationsNames = string.Join(",", docClasContainer.DestinationsUser.Select(x => x.FullName));
            destinationsNames += string.Join(",", docClasContainer.DestinationsAdministration.Select(x => x.Names));
        }

        private void HandleSendDocuments(MyEventArgs<SendDocumentDtoResponse> data)
        {
            sendDocumentsModal.UpdateModalStatus(data.ModalStatus);
            sendDocumentContainer = data.Data;

            if (sendDocumentContainer != null)
            {
                notificationModal.UpdateModal(ModalType.Information, "Confirmar acción", true);
            }
            else
            {
                Console.WriteLine("no se pudo crear la tarea");
            }

        }

        private void HandleDocumentRelation(MyEventArgs<int> data)
        {
            docRelationModal.UpdateModalStatus(data.ModalStatus);
            docRelationContainer = data.Data;
        }

        private void HandleAttachmentList(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            attachmentsModal.UpdateModalStatus(data.ModalStatus);
            attachmentsContainer = data.Data;
        }

        #region LoadFileFromMasterList

        private async Task HandleSelectedFile(MyEventArgs<TemplateModelDtoResponse> file)
        {
            formatMasterListModal.UpdateModalStatus(file.ModalStatus);
            TemplateId = file.Data.TemplateId;
            SelectedFile = file.Data.Archive.ToString();

            try
            {
                panel_2 = true;
                StateHasChanged();
                await richEditControl.NewDocumentAsync();

                byte[] fileContent = Convert.FromBase64String(SelectedFile);
                await richEditControl.LoadDocumentAsync(fileContent, DocumentFormat.OpenXml);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
        }

        #endregion

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

        #region CreateDocumentTask

        private async Task CreateDocumentTask()
        {
            #region Creación de Objeto
            var DocumentaryTask = new DocumentaryTaskDtoRequest
            {
                //En el primer paso se debe de capturar el Template
                TemplateId = TemplateId,

                //En el Segundo paso se captura: el id del Behavior, el asunto de la radicacion, el destinatario de la radicación
                DocumentaryTypologyBehaviorId = docClasContainer.IdTypology,
                ClassCode = docClasContainer.ClassCode,
                DocDescription = docClasContainer.Description ?? string.Empty,
                ShipingMethod = string.IsNullOrEmpty(docClasContainer.ShipingMethod) ? null : docClasContainer.ShipingMethod
            };


            //Se agregan los desinatarios que proviene de la clasificacion y pertenecen a los destinatarios de la radicación
            List<DestinationsDtoRequest> Destinations = new();

            if (docClasContainer.DestinationsUser.Count > 0)
            {
                foreach (var userDes in docClasContainer.DestinationsUser)
                {
                    var tempUser = new DestinationsDtoRequest { DestinyId = userDes.UserId, DestinyType = userDes.type, ItsCopy = false };
                    Destinations.Add(tempUser);
                }
            }
            else
            {
                foreach (var userDes in docClasContainer.DestinationsAdministration)
                {
                    var tempUser = new DestinationsDtoRequest { DestinyId = (int)(userDes.ThirdPartyId ?? userDes.ThirdUserId), DestinyType = userDes.type, ItsCopy = false };
                    Destinations.Add(tempUser);
                }
            }

            //En el Tercer Paso se captura : El process code
            if (docRelationContainer != 0)
            {
                DocumentaryTask.DocumentRelationId = docRelationContainer;
            }

            if (attachmentsContainer != null)
            {
                DocumentaryTask.Attachments = attachmentsContainer;
            }

            DocumentaryTask.TaskManagementRequest.ProcessCode = "TAINS," + ProcessCode;

            if (ProcessCode.Equals("PR"))
            {
                //En el Cuarto Paso se captura:  destinatario, instruction code, task description

                DocumentaryTask.TaskManagementRequest.UserForwardId = sendDocumentContainer.Recivers.UserId;
                DocumentaryTask.TaskManagementRequest.InstructionCode = sendDocumentContainer.Instruction;
                DocumentaryTask.TaskManagementRequest.Comentary = sendDocumentContainer.Description ?? string.Empty;
            }

            DocumentaryTask.TaskManagementRequest.UserTaskId = UserId;
            DocumentaryTask.TaskDescription = docClasContainer.Description ?? string.Empty;
            //Captura datos adicionales del Documentary Task
            DocumentaryTask.CreateUserId = UserId;

            //Fecha de Vencimiento es un parametro de APPKEYS para agregar al momento de la creacion del documento los dias que tiene para vencimiento.
            DocumentaryTask.TaskDate = DateTime.Now;
            DocumentaryTask.DueDate = DateTime.Now.AddDays(10);


            await richEditControl.SaveDocumentAsync();
            // Captura de datos del TaskManagemetn
            DocumentaryTask.TaskManagementRequest.Archive = TaskManagementFile;

            //Captura de Firmas
            DocumentaryTask.Signatures = signatures;

            DocumentaryTask.Destinations = Destinations;

            if (_Copys.Any())
            {
                DocumentaryTask.Destinations.AddRange(_Copys);
            }
            #endregion

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/CreateDocumentaryTask", DocumentaryTask);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentaryTaskDtoResponse>>();

            if (deserializeResponse.Succeeded)
            {
                notificationModal2.UpdateModal(ModalType.Success, $"La tarea documental ha sido creada correctamente", true);
                cancelTask = true;
            }
            else
            {
                notificationModal2.UpdateModal(ModalType.Error, $"Ocurrio un error al intentar crear una tarea documental", true);
                cancelTask = false;
            }

        }

        #endregion

        #region SaveDocumentContent

        private async Task OnDocumentContentChanged(byte[] content)
        {
            TaskManagementFile = content;
        }

        #endregion

        #region CustomizedSigns and Load/Validate Signs
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

        private async Task LoadSign(string type)
        {
            SignatureDtoRequest signRegister = new();
            signRegister.SignUserId = 4076;
            signRegister.SignatureDate = DateTime.Now;

            if (type.Equals("ME", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIME";
                signRegister.SignatureType = TypeSign;
                modalSecondPass.UpdateModalStatus(true);
                

            }
            else if (type.Equals("TU", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIRU";
                signRegister.SignatureType = TypeSign;
                modalSecondPass.UpdateModalStatus(true);

                
            }

            signatures.Add(signRegister);
        }

        private async Task<string> GetSign(int FileId)
        {
            HttpClient?.DefaultRequestHeaders.Remove("FileId");
            HttpClient?.DefaultRequestHeaders.Add("FileId", FileId.ToString());
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
            HttpClient?.DefaultRequestHeaders.Remove("FileId");

            if (deserializeResponse != null) { return deserializeResponse.Data!.DataFile; }

            return "";
        }

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

        #endregion

        #endregion

        #endregion

    }
}
