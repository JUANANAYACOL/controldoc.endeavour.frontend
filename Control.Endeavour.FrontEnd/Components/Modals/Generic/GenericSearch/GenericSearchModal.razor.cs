﻿using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Radication;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch
{
    public partial class GenericSearchModal
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Modals

        private UserSearchModal _userSearchComponet = new();
        private UserSearchModal _userSearchComponet2 = new();
        private ThirdPartySearchModal _thirdUserSearchComponet = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public bool hasEmailUsers { get; set; } = false;
        [Parameter] public bool hasEmailUsers2 { get; set; } = false;
        [Parameter] public bool hasEmailThirdUsers { get; set; } = false;

        [Parameter] public string? Title { get; set; }
        [Parameter] public bool showParamTrdDdl { get; set; } = true;

        [Parameter] public bool showCopiesColumn { get; set; } = true;
        [Parameter] public bool showCarge { get; set; } = true;
        [Parameter] public bool multipleSelection { get; set; } = true;
        [Parameter] public bool showNameField { get; set; } = true;
        [Parameter] public bool showLastNameField { get; set; } = true;
        [Parameter] public EventCallback<MyEventArgs<VUserDtoResponse>> OnStatusUserChanged { get; set; }
        [Parameter] public EventCallback<MyEventArgs<ThirdPartyDtoResponse>> OnStatusThirdPartyChanged { get; set; }
        [Parameter] public EventCallback<MyEventArgs<List<object>>> OnStatusMultipleUsersChanged { get; set; }

        [Parameter] public EventCallback<MyEventArgs<List<object>>> OnStatusChangedMultipleSelection { get; set; }

        [Parameter] public EventCallback<MyEventArgs<List<PersonInRadication>>> OnStatusChangeRadication { get; set; }

        [Parameter] public int ConfigurationInUse { get; set; } = 1;

        private bool modalStatus { get; set; } = false;

        #endregion Parameters

        #region Models

        private List<PersonInRadication> radicationRecievedToReturn { get; set; } = new();

        #endregion Models

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
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

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private void HandleModalClosed(bool value)
        {
            UpdateModalStatus(value);
        }

        #endregion HandleModalClosed

        #region UserModalOneSelection

        private async Task UserModalOneSelection(MyEventArgs<VUserDtoResponse> user)
        {
            await OnStatusUserChanged.InvokeAsync(user);
        }

        #endregion UserModalOneSelection

        #region UserModalMultipleSelection

        private async Task UserModalMultipleSelection(MyEventArgs<List<object>> users)
        {
            await OnStatusMultipleUsersChanged.InvokeAsync(users);
        }

        #endregion UserModalMultipleSelection

        #region OnClickAssignUserData

        private async Task OnClickAssignUserData()
        {
            await _userSearchComponet.HandleModalClosed(false);
        }

        #endregion OnClickAssignUserData

        #region ThirdPartyModalOneSelection

        private async Task ThirdPartyModalOneSelection(MyEventArgs<ThirdPartyDtoResponse> ThirdParty)
        {
            await OnStatusThirdPartyChanged.InvokeAsync(ThirdParty);
        }

        #endregion ThirdPartyModalOneSelection

        #region ThirdPartyMultipleSelection

        private async Task ThirdPartyMultipleSelection(MyEventArgs<List<object>> ThirdParties)
        {
            await OnStatusChangedMultipleSelection.InvokeAsync(ThirdParties);
        }

        #endregion ThirdPartyMultipleSelection

        #region OnClickAssignThirdPartyData

        private async Task OnClickAssignThirdPartyData()
        {
            await _thirdUserSearchComponet.HandleModalClosed(false);
        }

        #endregion OnClickAssignThirdPartyData

        #region SenderList

        private void SenderList(MyEventArgs<List<object>> listToManipulate)
        {
            if (listToManipulate?.Data?.Count == 2)
            {
                List<VUserDtoResponse> userSelected = (List<VUserDtoResponse>)( listToManipulate.Data?[0] ?? new() );
                List<VUserDtoResponse> userCopies = (List<VUserDtoResponse>)( listToManipulate.Data?[1] ?? new() );

                if (userSelected.Count != 0)
                {
                    transformUserToModel(userSelected, "Sender");
                }

                if (userCopies.Count != 0)
                {
                    transformUserToModel(userCopies, "Copy");
                }
            }
            else
            {
                List<ThirdPartyDtoResponse> ThirdPartySelected = (List<ThirdPartyDtoResponse>)( listToManipulate?.Data?[0] ?? new() );
                List<ThirdPartyDtoResponse> ThirdPartyCopies = (List<ThirdPartyDtoResponse>)( listToManipulate?.Data?[1] ?? new() );

                List<ThirdUserDtoResponse> ThirdUserSelected = (List<ThirdUserDtoResponse>)( listToManipulate?.Data?[2] ?? new() );
                List<ThirdUserDtoResponse> ThirdUserCopies = (List<ThirdUserDtoResponse>)( listToManipulate?.Data?[3] ?? new() );

                if (ThirdPartySelected.Count != 0)
                {
                    transformThirdPartyToModel(ThirdPartySelected, "Sender");
                }

                if (ThirdPartyCopies.Count != 0)
                {
                    transformThirdPartyToModel(ThirdPartyCopies, "Copy");
                }
                if (ThirdUserSelected.Count != 0)
                {
                    transformThirdUserToModel(ThirdUserSelected, "Sender");
                }

                if (ThirdUserCopies.Count != 0)
                {
                    transformThirdUserToModel(ThirdUserCopies, "Copy");
                }
            }
        }

        #endregion SenderList

        #region RecipientList

        private void RecipientList(MyEventArgs<List<object>> listToManipulate)
        {
            if (listToManipulate?.Data?.Count == 2)
            {
                List<VUserDtoResponse> userSelected = (List<VUserDtoResponse>)( listToManipulate.Data?[0] ?? new() );
                List<VUserDtoResponse> userCopies = (List<VUserDtoResponse>)( listToManipulate.Data?[1] ?? new() );

                if (userSelected.Count != 0)
                {
                    transformUserToModel(userSelected, "Recipient");
                }

                if (userCopies.Count != 0)
                {
                    transformUserToModel(userCopies, "Copy");
                }
            }
            else
            {
                List<ThirdPartyDtoResponse> ThirdPartySelected = (List<ThirdPartyDtoResponse>)( listToManipulate?.Data?[0] ?? new() );
                List<ThirdPartyDtoResponse> ThirdPartyCopies = (List<ThirdPartyDtoResponse>)( listToManipulate?.Data?[1] ?? new() );

                List<ThirdUserDtoResponse> ThirdUserSelected = (List<ThirdUserDtoResponse>)( listToManipulate?.Data?[2] ?? new() );
                List<ThirdUserDtoResponse> ThirdUserCopies = (List<ThirdUserDtoResponse>)( listToManipulate?.Data?[3] ?? new() );

                if (ThirdPartySelected.Count != 0)
                {
                    transformThirdPartyToModel(ThirdPartySelected, "Recipient");
                }

                if (ThirdPartyCopies.Count != 0)
                {
                    transformThirdPartyToModel(ThirdPartyCopies, "Copy");
                }
                if (ThirdUserSelected.Count != 0)
                {
                    transformThirdUserToModel(ThirdUserSelected, "Recipient");
                }

                if (ThirdUserCopies.Count != 0)
                {
                    transformThirdUserToModel(ThirdUserCopies, "Copy");
                }
            }
        }

        #endregion RecipientList

        #region transformUserToModel

        private void transformUserToModel(List<VUserDtoResponse> listToTransform, string typeOfPerson)
        {
            foreach (var item in listToTransform)
            {
                PersonInRadication personInRadication = new()
                {
                    TypeOfPersonInRadication = typeOfPerson ?? "",
                    Id = item.UserId,
                    CompanyId = item.CompanyId,

                    AddressId = default,

                    FullName = item.FullName ?? "",
                    IdentificationType = item.IdentificationTypeCode ?? "",
                    IdentificationTypeName = item.IdentificationType ?? "",

                    IdentificationNumber = item.Identification ?? "",

                    AdministrativeUnitId = ( item.AdministrativeUnitId ),

                    AdministrativeUnitCode = item.AdministrativeUnitCode ?? "",
                    AdministrativeUnitName = item.AdministrativeUnitName ?? "",

                    ProductionOfficeId = ( item.ProductionOfficeId ?? 0 ),

                    ProductionOfficeCode = item.ProductionOfficeCode ?? "",

                    ProductionOfficeName = item.ProductionOfficeName ?? "",

                    Email1 = item.Email ?? "",

                    Email2 = "",
                    ChargeCode = item.ChargeCode ?? "",

                    Charge = item.Charge ?? "",

                    Phone1 = item.PhoneNumber ?? "",

                    Phone2 = item.CellPhoneNumber ?? "",
                };

                radicationRecievedToReturn.Add(personInRadication);
            }
        }

        #endregion transformUserToModel

        #region transformThirdPartyToModel

        private void transformThirdPartyToModel(List<ThirdPartyDtoResponse> listToTransform, string typeOfPerson)
        {
            foreach (var item in listToTransform)
            {
                PersonInRadication personInRadication = new()
                {
                    TypeOfPersonInRadication = typeOfPerson ?? "",
                    Id = item.ThirdPartyId,
                    CompanyId = item.CompanyId,

                    AddressId = default,

                    FullName = item.FullName ?? "",
                    IdentificationType = "",
                    IdentificationTypeName = "",

                    IdentificationNumber = item.IdentificationNumber ?? "",

                    AdministrativeUnitId = default,

                    AdministrativeUnitCode = "",
                    AdministrativeUnitName = "",

                    ProductionOfficeId = default,

                    ProductionOfficeCode = "",

                    ProductionOfficeName = "",

                    Email1 = item.Email1 ?? "",

                    Email2 = "",
                    ChargeCode = item.ChargeCode ?? "",

                    Charge = item.ChargeName ?? "",

                    Phone1 = item.Phone1 ?? "",

                    Phone2 = item.Phone2 ?? "",
                };

                radicationRecievedToReturn.Add(personInRadication);
            }
        }

        #endregion transformThirdPartyToModel

        #region transformThirdUserToModel

        private void transformThirdUserToModel(List<ThirdUserDtoResponse> listToTransform, string typeOfPerson)
        {
            foreach (var item in listToTransform)
            {
                PersonInRadication personInRadication = new()
                {
                    TypeOfPersonInRadication = typeOfPerson ?? "",
                    Id = item.ThirdUserId,
                    CompanyId = default,

                    AddressId = default,

                    FullName = item.Names ?? "",
                    IdentificationType = item.IdentificationType,
                    IdentificationTypeName = item.IdentificationTypeName,

                    IdentificationNumber = item.IdentificationNumber ?? "",

                    AdministrativeUnitId = default,

                    AdministrativeUnitCode = "",
                    AdministrativeUnitName = "",

                    ProductionOfficeId = default,

                    ProductionOfficeCode = "",

                    ProductionOfficeName = "",

                    Email1 = item.Email ?? "",

                    Email2 = "",
                    ChargeCode = "",

                    Charge = "",

                    Phone1 = "",

                    Phone2 = "",
                };

                radicationRecievedToReturn.Add(personInRadication);
            }
        }

        #endregion transformThirdUserToModel

        #region OnClickAssignRadication

        private async Task OnClickAssignRadication()
        {
            radicationRecievedToReturn = new();
            await _userSearchComponet.HandleModalClosed(false);

            if (ConfigurationInUse == 5) { await _userSearchComponet2.HandleModalClosed(false); }
            else { await _thirdUserSearchComponet.HandleModalClosed(false); }

            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de su selección?", true, "Si", "No");
        }

        #endregion OnClickAssignRadication

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                var eventArgs = new MyEventArgs<List<PersonInRadication>>
                {
                    Data = radicationRecievedToReturn,
                    ModalStatus = false
                };
                await OnStatusChangeRadication.InvokeAsync(eventArgs);
            }
        }

        #endregion HandleModalNotiClose

        #endregion Methods
    }
}