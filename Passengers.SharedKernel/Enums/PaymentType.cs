using System;

namespace Passengers.SharedKernel.Enums;


public static class PaymentTypesHelper
{
    public static bool IsExportWithoutSalary(this PaymentType payment)
    {
        if (payment is PaymentType.Salary)
            return false;
        return IsExport(payment);
    }

    public static bool IsExport(this PaymentType payment)
    {
        if(payment is PaymentType.Salary or PaymentType.FixedExport or PaymentType.OtherExport)
            return true;
        return false;
    }

    public static bool IsFixed(this PaymentType payment)
    {
        if (payment is PaymentType.FixedExport or PaymentType.FixedImport)
            return true;
        return false;
    }

    public static int PaymentSign(this PaymentType payment) 
        => IsExport(payment) ? -1 : 1;

    public static PaymentType Map(this ImportType importType)
    {
        if (importType == ImportType.Delivery)
            return PaymentType.Delivery;
        else if (importType == ImportType.Fixed)
            return PaymentType.FixedImport;
        else
            return PaymentType.OtherImport;
    }

    public static ImportType MapImport(this PaymentType paymentType)
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

    public static PaymentType Map(this ExportType exportType)
    {
        if (exportType == ExportType.Fixed)
            return PaymentType.FixedExport;
        else
            return PaymentType.OtherExport;
    }

    public static ExportType MapExport(this PaymentType paymentType)
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
