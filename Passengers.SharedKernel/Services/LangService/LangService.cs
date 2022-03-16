using Microsoft.AspNetCore.Http;
using Passengers.SharedKernel.Services.LangService.Contant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Services.LangService
{

    public class LangService : ILangService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LangService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string CurrentLang
        {
            get
            {
                string lang = _httpContextAccessor?.HttpContext?.Request?.Headers["lang"];
                return String.IsNullOrEmpty(lang) ? LangConstant.En : lang;  
            }
        }
    }
}
