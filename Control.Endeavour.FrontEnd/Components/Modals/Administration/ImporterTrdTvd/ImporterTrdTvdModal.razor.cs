using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.State;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.ClipboardSource.SpreadsheetML;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Xml.Linq;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ImporterTrdTvd
{
    public partial class ImporterTrdTvdModal:ComponentBase
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
        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Parameters

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<ImporterDtoResponse> OnInfoChange { get; set; }

        #endregion

        #region Models
        private ImporterDtoRequest importDtoRequest = new();
        #endregion

        #region Environments

        #region Environments(String)
        private string description { get; set; } = "";
        private string textDocumentalVersion { get; set; } = "Seleccione una version documental...";

        #endregion

        #region Environments(Numeric)

        private int FileSize = 10;
        private decimal CharacterCounter { get; set; } = 0;
        private int idDocumentalVersion { get; set; }
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus { get; set; } = false;
        #endregion

        #region Environments(List & Dictionary)
        private List<FileInfoData> template { get; set; } = new();
        private List<DocumentalVersionDtoResponse> documentalVersionList = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersions();

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

        #region GetDocumentalVersions

        private async Task GetDocumentalVersions()
        {
            try
            {

                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    documentalVersionList = deserializeResponse.Data ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las versiones documentales: {ex.Message}");
            }
        }

        #endregion GetDocumentalVersions

        #region Modal

        #region UpdateModalStatus
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion UpdateModalStatus

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        #region HandleModalClosed
        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            ResetFormAsync();
            StateHasChanged();
        }
        #endregion HandleModalClosed

        #endregion Modal

        #region ResetFormAsync

        public void ResetFormAsync()
        {
            template = new();
            description = "";
            idDocumentalVersion = 0;
        }

        #endregion ResetFormAsync

        #region CountCharacters

        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                CharacterCounter = value.Length;
            }
            else
            {
                CharacterCounter = 0;
            }
        }

        #endregion CountCharacters

        #region CreateImport

        private async Task CreateImport()
        {
            try
            {
                importDtoRequest.DocumentalVersionId = idDocumentalVersion;
                importDtoRequest.DescriptionHistory = description;
                importDtoRequest.FileName = template[0].Name!;
                importDtoRequest.FileExt = template[0].Extension!.Replace(".", "");
                importDtoRequest.DataFile = template[0].Base64Data!;
                importDtoRequest.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Importer/CreateImport", importDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ImporterDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    await OnInfoChange.InvokeAsync(deserializeResponse!.Data);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el registro, por favor intente de nuevo!", true, "Aceptar");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el registro, por favor intente de nuevo!", true, "Aceptar");
            }
        }

        #endregion CreateImport


        #endregion OthersMethods

        #endregion

    }
}
