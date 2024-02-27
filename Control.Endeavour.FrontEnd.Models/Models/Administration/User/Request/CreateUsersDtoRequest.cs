﻿using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request
{
    public class CreateUsersDtoRequest
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int CompanyId { get; set; }

        public string FileExt { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public byte[] Archivo { get; set; } = null!;

        public int ProductionOfficeId { get; set; }
        public int BranchOfficeId { get; set; }
        public bool ActiveState { get; set; }

        public string? SeccondPassword { get; set; }

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string? IdentificationType { get; set; }

        public string Identification { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CellPhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? ChargeCode { get; set; }

        public string? ContractType { get; set; }

        public string? ContractNumber { get; set; }

        public List<int> Profiles { get; set; } = null!;
        public List<CreatePermissionDtoRequest> Permissions { get; set; } = null!;

        public List<UsersSignatureDtoRequest>? Signatures { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractFinishDate { get; set; }
        public string? CreateUser { get; set; }
    }
}
