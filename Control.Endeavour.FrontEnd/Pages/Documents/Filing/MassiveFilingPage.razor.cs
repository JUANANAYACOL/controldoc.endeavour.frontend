using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components.Editor;
using static Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response.SystemFieldsDtoResponse;
using Telerik.SvgIcons;
using static Telerik.Blazor.ThemeConstants;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response;
using DevExpress.DocumentView;
using Microsoft.VisualBasic;

namespace Control.Endeavour.FrontEnd.Pages.Documents.Filing
{
    public partial class MassiveFilingPage
    {
		#region Variables

		#region Inject 
		[Inject] private EventAggregatorService? EventAggregator { get; set; }

		[Inject] private HttpClient? HttpClient { get; set; }

        [Inject] private IJSRuntime Js { get; set; }
        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Models

        private BulkFilingsDtoRequest BulkFilingsDtoRequest = new ();
        private MetaModel Meta = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string AlertMessage = "";
        private string SystemParamCL = "";
        private string DefaulTextCL = "Seleccione una clase de comunciación...";
        private string panel_2 = "d-none"; //Paso 2
        private string panel_3 = "d-none"; //Paso 2

        #endregion

        #region Environments(Numeric)

        private int FileSize = 10;

        #endregion

        #region Environments(Bool)

        private bool readOnlyComu = false; 
        private bool DisableButtons { get; set; } = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> SystemFieldsCLList = new();
        private string[] AllowedExtensions { get; set; } = { ".xlsx", ".xls" };

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            AlertMessage = GetUploadMessage();
            await GetClassCom();

        }


		#endregion

		#region Methods

		#region HandleMethods

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}

        private async Task HandleFileFDP12(List<FileInfoData> data)
        {

            if (data != null && data.Count > 0)
            {
                BulkFilingsDtoRequest.ClassCode = SystemParamCL;
                BulkFilingsDtoRequest.FileExt = data[0].Extension.ToString().Replace(".", ""); //Es importante que no lleve punto
                BulkFilingsDtoRequest.Archive = data[0].Base64Data;
                BulkFilingsDtoRequest.UserId = 4072;
                panel_3 = "";
            }
            else
            {
                BulkFilingsDtoRequest = new BulkFilingsDtoRequest();
                panel_3 = "d-none";
            }
        }

        private void HandleDropdownListChange(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                SystemParamCL = value;
                DisableButtons = false;
                panel_2 = "";
            }
            else
            {
                SystemParamCL = DefaulTextCL;
                DisableButtons = true;
                panel_2 = "d-none";
            }
        }

        private async Task HandleFormMassiveFiling()
        {

            if(BulkFilingsDtoRequest.UserId > 0) {
                //PageLoadService.MostrarSpinnerReadLoad(Js);
                var responseApi = await HttpClient.PostAsJsonAsync("documents/Filing/BulkFilings", BulkFilingsDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BulkFilingsDtoResponse>>();

                if (deserializeResponse.Succeeded)
                {
                    string[] cuantos = deserializeResponse.Data.Message.Split(" ");
                    notificationModal.UpdateModal(ModalType.Success, "¡Se han radiado " + cuantos[0] +" documentos de forma exitosa!", true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error al radicar de forma masiva, por favor intententelo de nuevo!", true, "Aceptar", "", "", "");
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error al radicar de forma masiva, por favor verifique!", true, "Aceptar", "", "", "");
            }
        }

        #endregion

        #region GetMethods

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
                    SystemFieldsCLList = deserializeResponse.Data.ToList() ?? new();
                    Meta = deserializeResponse.Meta;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la clase de comunicacion: {ex.Message}");
            }
        }

        private async Task<FileDtoResponse?> GetFileBase64(int? id)
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
            catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener el archivo: {ex.Message}");
                return new FileDtoResponse();
            }

        }

        #endregion

        #region OthersMethods
        public string GetUploadMessage()
        {
            string extensions = string.Join(" ó ", AllowedExtensions).Replace(".", "").ToUpper();
            return $"Solo se permite subir archivo {extensions}. Máx {FileSize}Mb";
        }

        private async Task DownloadTemplate()
        {
            if (!string.IsNullOrEmpty(SystemParamCL))
            {
                Int32 fileId = 0;

                Dictionary<string, Int32> plantillasId = new() //Se debe validar que este dicionario venga de base y no este quemado - JR
                {
                     { "E", 6419 },
                     { "I", 6420 },
                     { "R", 6421 }
                };

                if (plantillasId.ContainsKey(SystemParamCL))
                {
                    fileId = plantillasId[SystemParamCL];
                }

                FileDtoResponse objFile = await GetFileBase64(fileId);
                string nombreArchivo = objFile.FileName + "." + objFile.FileExt;

                bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nombreArchivo, objFile.DataFile);
                if (download)
                {
                    readOnlyComu = true;
                    notificationModal.UpdateModal(ModalType.Success, "¡La plantilla: " + nombreArchivo + ", se ha descargado de forma exitosa!", true, "Aceptar", title: "¡Descarga exitosa!");
                }
                else
                {
                    readOnlyComu = false;
                    notificationModal.UpdateModal(ModalType.Error, "¡Se ha presentado un error a la hora de descargar la plantilla, por favor inténtelo de nuevo!", true, "Aceptar", "", "", "");
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Debe escoger una clase de comunicación para descargar una plantilla, por favor inténtelo de nuevo!", true, "Aceptar", "", "", "");
            }
        }

        #endregion

        #endregion


    }
}
