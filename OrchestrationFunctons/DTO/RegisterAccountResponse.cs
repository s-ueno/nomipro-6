using System;
using System.Collections.Generic;
using System.Text;

namespace OrchestrationFunctons.DTO
{
    public class RegisterAccountResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public Model.Account Account { get; set; }
    }
}
