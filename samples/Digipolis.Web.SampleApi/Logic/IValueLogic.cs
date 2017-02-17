using System.Collections.Generic;
using Digipolis.Web.Api;
using Digipolis.Web.SampleApi.Models;

namespace Digipolis.Web.SampleApi.Logic
{
    public interface IValueLogic
    {
        IEnumerable<ValueDto> GetAll(PageOptions queryOptions, out int total);
        IEnumerable<object> GetAll(PageOptions queryOptions, out int total, params string[] fields);
        ValueDto GetById(int id);
        dynamic GetById(int id, string[] fields);
        ValueDto Add(ValueDto value);
        ValueDto Update(int id, ValueDto value);
        void Delete(int id);
    }
}