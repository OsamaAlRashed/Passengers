using Passengers.SharedKernel.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Passengers.SharedKernel.Services.Selector
{
    public interface ISelector<TDto> { }

    public interface ISelector<TEntity, TDto> : ISelector<TDto>
    {
        static Expression<Func<TEntity, TDto>> Selector { get; set; }
        static Expression<Func<TDto, TEntity>> InverseSelector { get; set; }
        static Action<TDto, TEntity> AssignSelector { get; set; }
    }

    public static class Selector
    {
        public static Expression<Func<TEntity, TDto>> GetSelector<TEntity, TDto>()
            => Helpers.GetValueReflection<TDto, Expression<Func<TEntity, TDto>>>(nameof(ISelector<TEntity, TDto>.Selector));

        public static Expression<Func<TDto, TEntity>> GetInverseSelector<TDto, TEntity>()
            => Helpers.GetValueReflection<TDto, Expression<Func<TDto, TEntity>>>(nameof(ISelector<TDto, TEntity>.InverseSelector));

        public static Action<TDto, TEntity> GetAssignSelector<TDto, TEntity>()
            => Helpers.GetValueReflection<TDto, Action<TDto, TEntity>>(nameof(ISelector<TDto, TEntity>.AssignSelector));

        public static TDto CompileSelector<TEntity, TDto>(this Expression<Func<TEntity, TDto>> selector, TEntity entity)
         => selector.Compile().Invoke(entity);

        public static TEntity CompileInverseSelector<TDto, TEntity>(this Expression<Func<TDto, TEntity>> selector, TDto dto)
         => selector.Compile().Invoke(dto);

        public static void CompileAssignSelector<TDto, TEntity>(this Action<TDto, TEntity> selector, TDto dto, TEntity entity)
         => selector(dto, entity);

    }
}
