using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Document.DocumentaryTask
{
    public partial class CopiesModal
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components

        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Modals


        #endregion

        #region Parameters

        [Parameter] public EventCallback<bool> OnStatusChangedUser { get; set; }
        [Parameter] public EventCallback<int> ChangeModal { get; set; }
        [Parameter] public EventCallback<MyEventArgs<CopyDtoResponse>> OnStatusChanged { get; set; }

        #endregion

        #region Models

        private CopyDtoResponse destinationCopys = new();
         

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        public int ActiveTabIndex = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool isuser = false;
        private bool seenCopys = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<UserClasification> userList = new();
        private List<AdministrationUsers> thirdList = new();

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
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DestinationCopies"))
            {

                var eventArgs = new MyEventArgs<CopyDtoResponse>
                {
                    Data = destinationCopys,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(eventArgs);

            }
            else
            {
                var eventArgs = new MyEventArgs<CopyDtoResponse>();

                await OnStatusChanged.InvokeAsync(eventArgs);
            }

        }

        private async Task HandleCopys()
        {
            destinationCopys = new();

            if (userList.Count > 0) { destinationCopys.DestinationsUser = new(); destinationCopys.DestinationsUser.AddRange(userList.Where(x => x.TaskId == null).Select(x => x).ToList()); }
            if (thirdList.Count > 0) { destinationCopys.DestinationsAdministration = new(); destinationCopys.DestinationsAdministration.AddRange(thirdList.Where(x => x.TaskId == null).Select(x => x).ToList()); }

            notificationModal.UpdateModal(ModalType.Warning, "Confirmar acción \n " +

                " \n ¿Desea Continuar?", true, modalOrigin:"DestinationCopies");
        }

        #endregion

        #region OthersMethods

        #region UpdateCopies - TaskManagement

        public async Task UpdateCopys(int id, bool value, CopyDtoResponse copies)
        {
            seenCopys = !value;

            HttpClient?.DefaultRequestHeaders.Remove("TaskId");
            HttpClient?.DefaultRequestHeaders.Add("TaskId", id.ToString());
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<CopyDtoResponse>>("documentarytasks/DocumentaryTask/GetCopies");
            HttpClient?.DefaultRequestHeaders.Remove("TaskId");

            if (deserializeResponse.Data != null)
            {
                if (deserializeResponse.Data.DestinationsUser != null) { userList = deserializeResponse.Data.DestinationsUser; }
                if (deserializeResponse.Data.DestinationsAdministration != null) { thirdList = deserializeResponse.Data.DestinationsAdministration; }

            }

            if (copies != null)
            {
                if (copies.DestinationsUser != null && copies.DestinationsUser.Count > 0)
                {
                    userList.AddRange(copies.DestinationsUser);
                }
                if (copies.DestinationsAdministration != null && copies.DestinationsAdministration.Count > 0)
                {
                    thirdList.AddRange(copies.DestinationsAdministration);
                }
            }
        }

        #endregion

        #region DataContainerMethods
        public void GetReceiverUsersData(List<VUserDtoResponse> receiver)
        {
            userList.AddRange(receiver.Select(x => new UserClasification
            {
                UserId = x.UserId,
                type = "TDF,U",
                FullName = x.FullName,
                AdministrativeUnitName = x.AdministrativeUnitName,
                ProductionOfficeName = x.ProductionOfficeName,
                Charge = x.Charge
            }).ToList());

        }

        public void GetReceiverThirdData(List<ThirdPartyDtoResponse> receiver1, List<ThirdUserDtoResponse> receiver2)
        {
            thirdList.AddRange(receiver1.Select(x => new AdministrationUsers
            {
                ThirdPartyId = x.ThirdPartyId,
                type = "TDF,T",
                CompanyName = x.Names,
                IdentificationNumber = x.IdentificationNumber,
                IdentificationTypeName = x.IdentificationTypeName,
                Email = (string.IsNullOrEmpty(x.Email1) ? x.Email2 : x.Email1)
            }).ToList());

            thirdList.AddRange(receiver2.Select(x => new AdministrationUsers
            {
                ThirdUserId = x.ThirdUserId,
                type = "TDF,TU",
                CompanyName = x.CompanyName,
                IdentificationNumber = x.IdentificationNumber,
                IdentificationTypeName = x.IdentificationTypeName,
                Email = x.Email
            }).ToList());


        }

        #endregion

        #region ManageTabComponent
        private async Task OpenNewModalUser()
        {
            if (ActiveTabIndex == 0)
            {
                isuser = true;
            }
            else { isuser = false; }
            await ChangeModal.InvokeAsync(isuser ? 1 : 2);

            await OnStatusChangedUser.InvokeAsync(true);
            StateHasChanged();
        }
        #endregion

        #region DeleteUser/UserAdministration

        private async Task DeleteThirdParty(AdministrationUsers user)
        {
            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteCopys(user.TaskId.Value, (int)(user.ThirdPartyId ?? user.ThirdUserId));

                if (deleteValidation)
                {
                    thirdList.Remove(user);
                }
            }
            else
            {
                thirdList.Remove(user);
            }
        }


        private async Task DeleteUser(UserClasification user)
        {
            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteCopys(user.TaskId.Value, user.UserId);

                if (deleteValidation)
                {
                    userList.Remove(user);
                }
            }
            else
            {
                userList.Remove(user);
            }
        }

        private async Task<bool> DeleteCopys(int taskId, int id)
        {

            UserDeleteDtoRequest Copies = new()
            {
                TaskId = taskId,
                UserId = id
            };

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteCopies", Copies);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

            if (deserializeResponse.Succeeded)
            {
                return true;
            }

            return false;
        }

        #endregion

        #endregion

        #endregion

    }
}
