using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passengers.SharedKernel.OperationResult;

namespace Passengers.SharedKernel.Pagnation;

public static class ExtesionMethods
{
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public async static Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public static OperationResult<PagedList<T>> AddPagnationHeader<T>(this OperationResult<PagedList<T>> result, ControllerBase controllerBase)
    {
        var metadata = new
        {
            result.Result.TotalCount,
            result.Result.PageSize,
            result.Result.CurrentPage,
            result.Result.TotalPages,
            result.Result.HasNext,
            result.Result.HasPrevious
        };
        controllerBase.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        return result;
    }

    public static async Task<OperationResult<PagedList<T>>> AddPagnationHeaderAsync<T>(this Task<OperationResult<PagedList<T>>> result, ControllerBase controllerBase)
        => (await result).AddPagnationHeader(controllerBase);
}
