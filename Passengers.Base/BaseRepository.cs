using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Services.Selector;
using Passengers.SqlServer.DataBase;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Passengers.Models.Base;
using Passengers.SharedKernel.Enums;
using Passengers.Models.Security;

namespace Passengers.Repository.Base
{
    public abstract class BaseRepository
    {
        protected readonly PassengersDbContext Context;
        public BaseRepository(PassengersDbContext context)
        {
            Context = context;
        }
    }

    public abstract class BaseRepository<TEntity , TDto> where TEntity : class , IBaseEntity where TDto : class
    {
        protected readonly PassengersDbContext Context;
        public BaseRepository(PassengersDbContext context)
        {
            Context = context;
        }

        public virtual async Task<OperationResult<IEnumerable<TDto>>> GetAsync()
        {
            Expression<Func<TEntity, TDto>> selector = Selector.GetSelector<TEntity, TDto>();

            IEnumerable<TDto> entities = await Context.Set<TEntity>().Select(selector).ToListAsync();

            return _Operation.SetSuccess(entities);
        }

        public virtual async Task<OperationResult<TDto>> GetByIdAsync(Guid id)
        {
            TEntity entity = await Context.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id);
            if (entity is null)
                return (OperationResultTypes.NotExist, $"{typeof(TEntity).Name}: {id} not exist");
            return _Operation.SetSuccess(Selector.GetSelector<TEntity, TDto>().CompileSelector(entity));
        }

        public virtual async Task<OperationResult<TDto>> AddAsync(TDto dto)
        {
            TEntity entity = Selector.GetInverseSelector<TDto, TEntity>().CompileInverseSelector(dto);
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(Selector.GetSelector<TEntity, TDto>().CompileSelector(entity));
        }
        
        public virtual async Task<OperationResult<TDto>> UpdateAsync(TDto dto)
        {
            Guid id = dto.GetValueReflection<TDto, Guid>("Id");
            TEntity entity = await Context.Set<TEntity>().FirstOrDefaultAsync(ent => ent.Id.Equals(id));
            if (entity is null)
                return _Operation.SetFailed<TDto>($"{typeof(TEntity).Name}: {id} not exist", OperationResultTypes.NotExist);
            Selector.GetAssignSelector<TDto, TEntity>().CompileAssignSelector(dto, entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(Selector.GetSelector<TEntity, TDto>().CompileSelector(entity));
        }

        public virtual async Task<OperationResult<bool>> DeleteAsync(Guid id)
        {
            TEntity entity = await Context.FindAsync<TEntity>(id);
            if (entity is null)
                return (OperationResultTypes.NotExist, $"{typeof(TEntity).Name}: {id} not exist");
            entity.DateDeleted = DateTime.Now.ToLocalTime();
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true, $"Soft deleted {typeof(TEntity).Name} success");
        }

    }
}
