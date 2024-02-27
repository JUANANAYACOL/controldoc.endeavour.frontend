
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor.Primitives.Internal;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.SvgIcons;


namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments
{
    public partial class AttachmentTrayModal
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
        public EventCallback<bool> OnStatusChangedAtt { get; set; }

        [Parameter]
        public EventCallback<int> ChangeModal { get; set; }

        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        public int ActiveTabIndex { get; set; } = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool SeenAttachments = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<AttachmentsDtoResponse> AttachmentList = new();
        private List<AttachmentsDtoResponse> AttachmentDeleteList = new();

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

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion

        #region OthersMethods

        #region GetAttachmentData

        public void GetAttachmentData(List<AttachmentsDtoRequest> attachment)
        {
            AttachmentList.AddRange(attachment.Select(x => new AttachmentsDtoResponse { ArchiveName = x.ArchiveName, ExibitCodeName = x.ArchiveExt, AttDescription = x.AttDescription }).ToList());
        }

        #endregion

        #region UpdateAttachments

        public async Task UpdateAttachment(int id, bool value, List<AttachmentsDtoRequest> attachments)
        {
            SeenAttachments = !value;
            AttachmentList = new();
            AttachmentDeleteList = new();

            HttpClient?.DefaultRequestHeaders.Remove("TaskId");
            HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{id}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AttachmentsDtoResponse>>>("documentarytasks/DocumentaryTask/GetAttachments");
            HttpClient?.DefaultRequestHeaders.Remove("TaskId");



            if (deserializeResponse.Data != null)
            {
                foreach (var attachment in deserializeResponse.Data)
                {
                    if (attachment.Active)
                    {
                        AttachmentList.Add(attachment);
                    }
                    else { AttachmentDeleteList.Add(attachment); }
                }
            }
            else { Console.WriteLine("no se encontraron adjuntos"); }

            if (attachments.Count > 0)
            {
                AttachmentList.AddRange(attachments.Select(x => new AttachmentsDtoResponse { ArchiveName = x.ArchiveName, ExibitCodeName = x.ArchiveExt, AttDescription = x.AttDescription }).ToList());
            }
        }


        public async Task UpdateAttachmentDocument(int id, bool value)
        {
            SeenAttachments = !value;
            AttachmentList = new();
            AttachmentDeleteList = new();

            var data = new
            {
                controlId = id,
                attGenerated = true,                
            };
            var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/ByAttFilter", data);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AttachmentsDtoResponse>>>();
            if (deserializeResponse?.Data != null)
            {
                foreach (var attachment in deserializeResponse.Data)
                {
                    if (attachment.Active)
                    {
                        AttachmentList.Add(attachment);
                    }
                    else { AttachmentDeleteList.Add(attachment); }
                }
            }
            else { Console.WriteLine("no se encontraron adjuntos"); }
        }
        #endregion

        #region Call Modal

        private async Task OpenNewModalAttachment()
        {
            await OnStatusChangedAtt.InvokeAsync(true);
            StateHasChanged();
        }

        #endregion

        #region DeleteAttachments

        private async Task<bool> DeleteAttachment(int taskId, int id)
        {

            AttachmentsDeleteDtoRequest attachment = new AttachmentsDeleteDtoRequest()
            {
                TaskId = taskId,
                AttchmentId = id
            };

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteAttchments", attachment);
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

        #region RemoveAttachments

        private async Task DeleteAttachments(AttachmentsDtoResponse attachment)
        {

            if (attachment.TaskId != null)
            {
                var deleteValidation = await DeleteAttachment(attachment.TaskId.Value, attachment.AttachmentId);

                if (deleteValidation)
                {
                    AttachmentDeleteList.Add(attachment);
                    AttachmentList.Remove(attachment);
                }
            }
            else
            {
                AttachmentDeleteList.Add(attachment);
                AttachmentList.Remove(attachment);
            }
        }

        #endregion

        #endregion

        #endregion

    }
}
