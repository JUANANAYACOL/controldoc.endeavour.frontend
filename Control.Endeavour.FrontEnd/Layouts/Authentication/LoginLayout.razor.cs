using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Control.Endeavour.FrontEnd.Layouts.Authentication
{
    public partial class LoginLayout
    {
        #region Variables

        #region Variables(String)

        private string companyIcon = "";
        private string navbarCollapseCss => isNavbarCollapsed ? "collapse navbar-collapse" : "navbar-collapse";
        private string DropdownMenuCss => isDropdownOpen ? "dropdown-menu show" : "dropdown-menu";

        private string version { get; set; } = string.Empty;


        #endregion

        #region Variables(Bool)

        private bool isNavbarCollapsed = true;
        private bool isDropdownOpen = false;
        private bool darkMode = false;

        #endregion

        #region Inject 

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private IConfiguration? Configuration { get; set; }


        #endregion

        #endregion

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
          
            await Js.InvokeVoidAsync("checkTheme"); // Verificar Theme almacenado en el localStorage      
            version =  Configuration["version"].ToString();
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region TogglesMethods
        void ToggleNavbar()
        {
            isNavbarCollapsed = !isNavbarCollapsed;
        }
        void ToggleDropdown()
        {
            isDropdownOpen = !isDropdownOpen;
        }
        void ClosedDropdown()
        {
            isDropdownOpen = false;
        }
        async Task ToggleTheme()
        {
            darkMode = await Js.InvokeAsync<bool>("toggleTheme");
            changeImage(darkMode);
        }
        #endregion

        #region OthersMethods
        private void changeImage(bool darkMode)
        {
            if (darkMode)
            {
                #region Icons

                //companyIcon = "../img/menu/bpmWhite.svg";

                #endregion
            }
            else
            {
                #region Icons

                //companyIcon = "../img/menu/bpm.svg";

                #endregion
            }
        }
        #endregion

        #endregion



    }
}
