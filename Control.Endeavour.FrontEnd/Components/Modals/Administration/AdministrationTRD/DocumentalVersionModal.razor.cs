using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.State;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeAct.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeAct.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class DocumentalVersionModal : ComponentBase
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private InputModalComponent code = new();
        private InputModalComponent name = new();
        private string versionType { get; set; } = "";

        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal { get; set; } = new();
        #endregion

        #region Parameters

        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChangedUpdate { get; set; }

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }
        #endregion

        #region Models
        //Request and Response
        private DocumentalVersionDtoResponse docVersionResponse = new();
        private DocumentalVersionDtoResponse _selectedRecord = new();
        private DocumentalVersionDtoRequest docVersionDtoRequest = new();
        private DocumentalVersionUpdateDtoRequest docVersionDtoRequestUpdate = new DocumentalVersionUpdateDtoRequest();
        #endregion

        #region Environments

        #region Environments(String)
        private string description { get; set; } = "";
        #endregion

        #region Environments(Numeric)
        private int FileSize = 10;
        private decimal CharacterCounter { get; set; } = 0;
        #endregion

        #region Environments(DateTime)
        private DateTime? from { get; set; }
        private DateTime? to { get; set; }
        private DateTime? dateActs { get; set; }

        private DateTime minValueTo { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region Environments(Bool)
        //Create or Edit
        private bool IsEditForm { get; set; } = false;
        private bool IsDisabledCode { get; set; } = false;
        private bool modalStatus { get; set; } = false;
        private bool stateValue = true;
        private bool activeState = true;
        #endregion

        #region Environments(List & Dictionary)
        private string[] AllowedExtensions { get; set; } = { ".pdf" };
        private List<FileInfoData> organizationFiles { get; set; } = new();
        private List<FileInfoData> adminActs { get; set; } = new();
        private string[] documentalType = { "TVD", "TRD" };
        private string textType = "Seleccione un tipo...";
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

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

        #region ReceiveDocumentalVersion

        public void ReceiveDocumentalVersion(DocumentalVersionDtoResponse response)
        {
            _selectedRecord = response;

            docVersionDtoRequestUpdate.DocumentalVersionId = _selectedRecord.DocumentalVersionId;
            docVersionDtoRequest.CompanyId = _selectedRecord.CompanyId;
            docVersionDtoRequest.VersionType = _selectedRecord.VersionType;
            docVersionDtoRequest.Name = _selectedRecord.Name;
            docVersionDtoRequest.Description=_selectedRecord.Description;
            docVersionDtoRequest.Code = _selectedRecord.Code;
            versionType= _selectedRecord.VersionType;
            from = _selectedRecord.StartDate;
            to = _selectedRecord.EndDate;

            description = _selectedRecord.Description!.ToString();
            CharacterCounter = description.Length;
            stateValue = (bool)_selectedRecord.ActiveState!;

            updateMaxValue();
            updateMinValue();
            IsDisabledCode = true;
            IsEditForm = true;
        }

        #endregion ReceiveDocumentalVersion

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            ResetFormAsync();
            modalStatus = status;
            IsDisabledCode = false;
            IsEditForm = false;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region ResetFormAsync

        public void ResetFormAsync()
        {
            organizationFiles = new();
            adminActs=new();
            versionType = "";
            docVersionDtoRequest = new DocumentalVersionDtoRequest();
            description = "";
            IsDisabledCode = false;
            minValueTo = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            maxValueTo = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            from = null;
            to = null;
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region updateMinValue

        public void updateMinValue()
        {
            if (from != null)
            {
                minValueTo = (DateTime)from;
            }
            StateHasChanged();
        }

        #endregion updateMinValue

        #region updateMaxValue

        public void updateMaxValue()
        {
            if (to != null)
            {
                maxValueTo = (DateTime)to;
            }
            StateHasChanged();
        }

        #endregion updateMaxValue

        #region GetActName

        private async Task<String?> GetActName(int? id)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    return deserializeResponse.Data!.FileName;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return "";
            }
        }

        #endregion GetActName

        #region HandleActsToTableCreate

        private void HandleActsToTableCreate(List<FileInfoData> data)
        {
            var file = data.LastOrDefault();
            if (file != null)
            {
                AdministrativeActDVDtoRequest act = new()
                {
                    FileName = file.Name!,
                    FileExt = file.Extension!.Replace(".", ""),
                    DataFile = file.Base64Data!
                };
                docVersionDtoRequest.AdministrativeActs.Add(act);
            }
            
        }

        #endregion HandleActsToTableCreate

        #region FormMethods

        #region SelectedMethod

        private async Task HandleValidSubmit()
        {
           
            if (IsEditForm)
            {
                await Update();
            }
            else
            {
                await Create();
            }

            StateHasChanged();
        }

        #endregion SelectedMethod

        #region CreateDocumentalVersion

        private async Task Create()
        {
            try
            {
                docVersionDtoRequest.CompanyId = 17;
                docVersionDtoRequest.VersionType = versionType;
                docVersionDtoRequest.Code = code.InputValue;
                docVersionDtoRequest.Name=name.InputValue;
                docVersionDtoRequest.Description = description;
                docVersionDtoRequest.StartDate = from;
                docVersionDtoRequest.EndDate = to;

                //Organigrama
                docVersionDtoRequest.FileName = organizationFiles[0].Name!;
                docVersionDtoRequest.FileExt = organizationFiles[0].Extension!.Replace(".", "");
                docVersionDtoRequest.DataFile = organizationFiles[0].Base64Data!;

                docVersionDtoRequest.ActiveState = activeState;
                docVersionDtoRequest.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/CreateDocumentalVersion", docVersionDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentalVersionDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                    ResetFormAsync();
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

        #endregion CreateDocumentalVersion

        #region UpdateDocumentalVersion

        private async Task Update()
        {
            try
            {
                docVersionDtoRequestUpdate.CompanyId = docVersionDtoRequest.CompanyId;
                docVersionDtoRequestUpdate.VersionType = versionType;
                docVersionDtoRequestUpdate.Name = name.InputValue;
                docVersionDtoRequestUpdate.Description = description;
                docVersionDtoRequestUpdate.StartDate = from;
                docVersionDtoRequestUpdate.EndDate = to;
                docVersionDtoRequestUpdate.ActiveState = activeState;
                docVersionDtoRequestUpdate.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/UpdateDocumentalVersion", docVersionDtoRequestUpdate);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentalVersionDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
                    await Task.Delay(1000);
                    IsEditForm = false;
                    ResetFormAsync();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
                    
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
            }
        }

        #endregion UpdateDocumentalVersion

        #endregion FormMethods

        #region RemoveAct
        private void RemoveAdministrativeAct(AdministrativeActDVDtoRequest act)
        {
            docVersionDtoRequest.AdministrativeActs.Remove(act);
        }
        #endregion RemoveAct

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

        #endregion OthersMethods

        #endregion

    }
}
