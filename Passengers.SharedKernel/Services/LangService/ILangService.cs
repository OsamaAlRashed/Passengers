using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Services.LangService
{
    public interface ILangService
    {
        public string CurrentLang { get; }
    }
}
