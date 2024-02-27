﻿using Control.Endeavour.FrontEnd.Models.Enums.Documents;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Control.Endeavour.FrontEnd.StateContainer.ManagementTray;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Dashboard
{
    public partial class DashboardPage
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private DocumentsStateContainer? documentsStateContainer { get; set; }

        [Inject]
        private ManagementTrayStateContainer? managementTrayStateContainer { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }

        #endregion

        #region Components

        #endregion

        #region Modals


        #endregion

        #region Parameters


        #endregion

        #region Models

        private DataCardDtoResponse DataCards = new DataCardDtoResponse();
        private CompanyDtoResponse Companies = new CompanyDtoResponse();
        private DataCardDocTaskDtoResponse dataCardsDocTask = new DataCardDocTaskDtoResponse();

        #endregion

        #region Environments

        #region Environments(String)

        private string Gex = "";
        private string Enp = "";
        private string Etr = "";
        private string Cop = "";

        private string GexP = "";
        private string EnpP = "";
        private string EtrP = "";
        private string CopP = "";

        private string created = "";
        private string review = "";
        private string approve = "";
        private string toSign = "";
        private string signed = "";
        private string involved = "";

        private string codeRV = "TAINS,RV";
        private string codeAP = "TAINS,AP";
        private string codeFR = "TAINS,FR";
        private string codePR = "TAINS,PR";
        private string id1 = "UserTaskId";
        private string id2 = "UserForwardId";
        private string codeP = "ProcessCode";
        private string codeI = "InstructionCode";


        #endregion

        #region Environments(Numeric)

        private int userId = 4055;
        private int companyId = 17;

        #endregion

        #region Environments(DateTime)

        public DateTime? StartValue { get; set; } = DateTime.Now;
        public DateTime? EndValue { get; set; } = DateTime.Now.AddDays(10);
        public DateTime Min = new DateTime(1990, 1, 1, 8, 15, 0);
        public DateTime Max = new DateTime(3000, 1, 1, 19, 30, 45);

        #endregion

        #region Environments(Bool)

        private bool click = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<DataCardDtoRequest> Data = new List<DataCardDtoRequest>();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetCompayInfo();
            await GetDataCardsMT();
            await GetDataCardsDTT();
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private async Task HandleDocumentaryTaskSubmit(string code, string userId, List<string> codes, List<bool> value)
        {
            try
            {
                documentsStateContainer.Parametros(code, userId, codes, value);
                navigation.NavigateTo("/DocumentaryTaskTray");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al acceder a la otra ruta: {ex.Message}");
            }
        }

        private async Task HandleManagementTraySubmit(DocumentStatusEnum status)
        {
            try
            {
                managementTrayStateContainer.Parametros(status);
                navigation.NavigateTo("/ManagementTray");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al acceder a la otra ruta: {ex.Message}");
            }
        }

        #endregion

        #region GetDataCardsDocumentTask

        private async Task GetDataCardsDTT()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("UserId");
                HttpClient?.DefaultRequestHeaders.Add("UserId", $"{userId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DataCardDocTaskDtoResponse>>("documentarytasks/DocumentaryTask/GetCountTask");
                HttpClient?.DefaultRequestHeaders.Remove("UserId");
                dataCardsDocTask = deserializeResponse.Data;

                if (dataCardsDocTask != null)
                {
                    created = dataCardsDocTask.Created.ToString();
                    review = dataCardsDocTask.Review.ToString();
                    approve = dataCardsDocTask.Approve.ToString();
                    toSign = dataCardsDocTask.ToSign.ToString();
                    signed = dataCardsDocTask.Signed.ToString();
                    involved = dataCardsDocTask.Involved.ToString();
                }
                else
                {
                    created = "0";
                    review = "0";
                    approve = "0";
                    toSign = "0";
                    signed = "0";
                    involved = "0";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener N° de tareas documentales: {ex.Message}");
            }
        }

        #endregion

        #region GetDataCardsDocumentFiling

        private async Task GetDataCardsMT()
        {
            try
            {

                HttpClient?.DefaultRequestHeaders.Remove("AssingUserId");
                HttpClient?.DefaultRequestHeaders.Add("AssingUserId", $"{userId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DataCardDtoResponse>>("documentmanagement/Document/ByAssingUserId");
                HttpClient?.DefaultRequestHeaders.Remove("AssingUserId");
                DataCards = deserializeResponse.Data;

                if (DataCards != null)
                {
                    #region percentage
                    int total = DataCards.withoutProcessing + DataCards.inProgress + DataCards.successfulManagement + DataCards.copies;
                    double porcent = DataCards.withoutProcessing != 0 ? (Convert.ToDouble(DataCards.withoutProcessing) * 100) / total : 0;
                    double porcent2 = DataCards.inProgress != 0 ? (Convert.ToDouble(DataCards.inProgress) * 100) / total : 0;
                    double porcent3 = DataCards.successfulManagement != 0 ? (Convert.ToDouble(DataCards.successfulManagement) * 100) / total : 0;
                    double porcent4 = DataCards.copies != 0 ? (Convert.ToDouble(DataCards.copies) * 100) / total : 0;

                    EnpP = porcent != 0 ? porcent.ToString("N2") + "%" : "0";
                    EtrP = porcent2 != 0 ? porcent2.ToString("N2") + "%" : "0";
                    GexP = porcent3 != 0 ? porcent3.ToString("N2") + "%" : "0";
                    CopP = porcent4 != 0 ? porcent4.ToString("N2") + "%" : "0";

                    if (porcent == 0 && porcent2 == 0 && porcent3 == 0 && porcent4 == 0)
                    {
                        porcent = 33;
                        porcent2 = 33;
                        porcent3 = 33;
                        porcent4 = 33;
                    }
                    #endregion

                    #region DataCard
                    DataCardDtoRequest dato1 = new DataCardDtoRequest()
                    {
                        Category = DataCards.InProgressWord,
                        Value = porcent2,
                        color = "#EAD519"
                    };

                    DataCardDtoRequest dato2 = new DataCardDtoRequest()
                    {
                        Category = DataCards.WithoutProcessingWord,
                        Value = porcent,
                        color = "#AB2222"
                    };

                    DataCardDtoRequest dato3 = new DataCardDtoRequest()
                    {
                        Category = DataCards.SuccessfullManagementWord,
                        Value = porcent3,
                        color = "#82A738"
                    };

                    DataCardDtoRequest dato4 = new DataCardDtoRequest()
                    {
                        Category = DataCards.copiesWord,
                        Value = porcent4,
                        color = "#41BAEA"
                    };

                    Data.Add(dato1);
                    Data.Add(dato2);
                    Data.Add(dato3);
                    Data.Add(dato4);
                    #endregion

                    Enp = DataCards.withoutProcessing.ToString();
                    Etr = DataCards.inProgress.ToString();
                    Gex = DataCards.successfulManagement.ToString();
                    Cop = DataCards.copies.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        #endregion

        #region GetCompanyInfo

        private async Task GetCompayInfo()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("id");
                HttpClient?.DefaultRequestHeaders.Add("id", $"{companyId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<CompanyDtoResponse>>("companies/Company/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("id");
                Companies = deserializeResponse.Data;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener informacion de las compañias: {ex.Message}");
            }
        }

        #endregion

        #region GetDate

        private async Task ChangeDate()
        {
            if (click)
            {
                click = false;
            }
            else { click = true; }
        }

        #endregion

        #endregion

    }
}
