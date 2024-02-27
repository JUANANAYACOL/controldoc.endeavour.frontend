using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdParty;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch
{
    public partial class ThirdPartySearchModal : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent inputNames { get; set; } = new();

        private InputModalComponent inputEmail { get; set; } = new();

        private InputModalComponent inputIdentificcation { get; set; } = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion Components

        #region Modals

        private ThirdPartyModal modalThirdParty { get; set; } = new();
        private AddressModal modalAddress { get; set; } = new();

        #endregion Modals

        #region Parameters

        [Parameter] public bool multipleSelection { get; set; } = true;
        [Parameter] public bool showCopiesColumn { get; set; } = true;
        [Parameter] public bool hasEmail { get; set; } = false;

        [Parameter] public EventCallback<MyEventArgs<ThirdPartyDtoResponse>> OnStatusChanged { get; set; }

        [Parameter] public EventCallback<MyEventArgs<ThirdUserDtoResponse>> OnStatusThirdUserChanged { get; set; }

        [Parameter] public EventCallback<MyEventArgs<List<object>>> OnStatusChangedMultipleSelection { get; set; }

        #region Models

        private ThirdPartyFilterDtoRequest thirdPartyFilter { get; set; } = new();
        private List<ThirdPartyDtoResponse> thirdPartyList { get; set; } = new();

        private List<ThirdPartyDtoResponse> thirdPartiesManagerToReturn { get; set; } = new();
        private List<ThirdPartyDtoResponse> thirdPartiesCopiesToReturn { get; set; } = new();

        private List<ThirdUserDtoResponse> thirdUsersManagerToReturn { get; set; } = new();
        private List<ThirdUserDtoResponse> thirdUsersCopiesToReturn { get; set; } = new();

        private ThirdPartyDtoResponse thirdPartyToReturn { get; set; } = new();
        private ThirdUserDtoResponse thirdUserToReturn { get; set; } = new();

        private MetaModel meta { get; set; } = new() { PageSize = 10 };

        #endregion Models

        #endregion Parameters

        #region Environments

        private int companyId { get; set; } = 0;

        private bool searchByPN { get; set; } = true;
        private bool searchByPJ { get; set; } = false;

        private bool disableButton { get; set; } = true;
        private bool dataChargue { get; set; } = false;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            EnableButton();
            await OnClickButton();
            StateHasChanged();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region OnChangeSwitchPJ

        private async Task OnChangeSwitchPJ()
        {
            searchByPJ = !searchByPJ;
            searchByPN = !searchByPJ;
            thirdPartyList = new();
            dataChargue = false;
            meta = new() { PageSize = 10 };
            EnableButton();
            await OnClickButton();
        }

        #endregion OnChangeSwitchPJ

        #region OnChangeSwitchPN

        private async Task OnChangeSwitchPN()
        {
            searchByPN = !searchByPN;
            searchByPJ = !searchByPN;
            thirdPartyList = new();
            dataChargue = false;
            meta = new() { PageSize = 10 };
            EnableButton();
            await OnClickButton();
        }

        #endregion OnChangeSwitchPN

        #region EnableButton

        private void EnableButton()
        {
            if (searchByPJ != searchByPN)
            {
                disableButton = false;
            }
        }

        #endregion EnableButton

        #region OnClickPlus

        public void OnClickPlus()
        {
            modalThirdParty.selectPersonType(searchByPN ? 0 : 1);
            modalThirdParty.UpdateModalStatus(true);
        }

        #endregion OnClickPlus

        #region OnClickButton

        private async Task OnClickButton()
        {
            try
            {
                /*                PageLoadService.MostrarSpinnerReadLoad(Js);*/
                string typePersonToSearch = "";
                if (searchByPJ && !searchByPN) { typePersonToSearch = "PJ"; }
                else if (!searchByPJ && searchByPN) { typePersonToSearch = "PN"; }

                thirdPartyFilter = new()
                {
                    CompanyId = companyId,
                    PersonType = typePersonToSearch,
                    Names = inputNames.InputValue ?? "",
                    Email = inputEmail.InputValue ?? "",
                    IdentificationNumber = inputIdentificcation.InputValue ?? ""
                };

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/ByFilterWithUsers", thirdPartyFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ThirdPartyDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    thirdPartyList = deserializeResponse.Data!;
                    meta = deserializeResponse.Meta!;
                    dataChargue = true;
                    ReactiveExistingData(thirdPartyList);
                    StateHasChanged();
                }
                else
                {
                    dataChargue = false;
                    thirdPartyList = new();
                    meta = new() { PageSize = 10 };
                }
            }
            catch { thirdPartyList = new(); }

            StateHasChanged();
            /*           PageLoadService.OcultarSpinnerReadLoad(Js);*/
        }

        #endregion OnClickButton

        #region OnClickButtonClear

        public async Task OnClickButtonClear()
        {
            thirdPartyList = new();

            thirdPartyList = new();

            thirdPartiesManagerToReturn = new();
            thirdPartiesCopiesToReturn = new();

            thirdUsersManagerToReturn = new();
            thirdUsersCopiesToReturn = new();
            thirdPartyFilter = new();

            StateHasChanged();
            await OnInitializedAsync();
        }

        #endregion OnClickButtonClear

        #region HandleModalClosed

        public async Task HandleModalClosed(bool status)
        {
            if (!multipleSelection && thirdPartyToReturn != null)
            {
                var eventArgs = new MyEventArgs<ThirdPartyDtoResponse>
                {
                    Data = thirdPartyToReturn,
                    ModalStatus = status
                };
                await OnStatusChanged.InvokeAsync(eventArgs);
            }
            else if (!multipleSelection && thirdUserToReturn != null)
            {
                var eventArgs = new MyEventArgs<ThirdUserDtoResponse>
                {
                    Data = thirdUserToReturn,
                    ModalStatus = status
                };
                await OnStatusThirdUserChanged.InvokeAsync(eventArgs);
            }
            else
            {
                List<object> ListOfThirdPartiesAndCopiesToReturn = new() {
                    thirdPartiesManagerToReturn,
                    thirdPartiesCopiesToReturn,
                    thirdUsersManagerToReturn,
                    thirdUsersCopiesToReturn
                };

                var eventArgs = new MyEventArgs<List<object>>
                {
                    Data = ListOfThirdPartiesAndCopiesToReturn,
                    ModalStatus = status
                };
                await OnStatusChangedMultipleSelection.InvokeAsync(eventArgs);
            }
        }

        #endregion HandleModalClosed

        #region HandleStatusChanged

        private async Task HandleStatusChanged(bool status)
        {
            modalThirdParty.UpdateModalStatus(status);

            if (thirdPartyList.Count != 0)
            {
                await OnClickButton();
            }
        }

        #endregion HandleStatusChanged

        #region HandleId

        private async Task HandleId(int id)
        {
            await modalAddress.UpdateModalIdAsync(id);
        }

        #endregion HandleId

        #region HandleUserSelectedChanged

        private void HandleUserSelectedChanged(MyEventArgs<List<(string, AddressDtoRequest)>> address)
        {
            modalAddress.UpdateModalStatus(address.ModalStatus);
            modalThirdParty.updateAddressSelection(address.Data);
        }

        #endregion HandleUserSelectedChanged

        #region ShowModalEdit

        private void ShowModalEdit(ThirdPartyDtoResponse record)
        {
            modalThirdParty.selectPersonType(searchByPN ? 0 : 1);
            modalThirdParty.UpdateModalStatus(true);
            modalThirdParty.ReceiveThirdParty(record);
        }

        #endregion ShowModalEdit

        #region SelectThirdParty

        private void SelectThirdParty(ThirdPartyDtoResponse thirdParty)
        {
            thirdPartyList.Where(x => x.ThirdPartyId != thirdParty.ThirdPartyId).ToList().ForEach(x => { x.Selected = false; });
            thirdPartyToReturn = thirdParty;
        }

        #endregion SelectThirdParty

        #region SelectThirdUser

        private void SelectThirdUser(ThirdUserDtoResponse thirdUser)
        {
            thirdPartyList?
                .Where(x => x.ThirdPartyId == thirdUser.ThirdPartyId)
                .FirstOrDefault()?
                .ThirdUsers?
                .Where(y => y.ThirdUserId != thirdUser.ThirdUserId)
                .ToList()
                .ForEach(x => x.Selected = false);

            thirdUserToReturn = thirdUser;
        }

        #endregion SelectThirdUser

        #region changeStateThirdParty

        public async Task changeStateThirdParty(ThirdPartyDtoResponse thirdParty)
        {
            if (hasEmail && ( string.IsNullOrEmpty(thirdParty.Email1) || string.IsNullOrEmpty(thirdParty.Email2) ))
            {
                notificationModal.UpdateModal(ModalType.Error, "no es posible asignar un usuario que no tenga correo electronico", true);
                thirdParty.Selected = false;
            }

            if (!multipleSelection)
            {
                SelectThirdParty(thirdParty);
                notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de su selección?", true, "Si", "No");
            }
            else
            {
                if (thirdParty.Selected && thirdParty.Copy)
                {
                    thirdParty.Copy = false;
                    await ChangeStateThirdPartyCopies(thirdParty);
                }

                var allUsersSavedInManger = thirdPartiesManagerToReturn.Select(x => x.ThirdPartyId).ToList();
                if (allUsersSavedInManger.Contains(thirdParty.ThirdPartyId) && !thirdParty.Selected)
                {
                    var elementToErrase = thirdPartiesManagerToReturn.Find(x => x.ThirdPartyId == thirdParty.ThirdPartyId);
                    thirdPartiesManagerToReturn.Remove(elementToErrase!);
                }
                else if (!allUsersSavedInManger.Contains(thirdParty.ThirdPartyId) && thirdParty.Selected)
                {
                    thirdPartiesManagerToReturn.Add(thirdParty);
                }
            }
        }

        #endregion changeStateThirdParty

        #region ChangeStateThirdPartyCopies

        public async Task ChangeStateThirdPartyCopies(ThirdPartyDtoResponse thirdParty)
        {
            if (hasEmail && ( string.IsNullOrEmpty(thirdParty.Email1) || string.IsNullOrEmpty(thirdParty.Email2) ))
            {
                notificationModal.UpdateModal(ModalType.Error, "no es posible asignar un usuario que no tenga correo electronico", true);
                thirdParty.Copy = false;
            }

            if (thirdParty.Selected && thirdParty.Copy)
            {
                thirdParty.Selected = false;
                await changeStateThirdParty(thirdParty);
            }

            var allUsersSavedInCopies = thirdPartiesCopiesToReturn.Select(x => x.ThirdPartyId).ToList();
            if (allUsersSavedInCopies.Contains(thirdParty.ThirdPartyId) && !thirdParty.Copy)
            {
                var elementToErrase = thirdPartiesCopiesToReturn.Find(x => x.ThirdPartyId == thirdParty.ThirdPartyId);
                thirdPartiesCopiesToReturn.Remove(elementToErrase!);
            }
            else if (!allUsersSavedInCopies.Contains(thirdParty.ThirdPartyId) && thirdParty.Copy)
            {
                thirdPartiesCopiesToReturn.Add(thirdParty);
            }
        }

        #endregion ChangeStateThirdPartyCopies

        #region changeStateThirdUser

        public async Task changeStateThirdUser(ThirdUserDtoResponse thirdUser)
        {
            if (!multipleSelection)
            {
                SelectThirdUser(thirdUser);
                notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de su selección?", true, "Si", "No");
            }
            else
            {
                if (thirdUser.Selected && thirdUser.Copy)
                {
                    thirdUser.Copy = false;
                    await ChangeStateThirdUserCopies(thirdUser);
                }

                var allThirdUsersSavedInManger = thirdUsersManagerToReturn.Select(x => x.ThirdUserId).ToList();
                if (allThirdUsersSavedInManger.Contains(thirdUser.ThirdUserId) && !thirdUser.Selected)
                {
                    var elementToErrase = thirdUsersManagerToReturn.Find(x => x.ThirdUserId == thirdUser.ThirdUserId);
                    thirdUsersManagerToReturn.Remove(elementToErrase!);
                }
                else if (!allThirdUsersSavedInManger.Contains(thirdUser.ThirdUserId) && thirdUser.Selected)
                {
                    thirdUsersManagerToReturn.Add(thirdUser);
                }

                EnableCheckBox(thirdUser.ThirdPartyId, 1);
            }
        }

        #endregion changeStateThirdUser

        #region EnableCheckBox

        private void EnableCheckBox(int thirdPartyId, int SelectionOrCopies)
        {
            var thirdparty = thirdPartyList.Find(x => x.ThirdPartyId == thirdPartyId);

            if (thirdparty != null)
            {
                List<bool> selectionBools;
                if (SelectionOrCopies == 1)
                {
                    selectionBools = thirdparty.ThirdUsers!.Select(x => x.Selected).Distinct().ToList();

                    if (selectionBools.Count != 1)
                    {
                        thirdPartyList.Find(x => x.ThirdPartyId == thirdPartyId).EnableSelection = false;
                    }
                    else if (selectionBools.Count == 1 && !selectionBools[0])
                    {
                        thirdPartyList.Find(x => x.ThirdPartyId == thirdPartyId).EnableSelection = true;
                    }
                }
                else if (SelectionOrCopies == 2)
                {
                    selectionBools = thirdparty.ThirdUsers!.Select(x => x.Copy).Distinct().ToList();

                    if (selectionBools.Count != 1)
                    {
                        thirdPartyList.Find(x => x.ThirdPartyId == thirdPartyId).EnableCopy = false;
                    }
                    else if (selectionBools.Count == 1 && !selectionBools[0])
                    {
                        thirdPartyList.Find(x => x.ThirdPartyId == thirdPartyId).EnableCopy = true;
                    }
                }
            }
        }

        #endregion EnableCheckBox

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await HandleModalClosed(false);
            }
            else if (!multipleSelection)
            {
                thirdPartyList?.ForEach(x => { x.Selected = false; x.ThirdUsers?.ForEach(y => { y.Selected = false; }); });
            }
        }

        #endregion HandleModalNotiClose

        #region ChangeStateThirdUserCopies

        public async Task ChangeStateThirdUserCopies(ThirdUserDtoResponse thirdUser)
        {
            if (thirdUser.Selected && thirdUser.Copy)
            {
                thirdUser.Selected = false;
                await changeStateThirdUser(thirdUser);
            }

            var allUsersSavedInCopies = thirdUsersCopiesToReturn.Select(x => x.ThirdUserId).ToList();
            if (allUsersSavedInCopies.Contains(thirdUser.ThirdUserId) && !thirdUser.Copy)
            {
                var elementToErrase = thirdUsersCopiesToReturn.Find(x => x.ThirdUserId == thirdUser.ThirdUserId);
                thirdUsersCopiesToReturn.Remove(elementToErrase!);
            }
            else if (!allUsersSavedInCopies.Contains(thirdUser.ThirdUserId) && thirdUser.Copy)
            {
                thirdUsersCopiesToReturn.Add(thirdUser);
            }

            EnableCheckBox(thirdUser.ThirdPartyId, 2);
        }

        #endregion ChangeStateThirdUserCopies

        #region ReactiveExistingData

        private void ReactiveExistingData(List<ThirdPartyDtoResponse> thirdPartyToReactive)
        {
            var allThirdPartiesSavedInCopies = new HashSet<int>(thirdPartiesCopiesToReturn.Select(x => x.ThirdPartyId));
            var allThirdPartiesSavedInManager = new HashSet<int>(thirdPartiesManagerToReturn.Select(x => x.ThirdPartyId));
            var allThirdUsersSaveInManager = new HashSet<int>(thirdUsersManagerToReturn.Select(x => x.ThirdUserId));
            var allThirdUsersSaveInCopies = new HashSet<int>(thirdUsersCopiesToReturn.Select(x => x.ThirdUserId));

            thirdPartyToReactive
                .Where(x => allThirdPartiesSavedInManager.Contains(x.ThirdPartyId))
                .ToList()
                .ForEach(x => x.Selected = true);

            thirdPartyToReactive
                .Where(x => allThirdPartiesSavedInCopies.Contains(x.ThirdPartyId))
                .ToList()
                .ForEach(x => x.Copy = true);

            // Reactivar ThirdUsers en Manager y Copies
            if (thirdPartyToReactive != null)
            {
                foreach (var item in thirdPartyToReactive)
                {
                    item.ThirdUsers?
                        .Where(x => allThirdUsersSaveInManager.Contains(x.ThirdUserId))
                        .ToList()
                        .ForEach(x => x.Selected = true);

                    item.ThirdUsers?
                        .Where(x => allThirdUsersSaveInCopies.Contains(x.ThirdUserId))
                        .ToList()
                        .ForEach(x => x.Copy = true);
                }
            }
        }

        #endregion ReactiveExistingData

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<ThirdPartyDtoResponse> newDataList)
        {
            /* PageLoadService.MostrarSpinnerReadLoad(Js);*/
            thirdPartyList = newDataList;
            ReactiveExistingData(thirdPartyList);
            /*            PageLoadService.OcultarSpinnerReadLoad(Js);*/
        }

        #endregion HandlePaginationGrid

        #endregion Methods
    }
}