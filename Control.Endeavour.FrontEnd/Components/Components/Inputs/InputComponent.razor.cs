using Control.Endeavour.FrontEnd.Models.Enums.Components.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Components.Inputs
{
    public partial class InputComponent
    {
        #region Fields
        private string[] realValues = new string[6] { "", "", "", "", "", "" };
        private string[] displayValues = new string[6];
        private string inputValue = "";
        private string inputType = "password";
        private string iconClass = "fa fa-eye";
        #endregion

        #region Properties
        [Parameter]
        public bool ShowErrors { get; set; } = false;

        [Parameter]
        public InputTypeEnum InputType { get; set; } = InputTypeEnum.Text;

        [Parameter]
        public string Placeholder { get; set; } = "Introduzca un valor";

        [Parameter]
        public string ErrorMessage { get; set; } = "Error con la entrada";

        [Parameter]
        public EventCallback<bool> OnValidation { get; set; }

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        [Parameter]
        public int Length { get; set; } = 330;

        public bool IsInvalid { get; private set; }

        private async Task OnInput(ChangeEventArgs e)
        {
            InputValue = e.Value.ToString();
            await OnChange.InvokeAsync(InputValue);
        }

        public string InputValue
        {
            get => inputValue;
            set
            {
                if (inputValue != value)
                {
                    inputValue = value;
                }
            }
        }
        #endregion

        #region Methods
        public string GetCompleteCodeValue()
        {
            return string.Join("", realValues);
        }

        private void HandleInput(ChangeEventArgs e, int index)
        {
            string? newValue = e.Value?.ToString();
            if (newValue != null && newValue.Length > 0)
            {
                realValues[index] = newValue.Last().ToString().ToUpper();
                displayValues[index] = "•";
            }
            else
            {
                realValues[index] = "";
                displayValues[index] = "";
            }

            inputValue = string.Concat(realValues.Where(v => !string.IsNullOrEmpty(v)));
            this.StateHasChanged();
        }

        private void TogglePasswordVisibility()
        {
            if (InputType == InputTypeEnum.Password || InputType==InputTypeEnum.UpdatePassword)
            {
                inputType = inputType == "password" ? "text" : "password";
                iconClass = inputType == "password" ? "fa fa-eye" : "fa fa-eye-slash";
            }
        }

        public void Reset()
        {
            inputValue = "";
            realValues = new string[6] { "", "", "", "", "", "" };
            displayValues = new string[6];
            inputType = "password";
            iconClass = "fas fa-eye-slash";
            IsInvalid = false;
            StateHasChanged();
        }
        #endregion

        #region Validation
        public void ValidateInput()
        {
            switch (InputType)
            {
                case InputTypeEnum.Password:
                    ValidatePassword();
                    break;
                case InputTypeEnum.UpdatePassword:
                    ValidateUpdatePassword();
                    break;
                case InputTypeEnum.Email:
                    ValidateEmail();
                    break;
                case InputTypeEnum.Text:
                    ValidateText();
                    break;
                case InputTypeEnum.Code:
                    ValidateCode();
                    break;
                default:
                    throw new InvalidOperationException("Tipo de entrada no válido");
            }

            OnValidation.InvokeAsync(IsInvalid);
        }

        private void ValidateUpdatePassword()
        {
            var passwordRegex = new Regex(@"^(?=.*\d)(?=.*[A-Z])(?=.*[@#$%^&*()\-_+={}[\]|\\;:'<>,.?/~/`""'!^?#\$%&*§µ€£¥¢₹©®])[A-Za-z\d@#$%^&*()\-_+={}[\]|\\;:'<>,.?/~/`""'!^?#\$%&*§µ€£¥¢₹©®]{8,16}$");

            IsInvalid = string.IsNullOrWhiteSpace(inputValue) || !passwordRegex.IsMatch(inputValue);
        }

        private void ValidatePassword()
        {
            IsInvalid = string.IsNullOrWhiteSpace(inputValue) || inputValue.Length < 6;
        }

        private void ValidateEmail()
        {
            var emailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            IsInvalid = string.IsNullOrWhiteSpace(inputValue) || !emailRegex.IsMatch(inputValue);
        }

        private void ValidateText()
        {
            IsInvalid = string.IsNullOrWhiteSpace(inputValue);
        }

        private void ValidateCode()
        {
            IsInvalid = inputValue.Length != 5;
        }

        private void ResetValidation()
        {
            IsInvalid = false;
        }
        #endregion
    }
}
