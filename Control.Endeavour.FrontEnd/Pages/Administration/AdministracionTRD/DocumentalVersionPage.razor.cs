using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdParty;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class DocumentalVersionPage
    {

		#region Variables

		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }

        [Inject] 
        private IJSRuntime Js { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals
        private DocumentalVersionModal modalDocumentalVersion = new();
        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Parameters


        #endregion

        #region Models
        private MetaModel? meta { get; set; } = new() { PageSize = 10 };
        private DocumentalVersionFilterDtoRequest? filterDtoRequest { get; set; } = new();
        private PaginationComponent<DocumentalVersionDtoResponse, DocumentalVersionFilterDtoRequest> paginationComponent = new();
        private DocumentalVersionDtoResponse versionToDelete = new();
        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        public List<DocumentalVersionDtoResponse> documentalVersionsList { get; set; } = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersion();

        }

		#endregion

		#region Methods

		#region HandleMethods

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region Modal

        private void ShowModal()
        {
            modalDocumentalVersion.UpdateModalStatus(true);
        }

        private void ShowModalEdit(DocumentalVersionDtoResponse record)
        {
            modalDocumentalVersion.UpdateModalStatus(true);
            modalDocumentalVersion.ReceiveDocumentalVersion(record);
        }

        #endregion Modal

        #region DeleteProfile

        private void HandleRecordToDelete(DocumentalVersionDtoResponse record)
        {
            versionToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el mensaje?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new() { Id = versionToDelete.DocumentalVersionId, User = "user" };

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/DeleteDocumentalVersion", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        await HandleRefreshGridData(true);
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, "Aceptar");
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }

        #endregion DeleteProfile

        #region GetDocumentalVersion

        private async Task GetDocumentalVersion()
        {
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/ByFilter", filterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    documentalVersionsList = deserializeResponse.Data ?? new();
                    meta = deserializeResponse!.Meta;
                    paginationComponent.ResetPagination(meta!);
                }
                else
                {
                    documentalVersionsList = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las versiones documentales, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las versiones documentales: {ex.Message}");
            }
        }

        #endregion GetDocumentalVersion

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetDocumentalVersion();
        }

        #endregion RefreshGrid

        #region HandlePagination

        private void HandlePaginationGrid(List<DocumentalVersionDtoResponse> newDataList)
        {
            documentalVersionsList = newDataList;
        }

        #endregion HandlePagination

        #region DownloadFile
        private async Task DownloadFile(int? fileId)
        {
            if (fileId != 0 && fileId != null)
            {
                
                FileDtoResponse objFile = await GetFile(fileId);
                string nombreArchivo = objFile.FileName + "." + objFile.FileExt;

                bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nombreArchivo, objFile.DataFile);
                if (download)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡El organigrama se ha descargado de forma exitosa!", true, "Aceptar", title: "¡Descarga exitosa!");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se ha presentado un error a la hora de descargar el organigrama, por favor inténtelo de nuevo!", true, "Aceptar", "", "", "");
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Debe escoger una clase de comunicación para descargar el organigrama, por favor inténtelo de nuevo!", true, "Aceptar", "", "", "");
            }
        }
        #endregion DownloadFile

        #region GetFile
        private async Task<FileDtoResponse?> GetFile(int? id)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    return deserializeResponse.Data!;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion GetFile

        #endregion

        #endregion

    }
}
