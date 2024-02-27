using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Document.DocumentaryTask
{
    public partial class DocumentClasificationModal
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
        public EventCallback<bool> OnStatusChangedTRD { get; set; }
        [Parameter]
        public EventCallback<bool> OnStatusChangedUser { get; set; }
        [Parameter]
        public EventCallback<int> ChangeModal { get; set; }
        [Parameter]
        public EventCallback<DocumentClasificationDtoResponse> DocClasification { get; set; }

        #endregion

        #region Models

        private VDocumentaryTypologyDtoResponse TRDSelected = new();
        private MetaModel meta = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string systemParamCL = "";
        private string systemParamMR = "";
        private string defaulTextCL = "Selecciona Clase";
        private string defaulTextMR = "Seleccione medio de envío";

        public string descriptionInput { get; set; }

        #endregion

        #region Environments(Numeric)

        private int contadorcarac = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool dropdownsEnable = true;
        private bool texAreaEnable = false;
        private bool isuser = false;
        private bool SeenSortDocs = false;
        private bool isEnableTRDButton = true;
        private bool isEnableReceiverButton = true;
        private bool isEnableActionButton = true;
        private bool validList = false;
        private bool dropdown = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<UserClasification> UserList = new();
        private List<AdministrationUsers> ThirdList = new();
        private DocumentClasificationDtoResponse docClasification = new();
        private List<VSystemParamDtoResponse> systemFieldsCLList = new();
        private List<VSystemParamDtoResponse> systemFieldsMRList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetClassCom();
        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
        }

        #endregion

        #region OthersMethods

        #region GetComunicationClass

        public async Task GetClassCom()
        {
            try
            {

                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Data != null)
                {
                    systemFieldsCLList = deserializeResponse.Data.Where(x => !x.FieldCode.Equals("R")).Select(x => x).ToList() ?? new();
                    meta = deserializeResponse.Meta;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la clase de comunicacion: {ex.Message}");
            }
        }

        #endregion

        #region GetShippingMethod

        public async Task GetShippingM()
        {
            try
            {

                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "MR");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Data != null)
                {
                    systemFieldsMRList = deserializeResponse.Data;
                    meta = deserializeResponse.Meta;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el medio de envío: {ex.Message}");
            }
        }

        #endregion

        #region DropDown

        public async Task Dropdown(string value)
        {
            systemParamCL = value;

            if (value.Equals("CL,E"))
            {
                await GetShippingM();
                dropdown = true;
                isuser = true;
                isEnableTRDButton = false;
                TRDSelected = new();
                UserList = new();
                ThirdList = new();
            }
            else
            {
                dropdown = false;
                isuser = false;
                isEnableTRDButton = false;
                systemParamMR = null;
                TRDSelected = new();
                UserList = new();
                ThirdList = new();
            }
            await ChangeModal.InvokeAsync(isuser ? 2 : 1);
        }

        #endregion

        #region DeleteDestinations

        private async Task<bool> DeleteDestinations(int taskId, int id)
        {

            UserDeleteDtoRequest Destinations = new UserDeleteDtoRequest()
            {
                TaskId = taskId,
                UserId = id
            };

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteDestinations", Destinations);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

            if (deserializeResponse.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region DeleteUserResiver

        private async Task DeleteUserResiver(UserClasification user)
        {

            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteDestinations(user.TaskId.Value, user.UserId);

                if (deleteValidation)
                {
                    UserList.Remove(user);
                    docClasification.DestinationsUser.Remove(user);
                }
            }
            else
            {
                UserList.Remove(user);
                docClasification.DestinationsUser.Remove(user);
            }
        }

        #endregion

        #region DeleteThirdResiver

        private async Task DeleteThirdResiver(AdministrationUsers user)
        {

            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteDestinations(user.TaskId.Value, (int)(user.ThirdPartyId ?? user.ThirdUserId));

                if (deleteValidation)
                {
                    ThirdList.Remove(user);
                    docClasification.DestinationsAdministration.Remove(user);
                }
            }
            else
            {
                ThirdList.Remove(user);
                docClasification.DestinationsAdministration.Remove(user);
            }
        }

        #endregion

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;

                if (contadorcarac >= 5)
                {
                    isEnableActionButton = false;
                }
                else
                {
                    isEnableActionButton = true;
                }
            }
            else
            {
                contadorcarac = 0;
            }
        }

        #endregion

        #region SendDocumentClasification

        private async Task SelectSortDocAsync()
        {
            docClasification.Description = descriptionInput;
            docClasification.ClassCode = systemParamCL;

            if (systemParamMR != null)
            {
                docClasification.ShipingMethod = systemParamMR;
            }

            docClasification.ShipingMethod = systemParamMR;
            docClasification.IdTypology = TRDSelected.DocumentaryTypologyBehaviorId;
            docClasification.DestinationsUser = UserList;
            docClasification.DestinationsAdministration = ThirdList;

            await DocClasification.InvokeAsync(docClasification);
        }

        #endregion

        #region UpdateDocumentClasification

        public async Task UpdateDocClasification(int id, bool value, DocumentClasificationDtoResponse doc)
        {
            try
            {
                SeenSortDocs = !value;
                dropdownsEnable = !SeenSortDocs;
                texAreaEnable = SeenSortDocs;


                HttpClient?.DefaultRequestHeaders.Remove("TaskId");
                HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DocumentClasificationDtoResponse>>("documentarytasks/DocumentaryTask/GetClasificationTask");
                HttpClient?.DefaultRequestHeaders.Remove("TaskId");

                docClasification = deserializeResponse.Data;
                docClasification.DestinationsUser = docClasification.DestinationsUser ?? new();
                docClasification.DestinationsAdministration = docClasification.DestinationsAdministration ?? new();

                if (SeenSortDocs)
                {
                    defaulTextCL = docClasification.ComunicationClass;
                    systemParamCL = docClasification.ClassCode;
                    await Dropdown(systemParamCL);
                    defaulTextMR = docClasification.ReceptionCode;
                    isEnableTRDButton = true;
                    systemParamMR = docClasification.ShipingMethod;
                    descriptionInput = docClasification.Description;
                }
                else
                {
                    isEnableReceiverButton = false;
                    isEnableActionButton = false;

                    if (doc != null)
                    {
                        systemParamCL = doc.ClassCode;
                        await Dropdown(systemParamCL);
                        systemParamMR = doc.ShipingMethod;
                        docClasification.DestinationsAdministration.AddRange(doc.DestinationsAdministration);
                        docClasification.DestinationsUser.AddRange(doc.DestinationsUser);
                        descriptionInput = doc.Description;
                        docClasification.TypologyName = doc.TypologyName;
                        docClasification.AdministrativeUnitName = doc.AdministrativeUnitName;
                        docClasification.ProductionOfficeName = doc.ProductionOfficeName;
                        docClasification.SeriesName = doc.SeriesName;
                        docClasification.SubSeriesName = doc.SubSeriesName;
                    }
                    else
                    {
                        systemParamCL = docClasification.ClassCode;
                        await Dropdown(systemParamCL);
                        systemParamMR = docClasification.ShipingMethod;
                        descriptionInput = docClasification.Description;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la clasificacion del documento: {ex.Message}");
            }
        }

        #endregion

        #region Data Modals

        public void GetReceiverUsersData(List<VUserDtoResponse> receiver)
        {
            UserList.AddRange(receiver.Select(x => new UserClasification
            {
                UserId = x.UserId,
                type = "TDF,U",
                FullName = x.FullName,
                AdministrativeUnitName = x.AdministrativeUnitName,
                ProductionOfficeName = x.ProductionOfficeName,
                Charge = x.Charge
            }).ToList());

            if (docClasification.DestinationsUser == null)
            {
                docClasification.DestinationsUser = new();
            }

            docClasification.DestinationsUser.AddRange(UserList);
            validList = true;
        }

        public void GetReceiverThirdData(List<ThirdPartyDtoResponse> receiver1, List<ThirdUserDtoResponse> receiver2)
        {
            ThirdList.AddRange(receiver1.Select(x => new AdministrationUsers
            {
                ThirdPartyId = x.ThirdPartyId,
                type = "TDF,T",
                CompanyName = x.Names,
                IdentificationNumber = x.IdentificationNumber,
                IdentificationTypeName = x.IdentificationTypeName,
                Email = (string.IsNullOrEmpty(x.Email1) ? x.Email2 : x.Email1)
            }).ToList());

            ThirdList.AddRange(receiver2.Select(x => new AdministrationUsers
            {
                ThirdUserId = x.ThirdUserId,
                type = "TDF,TU",
                CompanyName = x.CompanyName,
                IdentificationNumber = x.IdentificationNumber,
                IdentificationTypeName = x.IdentificationTypeName,
                Email = x.Email
            }).ToList());

            if (docClasification.DestinationsAdministration == null)
            {
                docClasification.DestinationsAdministration = new();
            }

            docClasification.DestinationsAdministration.AddRange(ThirdList);

            validList = false;
        }

        public void GetTRDSelectedData(VDocumentaryTypologyDtoResponse trd)
        {
            TRDSelected = trd;
            isEnableReceiverButton = false;
            docClasification.TypologyName = TRDSelected.TypologyName;
            docClasification.AdministrativeUnitName = TRDSelected.AdministrativeUnitName;
            docClasification.ProductionOfficeName = TRDSelected.ProductionOfficeName;
            docClasification.SeriesName = TRDSelected.SeriesName;
            docClasification.SubSeriesName = TRDSelected.SubSeriesName;
        }
        #endregion Data Modals

        #region Call Modals
        private async Task OpenNewModalTRD()
        {
            await OnStatusChangedTRD.InvokeAsync(true);
        }

        private async Task OpenNewModalUser()
        {
            await OnStatusChangedUser.InvokeAsync(true);
            StateHasChanged();
        }

        #endregion Call Modals

        #endregion

        #endregion

    }
}