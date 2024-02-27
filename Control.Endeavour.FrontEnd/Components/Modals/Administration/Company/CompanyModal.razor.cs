using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Company
{
    public partial class CompanyModal
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent namebussiness = new();
        private InputModalComponent NIT = new();
        private InputModalComponent phone = new();
        private InputModalComponent web = new();
        private InputModalComponent email = new();
        private InputModalComponent identification = new();
        private InputModalComponent cellphone = new();
        private InputModalComponent nameAgentLegal = new();

        #endregion Components

        #region Modals
        private NotificationsComponentModal notificationModal = new();
        #endregion Modals

        #region Parameters

        [Parameter]
        public bool ModalStatus { get; set; }

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<int> OnIdSaved { get; set; }

        [Parameter]
        public EventCallback OnResetForm { get; set; }

        #endregion Parameters

        #region Models

        //Pictures
        private FileCompanyDtoRequest logoFile = new();
        private FileCompanyDtoRequest bannerFile = new();

        //Company
        private CompanyCreateDtoRequest CompaniesFormCreate = new CompanyCreateDtoRequest();
        private CompanyUpdateDtoRequest CompaniesFormUpdate = new CompanyUpdateDtoRequest();
        private CompanyDtoResponse CompaniesFormResponse = new CompanyDtoResponse();
        private CompanyDtoResponse _selectedRecord = new();

        //Address
        private AddressDtoRequest AddrressformCompaniesRequest = new();
        private AddressDtoResponse AddrressformCompaniesResponse = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        //pictures
        private string logoPictureSrc { get; set; } = "";

        private string bannerPictureSrc { get; set; } = "";
        private string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png" };

        //Address
        private string textAddress = "";

        //DocumentType
        private string legalAgentIdType { get; set; } = "";

        private string identificationType { get; set; } = "";

        private string textTDIJ = "Seleccione un tipo de documento...";
        private string textTDIN = "Seleccione un tipo de documento...";

        #endregion Environments(String)

        #region Environments(Numeric)

        //pictures
        private int FileSize = 10;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool IsEditForm = false;
        private bool IsDisabledCode = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<SystemFieldsDtoResponse>? documentTypeTDIJ;
        private List<SystemFieldsDtoResponse>? documentTypeTDIN;

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            ModalStatus = false;
            await GetDocumentTypeTDIJ();
            await GetDocumentTypeTDIN();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OtherMethods

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

        #region Create

        private async Task Create()
        {
            try
            {
                CompaniesFormCreate.CompanyData = new();
                CompaniesFormCreate.CompanyData.Address = new();

                CompaniesFormCreate.CompanyData.Address = AddrressformCompaniesRequest;

                //Datos CompanyData
                CompaniesFormCreate.CompanyData.LegalAgentFullName = nameAgentLegal.InputValue;
                CompaniesFormCreate.CompanyData.CellPhoneNumber = cellphone.InputValue;
                CompaniesFormCreate.CompanyData.LegalAgentId = identification.InputValue;
                CompaniesFormCreate.CompanyData.PhoneNumber = phone.InputValue;
                CompaniesFormCreate.CompanyData.Email = email.InputValue;
                CompaniesFormCreate.CompanyData.WebAddress = web.InputValue;
                CompaniesFormCreate.CompanyData.Domain = "GoDaddy";
                CompaniesFormCreate.CompanyData.LegalAgentIdType = "TDIN," + legalAgentIdType;

                //Datos Companies
                CompaniesFormCreate.Identification = NIT.InputValue;
                CompaniesFormCreate.IdentificationType = "TDIJ," + identificationType;
                CompaniesFormCreate.BusinessName = namebussiness.InputValue;
                CompaniesFormCreate.User = "user";

                //Logo y Banner
                CompaniesFormCreate.CompanyData.LogoFile = logoFile.DataFile == null ? null : logoFile;
                CompaniesFormCreate.CompanyData.BannerFile = bannerFile.DataFile == null ? null : bannerFile;

                var responseApi = await HttpClient!.PostAsJsonAsync("companies/Company/CreateCompany", CompaniesFormCreate);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CompanyDtoResponse>>();
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

        #endregion Create

        #region Update

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

        public async Task Update()
        {
            try
            {
                CompaniesFormUpdate = new();
                CompaniesFormUpdate.CompanyData = new();
                await GetAddressAsync(CompaniesFormResponse);
                var addressDtoRequestOld = fillAddress(AddrressformCompaniesResponse);

                //CompanyData Response
                _selectedRecord.LegalAgentFullName = nameAgentLegal.InputValue;
                _selectedRecord.CellPhoneNumber = cellphone.InputValue;
                _selectedRecord.LegalAgentId = identification.InputValue;
                _selectedRecord.PhoneNumber = phone.InputValue;
                _selectedRecord.Email = email.InputValue;
                _selectedRecord.WebAddress = web.InputValue;
                _selectedRecord.LegalAgentIdType = legalAgentIdType;

                //Companies Response
                _selectedRecord.Identification = NIT.InputValue;
                _selectedRecord.IdentificationType = identificationType;
                _selectedRecord.BusinessName = namebussiness.InputValue;

                //Company Data Request

                CompaniesFormUpdate.CompanyData.LegalAgentFullName = _selectedRecord.LegalAgentFullName;
                CompaniesFormUpdate.CompanyData.CellPhoneNumber = _selectedRecord.CellPhoneNumber;
                CompaniesFormUpdate.CompanyData.LegalAgentId = _selectedRecord.LegalAgentId;
                CompaniesFormUpdate.CompanyData.PhoneNumber = _selectedRecord.PhoneNumber;
                CompaniesFormUpdate.CompanyData.Email = _selectedRecord.Email;
                CompaniesFormUpdate.CompanyData.WebAddress = _selectedRecord.WebAddress;
                CompaniesFormUpdate.CompanyData.Domain = _selectedRecord.Domain!;
                CompaniesFormUpdate.CompanyData.LegalAgentIdType = (legalAgentIdType == "" ? CompaniesFormResponse.LegalAgentIdType : $"TDIN,{legalAgentIdType}");

                CompaniesFormUpdate.CompanyData.Address = (AddrressformCompaniesRequest == null || IsEmptyAddress(AddrressformCompaniesRequest)) ? addressDtoRequestOld : AddrressformCompaniesRequest;

                //Companies Request
                CompaniesFormUpdate.Identification = _selectedRecord.Identification;
                CompaniesFormUpdate.IdentificationType = (identificationType == "" ? CompaniesFormResponse.IdentificationType : $"TDIJ,{identificationType}");
                CompaniesFormUpdate.BusinessName = _selectedRecord.BusinessName;

                FileCompanyDtoRequest? fileLogo = null;
                FileCompanyDtoRequest? fileBanner = null;
                if (CompaniesFormResponse.LogoFileId != null && logoFile.DataFile != null)
                {
                    fileLogo = new FileCompanyDtoRequest { FileExt = logoFile!.FileExt, FileName = logoFile.FileName, DataFile = logoFile.DataFile };
                }
                if (CompaniesFormResponse.BannerFileId != null && bannerFile.DataFile != null)
                {
                    fileBanner = new FileCompanyDtoRequest { FileExt = bannerFile!.FileExt, FileName = bannerFile.FileName, DataFile = bannerFile.DataFile };
                }

                CompaniesFormUpdate.CompanyData.LogoFile = logoFile.DataFile == null ? fileLogo : logoFile;
                CompaniesFormUpdate.CompanyData.BannerFile = bannerFile.DataFile == null ? fileBanner : bannerFile;
                CompaniesFormUpdate.Id = _selectedRecord.CompanyId;
                CompaniesFormUpdate.User = "user";

                var responseApi = await HttpClient!.PostAsJsonAsync("companies/Company/UpdateCompany", CompaniesFormUpdate);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CompanyDtoResponse>>();
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

        #endregion Update

        #endregion FormMethods

        #region GetDocumentType

        #region GetDocumentTypeTDIN

        private async Task GetDocumentTypeTDIN()
        {
            try
            {
                SystemFieldsDtoRequest request = new() { ParamCode = "TDIN" };

                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    documentTypeTDIN = deserializeResponse.Data!;
                }
                else
                {
                    documentTypeTDIN = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de identificación: {ex.Message}");
            }
        }

        #endregion GetDocumentTypeTDIN

        #region GetDocumentTypeTDIJ

        private async Task GetDocumentTypeTDIJ()
        {
            try
            {
                SystemFieldsDtoRequest request = new() { ParamCode = "TDIJ" };

                var responseApi = await HttpClient!.PostAsJsonAsync("params/SystemFields/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    documentTypeTDIJ = deserializeResponse.Data!;
                }
                else
                {
                    documentTypeTDIJ = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de identificación: {ex.Message}");
            }
        }

        #endregion GetDocumentTypeTDIJ

        #endregion GetDocumentType

        #region GetAddress

        private async Task GetAddressAsync(CompanyDtoResponse dato)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");
                HttpClient?.DefaultRequestHeaders.Add("IdAddress", $"{dato.AddressId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<AddressDtoResponse>>("administration/Address/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");

                if (deserializeResponse!.Succeeded)
                {
                    AddrressformCompaniesResponse = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la dirección: {ex.Message}");
            }
        }

        #endregion GetAddress

        #region GetPictures

        private async Task<FileDtoResponse?> GetPictures(int? id)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    return deserializeResponse.Data!;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return new FileDtoResponse();
            }
        }

        #endregion GetPictures

        #region HandlePictures

        private void HandleBannerPicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                var pictureBanner = data[0];
                bannerPictureSrc = $"data:image/{pictureBanner.Extension!.Split('.')[0]};base64,{Convert.ToBase64String(pictureBanner.Base64Data!)}";
                bannerFile = new FileCompanyDtoRequest();
                bannerFile.FileExt = pictureBanner.Extension.Replace(".", "");
                bannerFile.DataFile = pictureBanner.Base64Data!;
                bannerFile.FileName = pictureBanner.Name!;
            }
        }

        private void HandleLogoPicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                var pictureLogo = data[0];
                logoPictureSrc = $"data:image/{pictureLogo.Extension!.Split('.')[0]};base64,{Convert.ToBase64String(pictureLogo.Base64Data!)}";
                logoFile = new FileCompanyDtoRequest();
                logoFile.FileExt = pictureLogo.Extension.Replace(".", "");
                logoFile.DataFile = pictureLogo.Base64Data!;
                logoFile.FileName = pictureLogo.Name!;
            }
        }

        #endregion HandlePictures

        #region Record

        public async Task RecibirRegistro(CompanyDtoResponse response)
        {
            _selectedRecord = response;
            IsDisabledCode = true;
            IsEditForm = true;

            CompaniesFormResponse.CompanyId = _selectedRecord.CompanyId;
            CompaniesFormResponse.AddressId = _selectedRecord.AddressId;
            CompaniesFormResponse.Identification = _selectedRecord.Identification;
            //Request
            CompaniesFormResponse.AddressId = _selectedRecord.AddressId;
            CompaniesFormResponse.Address = _selectedRecord.Address;
            textAddress = _selectedRecord.Address;
            CompaniesFormResponse.LegalAgentFullName = _selectedRecord.LegalAgentFullName;
            CompaniesFormResponse.CellPhoneNumber = _selectedRecord.CellPhoneNumber;
            CompaniesFormResponse.LegalAgentId = _selectedRecord.LegalAgentId;
            CompaniesFormResponse.PhoneNumber = _selectedRecord.PhoneNumber;
            CompaniesFormResponse.Email = _selectedRecord.Email;
            CompaniesFormResponse.WebAddress = _selectedRecord.WebAddress;
            CompaniesFormResponse.Domain = _selectedRecord.Domain;
            CompaniesFormResponse.LegalAgentIdType = _selectedRecord.LegalAgentIdType;
            textTDIJ = _selectedRecord.IdentificationTypeName;
            textTDIN = _selectedRecord.LegalAgentIdTypeName!;

            //Companies Request
            CompaniesFormResponse.Identification = _selectedRecord.Identification;
            CompaniesFormResponse.IdentificationType = _selectedRecord.IdentificationType;
            CompaniesFormResponse.BusinessName = _selectedRecord.BusinessName;
            CompaniesFormResponse.LogoFileId = null;
            CompaniesFormResponse.BannerFileId = null;

            if (_selectedRecord.LogoFileId != null && _selectedRecord.LogoFileId != 0)
            {
                var pictureLogo = await GetPictures(_selectedRecord.LogoFileId);
                logoPictureSrc = $"data:image/{pictureLogo!.FileExt};base64,{pictureLogo.DataFile}";
                CompaniesFormResponse.LogoFileId = _selectedRecord.LogoFileId;
            }
            if (_selectedRecord.BannerFileId != null && _selectedRecord.BannerFileId != 0)
            {
                var pictureBanner = await GetPictures(_selectedRecord.BannerFileId);
                bannerPictureSrc = $"data:image/{pictureBanner!.FileExt};base64,{pictureBanner.DataFile}";
                CompaniesFormResponse.BannerFileId = _selectedRecord.BannerFileId;
            }

            identificationType = _selectedRecord.IdentificationType;
            legalAgentIdType = _selectedRecord.LegalAgentIdType!;

            StateHasChanged();
        }

        #endregion Record

        #region ModalAddressMethods

        #region UpdateAddress

        public void updateAddressSelection(List<(string, AddressDtoRequest)> address)
        {
            if (address != null && address.Count > 0)
            {
                (textAddress, AddrressformCompaniesRequest) = address[0];
            }
        }

        #endregion UpdateAddress

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

        #endregion ModalAddressMethods

        #region Modal

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;
            await OnResetForm.InvokeAsync();
            ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                ResetFormAsync();
                await OnResetForm.InvokeAsync();
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        #region OpenNewModal

        //metodo para abrir nueva modal dentro de otra modal
        private async Task OpenNewModal()
        {
            await OnStatusChanged.InvokeAsync(true);
            if (textAddress != "" && _selectedRecord != null)
            {
                await OnIdSaved.InvokeAsync(_selectedRecord.AddressId);
            }
        }

        #endregion OpenNewModal

        #endregion Modal

        #region ResetFormAsync

        public void ResetFormAsync()
        {
            CompaniesFormResponse = new CompanyDtoResponse();
            textTDIJ = "Seleccione un tipo de documento...";
            textTDIN = "Seleccione un tipo de documento...";
            AddrressformCompaniesRequest = new AddressDtoRequest();
            textAddress = "";
            logoPictureSrc = "";
            bannerPictureSrc = "";
        }

        #endregion ResetFormAsync

        #endregion OtherMethods

        #endregion Methods
    }
}