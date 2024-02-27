using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Replacement
{
    public partial class ReplacementModal : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private ButtonGroupComponent inputUserFullname { get; set; } = new();
        private ButtonGroupComponent inputReplacementFullname { get; set; } = new();

        private InputModalComponent inputReason { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter] public string idModalIdentifier { get; set; } = null!;
        [Parameter] public bool modalStatus { get; set; }
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        [Parameter] public EventCallback<bool> OnStatusChangedUpdate { get; set; }

        #endregion Parameters

        #region Models

        private ReplacementCreateDtoRequest replacemenetCreateDtoRequest { get; set; } = new();
        private ReplacementUpdateDtoRequest replacemenetUpdateDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments

        private decimal CharacterCounter { get; set; } = 0;
        public bool userSearchModalStatus { get; set; } = new();
        public string userFullname { get; set; } = "";

        public string replacementFullname { get; set; } = "";

        private DateTime? from { get; set; }
        private DateTime? to { get; set; }

        private DateTime minValueTo { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private bool userToFill { get; set; } = false;
        private bool isEditForm { get; set; } = false;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            isEditForm = false;
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

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

        #region updateUserSelection

        public void updateUserSelection(VUserDtoResponse userSelected)
        {
            if (!userToFill)
            {
                userFullname = userSelected.FullName;
                replacemenetCreateDtoRequest.UserId = userSelected.UserId;
            }
            else
            {
                replacementFullname = userSelected.FullName;
                replacemenetCreateDtoRequest.UserReplacementId = userSelected.UserId;
            }

            StateHasChanged();
        }

        #endregion updateUserSelection

        #region OpenNewModal

        private async Task OpenNewModal(bool typeOfSelection)
        {
            userToFill = typeOfSelection;
            await OnStatusChanged.InvokeAsync(true);
        }

        #endregion OpenNewModal

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            from = null;
            to = null;
            userFullname = "";
            replacementFullname = "";
            replacemenetCreateDtoRequest = new();
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            isEditForm = false;
            modalStatus = newValue;

            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            ResetFormAsync();
            modalStatus = status;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            if (isEditForm)
            {
                await HandleFormUpdate();
                isEditForm = false;
            }
            else
            {
                await HandleFormCreate();
            }
            ResetForm();
        }

        #endregion HandleValidSubmit

        #region ResetForm

        public void ResetForm()
        {
            replacemenetCreateDtoRequest = new();
        }

        #endregion ResetForm

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            try
            {
                if (inputReplacementFullname.IsInputValid && inputUserFullname.IsInputValid)
                {
                    replacemenetCreateDtoRequest.StartDate = (DateTime)from;
                    replacemenetCreateDtoRequest.EndDate = (DateTime)to;

                    var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/CreateReplacement", replacemenetCreateDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VReplacementDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, "El reemplazo a sido creado con exito", true, "Aceptar");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "No se puedo crear el reemplazo", true, "Aceptar");
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "No se pudo crear el reemplazo por uno o mas errores de validación", true, "Aceptar");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "Error a la hora de crear información", true, "Aceptar");
            }
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            try
            {
                if (inputReplacementFullname.IsInputValid && inputUserFullname.IsInputValid)
                {
                    replacemenetUpdateDtoRequest.StartDate = (DateTime)from;
                    replacemenetUpdateDtoRequest.EndDate = (DateTime)to;

                    replacemenetUpdateDtoRequest.Reason = replacemenetCreateDtoRequest.Reason;
                    replacemenetUpdateDtoRequest.UserId = replacemenetCreateDtoRequest.UserId;
                    replacemenetUpdateDtoRequest.UserReplacementId = replacemenetCreateDtoRequest.UserReplacementId;

                    var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/UpdateReplacement", replacemenetUpdateDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VReplacementDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, "El reemplazo a sido actualizado con exito", true, "Aceptar");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "No se puedo actualizar el reemplazo", true, "Aceptar");
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "No se pudo actualizar el reemplazo por uno o mas errores de validacion", true, "Aceptar");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "Error a la hora de actualizar información", true, "Aceptar");
            }
        }

        #endregion HandleFormUpdate

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnStatusChangedUpdate.InvokeAsync(false);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region UpdateSelectedRemplacement

        public void UpdateSelectedRemplacement(VReplacementDtoResponse replacement)
        {
            isEditForm = true;
            replacemenetCreateDtoRequest.UserReplacementId = replacement.UserReplacementId;

            replacemenetCreateDtoRequest.UserId = replacement.UserId;
            to = replacement.EndDate;
            from = replacement.StartDate;
            replacemenetCreateDtoRequest.Reason = replacement.Reason;
            userFullname = replacement.UserFullName;
            replacementFullname = replacement.ReplacementFullName;
            replacemenetUpdateDtoRequest.ReplacementId = replacement.ReplacementId;

            updateMaxValue();
            updateMinValue();
        }

        #endregion UpdateSelectedRemplacement

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

        #endregion Methods
    }
}