using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Audit.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Audit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class AuditPage
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Models

        private List<VWLogsAuditDtoBugResponse> vWLogsAuditDtoBugList { get; set; } = new();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        private LogByFilterDtoRequest auditFilterDtoRequest { get; set; } = new();

        #endregion Models

        #region Components

        private InputModalComponent detailInput { get; set; } = new();

        #endregion Components

        #region Enviroment

        private bool dataChargue { get; set; } = false;

        #endregion Enviroment

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetAudit();
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

        #region GetAudit

        private async Task GetAudit()
        {
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("audit/Log/ByFilter", auditFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VWLogsAuditDtoBugResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    vWLogsAuditDtoBugList = deserializeResponse.Data ?? new();

                    meta = deserializeResponse.Meta ?? new() { PageSize = 10 };
                    dataChargue = true;
                }
                else
                {
                    vWLogsAuditDtoBugList = new();
                    meta = new() { PageSize = 10 };
                    dataChargue = false;
                }
            }
            catch
            {
                vWLogsAuditDtoBugList = new();
            }
        }

        #endregion GetAudit

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<VWLogsAuditDtoBugResponse> newDataList)
        {
            vWLogsAuditDtoBugList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region OnClickSearch

        private async Task OnClickSearch()
        {
            await GetAudit();
        }

        #endregion OnClickSearch

        #region OnClickReset

        private void OnClickReset()
        {
            auditFilterDtoRequest = new();

            detailInput = new();
            StateHasChanged();
        }

        #endregion OnClickReset

        #endregion Methods
    }
}