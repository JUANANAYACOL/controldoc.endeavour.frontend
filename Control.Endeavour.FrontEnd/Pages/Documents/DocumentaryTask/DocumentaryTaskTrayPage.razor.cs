using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.City;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Documents.DocumentaryTask
{
    public partial class DocumentaryTaskTrayPage
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

        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModalSucces = new();
        private GenericSearchModal genericSearchModal = new();

        #endregion

        #region Parameters

        #endregion

        #region Models

        private MetaModel meta = new();
        private VDocumentaryTaskDtoResponse docToDelete = new();
        private VUserDtoResponse vUserSelected = new();
        private DataCardDocTaskDtoResponse dataCardsDocTask = new DataCardDocTaskDtoResponse();
        private PaginationComponent<VDocumentaryTaskDtoResponse, FilterManagementDtoRequest> paginationComponent = new();
        private FilterManagementDtoRequest filtro { get; set; } = new();
        public InputModalComponent docTaskInput { get; set; } = new();
        public InputModalComponent userInput { get; set; } = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string created = "";
        private string review = "";
        private string approve = "";
        private string toSign = "";
        private string signed = "";
        private string involved = "";
        private string title = "Enviado a";
        private string codeRV = "TAINS,RV";
        private string codeAP = "TAINS,AP";
        private string codeFR = "TAINS,FR";
        private string codePR = "TAINS,PR";
        private string id1 = "UserTaskId";
        private string id2 = "UserForwardId";
        private string codeP = "ProcessCode";
        private string codeI = "InstructionCode";
        public string descriptionInput { get; set; } = "";

        #endregion

        #region Environments(Numeric)

        private int id = 4055;
        private int contadorcarac = 0;

        #endregion

        #region Environments(DateTime)

        public DateTime? StartValue { get; set; } = DateTime.Now;
        public DateTime? EndValue { get; set; } = DateTime.Now.AddDays(10);

        DateTime Min = new DateTime(1990, 1, 1, 8, 15, 0);
        DateTime Max = new DateTime(2025, 1, 1, 19, 30, 45);

        #endregion

        #region Environments(Bool)

        private bool viewState = false;
        private bool startDate = false;
        private bool endDate = false;
        private bool activeState = false;

        private bool grid1 = false;
        private bool grid2 = false;
        private bool grid3 = false;
        private bool grid4 = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<string> codesList = new List<string>();
        private List<bool> allowsList = new List<bool>();
        private List<VDocumentaryTaskDtoResponse> documentaryTaskList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {

            if (string.IsNullOrEmpty(documentsStateContainer.Code))
            {
                await GetDataCards(codeP, id1, new List<string>() { codePR, codeFR }, new List<bool>() { true, false, true, true });
            }
            else
            {
                await GetDataCards(documentsStateContainer.Code, documentsStateContainer.UserType, documentsStateContainer.Codes, documentsStateContainer.Values);
            }

            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetNumericDataCards();
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void ShowUsersModal(bool value)
        {
            genericSearchModal.UpdateModalStatus(true);
        }

        private void HandlePaginationGrid(List<VDocumentaryTaskDtoResponse> newDataList)
        {
            documentaryTaskList = newDataList;
        }

        private async Task HandleTaskManagementSubmit(VDocumentaryTaskDtoResponse doc)
        {
            try
            {
                documentsStateContainer.ParametrosVisor(doc.TaskId, id);
                await CheckReadDocument(doc);
                navigation.NavigateTo("/TaskManagement");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private void HandleUserSelectedChanged(MyEventArgs<VUserDtoResponse> user)
        {
            genericSearchModal.UpdateModalStatus(user.ModalStatus);
            vUserSelected = user.Data;
        }

        #endregion

        #region OthersMethods

        #region GetNumericDataCardsDocumentTask

        private async Task GetNumericDataCards()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("UserId");
                HttpClient?.DefaultRequestHeaders.Add("UserId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DataCardDocTaskDtoResponse>>("documentarytasks/DocumentaryTask/GetCountTask");
                HttpClient?.DefaultRequestHeaders.Remove("UserId");
                dataCardsDocTask = deserializeResponse.Data;

                if (dataCardsDocTask != null)
                {
                    created = dataCardsDocTask.Created.ToString();
                    review = dataCardsDocTask.Review.ToString();
                    approve = dataCardsDocTask.Approve.ToString();
                    toSign = dataCardsDocTask.ToSign.ToString();
                    signed = dataCardsDocTask.Signed.ToString();
                    involved = dataCardsDocTask.Involved.ToString();
                }
                else
                {
                    created = "0";
                    review = "0";
                    approve = "0";
                    toSign = "0";
                    signed = "0";
                    involved = "0";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener N° de tareas documentales: {ex.Message}");
            }
        }

        #endregion

        #region GetDataCardsDocumentTask

        private async Task GetDataCards(string code, string userId, List<string> codes, List<bool> value)
        {
            try
            {

                filtro = new();

                filtro.UserForwardId = (userId.Equals(id2)) ? id : 0;
                filtro.UserTaskId = (userId.Equals(id1)) ? id : 0;
                filtro.InstructionCode = (code.Equals(codeI)) ? codes : new();
                filtro.ProcessCode = (code.Equals(codeP)) ? codes : new();

                if (codes.Count > 1)
                {
                    filtro.Indicted = null;
                }
                else
                {
                    filtro.Indicted = value[3];
                }

                grid1 = value[0];
                grid2 = value[1];
                grid3 = value[2];
                grid4 = value[0];

                if ((grid1 && grid3) && codes[0].Equals(codeFR))
                {
                    title = "Firmado por";
                    grid4 = false;
                }
                else
                {
                    title = "Enviado a";
                }

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/ByFilter", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VDocumentaryTaskDtoResponse>>>();

                if (deserializeResponse.Succeeded)
                {
                    documentaryTaskList = deserializeResponse.Data;
                    meta = deserializeResponse.Meta;
                    paginationComponent.ResetPagination(meta);
                }
                else
                {
                    documentaryTaskList = new();
                    meta = new();
                    paginationComponent.ResetPagination(meta);
                    Console.WriteLine(deserializeResponse.Message);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tareas documentales: {ex.Message}");
            }
        }

        #endregion

        #region GetDocumenTaskFilter

        private async Task GetDocumentsTaskFilter()
        {
            try
            {
                int taskId = string.IsNullOrEmpty(docTaskInput.InputValue) ? 0 : int.Parse(docTaskInput.InputValue);
                string description = string.IsNullOrEmpty(descriptionInput) ? "" : descriptionInput;
                DateTime? stDate = StartValue;
                DateTime? enDate = EndValue;

                if (!(startDate && endDate))
                {
                    stDate = null;
                    enDate = null;
                }

                FilterManagementDtoRequest filtro = new()
                {
                    TaskId = taskId,
                    StartDate = stDate,
                    EndDate = enDate,
                    TaskDescription = description,
                    UserForwardId = vUserSelected.UserId
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/ByFilter", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VDocumentaryTaskDtoResponse>>>();
                documentaryTaskList = deserializeResponse.Data.Where(x => x.UserTaskId == id).Select(x => x).ToList() ?? new();

                if (documentaryTaskList.Count != 0)
                {
                    meta = deserializeResponse.Meta;
                    paginationComponent.ResetPagination(meta);
                }
                else
                {
                    documentaryTaskList = new();
                    meta = deserializeResponse.Meta;
                    paginationComponent.ResetPagination(meta);
                    Console.WriteLine("something wrong will hapen");
                    notificationModal.UpdateModal(ModalType.Error, "No fue posible encontrar tareas documentales con ese filtro de busqueda", true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tareas documentales: {ex.Message}");
                notificationModal.UpdateModal(ModalType.Error, "Ocurrio un error inesperado", true);
            }
        }

        #endregion

        #region Check Read Document

        private async Task CheckReadDocument(VDocumentaryTaskDtoResponse doc)
        {
            try
            {
                ViewStateDtoRequest viewDoc = new()
                {
                    TaskManagementId = doc.TaskManagementId,
                    UpdateUserId = doc.UserTaskId.Value
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/ViewStateTasksManagement", viewDoc);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse.Succeeded)
                {
                    Console.WriteLine("checkbox marcado con exito");
                }
                else
                {
                    Console.WriteLine("no se pudo marcar el checkbox");
                }

            }catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el checkbox: {ex.Message}");
            }
        }

        #endregion

        #region DeleteDocumentTask

        private void ShowDeleteModal(VDocumentaryTaskDtoResponse doc)
        {
            docToDelete = doc;
            notificationModal.UpdateModal(ModalType.Warning, "¿Esta seguro de eliminar una tarea documental?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                DeleteGeneralDtoRequest Task = new DeleteGeneralDtoRequest()
                {
                    Id = docToDelete.TaskId,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteDocumentaryTask", Task);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

                if (deserializeResponse.Succeeded)
                {
                    notificationModalSucces.UpdateModal(ModalType.Success, "Se ha borrado el registro existosamente", true);
                    await GetDataCards(codeP, id1, new List<string>() { codePR, codeFR }, new List<bool>() { true, false, true });
                    await GetNumericDataCards();
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "No fue posible eliminar el registro", true);
            }

        }

        #endregion

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;
            }
            else
            {
                contadorcarac = 0;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
