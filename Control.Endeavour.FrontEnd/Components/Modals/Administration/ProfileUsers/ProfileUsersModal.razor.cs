using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ProfileUsers
{
    public partial class ProfileUsersModal : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent profileId { get; set; } = new();
        private InputModalComponent profile1 { get; set; } = new();
        private InputModalComponent profileCode { get; set; } = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public bool ModalStatus { get; set; } = false;
        [Parameter] public string Id { get; set; } = "";
        [Parameter] public bool CrearEditar { get; set; }
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        #endregion Parameters

        #region Models

        //Request and Response
        private ProfileDtoResponse _selectedRecord = new();

        private ProfileCreateDtoRequest profileDtoRequest = new();
        private ProfileUpdateDtoRequest profileUsersDtoRequestUpdate = new ProfileUpdateDtoRequest();

        #endregion Models

        #region Environments

        //Inputs
        private bool stateValue = true;

        private string description { get; set; } = "";

        //Create or Edit
        private bool IsEditForm = false;
        private bool modalStatus { get; set; } = false;
        private bool IsDisabledCode = false;
        private string IdProfile = "";

        #endregion Environments

        #endregion Variables

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OtherMethods

        #region CreateProfileUser

        private async Task Create()
        {
            try
            {
                profileDtoRequest.Profile1 = profile1.InputValue;
                profileDtoRequest.CompanyId = 16;
                profileDtoRequest.ProfileCode = profileCode.InputValue;
                profileDtoRequest.Description = description ?? "";
                profileDtoRequest.ActiveState = stateValue;
                profileDtoRequest.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("permission/Profile/CreateProfile", profileDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProfileDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
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

        #endregion CreateProfileUser

        #region UpdateProfileUser

        private async Task Update()
        {
            try
            {
                profileUsersDtoRequestUpdate.ProfileId = int.Parse(profileId.InputValue);
                profileUsersDtoRequestUpdate.Profile1 = profile1.InputValue;
                profileUsersDtoRequestUpdate.Description = description ?? "";
                profileUsersDtoRequestUpdate.ActiveState = stateValue;
                profileUsersDtoRequestUpdate.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("permission/Profile/UpdateProfile", profileUsersDtoRequestUpdate);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProfileDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                    await OnChangeData.InvokeAsync(true);
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

        #endregion UpdateProfileUser

        #region ResetFormAsync

        public void ResetFormAsync()
        {
            profileDtoRequest = new ProfileCreateDtoRequest();
            description = "";
            IdProfile = "";
            IsDisabledCode = false;
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            try
            {
                if (IsEditForm)
                {
                    await Update();
                    IsEditForm = false;
                }
                else
                {
                    await Create();
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, "Aceptar");
            }

            ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleValidSubmit

        #region ReceiveProfileUser

        public void ReceiveProfileUser(ProfileDtoResponse response)
        {
            _selectedRecord = response;

            IdProfile = _selectedRecord.ProfileId.ToString();

            profileDtoRequest.Profile1 = _selectedRecord.Profile1;
            profileDtoRequest.Description = _selectedRecord.Description;
            profileDtoRequest.ActiveState = _selectedRecord.ActiveState;
            profileDtoRequest.ProfileCode = _selectedRecord.ProfileCode;

            description = _selectedRecord.Description!.ToString();
            stateValue = _selectedRecord.ActiveState;
            IsDisabledCode = true;
            IsEditForm = true;
        }

        #endregion ReceiveProfileUser

        #region Modal

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
            ModalStatus = status;
            ResetFormAsync();
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
        }

        #endregion HandleModalNotiClose

        #endregion Modal

        #endregion OtherMethods

        #endregion Methods
    }
}