using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Layouts.NotFound
{
    public partial class NotFoundLayout
    {


        #region Variables

        #region Inject 
        [Inject] private NavigationManager NavigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals


        #endregion

        #region Parameters


        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)

        #endregion

        #endregion

        #endregion



        #region Methods


        #region RedirectPage

        public async Task RedirectPage()
		{
            var authState = await AuthenticationState;
            if (!authState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                NavigationManager.NavigateTo("/Home");

            }
		}

		#endregion

		#endregion
			

	}
}
