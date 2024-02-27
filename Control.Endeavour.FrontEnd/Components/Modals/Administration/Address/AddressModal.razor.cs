using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Address
{
    public partial class AddressModal : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent stNumber = new();
        private InputModalComponent stLetter = new();
        private InputModalComponent stComplement = new();
        private InputModalComponent crNumber = new();
        private InputModalComponent crLetter = new();
        private InputModalComponent crComplement = new();
        private InputModalComponent houseNumber = new();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };

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
        public EventCallback<AddressDtoRequest> OnModalSaved { get; set; }

        [Parameter]
        public EventCallback<int> OnIdSaved { get; set; }

        [Parameter]
        public bool multipleSelection { get; set; }

        [Parameter]
        public EventCallback<MyEventArgs<List<(string, AddressDtoRequest)>>> OnStatusChangedMultipleSelection { get; set; }

        #endregion Parameters

        #region Models

        //Request and Response
        private AddressDtoRequest addressDtoRequest = new();

        private AddressDtoResponse _selectedRecord = new();
        private AddressDtoResponse addressDtoResponse = new();

        //Listas de DropDownLists
        private List<SystemFieldsDtoResponse> cardinalityList = new();

        private List<SystemFieldsDtoResponse> houseClassList = new();
        private List<SystemFieldsDtoResponse> houseTypeList = new();
        private List<SystemFieldsDtoResponse> scTypeList = new();
        private List<CountryDtoResponse> countryList = new();
        private List<StateDtoResponse> stateList = new();
        private List<CityDtoResponse> cityList = new();

        #endregion Models

        #region Environments

        private bool EnabledDepartamento { get; set; } = true;
        private bool EnabledMunicipio { get; set; } = true;

        //Direccion
        private string address = "";

        //Inputs
        private int addressId;

        private string stType = "";
        private bool stBis = false;
        private string stCardinality = "";
        private string crType = "";
        private bool crBis = false;
        private string crCardinality = "";
        private string houseType = "";
        private string houseClass = "";
        private int country;
        private int state;
        private int city;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            ModalStatus = false;
            await GetCardinality();
            await GetHouseClass();
            await GetHouseType();
            await GetSCType();
            await GetCountry();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region GetAddress

        private async Task GetAddressAsync()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");
                HttpClient?.DefaultRequestHeaders.Add("IdAddress", $"{addressId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<AddressDtoResponse>>("administration/Address/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");

                if (deserializeResponse!.Succeeded)
                {
                    addressDtoResponse = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la dirección: {ex.Message}");
            }
        }

        #endregion GetAddress

        #region GetsLocation

        #region GetCountry

        private async Task GetCountry()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilter");
                countryList = deserializeResponse!.Data != null ? deserializeResponse.Data : new List<CountryDtoResponse>();

                if (countryList.Count > 0)
                {
                    meta = deserializeResponse.Meta!;
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
                else
                {
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los Paises: {ex.Message}");
            }
        }

        #endregion GetCountry

        #region GetState

        private async Task GetState()
        {
            if (country > 0)
            {
                try
                {
                    EnabledDepartamento = true;
                    EnabledMunicipio = false;

                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    HttpClient?.DefaultRequestHeaders.Add("countryId", country.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");

                    stateList = deserializeResponse!.Data != null ? deserializeResponse.Data : new List<StateDtoResponse>();

                    if (stateList.Count > 0)
                    {
                        meta = deserializeResponse.Meta!;
                        state = 0;
                    }
                    else
                    {
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener los Departamentos: {ex.Message}");
                }
            }
            else
            {
                state = 0;
                city = 0;
                EnabledDepartamento = false;
                EnabledMunicipio = false;
            }
        }

        #endregion GetState

        #region GetCity

        private async Task GetCity()
        {
            if (state > 0)
            {
                try
                {
                    EnabledMunicipio = true;

                    HttpClient?.DefaultRequestHeaders.Remove("stateId");
                    HttpClient?.DefaultRequestHeaders.Add("stateId", state.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CityDtoResponse>>>("location/City/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");

                    cityList = deserializeResponse!.Data != null ? deserializeResponse.Data : new List<CityDtoResponse>();

                    if (cityList.Count > 0)
                    {
                        meta = deserializeResponse.Meta!;
                    }
                    else
                    {
                        EnabledMunicipio = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener los Municipios: {ex.Message}");
                }
            }
            else
            {
                city = 0;
                EnabledMunicipio = false;
            }
        }

        #endregion GetCity

        #endregion GetsLocation

        #region GetsParams

        #region GetCardinality

        private async Task GetCardinality()
        {
            try
            {
                SystemFieldsDtoRequest request = new() { ParamCode = "CARD" };

                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    cardinalityList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los puntos cardinales: {ex.Message}");
            }
        }

        #endregion GetCardinality

        #region GetHouseClass

        private async Task GetHouseClass()
        {
            try
            {
                SystemFieldsDtoRequest request = new() { ParamCode = "HC" };

                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    houseClassList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las clases de casas: {ex.Message}");
            }
        }

        #endregion GetHouseClass

        #region GetHouseType

        private async Task GetHouseType()
        {
            try
            {
                SystemFieldsDtoRequest request = new() { ParamCode = "HT" };

                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    houseTypeList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de casas: {ex.Message}");
            }
        }

        #endregion GetHouseType

        #region GetSCType

        private async Task GetSCType()
        {
            try
            {
                SystemFieldsDtoRequest request = new() { ParamCode = "SCT" };

                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    scTypeList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las calles y carreras: {ex.Message}");
            }
        }

        #endregion GetSCType

        #endregion GetsParams

        #region GetRecord

        #region GetRecordSelected

        public async Task UpdateModalIdAsync(int id)
        {
            addressId = id;
            await GetAddressAsync();
            await ReceiveRecordsAsync(addressDtoResponse!);
            StateHasChanged();
        }

        public void ResetForm()
        {
            ResetFormAsync();
            StateHasChanged();
        }

        #endregion GetRecordSelected

        #region ReceiveRecordsAsync

        private string textStCardinality = "Seleccione una Cardinalidad...";
        private string textCrCardinality = "Seleccione una Cardinalidad...";
        private string textStType = "Seleccione un tipo de Via...";
        private string textCrType = "Seleccione un tipo de Via...";
        private string textHouseType = "Seleccione un tipo de Casa...";
        private string textHouseClass = "Seleccione una clase de Casa...";

        private string textCountry = "Seleccione un Pais...";
        private string textState = "Seleccione un Departamento...";
        private string textCity = "Seleccione un Municipio...";

        public async Task ReceiveRecordsAsync(AddressDtoResponse response)
        {
            _selectedRecord = response;

            addressDtoRequest.StNumber = _selectedRecord.StNumber!;
            addressDtoRequest.StType = _selectedRecord.StType!;
            addressDtoRequest.StLetter = _selectedRecord.StLetter!;
            addressDtoRequest.StBis = (bool)_selectedRecord.StBis!;
            addressDtoRequest.StComplement = _selectedRecord.StComplement!;
            addressDtoRequest.CrNumber = _selectedRecord.CrNumber!;
            addressDtoRequest.CrType = _selectedRecord.CrType!;
            addressDtoRequest.CrLetter = _selectedRecord.CrLetter!;
            addressDtoRequest.CrBis = (bool)_selectedRecord.CrBis!;
            addressDtoRequest.CrComplement = _selectedRecord.CrComplement!;
            addressDtoRequest.HouseType = _selectedRecord.HouseType!;
            addressDtoRequest.HouseClass = _selectedRecord.HouseClass!;
            addressDtoRequest.HouseNumber = _selectedRecord.HouseNumber!;

            stBis = addressDtoRequest.StBis;
            crBis = addressDtoRequest.CrBis;

            // dropdown StCardinality
            textStCardinality = valueCardinality(_selectedRecord.StCardinality!);
            stCardinality = _selectedRecord.StCardinality!;

            // dropdown CrCardinality
            textCrCardinality = valueCardinality(_selectedRecord.CrCardinality!);
            crCardinality = _selectedRecord.CrCardinality!;

            //dropdown StType
            textStType = valueStType(_selectedRecord.StType!);
            stType = _selectedRecord.StType!;

            //dropdown CrType
            textCrType = valueCrType(_selectedRecord.CrType!);
            crType = _selectedRecord.CrType!;

            //dropdown HouseType
            textHouseType = valueHouseType(_selectedRecord.HouseType!);
            houseType = _selectedRecord.HouseType!;

            //dropdown HouseCLass
            textHouseClass = valueHouseClass(_selectedRecord.HouseClass!);
            houseClass = _selectedRecord.HouseClass!;

            //dropdown Country
            textCountry = nameCountry(_selectedRecord.CountryId!);
            addressDtoRequest.CountryId = _selectedRecord.CountryId!;
            country = _selectedRecord.CountryId;

            //dropdown State
            state = _selectedRecord.StateId;
            addressDtoRequest.StateId = _selectedRecord.StateId;
            textState = await nameState(_selectedRecord.StateId);

            //dropdown City
            city = _selectedRecord.CityId;
            addressDtoRequest.CityId = _selectedRecord.CityId;
            textCity = await nameCity(_selectedRecord.CityId);

            ActualizarDireccion();
            StateHasChanged();
        }

        #endregion ReceiveRecordsAsync

        #endregion GetRecord

        #region GetValuesParams

        //Metodo para tomar el Value de StType, CrType, HouseType y HouseClass
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
                string[] partes = selectedValue.Split(',');
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

        //Uso del Metodo de obtener valor, para cada Param
        private string valueCardinality(string selectedValue)
        {
            return GetValueFromList(cardinalityList, selectedValue, "CARD");
        }

        private string valueStType(string selectedValue)
        {
            return GetValueFromList(scTypeList, selectedValue, "SCT");
        }

        private string valueCrType(string selectedValue)
        {
            return GetValueFromList(scTypeList, selectedValue, "SCT");
        }

        private string valueHouseType(string selectedValue)
        {
            return GetValueFromList(houseTypeList, selectedValue, "HT");
        }

        private string valueHouseClass(string selectedValue)
        {
            return GetValueFromList(houseClassList, selectedValue, "HC");
        }

        #endregion GetValuesParams

        #region GetNamesLocation

        #region Country

        private string nameCountry(int selectedValue)
        {
            var selectedDataItem = countryList.Find(item => item.CountryId == selectedValue);
            var text = selectedDataItem!.Name;
            return text;
        }

        #endregion Country

        #region State

        private async Task<List<StateDtoResponse>> GetStateSelected(int country)
        {
            HttpClient?.DefaultRequestHeaders.Remove("countryId");
            HttpClient?.DefaultRequestHeaders.Add("countryId", country.ToString());
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilter");
            HttpClient?.DefaultRequestHeaders.Remove("countryId");

            if (deserializeResponse!.Succeeded)
            {
                stateList = deserializeResponse.Data!;
            }

            return stateList!;
        }

        private async Task<string> nameState(int selectedValue)
        {
            var stateList2 = await GetStateSelected(country);
            var selectedDataItem = stateList2.Find(item => item.StateId == selectedValue);
            var text = selectedDataItem!.Name;
            return text;
        }

        #endregion State

        #region City

        private async Task<List<CityDtoResponse>> GetCitySelected(int state)
        {
            HttpClient?.DefaultRequestHeaders.Remove("stateId");
            HttpClient?.DefaultRequestHeaders.Add("stateId", state.ToString());
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CityDtoResponse>>>("location/City/ByFilter");
            HttpClient?.DefaultRequestHeaders.Remove("stateId");

            if (deserializeResponse!.Succeeded)
            {
                cityList = deserializeResponse.Data!;
            }

            return cityList!;
        }

        private async Task<string> nameCity(int selectedValue)
        {
            var cityList2 = await GetCitySelected(state);
            var selectedDataItem = cityList2.Find(item => item.CityId == selectedValue);
            var text = selectedDataItem!.Name;
            return text;
        }

        #endregion City

        #endregion GetNamesLocation

        #region GetCodeParams

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

        private string codeStType(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "SCT");
        }

        private string codeCrType(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "SCT");
        }

        private string codeHouseType(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "HT");
        }

        private string codeHouseClass(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "HC");
        }

        private string codeStCardinality(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "CARD");
        }

        private string codeCrCardinality(string selectedValue)
        {
            return GetCodeFromList(selectedValue, "CARD");
        }

        #endregion GetCodeParams

        #region RefreshAddress

        private void ActualizarDireccion()
        {
            var stnumber = stNumber!.InputValue;
            var stletter = stLetter!.InputValue;
            var stcomplement = stComplement!.InputValue;
            var crnumber = crNumber!.InputValue;
            var crletter = crLetter!.InputValue;
            var crcomplement = crComplement!.InputValue;
            var housenumbert = houseNumber!.InputValue;

            var stcardinality = valueCardinality(stCardinality);
            var crcardinality = valueCardinality(crCardinality);
            var sct = valueStType(stType);
            var crt = valueCrType(crType);
            var hst = valueHouseType(houseType);
            var hsc = valueHouseClass(houseClass);

            address = $"{sct} {stnumber} {stletter} {(stBis ? "Bis" : "")} {stcomplement} {stcardinality} {crt} {crnumber} {crletter} {(crBis ? "Bis" : "")} {crcomplement} {crcardinality} {hst} {hsc} {housenumbert}";
            StateHasChanged();
        }

        #endregion RefreshAddress

        #region ResetForm

        public void ResetFormAsync()
        {
            addressDtoRequest = new AddressDtoRequest();
            address = "";
            stBis = false;
            crBis = false;
            textStCardinality = "Seleccione una Cardinalidad...";
            stCardinality = "";
            textCrCardinality = "Seleccione una Cardinalidad...";
            crCardinality = "";
            textStType = "Seleccione un tipo de Via...";
            stType = "";
            textCrType = "Seleccione un tipo de Via...";
            crType = "";
            textHouseType = "Seleccione un tipo de Casa...";
            houseType = "";
            textHouseClass = "Seleccione una clase de Casa...";
            houseClass = "";
            textCountry = "Seleccione un Pais...";
            country = 0;
            textState = "Seleccione un Departamento...";
            state = 0;
            textCity = "Seleccione un Departamento...";
            city = 0;
        }

        #endregion ResetForm

        #region FormMethods

        private List<(string, AddressDtoRequest)> Create()
        {
            var AddressDtoRequest = new AddressDtoRequest
            {
                CountryId = country,
                StateId = state,
                CityId = city,
                StType = codeStType(stType),
                StNumber = stNumber!.InputValue,
                StLetter = stLetter!.InputValue,
                StBis = stBis,
                StComplement = stComplement!.InputValue,
                StCardinality = codeStCardinality(stCardinality),
                CrType = codeCrType(crType),
                CrNumber = crNumber!.InputValue,
                CrLetter = crLetter!.InputValue,
                CrBis = crBis,
                CrComplement = crComplement!.InputValue,
                CrCardinality = codeCrCardinality(crCardinality),
                HouseType = codeHouseType(houseType),
                HouseClass = codeHouseClass(houseClass),
                HouseNumber = houseNumber!.InputValue
            };

            // Devuelve el mensaje y el objeto AddressDtoRequest
            List<(string, AddressDtoRequest)> result = new List<(string, AddressDtoRequest)>
            {
                (address, AddressDtoRequest),
            };

            return result;
        }

        private async Task HandleModalClosed(bool status)
        {
            var createResult = Create();

            var eventArgs = new MyEventArgs<List<(string, AddressDtoRequest)>>
            {
                Data = createResult,
                ModalStatus = status
            };
            await OnStatusChangedMultipleSelection.InvokeAsync(eventArgs);
        }

        private async Task HandleValidSubmit()
        {
            Create();
            StateHasChanged();
            await HandleModalClosed(false);
        }

        #endregion FormMethods

        #region ModalNotification

        public void UpdateModalStatus(bool newValue)
        {
            ModalStatus = newValue;
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion ModalNotification

        #endregion Methods
    }
}