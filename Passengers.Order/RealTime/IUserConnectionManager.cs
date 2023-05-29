using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Order.RealTime
{
    public interface IUserConnectionManager
    {
        void KeepUserConnection(Guid userId, string connectionId);
        void RemoveUserConnection(string connectionId);
        List<string> GetConnections(Guid userId);
        List<string> GetConnections(List<Guid> userIds);
        List<string> GetConnections(UserType type);
        List<string> GetConnections(UserType type, List<Guid> expectedUserIds);
    }
}
