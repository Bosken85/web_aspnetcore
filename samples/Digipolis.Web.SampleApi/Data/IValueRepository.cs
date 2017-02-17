using System.Collections.Generic;
using Digipolis.Web.Api;
using Digipolis.Web.SampleApi.Data.Entiteiten;

namespace Digipolis.Web.SampleApi.Data
{
    public interface IValueRepository
    {
        IEnumerable<TSelect> GetAll<TSelect>(PageOptions queryOptions, out int total) where TSelect: class, new();
        IEnumerable<dynamic> GetAll<TSelect>(PageOptions queryOptions, out int total, params string[] fields) where TSelect : class, new();
        TSelect GetById<TSelect>(int id) where TSelect : class, new();
        dynamic GetById<TSelect>(int id, params string[] fields) where TSelect : class, new();
        Value Add(Value value);
        Value Update(int id, Value value);
        void Delete(int id);
        void Delete(Value entity);
    }
}
