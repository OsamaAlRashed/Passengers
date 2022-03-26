using Passengers.SharedKernel.Services.LangService.Contant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Services.LangService.LangErrorStore
{
    public static class LangErrorStore
    {
        private static readonly Dictionary<(string, string), string> ErrorDictionary = new()
        {
            { (ErrorCodeConstant.UserNotFound, LangConstant.En), "User Not Found."},
            { (ErrorCodeConstant.UserNotFound, LangConstant.Ar), "المستخدم غير موجود." },

            { (ErrorCodeConstant.DiscountNotFound, LangConstant.En), "Discount Not Found." },
            { (ErrorCodeConstant.DiscountNotFound, LangConstant.Ar), "الحسم غير موجود." },

            { (ErrorCodeConstant.DiscountAlreadyExist, LangConstant.En), "Discount Already Exist to this product." },
            { (ErrorCodeConstant.DiscountAlreadyExist, LangConstant.Ar), "الحسم موجود بالفعل لهذا المنتج." },

            { (ErrorCodeConstant.TimeFormatIsNotValid, LangConstant.En), "Time format is not valid." },
            { (ErrorCodeConstant.TimeFormatIsNotValid, LangConstant.Ar), "" },

            { (ErrorCodeConstant.ChooseDayAtLeast, LangConstant.En), "Choose day at least." },
            { (ErrorCodeConstant.ChooseDayAtLeast, LangConstant.Ar), "" },
        };

        public static string Get(string errorCode, string lang = LangConstant.En)
        {
            if (ErrorDictionary.ContainsKey((errorCode,lang)))
                return ErrorDictionary[(errorCode,lang)];
            if (lang == LangConstant.Ar)
                return "خطأ غير معروف.";
            return "Unknown Error.";
        }

    }
}
