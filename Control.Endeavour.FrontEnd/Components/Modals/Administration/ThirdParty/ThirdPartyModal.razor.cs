using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdParty
{
    public partial class ThirdPartyModal : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        //Listas
        private List<SystemFieldsDtoResponse>? identificationTypeList;

        private List<SystemFieldsDtoResponse>? chargeList;
        private List<SystemFieldsDtoResponse>? natureList;

        //Inputs
        private InputModalComponent thirdPartyId { get; set; } = new();

        private InputModalComponent identification { get; set; } = new();
        private InputModalComponent names { get; set; } = new();
        private InputModalComponent email1 { get; set; } = new();
        private InputModalComponent email2 { get; set; } = new();
        private InputModalComponent webpage { get; set; } = new();
        private InputModalComponent lastNames { get; set; } = new();
        private InputModalComponent initials { get; set; } = new();
        private InputModalComponent phone1 { get; set; } = new();
        private InputModalComponent phone2 { get; set; } = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public bool ModalStatus { get; set; } = false;

        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public bool CrearEditar { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        [Parameter]
        public EventCallback<string> OnPersonType { get; set; }

        [Parameter]
        public string PersonType { get; set; } = "";

        [Parameter]
        public EventCallback<int> OnIdSaved { get; set; }

        [Parameter]
        public EventCallback OnResetForm { get; set; }

        #endregion Parameters

        #region Models

        //Request and Response
        private ThirdPartyDtoResponse _selectedRecord = new();

        private ThirdPartyCreateDtoRequest thirdPartyDtoRequest = new();
        private ThirdPartyUpdateDtoRequest thirdPartyUpdateDtoRequest = new();
        private ThirdPartyDtoResponse thirdPartyDtoResponse = new();
        private AddressDtoRequest addressDtoRequest = new();
        private AddressDtoResponse addressDtoResponse = new();

        #endregion Models

        #region Environments

        private string textAddress = "";

        //Inputs
        private string personType { get; set; } = "";

        private bool lastNameActive { get; set; }
        private string identificationType = "";
        private bool activeState = true;
        private int addressId;

        //ThirData
        private string charge = "";

        private string nature { get; set; } = "";

        //Create or Edit
        private bool IsEditForm = false;

        private int IdThirdParty = 0;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            ModalStatus = false;
            await GetIdentificationType();
            await GetCharge();
            await GetNature();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region OtherMethods

        #region GetDropDownLists

        #region GetIdentificationType

        private async Task GetIdentificationType()
        {
            var type = "";
            if (personType == "" || personType == null)
            {
                type = "TDIN";
            }
            else
            {
                type = (personType == "PN") ? "TDIN" : "TDIJ";
            }

            try
            {
                SystemFieldsDtoRequest? request = new() { ParamCode = type };
                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    identificationTypeList = deserializeResponse.Data;
                }
                else
                {
                    identificationTypeList = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de identificaciones: {ex.Message}");
            }
        }

        #endregion GetIdentificationType

        #region GetCharge

        private async Task GetCharge()
        {
            try
            {
                SystemFieldsDtoRequest? request = new() { ParamCode = "CAR" };
                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    chargeList = deserializeResponse.Data;
                }
                else
                {
                    chargeList = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los cargos: {ex.Message}");
            }
        }

        #endregion GetCharge

        #region GetNature

        private async Task GetNature()
        {
            try
            {
                SystemFieldsDtoRequest? request = new() { ParamCode = "NAT" };
                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    natureList = deserializeResponse.Data;
                }
                else
                {
                    natureList = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las naturalezas: {ex.Message}");
            }
        }

        #endregion GetNature

        #endregion GetDropDownLists

        #region GetValues

        private string GetValueFromList(List<SystemFieldsDtoResponse> itemList, string selectedValue, string systemParamCode)
        {
            var systemparam = "";
            var code = "";

            // Verificar si selectedValue es nulo o vacío
            if (string.IsNullOrEmpty(selectedValue))
            {
                return "";
            }

            if (selectedValue.Contains(","))
            {
                string[]? partes = selectedValue.Split(',');
                (systemparam, code) = (partes[0].Trim(), partes[1].Trim());
            }
            else
            {
                code = selectedValue.Trim();
                systemparam = systemParamCode;
            }

            var selectedDataItem = itemList.Find(item => item.Code == code && item.SystemParam.ParamCode == systemparam);
            return selectedDataItem?.Value ?? "";
        }

        private string valueIdentification(string selectedValue)
        {
            return GetValueFromList(identificationTypeList!, selectedValue, "TDI");
        }

        private string valueCharge(string selectedValue)
        {
            return GetValueFromList(chargeList!, selectedValue, "CAR");
        }

        private string valueNature(string selectedValue)
        {
            return GetValueFromList(natureList!, selectedValue, "NAT");
        }

        #endregion GetValues

        #region GetCodes

        //Metodo para tomar el Code de StType, CrType, HouseType y HouseClass
        private string GetCodeFromList(string selectedValue, string systemParamCode)
        {
            var selectedDataItem = "";

            // Verificar si selectedValue es nulo o vacío
            if (string.IsNullOrEmpty(selectedValue))
            {
                return "";
            }

            if (selectedValue.Contains(","))
            {
                selectedDataItem = selectedValue;
            }
            else
            {
                selectedDataItem = $"{systemParamCode},{selectedValue}";
            }

            return selectedDataItem ?? "";
        }

        private string codeIdentification(string selectedValue)
        {
            return GetCodeFromList(selectedValue, (personType == "PN" ? "TDIN" : "TDIJ"));
        }

        private string codeCharge(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "CAR");
        }

        private string codeNature(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "NAT");
        }

        #endregion GetCodes

        #region Address

        #region GetAddress

        private async Task GetAddressAsync(int addressId)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");
                HttpClient?.DefaultRequestHeaders.Add("IdAddress", addressId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<AddressDtoResponse>>("administration/Address/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");

                if (deserializeResponse!.Succeeded)
                {
                    addressDtoResponse = deserializeResponse.Data!;
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar la dirección, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las naturalezas: {ex.Message}");
            }
        }

        #endregion GetAddress

        #region FillAddress

        private AddressDtoRequest fillAddress(AddressDtoResponse address)
        {
            AddressDtoRequest? request = new AddressDtoRequest();
            request.CountryId = address.CountryId;
            request.StateId = address.StateId;
            request.CityId = address.CityId;
            request.StType = address.StType!;
            request.StNumber = address.StNumber!;
            request.StLetter = address.StLetter!;
            request.StBis = (bool)address.StBis!;
            request.StComplement = address.StComplement!;
            request.StCardinality = address.StCardinality!;
            request.CrType = address.CrType!;
            request.CrNumber = address.CrNumber!;
            request.CrLetter = address.CrLetter!;
            request.CrBis = (bool)address.CrBis!;
            request.CrComplement = address.CrComplement!;
            request.CrCardinality = address.CrCardinality!;
            request.HouseType = address.HouseType!;
            request.HouseClass = address.HouseClass!;
            request.HouseNumber = address.HouseNumber!;
            return request;
        }

        #endregion FillAddress

        #region UpdateAddress

        public void updateAddressSelection(List<(string, AddressDtoRequest)> address)
        {
            if (address != null && address.Count > 0)
            {
                (textAddress, addressDtoRequest) = address[0];
            }
        }

        #endregion UpdateAddress

        #endregion Address

        #region Record

        #region ReceiveThirdParty

        private string? textIdentificationType = "Seleccione un tipo de identificación...";
        private string? textCharge = "Seleccione un Cargo...";
        private string? textNature = "Seleccione una Naturaleza...";

        public void ReceiveThirdParty(ThirdPartyDtoResponse response)
        {
            _selectedRecord = response;
            IdThirdParty = _selectedRecord.ThirdPartyId;
            addressId = _selectedRecord.AddressId;
            textAddress = _selectedRecord.Address!;
            activeState = _selectedRecord.ActiveState;
            thirdPartyDtoResponse.IdentificationNumber = _selectedRecord.IdentificationNumber;
            thirdPartyDtoResponse.Names = _selectedRecord.Names;
            thirdPartyDtoResponse.ActiveState = _selectedRecord.ActiveState;
            thirdPartyDtoResponse.Email1 = _selectedRecord.Email1;
            thirdPartyDtoResponse.Email2 = _selectedRecord.Email2;
            thirdPartyDtoResponse.WebPage = _selectedRecord.WebPage;
            thirdPartyDtoResponse.FullName = _selectedRecord.FullName;
            thirdPartyDtoResponse.FirstName = _selectedRecord.FirstName;
            thirdPartyDtoResponse.MiddleName = _selectedRecord.MiddleName;
            thirdPartyDtoResponse.LastName = _selectedRecord.LastName;
            thirdPartyDtoResponse.Initials = _selectedRecord.Initials;
            thirdPartyDtoResponse.Phone1 = _selectedRecord.Phone1;
            thirdPartyDtoResponse.Phone2 = _selectedRecord.Phone2;

            textIdentificationType = valueIdentification(_selectedRecord.IdentificationType);
            identificationType = _selectedRecord.IdentificationType;
            thirdPartyDtoRequest.IdentificationType = _selectedRecord.IdentificationType;

            textCharge = valueCharge(_selectedRecord.ChargeCode!);
            charge = _selectedRecord.ChargeCode!;
            thirdPartyDtoResponse.ChargeCode = _selectedRecord.ChargeCode;

            textNature = valueNature(_selectedRecord.NatureCode!);
            nature = _selectedRecord.NatureCode!;
            thirdPartyDtoResponse.NatureCode = _selectedRecord.NatureCode;
            IsEditForm = true;
        }

        #endregion ReceiveThirdParty

        #region ResetForm

        public void ResetFormAsync()
        {
            thirdPartyDtoRequest = new ThirdPartyCreateDtoRequest();
            thirdPartyUpdateDtoRequest = new ThirdPartyUpdateDtoRequest();
            thirdPartyDtoResponse = new ThirdPartyDtoResponse();
            addressDtoRequest = new AddressDtoRequest();
            addressId = 0;
            textAddress = "";
            IdThirdParty = 0;
            personType = "";
            identificationType = "";
            charge = "";
            nature = "";
            textIdentificationType = "Seleccione un tipo de identificación...";
            textCharge = "Seleccione un Cargo...";
            textNature = "Seleccione una Naturaleza...";
            activeState = true;
            StateHasChanged();
        }

        #endregion ResetForm

        #endregion Record

        #region Modal

        //metodo para abrir nueva modal dentro de otra modal
        private async Task OpenNewModal()
        {
            await OnStatusChanged.InvokeAsync(true);
            if (_selectedRecord != null && addressId != 0)
            {
                await OnIdSaved.InvokeAsync(_selectedRecord.AddressId);
            }
        }

        public void UpdateModalStatus(bool newValue)
        {
            ModalStatus = newValue;
            StateHasChanged();
        }

        public async Task selectPersonType(int persontype)
        {
            personType = (persontype == 0) ? "PN" : "PJ";
            lastNameActive = (persontype == 0);
            await GetIdentificationType();
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            ModalStatus = status;
            OnResetForm.InvokeAsync();
            ResetFormAsync();
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                ResetFormAsync();
                OnResetForm.InvokeAsync();
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion Modal

        #region FormMethods

        #region SelectedMethod

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

            StateHasChanged();
        }

        #endregion SelectedMethod

        #region FullName

        public static (string firstName, string middleName) SplitFirstAndMiddleName(string fullName)
        {
            string[]? nameParts = fullName.Trim().Split(' ');

            string? firstName = nameParts[0];
            string? middleName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            return (firstName, middleName);
        }

        #endregion FullName

        #region CreateThirdParty

        private async Task Create()
        {
            try
            {
                thirdPartyDtoRequest.PersonType = personType!;
                thirdPartyDtoRequest.Address = addressDtoRequest;

                //Asignar la compañia del usuario
                thirdPartyDtoRequest.CompanyId = 16;
                thirdPartyDtoRequest.IdentificationType = codeIdentification(identificationType!);
                thirdPartyDtoRequest.IdentificationNumber = identification!.InputValue;
                thirdPartyDtoRequest.ActiveState = activeState;
                thirdPartyDtoRequest.Names = names!.InputValue;
                thirdPartyDtoRequest.Login = "";
                thirdPartyDtoRequest.Password = "";

                // ThirData
                if (personType == "PJ")
                {
                    thirdPartyDtoRequest.Names = names.InputValue;
                    thirdPartyDtoRequest.FirstName = "";
                    thirdPartyDtoRequest.MiddleName = "";
                    thirdPartyDtoRequest.LastName = "";
                    thirdPartyDtoRequest.Initials = initials.InputValue;
                }
                else
                {
                    var firstAndMiddleNameValue = names.InputValue ?? string.Empty;
                    (string firstName, string middleName) = SplitFirstAndMiddleName(firstAndMiddleNameValue);
                    thirdPartyDtoRequest.FirstName = firstName;
                    thirdPartyDtoRequest.MiddleName = middleName;
                    thirdPartyDtoRequest.LastName = lastNames.InputValue;
                    thirdPartyDtoRequest.Names = $"{firstName} {middleName} {lastNames.InputValue}";
                    thirdPartyDtoRequest.Initials = "";
                }
                thirdPartyDtoRequest.Email1 = email1!.InputValue;
                thirdPartyDtoRequest.Email2 = email2!.InputValue == null ? "" : email2.InputValue;
                thirdPartyDtoRequest.WebPage = webpage!.InputValue;
                thirdPartyDtoRequest.ChargeCode = codeCharge(charge);
                thirdPartyDtoRequest.NatureCode = codeNature(nature);
                thirdPartyDtoRequest.Phone1 = phone1.InputValue;
                thirdPartyDtoRequest.Phone2 = phone2.InputValue == null ? "" : phone2.InputValue;
                thirdPartyDtoRequest.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/CreateThirdParty", thirdPartyDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ThirdPartyDtoResponse>>();

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

        #endregion CreateThirdParty

        #region UpdateThirdParty

        private async Task Update()
        {
            try {
                await GetAddressAsync(addressId);
                var addressDtoRequestOld = fillAddress(addressDtoResponse!);
                thirdPartyUpdateDtoRequest.Address = (addressDtoRequest == null || IsEmptyAddress(addressDtoRequest)) ? addressDtoRequestOld : addressDtoRequest;
                thirdPartyUpdateDtoRequest.IdentificationType = codeIdentification(identificationType!);
                thirdPartyUpdateDtoRequest.IdentificationNumber = identification!.InputValue;
                thirdPartyUpdateDtoRequest.Names = names!.InputValue;
                thirdPartyUpdateDtoRequest.Login = "";
                thirdPartyUpdateDtoRequest.Password = "";
                thirdPartyUpdateDtoRequest.ActiveState = activeState;
                // ThirData
                if (personType == "PJ")
                {
                    thirdPartyUpdateDtoRequest.Names = names.InputValue;
                    thirdPartyUpdateDtoRequest.FirstName = "";
                    thirdPartyUpdateDtoRequest.MiddleName = "";
                    thirdPartyUpdateDtoRequest.LastName = "";
                    thirdPartyUpdateDtoRequest.Initials = initials.InputValue;
                    thirdPartyUpdateDtoRequest.Email2 = email2!.InputValue;
                }
                else
                {
                    var firstAndMiddleNameValue = names.InputValue ?? string.Empty;
                    (string firstName, string middleName) = SplitFirstAndMiddleName(firstAndMiddleNameValue);
                    thirdPartyUpdateDtoRequest.FirstName = firstName;
                    thirdPartyUpdateDtoRequest.MiddleName = middleName;
                    thirdPartyUpdateDtoRequest.LastName = lastNames.InputValue;
                    thirdPartyUpdateDtoRequest.Names = $"{firstName} {middleName}";
                    thirdPartyUpdateDtoRequest.Initials = "";
                    thirdPartyUpdateDtoRequest.Email2 = "";
                }
                thirdPartyUpdateDtoRequest.Email1 = email1!.InputValue;
                thirdPartyUpdateDtoRequest.WebPage = webpage!.InputValue;
                thirdPartyUpdateDtoRequest.ChargeCode = codeCharge(charge);
                thirdPartyUpdateDtoRequest.NatureCode = codeNature(nature);
                thirdPartyUpdateDtoRequest.Phone1 = phone1.InputValue;
                thirdPartyUpdateDtoRequest.Phone2 = phone2.InputValue;
                thirdPartyUpdateDtoRequest.ThirdPartyId = IdThirdParty;
                thirdPartyUpdateDtoRequest.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/UpdateThirdParty", thirdPartyUpdateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ThirdPartyDtoResponse>>();
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

        #endregion UpdateThirdParty

        #region EmptyAddress

        public bool IsEmptyAddress(AddressDtoRequest address)
        {
            return address.CountryId == 0 &&
                   address.StateId == 0 &&
                   address.CityId == 0 &&
                   string.IsNullOrEmpty(address.StType) &&
                   string.IsNullOrEmpty(address.StNumber) &&
                   string.IsNullOrEmpty(address.StLetter) &&
                   !address.StBis &&
                   string.IsNullOrEmpty(address.StComplement) &&
                   string.IsNullOrEmpty(address.StCardinality) &&
                   string.IsNullOrEmpty(address.CrType) &&
                   string.IsNullOrEmpty(address.CrNumber) &&
                   string.IsNullOrEmpty(address.CrLetter) &&
                   !address.CrBis &&
                   string.IsNullOrEmpty(address.CrComplement) &&
                   string.IsNullOrEmpty(address.CrCardinality) &&
                   string.IsNullOrEmpty(address.HouseType) &&
                   string.IsNullOrEmpty(address.HouseClass) &&
                   string.IsNullOrEmpty(address.HouseNumber);
        }

        #endregion EmptyAddress

        #endregion FormMethods

        #endregion OtherMethods

        #endregion Methods
    }
}