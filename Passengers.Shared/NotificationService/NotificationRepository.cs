using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.NotificationDtos;
using Passengers.Models.Shared;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Services.NotificationService;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Passengers.Shared.NotificationService
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        private readonly IHttpClientFactory httpClient;
        private readonly INotificationService notificationService;

        public NotificationRepository(PassengersDbContext context, IHttpClientFactory httpClient): base(context)
        {
            this.httpClient = httpClient;
        }

        public async Task<OperationResult<NotificationDto>> Add(NotificationDto dto)
        {
            var users = await Context.Users.Where(x => dto.UserIds.Contains(x.Id)).ToListAsync();
            if (!users.Any())
                return _Operation.SetContent<NotificationDto>(OperationResultTypes.NotExist, "");

            var notification = new Notification
            {
                Body = dto.Body,
                Title = dto.Title,
                NotificationUsers = dto.UserIds.Select(userId => new NotificationUser
                {
                    UserId = userId
                }).ToList()
            };
            Context.Notifications.Add(notification);
            await Context.SaveChangesAsync();

            var tokens = users.SelectMany(x => x.DeviceTokens.Split(",").ToList()).ToList();

            await notificationService.SendNotification(tokens, dto.Title, dto.Body);

            dto.Id = notification.Id;
            return _Operation.SetSuccess(dto);
        }

        public async Task<OperationResult<List<NotificationDto>>> Get()
        {
            var result = await Context.NotificationUsers
                .Where(x => x.Id == x.UserId).Select(x => new NotificationDto
                {
                    Id = x.NotificationId,
                    Body = x.Notification.Body,
                    Title = x.Notification.Title
                }).ToListAsync();

            return _Operation.SetSuccess(result);
        }

        public async Task<HttpResponseMessage> FireAsync(string applicationId, string deviceId, string title, string body, object data)
        {
            var _fcmHttp = httpClient.CreateClient("fcm");
            var request = new
            {
                to = deviceId,
                notification = new
                {
                    title = title,
                    body = body,
                },
                data = data,
            };
            var json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            _fcmHttp.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={applicationId}");
            _fcmHttp.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={applicationId}");
            return await _fcmHttp.PostAsync("fcm/send", httpContent);
        }
    }
}
