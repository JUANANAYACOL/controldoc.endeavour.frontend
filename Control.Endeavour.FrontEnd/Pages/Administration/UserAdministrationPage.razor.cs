using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.UsersAdministration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class UserAdministrationPage
    {

		#region Variables

		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }
        #endregion

        #region Components

        PaginationComponent<VUserDtoResponse, VUserDtoRequest> paginationComponetPost = new();

        #endregion

        #region Modals
        
        private NotificationsComponentModal notificationModal = new();
        private UserProfilesModal modalUserProfiles = new();
        private UserPermissionModal modalUserPermission = new();

        #endregion

        #region Parameters


        #endregion

        #region Models
        private MetaModel userMeta = new();
        private VUserDtoRequest vUserFilterDtoRequest = new();
        private CreateUsersDtoRequest _createUsersDtoRequest  = new();
        private VUserDtoResponse recordToDelete = new();


        #endregion

        #region Environments

        #region Environments(String)

        private string? userNameFilter;
        private string? userLastNameFilter;
        private string? userLoginFilter;
        private string Panel1Class = "";
        private string Panel2Class = "d-none";
        private string ProfileName = "";
        private string profilePictureSrc  = string.Empty;
        private string idUserSelected = string.Empty;
        private string starContractDate = string.Empty;
        private string endContractDate = string.Empty;
        private string birthDate = string.Empty;


        #region Variables Tabs Signatures
        private string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png" };
        
        private string AlertMessage = string.Empty;
        private string SourceMechanicalSignature = string.Empty;
        private string SourceSignatureHeading = string.Empty;
        private string SourceDigitalSignature = string.Empty;
        #endregion

        #region Variables Additional Signature Tabs

        private string SourceMechanicalSignatureAdditional1 = string.Empty;
        private string SourceMechanicalSignatureAdditional2 = string.Empty;
        private string SourceMechanicalSignatureAdditional3 = string.Empty;

        #endregion

        #region Default Text Combos

        private string ContracTypeText = "Tipo de contrato";
        private string CharguesTypeText = "Seleccione un cargo";
        private string BranchOfficesText = "Sucursales";
        private string ProductionOfficesText = "Oficinas productoras";
        private string AdministrativeUnitText = "Unidades Administrativas";
        private string IdentificationText = "Seleccione un tipo de documento...";

        #endregion
        #endregion

        #region Environments(Numeric)
        private int FileSize = 10;
        private int PageSizeProfile = 10;
        private int ValueTipoDocumento2 = 0;
        private int companyId = 0;
        private int documentalVersionId = 1;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)


        private bool isEnableProOfficeDrop = false;
        private bool editFormUser = false;

        #endregion

        #region Environments(List & Dictionary)

        #region GetDataList
        private List<VUserDtoResponse> userListData = new();
        private List<UsersSignatureDtoRequest> usersSignatureList = new();
        private List<VSystemParamDtoResponse> lstDocumentType  = new();
        private List<VSystemParamDtoResponse> lstCharguesTypes  = new();
        private List<AdministrativeUnitsDtoResponse> lstAdministrativeUnit  = new();
        private List<ProductionOfficesDtoResponse> lstProductionOffices  = new();
        private List<BranchOfficesDtoResponse> lstBranchOffices  = new();
        private List<VSystemParamDtoResponse> lstContracTypes  = new();
        private List<ProfilesDtoResponse> lstProfilesByUserId  = new();
        private List<ProfilesDtoResponse> lstProfilesByCompanyID  = new();
        private List<CreatePermissionDtoRequest> lstPermissionByUserId  = new();
        private List<ProfilesDtoResponse> selectedProfiles  = new();
        private List<int> selectedProfilesId  = new();
        private List<CreatePermissionDtoRequest> lstUserPermissionSelected = new();
        private List<CreatePermissionDtoRequest> lstUserPermissionRequest = new();
        private List<UsersSignatureDtoRequest> lstSignatures = new();
        #endregion

        #region SignaturesList

        private List<FileInfoData> lstSignaturesMC = new();
        private List<FileInfoData> lstSignaturesRB = new();
        private List<FileInfoData> lstSignaturesDP12 = new();
        private List<FileInfoData> lstSignaturesAMC1 = new();
        private List<FileInfoData> lstSignaturesAMC2 = new();
        private List<FileInfoData> lstSignaturesAMC3 = new();

        #endregion

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {

            try
            {
                await GetUsersData();
                await GetDocumentPersonType();
                await GetContractType();
                await GetAdministrativeUnits();
                await GetBranchsOffices();
                await GetChargueType();
                await GetDefaultProfilePicture();
                AlertMessage = GetUploadMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            //PageLoadService.OcultarSpinnerReadLoad(Js);
        }


        #endregion

        #region Methods

        #region HandleMethods

        #region Form Methods
        private async Task HandleValidSubmit()
        {

            if (!editFormUser)
            {
                await HandleFormCreate();
            }


            StateHasChanged();

        }

        private async Task HandleFormCreate()
        {
            try
            {
                _createUsersDtoRequest.Signatures = lstSignatures;
                _createUsersDtoRequest.CompanyId = 17;
                _createUsersDtoRequest.ContractStartDate = DateTime.Parse(starContractDate);
                _createUsersDtoRequest.ContractFinishDate = DateTime.Parse(endContractDate);
                _createUsersDtoRequest.BirthDate = DateTime.Parse(birthDate);
                _createUsersDtoRequest.ContractType = _createUsersDtoRequest.ContractType;
                _createUsersDtoRequest.ChargeCode = _createUsersDtoRequest.ChargeCode;
                _createUsersDtoRequest.IdentificationType =  _createUsersDtoRequest.IdentificationType;
                _createUsersDtoRequest.CreateUser = "Front"; //poner variable de usuario
                var responseApi = await HttpClient.PostAsJsonAsync("security/User/CreateUser", _createUsersDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VUserDtoResponse>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se creó el usuario de forma exitosa!", true, "Aceptar");
                    await CloseCreateView();
                    await GetUsersData();
                }
                else
                {
                    
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el usuario, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear: {ex.Message}");
            }
        }


        #endregion

        #region Signatures Methods
        private async Task HandleFileFMC(List<FileInfoData> data)
        {

            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                SourceMechanicalSignature = data[0].PathView;

                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.Archivo = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica ";
                lstSignatures.Add(signatureObject);

            }
            else
            {
                SourceMechanicalSignature = string.Empty;
            }
        }

        private async Task HandleFileFRB(List<FileInfoData> data)
        {
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                SourceSignatureHeading = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.Archivo = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIRU";
                signatureObject.SignatureName = "Firma rubrica";
                lstSignatures.Add(signatureObject);

            }
            else
            {
                SourceSignatureHeading = string.Empty;
            }
        }



        private async Task HandleFileFDP12(List<FileInfoData> data)
        {

            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                SourceDigitalSignature = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.Archivo = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,DIGI";
                signatureObject.SignatureName = "Firma digital";
                lstSignatures.Add(signatureObject);
            }
            else
            {
                SourceDigitalSignature = string.Empty;
            }
        }
        #endregion

        #region Additional Signatures Methods 
        private async Task MechanicalSignatureAdditional1(List<FileInfoData> data)
        {
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                SourceMechanicalSignatureAdditional1 = data[0].PathView;

                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.Archivo = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica Adicional 1";
                lstSignatures.Add(signatureObject);
            }
            else
            {
                SourceMechanicalSignatureAdditional1 = string.Empty;
            }
        }

        private async Task MechanicalSignatureAdditional2(List<FileInfoData> data)
        {
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                SourceMechanicalSignatureAdditional2 = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.Archivo = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica Adicional 2";
                lstSignatures.Add(signatureObject);
            }
            else
            {
                SourceMechanicalSignatureAdditional2 = string.Empty;
            }
        }

        private async Task MechanicalSignatureAdditional3(List<FileInfoData> data)
        {
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                SourceMechanicalSignatureAdditional3 = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.Archivo = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica Adicional 3";
                lstSignatures.Add(signatureObject);
            }
            else
            {
                SourceMechanicalSignatureAdditional3 = string.Empty;
            }
        }
        #endregion

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region Get Data Methods

        private async Task GetUsersData()
        {
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("generalviews/VUser/ByFilter", vUserFilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    userListData = deserializeResponse.Data;
                    userMeta = deserializeResponse.Meta;
                    paginationComponetPost.ResetPagination(userMeta);
                }
                else
                {
                    userListData = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar los usarios, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task GetContractType()
        {
            try
            {

                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TDC");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstContracTypes = deserializeResponse.Data;
                }
                else { lstContracTypes = new(); }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task GetChargueType()
        {
            try
            {

                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CAR");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstCharguesTypes = deserializeResponse.Data;
                }
                else { lstCharguesTypes = new(); }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task GetDocumentPersonType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TDIN");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstDocumentType = deserializeResponse.Data;
                }
                else { lstDocumentType = new(); }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        public async Task GetAdministrativeUnits()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", documentalVersionId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstAdministrativeUnit = deserializeResponse.Data;
                    
                }
                else
                {
                    lstAdministrativeUnit = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar información, por favor intente de nuevo!", true);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Unidades Administrativas: {ex.Message}");
            }
        }

        public async Task GetProducOffice(int id)
        {
            try
            {
                ValueTipoDocumento2 = id;
                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", id.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstProductionOffices = deserializeResponse.Data;
                    isEnableProOfficeDrop = true;
                }
                else
                {
                    lstProductionOffices = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar información, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
        }

        private async Task GetBranchsOffices()
        {
            try
            {
                BranchOfficeFilterDtoRequest? FilterDtoRequest  = new();
                var responseApi = await HttpClient.PostAsJsonAsync("params/BranchOffice/ByFilter", FilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<BranchOfficesDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstBranchOffices = deserializeResponse.Data;
                    
                    
                }
                else
                {
                    lstBranchOffices = new List<BranchOfficesDtoResponse>();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las oficinas productoras, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de fallo al obtener las sucursales.
                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
        }

        private async Task GetProfileByUserId(int userId)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ProUserid");
                HttpClient?.DefaultRequestHeaders.Add("ProUserid", userId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProfilesDtoResponse>>>("permission/Profile/ByFilterUserId");
                HttpClient?.DefaultRequestHeaders.Remove("ProUserid");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstProfilesByUserId = deserializeResponse.Data;
                }
                else
                {
                    lstProfilesByUserId = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar información, por favor intente de nuevo!", true);
                }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
        }

        private async Task GetProfileByCompany(int companyID)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("companyId");
                HttpClient?.DefaultRequestHeaders.Add("companyId", companyID.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProfilesDtoResponse>>>("permission/Profile/ByFilter");
                HttpClient?.DefaultRequestHeaders.Remove("companyId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstProfilesByCompanyID = deserializeResponse.Data;
                }
                else
                {
                    lstProfilesByCompanyID = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar información, por favor intente de nuevo!", true);
                }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
        }

        private async Task GetPermissionsByUserId(int userProfileID)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("PerUserid");
                HttpClient?.DefaultRequestHeaders.Add("PerUserid", userProfileID.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CreatePermissionDtoRequest>>>("permission/Permission/ByFilterUserId");
                HttpClient?.DefaultRequestHeaders.Remove("PerUserid");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstPermissionByUserId = deserializeResponse.Data;
                }
                else
                {
                    lstPermissionByUserId = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar información, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
        }

        private async Task LoadProfileDatabyUser(int userId)
        {
            try
            {
                await GetProfileByUserId(userId);
                await GetPermissionsByUserId(userId);
            }
            catch (Exception ex)
            {
                
                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
        }

        private async Task GetDefaultProfilePicture()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", "6791");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if(deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    profilePictureSrc = $"data:image/{deserializeResponse.Data.FileExt.ToLowerInvariant()};base64,{deserializeResponse.Data.DataFile}";     
                }
                else
                {
                   
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar información, por favor intente de nuevo!", true);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
        }

        private async Task<FileDtoResponse> GetFileInfoObject(int file)
        {
            FileDtoResponse fileDtoResponse = new();
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", file.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    fileDtoResponse = deserializeResponse.Data;
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar información, por favor intente de nuevo!", true);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "!Ha ocurrido un Error¡", true);
            }
            return fileDtoResponse;
        }


        private async Task GetSignaturesByUserId(int userId)
        {
            try
            {
                UserSignatureFilterDtoRequest userSignatureFilter = new();
                userSignatureFilter.UserId = userId;
                var responseApi = await HttpClient.PostAsJsonAsync("security/Signature/ByFilterSignatures", userSignatureFilter);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<UsersSignatureDtoRequest>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    usersSignatureList = deserializeResponse.Data;
                }
                else
                {
                    usersSignatureList = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las firmas, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }




        #endregion

        #region General Methods

        private void DeletePermission(CreatePermissionDtoRequest record)
        {
            

            switch (editFormUser)
            {
                case true:
                    // Aquí puedes poner el código que se ejecutará cuando editFormUser sea true
                    
                    //lstUserPermissionRequest.Remove()
                    break;
                case false:
                    lstUserPermissionSelected.Remove(record);
                    // Aquí puedes poner el código que se ejecutará cuando editFormUser sea false
                    break;

            }

            
        }

        public string GetUploadMessage()
        {
            string extensions = string.Join(" ó ", AllowedExtensions).Replace(".", "").ToUpper();
            return $"Solo se permite subir archivo {extensions}. Máx {FileSize}Mb";
        }
        private async Task CleanFilter()
        {
            userNameFilter = string.Empty;
            userLoginFilter = string.Empty;
            userLastNameFilter = string.Empty;
            vUserFilterDtoRequest = new();
            await GetUsersData();
        }
        private async Task SearchButtom()
        {
            try
            {
                if (!string.IsNullOrEmpty(userNameFilter) || !string.IsNullOrEmpty(userLastNameFilter) || !string.IsNullOrEmpty(userLoginFilter))
                {
                    vUserFilterDtoRequest.Username = userNameFilter;
                    vUserFilterDtoRequest.LastName = userLastNameFilter;
                    vUserFilterDtoRequest.Username = userLoginFilter;
                    
                    
                    await GetUsersData();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, "¡Debe ingresar al menos un criterio de búsqueda!", true, "Aceptar");
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task AddNewProfiles()
        {
            if (companyId == 0)
            {
                companyId = 17;
            }
            if (editFormUser)
            {
                await GetProfileByCompany(companyId);
            }
            else
            {
                modalUserProfiles.UpdateModalStatus(true);
            }

        }

        private async Task AddNewPermission()
        {
            modalUserPermission.UpdateModalStatus(true);

        }

        

        private async Task HandleUserProfilePicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                profilePictureSrc = data[0].PathView;
                _createUsersDtoRequest.BirthDate = DateTime.Now;
                _createUsersDtoRequest.FileExt = data[0].Extension.Replace(".", "");
                _createUsersDtoRequest.FileName = data[0].Name;
                _createUsersDtoRequest.Archivo = data[0].Base64Data;
            }

        }

        private void ResetSignature()
        {
            lstSignaturesAMC1 = new();
            lstSignaturesAMC2 = new();
            lstSignaturesAMC3 = new();
            lstSignaturesMC = new();
            lstSignaturesRB = new();
            lstSignaturesDP12 = new();
            SourceMechanicalSignature = string.Empty;
            SourceSignatureHeading = string.Empty;
            SourceDigitalSignature = string.Empty;
            SourceMechanicalSignatureAdditional1 = string.Empty;
            SourceMechanicalSignatureAdditional2 = string.Empty;
            SourceMechanicalSignatureAdditional3 = string.Empty;


        }
        #endregion

        #region ModalMethods
        private async Task CloseCreateView()
        {
            editFormUser = false;
            Panel1Class = "";
            Panel2Class = "d-none";
            lstPermissionByUserId = new();
            lstProfilesByUserId = new();
            lstUserPermissionSelected = new();
            lstProfilesByUserId.Clear();
            companyId = 0;
            starContractDate = string.Empty;
            endContractDate = string.Empty;
            birthDate = string.Empty;


        ResetSignature();



        }
        private async Task OpenCreateView()
        {
            editFormUser = false;
            idUserSelected = string.Empty;
            _createUsersDtoRequest = new CreateUsersDtoRequest();
            Panel1Class = "d-none";
            Panel2Class = "";

        }
        private async Task OpenEditView(VUserDtoResponse record)
        {
            editFormUser = true;
            idUserSelected = record.UserId.ToString();
            await LoadProfileDatabyUser(record.UserId);

            _createUsersDtoRequest.IdentificationType = record.IdentificationType;
            //IdentificationText = lstDocumentType.Where(s => s.code.Equals(record.IdentificationType)).Select(x => x.value).First();
            _createUsersDtoRequest.Identification = record.Identification;
            _createUsersDtoRequest.UserName = record.UserName;
            _createUsersDtoRequest.FirstName = record.FirstName;
            _createUsersDtoRequest.MiddleName = record.MiddleName;
            _createUsersDtoRequest.LastName = record.LastName;
            _createUsersDtoRequest.Email = record.Email;
            ValueTipoDocumento2 = record.AdministrativeUnitId;
            if (record.ProductionOfficeId.HasValue)
            {
                _createUsersDtoRequest.ProductionOfficeId = (int)record.ProductionOfficeId;
            }
            if (record.BranchOfficeId.HasValue)
            {
                _createUsersDtoRequest.BranchOfficeId = (int)record.BranchOfficeId;
            }
            _createUsersDtoRequest.ChargeCode = record.ChargeCode;
            _createUsersDtoRequest.ContractType = record.ContractType;
            _createUsersDtoRequest.ContractNumber = record.ContractNumber;
            Panel1Class = "d-none";
            Panel2Class = "";
            
        }
        private void ShowModalDelete(VUserDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el usuario?", true, "Si", "No", modalOrigin: "DeleteModal");
        }
        
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.UserId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("security/User/DeleteUser", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetUsersData();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el usuario de forma exitosa!", true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        private void HandlePaginationGrid(List<VUserDtoResponse> newDataList)
        {
            userListData = newDataList;
        }

        private async Task OnProfilesSelected(IEnumerable<ProfilesDtoResponse> list)
        {
            selectedProfiles = list.ToList();
            selectedProfilesId.Clear();

            foreach (var profile in selectedProfiles)
            {
                selectedProfilesId.Add(profile.ProfileId);
            }

            _createUsersDtoRequest.Profiles = selectedProfilesId;
        }

        private async Task OnRecivePermissionSelected(CreatePermissionDtoRequest record)
        {
            
            lstUserPermissionSelected.Add(record);
            //CreatePermissionDtoRequest recordRequest = new CreatePermissionDtoRequest();
            //recordRequest.FunctionalityId = record.FunctionalityId;
            //recordRequest.AccessF = record.AccessF;
            //recordRequest.CreateF = record.CreateF;
            //recordRequest.ModifyF = record.ModifyF;
            //recordRequest.ConsultF = record.ConsultF;
            //recordRequest.DeleteF = record.DeleteF;
            //recordRequest.PrintF = record.PrintF;
            //recordRequest.ActiveState = record.ActiveState;
            //lstUserPermissionRequest.Add(recordRequest);
            _createUsersDtoRequest.Permissions = lstUserPermissionSelected;



        }

        #endregion

        #endregion

        #endregion

    }
}
