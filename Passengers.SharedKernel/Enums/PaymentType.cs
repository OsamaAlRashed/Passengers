using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Enums
{

    public static class PaymentTypesHelper
    {
        public static bool IsExportWithoutSalary(PaymentType payment)
        {
            if(payment is PaymentType.Salary)
                throw new Exception("Invalid type");
            return IsExport(payment);
        }

        public static bool IsExport(PaymentType payment)
        {
            if(payment is PaymentType.Salary or PaymentType.FixedExport or PaymentType.OtherExport)
                return false;
            return true;
        }

        public static int PaymentSign(PaymentType payment) 
            => IsExport(payment) ? -1 : 1;

        public static PaymentType Map(ImportType importType)
        {
            if (importType == ImportType.Delivery)
                return PaymentType.Delivery;
            else if (importType == ImportType.Fixed)
                return PaymentType.FixedImport;
            else
                return PaymentType.OtherImport;
        }

        public static ImportType MapImport(PaymentType paymentType)
        {
            if (paymentType == PaymentType.FixedImport)
                return ImportType.Fixed;
            else if (paymentType == PaymentType.Delivery)
                return ImportType.Delivery;
            else if (paymentType == PaymentType.OtherImport)
                return ImportType.Other;
            else
                throw new Exception("Invalid type");
        }

        public static PaymentType Map(ExportType exportType)
        {
            if (exportType == ExportType.Fixed)
                return PaymentType.FixedExport;
            else
                return PaymentType.OtherExport;
        }

        public static ExportType MapExport(PaymentType paymentType)
        {
            if (paymentType == PaymentType.FixedExport)
                return ExportType.Fixed;
            else if (paymentType == PaymentType.OtherExport)
                return ExportType.Other;
            else
                throw new Exception("Invalid type");
        }

    }

    public enum PaymentType
    {
        Salary = 1,
        FixedExport,
        OtherExport,

        Delivery,
        FixedImport,
        OtherImport
    }

    public enum ImportType
    {
        Delivery = 1,
        Fixed,
        Other
    }

    public enum ExportType
    {
        Fixed = 1,
        Other
    }
}
