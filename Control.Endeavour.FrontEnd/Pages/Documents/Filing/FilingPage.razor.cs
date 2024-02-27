using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.MetaData;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Label;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Models.Enums.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Radication;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Documents.Filing
{
    public partial class FilingPage
    {
        #region Variables

        #region Inject

        [Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private FilingStateContainer? FilingSC { get; set; }

        #endregion Inject

        #region Components

        private string DocDescription { get; set; } = string.Empty;
        private string Pages { get; set; } = string.Empty;
        private string ReceptionCode { get; set; } = string.Empty;
        private string PriorityCode { get; set; } = "RPRI,SE";
        private DateTime DueDate { get; set; } = DateTime.Now.AddDays(1);
        private string InternalDocument { get; set; } = string.Empty;
        private string GuideNumber { get; set; } = string.Empty;
        private int CountryId { get; set; }
        private int StateId { get; set; }
        private int CityId { get; set; } = 1002;
        private string? ValueNotificacion { get; set; }

        #endregion Components

        #region Parameters

        [Parameter] public string? FilingClass { get; set; }

        #endregion Parameters

        #region Models

        private MetaDataRelationModal metaDataRelationModal { get; set; } = new();
        private GenericSearchModal GenericSearchModal { get; set; } = new();
        private MetaModel Meta = new();
        private GenericDocTypologySearchModal DocTypologySearchModal = new();
        private AttachmentsModal ModalAttachments = new();

        private MetaDataValueModal metaDataValueModal { get; set; } = new();
        private NotificationsComponentModal notificationModal = new();

        //private ModalVisualizacionMetadatos VisualizacionMetadatos = new();
        private VDocumentaryTypologyDtoResponse TRDSelected = new();

        private FilingDtoRequest FilingRequest = new();
        private LabelModal LabelModal = new();
        private AttachmentsType Filing = AttachmentsType.Filing;

        #endregion Models

        #region Environments

        #region Environments(String)

        //Importante para la modal de radicación
        private readonly string defaultTitle = "Comunicaciones -  Radicación de comunicación recibida";

        private string title { get; set; } = string.Empty;

        // <--[ Variables para ocultar pasos de la radicación ]-->

        private string panel_1 = ""; //Paso 1
        private string panel_2 = "d-none"; //Paso 2
        private string panel_3 = "d-none"; //Paso 3
        private string panel_4 = "d-none"; //Paso 4
        private string panel_5 = "d-none"; //Paso 5
        private string panel_6 = "d-none"; //Paso 6
        private string panel_7 = "d-none"; //Paso 7
        private string panel_8 = "d-none"; //Paso 8
        private string panel_9 = "d-none"; //Paso 9

        private string TablaUsers = "d-none";
        private string TablaAdjuntos = "d-none";
        private string Radicado = "";
        private string IdDocumento = "";
        private string Anio = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        //Importante para la modal de radicacion
        private readonly int defaultConfiguration = 3;

        private int configuration { get; set; } = 0;
        private decimal contadorcarac = 0;

        #endregion Environments(Numeric)

        #region Environments(DateTime)

        private DateTime Min = DateTime.Now;
        private DateTime Max = new DateTime(2025, 1, 1, 19, 30, 45);

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool multipleSelection { get; set; } = false;
        private bool EnabledDepartamento { get; set; } = true;
        private bool EnabledMunicipio { get; set; } = true;
        private bool DisableButtons { get; set; } = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<MetaDataRelationDtoRequest> lstMetaDataRelation { get; set; } = new();
        private List<VSystemParamDtoResponse>? lstReceptionCode { get; set; } = new();
        private List<VSystemParamDtoResponse>? lstPriorityCode { get; set; } = new();
        private List<VSystemParamDtoResponse>? lstNotificacion { get; set; } = new();
        private List<CountryDtoResponse>? lstCountryId { get; set; } = new();
        private List<StateDtoResponse>? lstStateId { get; set; } = new();
        private List<CityDtoResponse>? lstCityId { get; set; } = new();
        private List<PersonInRadication> listOfRadication { get; set; } = new();
        private List<AttachmentsDtoRequest> listAttachment { get; set; } = new();
        private List<PersonInRadication> listSender { get; set; } = new();
        private List<PersonInRadication> listCopy { get; set; } = new();
        private List<PersonInRadication> listRecipient { get; set; } = new();
        private List<FileInfoData> lstFileInfoData { get; set; } = new();
        private List<AttachmentsDtoRequest> Attachments { get; set; } = new();

        // <--[ Diccionarios que determinan los valores que se van a validar para habilitar los diferentes pasos de la radicación ]-->

        private Dictionary<string, string> Panel1 = new Dictionary<string, string>();
        private Dictionary<string, string> Panel2 = new Dictionary<string, string>();
        private Dictionary<string, string> Panel4 = new Dictionary<string, string>();
        private Dictionary<string, string> Panel5 = new Dictionary<string, string>();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetDocumentTypeTCR();
                await GetPriority();
                await GetCountry();
                await GetNotification();
                GeneratePanels();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            //PageLoadService.OcultarSpinnerReadLoad(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleTRDSelectedChanged(MyEventArgs<VDocumentaryTypologyDtoResponse> trd)
        {
            DocTypologySearchModal.resetModal();
            DocTypologySearchModal.UpdateModalStatus(trd.ModalStatus);
            TRDSelected = trd.Data;
        }

        private void HandleMetaDataRelationSelected(MyEventArgs<MetaDataRelationDtoRequest> data)
        {
            metaDataValueModal.UpdateModalStatus(data.ModalStatus);
            metaDataRelationModal.UpdateModalStatus(!data.ModalStatus);
        }

        private void HandleSerachModalToUse(MyEventArgs<SearchConfigurationArgs> data)
        {
            configuration = data.Data.Configuration;
            title = data.Data.Title;
            multipleSelection = data.Data.MultipleSelection;
            GenericSearchModal.UpdateModalStatus(data.ModalStatus);
            metaDataValueModal.UpdateModalStatus(!data.ModalStatus);
        }

        private void HandleUsersMetaDataSelected(MyEventArgs<List<object>> request)
        {
            metaDataValueModal.UserSelectionMetaData((List<VUserDtoResponse>)request.Data[0]);
            GenericSearchModal.UpdateModalStatus(request.ModalStatus);
            metaDataValueModal.UpdateModalStatus(!request.ModalStatus);
        }

        private void HandleThirdPartyMetaDataSelected(MyEventArgs<ThirdPartyDtoResponse> request)
        {
            metaDataValueModal.ThirdPartySelectionMetaData(request.Data);
            GenericSearchModal.UpdateModalStatus(request.ModalStatus);
            metaDataValueModal.UpdateModalStatus(!request.ModalStatus);
        }

        private async Task HandleAttachments(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            Attachments = data.Data;

            if (Attachments?.Count > 0)
            {
                foreach (var item in Attachments)
                {
                    FileInfoData fileData = new FileInfoData()
                    {
                        Name = item.ArchiveName,
                        Extension = item.ArchiveExt,
                        Size = item.Size,
                        IconPath = item.IconPath,
                        Description = item.AttDescription
                    };

                    lstFileInfoData.Add(fileData);

                    AttachmentsDtoRequest attachmentsData = new AttachmentsDtoRequest()
                    {
                        DataFile = item.DataFile,
                        ArchiveName = item.ArchiveName,
                        ArchiveExt = item.ArchiveExt.ToString().Replace(".", ""), //Es importante que no lleve punto
                        ExhibitCode = item.ExhibitCode,
                        AttCode = item.AttCode,
                        AttDescription = item.AttDescription
                    };

                    listAttachment.Add(attachmentsData);
                }

                if (lstFileInfoData.Count > 0)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡El/Los adjunto(s) fueron cargados de forma exitosa!", true, "Aceptar", "", "", "");
                    TablaAdjuntos = "";
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar el/los adjunto(s), por favor intente de nuevo!!", true);
                    TablaAdjuntos = "d-none";
                }
            }
            else
            {
                listAttachment = new List<AttachmentsDtoRequest>();
            }

            await Task.Delay(1000);
        }

        private void HandleRadicationChanged(MyEventArgs<List<PersonInRadication>> listOfRadicationUsers)
        {
            listOfRadication = new();
            listOfRadication = listOfRadicationUsers.Data;
            GenericSearchModal.UpdateModalStatus(listOfRadicationUsers.ModalStatus);

            if (listOfRadication.Count > 0)
            {
                listRecipient = listOfRadication.Where(x => x.TypeOfPersonInRadication == "Recipient").ToList();
                listSender = listOfRadication.Where(x => x.TypeOfPersonInRadication == "Sender").ToList();
                listCopy = listOfRadication.Where(x => x.TypeOfPersonInRadication == "Copy").ToList();
                ActivarPanel(listOfRadication.Count.ToString(), "DESTINATARIOS", 4);
                TablaUsers = "";
            }
            else
            {
                listRecipient = new();
                listSender = new();
                listCopy = new();
                ActivarPanel("", "DESTINATARIOS", 4);
                TablaUsers = "d-none";
            }
        }

        private async Task HandleFormCreate()
        {
            //PageLoadService.MostrarSpinnerReadLoad(Js);
            List<DocumentSignatory> listSignatory = new List<DocumentSignatory>();
            List<DocumentReceiver> listReceiver = new List<DocumentReceiver>();
            List<MetaData> _lstMetaDataRelation = LinkMetaDataToPetition(lstMetaDataRelation);

            foreach (var item in listSender)
            {
                var obj = new DocumentSignatory() { SignatoryId = item.Id, TypeSignatory = "TDF,T" }; //TDF,TU
                listSignatory.Add(obj);
            }

            foreach (var item in listRecipient)
            {
                var obj = new DocumentReceiver() { ReceiverId = item.Id, TypeReceiver = "TDF,U", ItsCopy = false };
                listReceiver.Add(obj);
            }

            foreach (var item in listCopy)
            {
                bool copy = item.TypeOfPersonInRadication == "Copy" ? true : false;
                var obj = new DocumentReceiver() { ReceiverId = item.Id, TypeReceiver = "TDF,U", ItsCopy = copy };
                listReceiver.Add(obj);
            }

            FilingRequest = new FilingDtoRequest()
            {
                DocumentaryTypologyBehaviorId = TRDSelected.DocumentaryTypologyBehaviorId,
                RecordId = 0,
                FileId = 0,
                DocDescription = DocDescription,
                Pages = Convert.ToInt32(Pages),
                ReceptionCode = ReceptionCode,
                PriorityCode = PriorityCode,
                DueDate = DueDate,
                Parameters = null,
                DocumentSignatory = listSignatory,
                DocumentReceiver = listReceiver,
                Attachment = listAttachment,//new List<Attachments>(),
                metaData = _lstMetaDataRelation,// new List<MetaData>(),
                AutomaticShipping = "NO",
                EndManagement = "NO",
                NameSignatory = "NO",
                DueDays = 0,
                DueHours = 0,
                InternalDocument = InternalDocument,
                GuideNumber = GuideNumber,
                CountryId = CountryId,
                StateId = StateId,
                CityId = CityId,
                User = "JulianDB",
                UserId = 4072,
                UserAssingId = listReceiver.First().ReceiverId
            };

            if (!string.IsNullOrWhiteSpace(ValueNotificacion))
            {
                FilingRequest.Parameters = new(){
                    { "NOTIFICACION", ValueNotificacion }
                };
            }

            var responseApi = await HttpClient.PostAsJsonAsync("documents/Filing/Filing", FilingRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<FilingDtoResponse>>();

            if (deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Success, "¡Se ha generado de forma exitosa el radicado: " + deserializeResponse.Data.FilingCode + "!", true);
                Radicado = deserializeResponse.Data.FilingCode;
                IdDocumento = deserializeResponse.Data.ControlId.ToString();
                Anio = DateTime.Now.Year.ToString();

                #region FillOutFiling

                FilingSC?.Parametros(false);
                FilingSC.FilingNumber = Radicado;
                FilingSC.DocumentId = IdDocumento;
                FilingSC.Folios = Pages;
                FilingSC.Annexes = listAttachment.Count.ToString();

                if (listRecipient.Count > 1)
                {
                    FilingSC.Recipients = listRecipient.First().FullName + " - " + listRecipient.First().Charge + ", Otro(s)";
                }
                else
                {
                    FilingSC.Recipients = listRecipient.First().FullName + " - " + listRecipient.First().Charge;
                }

                #endregion FillOutFiling

                SubsequentProcesses(1);
                //PageLoadService.OcultarSpinnerReadLoad(Js);
                ActivarPanel(deserializeResponse.Data.FilingCode, "RADICACION", 5);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de generar el radicado, por favor intente de nuevo!", true, "Aceptar");
                FilingSC?.Parametros(false);
                //PageLoadService.OcultarSpinnerReadLoad(Js);
            }
        }

        private void HandleMetaDataSelected(MyEventArgs<MetaDataRelationDtoRequest> data)
        {
            metaDataRelationModal.UpdateModalStatus(!data.ModalStatus);
            metaDataValueModal.MetaFieldSelected(data.Data);
            metaDataValueModal.UpdateModalStatus(data.ModalStatus);
        }

        private void HandleMetaDataUpdated(MyEventArgs<List<MetaDataRelationDtoRequest>> data)
        {
            lstMetaDataRelation = data.Data;

            if (lstMetaDataRelation == null || lstMetaDataRelation.Count == 0)
            {
                lstMetaDataRelation = new();
            }
        }

        private void HandleModalClosed()
        {
            LabelModal.UpdateModalStatus(false);
        }

        #endregion HandleMethods

        #region GetMethods

        private async Task GetDocumentTypeTCR()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "MR");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstReceptionCode = deserializeResponse.Data;
                }
                else { lstReceptionCode = new(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

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
                    lstPriorityCode = deserializeResponse.Data;
                }
                else { lstPriorityCode = new(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
            }
        }

        private async Task GetCountry()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilter");

                if (deserializeResponse!.Succeeded)
                {
                    lstCountryId = deserializeResponse.Data;

                    if (lstCountryId.Count > 0)
                    {
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                    else
                    {
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                else
                {
                    lstCountryId = new();
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el país: {ex.Message}");
            }
        }

        private async Task GetState()
        {
            try
            {
                if (CountryId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    HttpClient?.DefaultRequestHeaders.Add("countryId", CountryId.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");

                    if (deserializeResponse!.Succeeded)
                    {
                        lstStateId = deserializeResponse.Data;

                        if (lstStateId.Count > 0)
                        {
                            CityId = 0;
                            EnabledDepartamento = true;
                            EnabledMunicipio = false;
                        }
                        else
                        {
                            EnabledDepartamento = false;
                            EnabledMunicipio = false;
                        }
                    }
                    else
                    {
                        lstStateId = new();
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                else
                {
                    StateId = 0;
                    CityId = 0;
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el departamento: {ex.Message}");
            }
        }

        private async Task GetCity()
        {
            try
            {
                if (StateId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");
                    HttpClient?.DefaultRequestHeaders.Add("stateId", StateId.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CityDtoResponse>>>("location/City/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");

                    if (deserializeResponse!.Succeeded)
                    {
                        lstCityId = deserializeResponse.Data == null ? new List<CityDtoResponse>() : deserializeResponse.Data;

                        if (lstCityId.Count > 0)
                        {
                            EnabledMunicipio = true;
                        }
                        else
                        {
                            EnabledMunicipio = false;
                        }
                    }
                    else
                    {
                        lstStateId = new();
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                else
                {
                    CityId = 0;
                    EnabledMunicipio = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el municipio: {ex.Message}");
            }
        }

        private async Task GetNotification()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RNOTI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstNotificacion = deserializeResponse.Data;
                }
                else { lstNotificacion = new(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la notificación: {ex.Message}");
            }
        }

        #endregion GetMethods

        #region ShowModalsMethods

        private void showModal(bool value)
        {
            DocTypologySearchModal.UpdateModalStatus(true);
        }

        private void showModalAttachments()
        {
            ModalAttachments.UpdateModalStatus(true);
        }

        private async Task showModalMetadatos()
        {
            if (!TRDSelected.DocumentaryTypologyBagId.HasValue)
            {
                notificationModal.UpdateModal(ModalType.Error, "¡No existen metadatos asociados en la bolsa de tipologías!", true);
            }

            if (lstMetaDataRelation == null || lstMetaDataRelation.Count == 0)
            {
                await metaDataRelationModal.SearchByDocumentaryTypology((int)TRDSelected.DocumentaryTypologyBagId);
            }
            else
            {
                metaDataRelationModal.existingMetaDataRelations(lstMetaDataRelation);
            }

            metaDataRelationModal.UpdateModalStatus(true);
        }

        private void showModalLabel()
        {
            LabelModal.UpdateModalStatus(true);
        }

        private void showModalUploaPDF()
        {
            // cargarImagen.UpdateModalStatus(true);
        }

        private void showRecipient()
        {
            title = defaultTitle;
            configuration = defaultConfiguration;

            GenericSearchModal.UpdateModalStatus(true);
        }

        #endregion ShowModalsMethods

        #region ValidationMethods

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                contadorcarac = value.Length;
                if (contadorcarac >= 10)
                {
                    ActivarPanel(value, "ASUNTO", 2);
                }
                else
                {
                    ActivarPanel("", "ASUNTO", 2);
                }
            }
            else
            {
                contadorcarac = 0;
                ActivarPanel(value, "ASUNTO", 2);
            }
        }

        private void GeneratePanels()
        {
            #region PANEL 1

            Panel1["TIPODOCUMENTO"] = "";
            Panel1["TRAMITEGESTOR"] = "";

            #endregion PANEL 1

            #region PANEL 2

            Panel2["PAIS"] = "";
            Panel2["DEPARTAMENTO"] = "";
            Panel2["MUNICIPIO"] = "";
            Panel2["FOLIOS"] = "";
            Panel2["ASUNTO"] = "";

            #endregion PANEL 2

            #region PANEL 4

            Panel4["DESTINATARIOS"] = "";

            #endregion PANEL 4

            #region PANEL 5

            Panel5["RADICACION"] = "";

            #endregion PANEL 5
        }

        private void ActivarPanel(string value, string componente, decimal panel)
        {
            bool habPanel2 = false;
            bool habPanel3 = false;
            bool habPanel5 = false;
            bool habPanel6 = false;

            if (Panel1.Count > 0 && Panel2.Count > 0 && Panel4.Count > 0)
            {
                switch (panel)
                {
                    case 1:
                        if (Panel1.ContainsKey(componente))
                        {
                            Panel1[componente] = value;
                            ReceptionCode = componente == "TIPODOCUMENTO" ? Panel1[componente] : ReceptionCode != "" ? ReceptionCode : "";
                        }

                        habPanel2 = Panel1.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel3 = Panel2.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel5 = Panel4.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel2) //Habilita el paso 2
                        {
                            panel_2 = "d-flex justify-content-center";

                            if (habPanel3) //Habilita el paso 3
                            {
                                panel_3 = panel_4 = "d-flex justify-content-center";

                                if (habPanel5) //Habilita el paso 5
                                {
                                    panel_5 = "d-flex justify-content-center";

                                    if (habPanel6) //Habilita el paso 5 solo si el paso 4 esta completo
                                    {
                                        panel_6 = panel_7 = "d-flex justify-content-center";
                                    }
                                    else
                                    {
                                        panel_6 = panel_7 = "d-none";
                                    }
                                }
                                else
                                {
                                    panel_5 = panel_6 = panel_7 = "d-none";
                                }
                            }
                            else
                            {
                                panel_3 = panel_4 = panel_5 = panel_6 = panel_7 = "d-none";
                            }
                        }
                        else
                        {
                            panel_2 = panel_3 = panel_4 = panel_5 = panel_6 = panel_7 = "d-none";
                        }
                        break;

                    case 2:
                        if (Panel2.ContainsKey(componente))
                        {
                            Panel2[componente] = value;
                            CountryId = componente == "PAIS" ? Convert.ToInt32(Panel2[componente]) : CountryId > 0 ? CountryId : 0;
                            StateId = componente == "DEPARTAMENTO" ? Convert.ToInt32(Panel2[componente]) : StateId > 0 ? StateId : 0;
                            CityId = componente == "MUNICIPIO" ? Convert.ToInt32(Panel2[componente]) : CityId > 0 ? CityId : 0;

                            if (CountryId == 0)
                            {
                                Panel2["DEPARTAMENTO"] = "";
                                Panel2["MUNICIPIO"] = "";
                            }
                        }

                        habPanel3 = Panel2.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel5 = Panel4.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel3) //Habilita el paso 3
                        {
                            panel_3 = panel_4 = "d-flex justify-content-center";
                            FilingSC?.Parametros(true);

                            if (habPanel5) //Habilita el paso 5
                            {
                                panel_5 = "d-flex justify-content-center";

                                if (habPanel6) //Habilita el paso 6 solo si el paso 5 esta completo
                                {
                                    panel_6 = panel_7 = "d-flex justify-content-center";
                                }
                                else
                                {
                                    panel_6 = panel_7 = "d-none";
                                }
                            }
                            else
                            {
                                panel_5 = panel_6 = panel_7 = "d-none";
                            }
                        }
                        else
                        {
                            panel_3 = panel_4 = panel_5 = panel_6 = panel_7 = "d-none";
                            FilingSC?.Parametros(false);
                        }
                        break;

                    case 4:
                        if (Panel4.ContainsKey(componente))
                        {
                            Panel4[componente] = value;
                        }

                        habPanel5 = Panel4.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel5) //Habilita el paso 5
                        {
                            panel_5 = "d-flex justify-content-center";

                            if (habPanel6) //Habilita el paso 6 solo si el paso 5 esta completo
                            {
                                panel_6 = panel_7 = "d-flex justify-content-center";
                            }
                            else
                            {
                                panel_6 = panel_7 = "d-none";
                            }
                        }
                        else
                        {
                            panel_5 = panel_6 = panel_7 = "d-none";
                        }
                        break;

                    case 5:
                        if (Panel5.ContainsKey(componente))
                        {
                            Panel5[componente] = value;
                        }

                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel6) //Habilita el paso 6 solo si el paso 5 esta completo
                        {
                            panel_6 = panel_7 = "d-flex justify-content-center";
                        }
                        else
                        {
                            panel_6 = panel_7 = "d-none";
                        }

                        break;
                }
            }
        }

        #endregion ValidationMethods

        #region OthersMethods

        private void RemoverUser(int idUser, string typeOfPersonInRadication)
        {
            if (typeOfPersonInRadication == "Sender")
            {
                listSender.RemoveAll(x => x.Id == idUser);
                listOfRadication.RemoveAll(x => x.Id == idUser);
            }
            else if (typeOfPersonInRadication == "Recipient")
            {
                listRecipient.RemoveAll(x => x.Id == idUser);
                listOfRadication.RemoveAll(x => x.Id == idUser);
            }
            else if (typeOfPersonInRadication == "Copy")
            {
                listCopy.RemoveAll(x => x.Id == idUser);
                listOfRadication.RemoveAll(x => x.Id == idUser);
            }

            if (listOfRadication.Count > 0)
            {
                ActivarPanel(listOfRadication.Count.ToString(), "DESTINATARIOS", 4);
            }
            else
            {
                ActivarPanel("", "DESTINATARIOS", 4);
            }
        }

        private void SubsequentProcesses(decimal processes)
        {
            switch (processes)
            {
                case 1: //Bloquear funcionamiento

                    #region DisablePanels

                    panel_1 += " deshabilitar-content";
                    panel_2 += " deshabilitar-content";
                    panel_3 += " deshabilitar-content";
                    panel_4 += " deshabilitar-content";
                    panel_5 += " deshabilitar-content";

                    #endregion DisablePanels

                    DisableButtons = true;

                    break;

                case 2: //Nueva radicación

                    #region DisguisePaneles

                    panel_2 = "d-none";
                    panel_3 = "d-none";
                    panel_4 = "d-none";
                    panel_5 = "d-none";
                    panel_6 = "d-none";
                    panel_7 = "d-none";
                    panel_8 = "d-none";
                    panel_9 = "d-none";

                    #endregion DisguisePaneles

                    #region DeleteComponents

                    TRDSelected = new();
                    PriorityCode = "RPRI,SE";
                    CountryId = 0;
                    StateId = 0;
                    CityId = 0;
                    GuideNumber = "";
                    Pages = "";
                    InternalDocument = "";
                    ValueNotificacion = "";
                    DocDescription = "";
                    listAttachment = new();
                    listSender = new();
                    listRecipient = new();
                    listCopy = new();

                    #endregion DeleteComponents

                    DisableButtons = false;

                    break;

                case 3: // Mantener Datos

                    #region DisguisePaneles

                    panel_4 = " d-none";
                    panel_5 = " d-none";
                    panel_6 = " d-none";
                    panel_7 = " d-none";
                    panel_8 = " d-none";
                    panel_9 = " d-none";

                    #endregion DisguisePaneles

                    #region DeleteComponents

                    listAttachment = new();
                    listSender = new();
                    listRecipient = new();
                    listCopy = new();

                    #endregion DeleteComponents

                    DisableButtons = false;

                    break;
            }
        }

        private List<MetaData> LinkMetaDataToPetition(List<MetaDataRelationDtoRequest> dataToLink)
        {
            List<MetaData> metadataToLink = new();
            int cont = 1;
            foreach (var item in dataToLink)
            {
                metadataToLink.Add(
                    new()
                    {
                        MetaTitleId = item.MetaFieldId,
                        ColorData = item.ColorData,
                        DataText = item.DataText,
                        OrderData = cont,
                    }
                    );

                cont += 1;
            }
            return metadataToLink;
        }

        #endregion OthersMethods

        #endregion Methods
    }
}