using System;

namespace Passengers.SharedKernel.Enums;

public static class OrderStatusHelper
{
    public static CustomerOrderStatus MapCustomer(this OrderStatus orderStatus)
    {
        if (orderStatus == OrderStatus.Sended)
            return CustomerOrderStatus.Pending;
        if (orderStatus == OrderStatus.Accepted)
            return CustomerOrderStatus.Inprogress1;
        if (orderStatus == OrderStatus.Assigned)
            return CustomerOrderStatus.Inprogress2;
        if (orderStatus == OrderStatus.Collected)
            return CustomerOrderStatus.Onway;
        if (orderStatus == OrderStatus.Completed)
            return CustomerOrderStatus.Completed;
        if (orderStatus == OrderStatus.Canceled)
            return CustomerOrderStatus.Canceled;
        if (orderStatus == OrderStatus.Refused)
            return CustomerOrderStatus.Refused;

        throw new Exception("Invalid type");
    }


    public static DeliveryCompanyOrderStatus MapCompany(this OrderStatus orderStatus)
    {
        if (orderStatus == OrderStatus.Sended)
            return DeliveryCompanyOrderStatus.Received;
        if (orderStatus == OrderStatus.Accepted)
            return DeliveryCompanyOrderStatus.Unassigned;
        if (orderStatus == OrderStatus.Assigned)
            return DeliveryCompanyOrderStatus.Assigned;
        if (orderStatus == OrderStatus.Collected)
            return DeliveryCompanyOrderStatus.Collected;
        if (orderStatus == OrderStatus.Completed)
            return DeliveryCompanyOrderStatus.Completed;
        if (orderStatus == OrderStatus.Canceled)
            return DeliveryCompanyOrderStatus.Canceled;
        if (orderStatus == OrderStatus.Refused)
            return DeliveryCompanyOrderStatus.Refused;

        throw new Exception("Invalid type");
    }

    public static DriverOrderStatus MapDriver(this OrderStatus orderStatus)
    {
        if (orderStatus == OrderStatus.Accepted)
            return DriverOrderStatus.Avilable;
        if (orderStatus == OrderStatus.Assigned)
            return DriverOrderStatus.Inprogress;
        if (orderStatus == OrderStatus.Collected)
            return DriverOrderStatus.Collected;
        if (orderStatus == OrderStatus.Completed)
            return DriverOrderStatus.Completed;

        throw new Exception("Invalid type");
    }
}

public enum OrderStatus
{
    Sended = 1,
    Accepted,
    Assigned,
    Collected,
    Completed,
    Canceled,
    Refused,
}

public enum CustomerOrderStatus
{
    Pending = 1,
    Inprogress1,
    Inprogress2,
    Onway,
    Completed,
    Canceled,
    Refused
}

public enum DeliveryCompanyOrderStatus
{
    Received = 1,
    Unassigned,
    Assigned,
    Collected,
    Completed,
    Canceled,
    Refused
}

public enum DriverOrderStatus
{
    Avilable = 1,
    Inprogress,
    Collected,
    Completed
}
