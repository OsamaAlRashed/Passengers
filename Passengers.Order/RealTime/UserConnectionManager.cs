using Passengers.Security.AccountService;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Passengers.Order.RealTime
{
    public class UserConnectionManager : IUserConnectionManager
    {
        public UserConnectionManager(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }
        private static Dictionary<Guid, List<string>> userConnectionMap = new();
        private static string userConnectionMapLocker = string.Empty;
        private readonly IAccountRepository accountRepository;

        public void KeepUserConnection(Guid userId, string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                if (!userConnectionMap.ContainsKey(userId))
                {
                    userConnectionMap[userId] = new List<string>();
                }
                userConnectionMap[userId].Add(connectionId);
            }
        }
        public void RemoveUserConnection(string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userConnectionMap.Keys)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        if (userConnectionMap[userId].Contains(connectionId))
                        {
                            userConnectionMap[userId].Remove(connectionId);
                            break;
                        }
                    }
                }
            }
        }
        public List<string> GetConnections(Guid userId)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                if (userConnectionMap.ContainsKey(userId))
                {
                    conn = userConnectionMap[userId];
                }
                else
                {
                    return new List<string>();
                }
            }
            return conn;
        }
        public List<string> GetConnections(UserType type)
        {
            var conn = new List<string>();

            var userIds = accountRepository.GetUserIds(type);

            foreach (var userId in userIds)
            {
                conn.AddRange(GetConnections(userId));
            }
            return conn;
        }

        public List<string> GetConnections(List<Guid> userIds)
        {
            var conn = new List<string>();

            foreach (var userId in userIds)
            {
                conn.AddRange(GetConnections(userId));
            }
            return conn;
        }

        public List<string> GetConnections(UserType type, List<Guid> exceptUserIds)
        {
            var conn = new List<string>();

            var userIds = accountRepository.GetUserIds(type);
            userIds = userIds.Except(exceptUserIds).ToList();

            foreach (var userId in userIds)
            {
                conn.AddRange(GetConnections(userId));
            }
            return conn;
        }

    }
}
