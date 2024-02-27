﻿using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch
{
    public partial class UserSearchModal : ComponentBase
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

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        public InputModalComponent? firstNameInput { get; set; } = new();
        public InputModalComponent? lastNameInput { get; set; } = new();
        private PaginationComponent<VUserDtoResponse, VUserDtoRequest> PaginationComponet = new();

        #endregion Modals

        #region Parameters

        [Parameter] public bool hasEmail { get; set; } = false;
        [Parameter] public bool showParamTrdDdl { get; set; } = new();

        [Parameter] public bool showCopiesColumn { get; set; } = new();
        [Parameter] public bool showCarge { get; set; } = new();
        [Parameter] public bool multipleSelection { get; set; } = new();

        [Parameter] public EventCallback<MyEventArgs<VUserDtoResponse>> OnStatusChanged { get; set; } = new();

        [Parameter] public EventCallback<MyEventArgs<List<object>>> OnStatusChangedMultipleSelection { get; set; } = new();
        [Parameter] public bool showNameField { get; set; } = new();
        [Parameter] public bool showLastNameField { get; set; } = new();

        #region Models

        private List<AdministrativeUnitsDtoResponse>? administrativeUnitList { get; set; } = new();
        private List<ProductionOfficesDtoResponse>? productionOfficeList { get; set; } = new();
        private List<VSystemParamDtoResponse>? chargeList { get; set; }

        public List<VUserDtoResponse>? UserList { get; set; } = new();
        private List<VUserDtoResponse> usersInManagerToReturn { get; set; } = new();
        private List<VUserDtoResponse> usersInCopiesToReturn { get; set; } = new();

        public VUserDtoResponse? userToReturn { get; set; } = new();

        private VUserDtoRequest vUserRequest { get; set; } = new();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };

        #endregion Models

        #region Enviroment

        public string? firstName { get; set; }
        public string? lastName { get; set; }

        public string? IdModalIdentifier { get; set; }
        private bool selectAllManager { get; set; } = new();
        private bool selectAllCopies { get; set; } = new();
        private bool dataChargue { get; set; } = false;

        private bool isEnableCharge { get; set; }

        private string? selectChargeCode { get; set; }

        private bool isEnableProductionOffice { get; set; } = new();

        private int selectProductionOfficetId { get; set; } = new();

        private bool isEnableAdministriveUnit { get; set; } = new();

        private int selectAdministriveUnitId { get; set; } = new();

        #endregion Enviroment

        #endregion Parameters

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            await OnClickButton();
            StateHasChanged();
            if (showParamTrdDdl)
            {
                isEnableAdministriveUnit = true;

                await FillAdministrativeUnitDdl();
            }

            if (showCarge)
            {
                isEnableCharge = true;

                await FillChargeDdl();
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region FillAdministrativeUnitDdl

        private async Task FillAdministrativeUnitDdl()
        {
            HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
            HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", "0");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
            HttpClient?.DefaultRequestHeaders.Remove("key");

            if (deserializeResponse!.Succeeded) { administrativeUnitList = deserializeResponse.Data; }
            else { administrativeUnitList = new(); }
        }

        #endregion FillAdministrativeUnitDdl

        #region FillChargeDdl

        private async Task FillChargeDdl()
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "CAR");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                chargeList = deserializeResponse.Data;
            }
            else { chargeList = new(); }
        }

        #endregion FillChargeDdl

        #region FillPoductionOfficeDdl

        private async Task FillPoductionOfficeDdl()
        {
            HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
            HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{selectAdministriveUnitId}");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
            HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");

            if (deserializeResponse!.Succeeded)
            {
                productionOfficeList = deserializeResponse.Data!;
            }
            else
            {
                productionOfficeList = new();
            }
        }

        #endregion FillPoductionOfficeDdl

        #region OnClickButtonClear

        public async Task OnClickButtonClear()
        {
            usersInManagerToReturn = new();
            usersInCopiesToReturn = new();
            userToReturn = null;

            isEnableProductionOffice = false;
            selectAdministriveUnitId = 0;
            selectProductionOfficetId = 0;

            selectChargeCode = "";
            vUserRequest.FirstName = "";
            vUserRequest.LastName = "";
            vUserRequest = new();

            StateHasChanged();
            await OnInitializedAsync();
        }

        #endregion OnClickButtonClear

        #region OnClickButton

        public async Task OnClickButton()
        {
            selectAllManager = false;
            selectAllCopies = false;

            var responseApi = await HttpClient!.PostAsJsonAsync("generalviews/VUser/ByFilter", vUserRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VUserDtoResponse>>>();
            if (deserializeResponse!.Succeeded)
            {
                UserList = deserializeResponse.Data!;
                meta = deserializeResponse.Meta!;
                dataChargue = true;
                PaginationComponet.ResetPagination(meta);
                ReactiveExistingData(UserList);
                StateHasChanged(); //Logica Exitosa
            }
            else
            {
                dataChargue = false;
                UserList = new();
                meta = new() { CurrentPage = 0, PageSize = 10 };
            }
            StateHasChanged();
        }

        #endregion OnClickButton

        #region Reset

        public async Task Reset()
        {
            usersInManagerToReturn = new();
            usersInCopiesToReturn = new();
            userToReturn = null;

            administrativeUnitList = new();
            isEnableAdministriveUnit = false;

            selectAdministriveUnitId = 0;

            productionOfficeList = new();
            isEnableProductionOffice = false;

            selectProductionOfficetId = 0;

            chargeList = new();
            isEnableCharge = false;

            selectChargeCode = "";

            vUserRequest = new();
            UserList = new();

            chargeList = new();
            await OnInitializedAsync();

            StateHasChanged();
        }

        #endregion Reset

        #region reactiveExistingData

        private void ReactiveExistingData(List<VUserDtoResponse> usersToReactive)
        {
            var allUsersSavedInCopies = usersInCopiesToReturn.Select(x => x.UserId).ToList();
            var allUsersSavedInManger = usersInManagerToReturn.Select(x => x.UserId).ToList();
            usersToReactive.Where(x => allUsersSavedInManger.Contains(x.UserId)).ToList().ForEach(x => { x.Selected = true; });
            usersToReactive.Where(x => allUsersSavedInCopies.Contains(x.UserId)).ToList().ForEach(x => { x.Copy = true; });
        }

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<VUserDtoResponse> newDataList)
        {
            UserList = newDataList;
            ReactiveExistingData(UserList);
        }

        #endregion HandlePaginationGrid

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await HandleModalClosed(false);
            }
            else if (!multipleSelection)
            {
                UserList?.ForEach(x => x.Selected = false);
            }
        }

        #endregion HandleModalNotiClose

        #region OnChangeUA

        public async Task OnChangeUA(int value)
        {
            selectAdministriveUnitId = value;
            productionOfficeList = new();
            if (selectAdministriveUnitId != 0)
            {
                await FillPoductionOfficeDdl();
            }

            if (productionOfficeList.Count != 0)
            {
                isEnableProductionOffice = true;
            }
            else
            {
                isEnableProductionOffice = false;
            }
        }

        #endregion OnChangeUA

        #region OnChangeOP

        public void OnChangeOP(int value)
        {
            selectProductionOfficetId = value;
        }

        #endregion OnChangeOP

        #region OnChangeOP

        public void OnChangeCh(string value)
        {
            selectChargeCode = value;
        }

        #endregion OnChangeOP

        private void SelecteUser(VUserDtoResponse user)
        {
            UserList?.Where(x => x.UserId != user.UserId).ToList().ForEach(x => { x.Selected = false; });
            userToReturn = user;
        }

        #endregion reactiveExistingData

        #region ChangeStateManager

        private async Task ChangeStateManager(VUserDtoResponse user)
        {
            if (hasEmail && ( string.IsNullOrEmpty(user.Email) ))
            {
                notificationModal.UpdateModal(ModalType.Error, "no es posible asignar un usuario que no tenga correo electronico", true);
                user.Selected = false;
            }

            if (!multipleSelection)
            {
                SelecteUser(user);
                notificationModal.UpdateModal(ModalType.Warning, "¿Esta seguro de su selección?", true, "Si", "No");
            }
            else
            {
                selectAllManager = false;

                if (user.Selected && user.Copy)
                {
                    user.Copy = false;
                    await ChangeStateCopies(user);
                }

                var allUsersSavedInManger = usersInManagerToReturn.Select(x => x.UserId).ToList();
                if (allUsersSavedInManger.Contains(user.UserId) && !user.Selected)
                {
                    var elementToErrase = usersInManagerToReturn.Find(x => x.UserId == user.UserId);
                    usersInManagerToReturn.Remove(elementToErrase!);
                }
                else if (!allUsersSavedInManger.Contains(user.UserId) && user.Selected)
                {
                    usersInManagerToReturn.Add(user);
                }
            }
        }

        #endregion ChangeStateManager

        #region ChangeStateCopies

        private async Task ChangeStateCopies(VUserDtoResponse user)
        {
            if (hasEmail && ( string.IsNullOrEmpty(user.Email) ))
            {
                notificationModal.UpdateModal(ModalType.Error, "no es posible asignar un usuario que no tenga correo electronico", true);
                user.Copy = false;
            }

            selectAllCopies = false;

            if (user.Selected && user.Copy)
            {
                user.Selected = false;
                await ChangeStateManager(user);
            }

            var allUsersSavedInCopies = usersInCopiesToReturn.Select(x => x.UserId).ToList();
            if (allUsersSavedInCopies.Contains(user.UserId) && !user.Copy)
            {
                var elementToErrase = usersInCopiesToReturn.Find(x => x.UserId == user.UserId);
                usersInCopiesToReturn.Remove(elementToErrase!);
            }
            else if (!allUsersSavedInCopies.Contains(user.UserId) && user.Copy)
            {
                usersInCopiesToReturn.Add(user);
            }
        }

        #endregion ChangeStateCopies

        #region ChangeAllStateManager

        private void ChangeAllStateManager()
        {
            usersInManagerToReturn = new();
            usersInCopiesToReturn = new();
            if (selectAllCopies) { selectAllCopies = !selectAllManager; }
            UserList?.ForEach(x => { x.Selected = selectAllManager; x.Copy = selectAllCopies; });
            if (selectAllManager)
            {
                usersInManagerToReturn = UserList ?? new();
            }
        }

        #endregion ChangeAllStateManager

        #region ChangeAllStateCopies

        private void ChangeAllStateCopies()
        {
            usersInManagerToReturn = new();
            usersInCopiesToReturn = new();

            if (selectAllManager) { selectAllManager = !selectAllCopies; }

            UserList?.ForEach(x => { x.Copy = selectAllCopies; x.Selected = selectAllManager; });
            if (selectAllCopies)
            {
                usersInCopiesToReturn = UserList ?? new();
            }
        }

        #endregion ChangeAllStateCopies

        #region HandleModalClosed

        public async Task HandleModalClosed(bool status)
        {
            if (multipleSelection)
            {
                List<object> ListOfUserAndCopiesToReturn = new() {
                    usersInManagerToReturn,
                    usersInCopiesToReturn
                };

                var eventArgs = new MyEventArgs<List<object>>
                {
                    Data = ListOfUserAndCopiesToReturn,
                    ModalStatus = status
                };
                await OnStatusChangedMultipleSelection.InvokeAsync(eventArgs);
            }
            else
            {
                var eventArgs = new MyEventArgs<VUserDtoResponse>
                {
                    Data = userToReturn!,
                    ModalStatus = status
                };
                await OnStatusChanged.InvokeAsync(eventArgs);
            }

            StateHasChanged();
        }

        #endregion HandleModalClosed

        #endregion Methods
    }
}