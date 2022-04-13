using Passengers.Models.Main;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.PaymentDtos
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public Guid? UserId { get; set; }
        public PaymentType Type { get; set; }

        public static Expression<Func<Payment, PaymentDto>> Selector { get; set; } = entity => new PaymentDto() { Id = entity.Id, Note = entity.Note, UserId = entity.UserId, Amount = entity.Amount, Date = entity.Date, Type = entity.Type };
        public static Expression<Func<PaymentDto, Payment>> InverseSelector { get; set; } = dto => new Payment() { Id = dto.Id, Note = dto.Note, UserId = dto.UserId, Amount = dto.Amount, Date = dto.Date, Type = dto.Type };
        public static Action<PaymentDto, Payment> AssignSelector { get; set; } = (dto, entity) => { entity.Note = dto.Note; entity.UserId = dto.UserId; entity.Date = dto.Date; entity.Amount = dto.Amount; entity.Type = dto.Type; };

    }
}
