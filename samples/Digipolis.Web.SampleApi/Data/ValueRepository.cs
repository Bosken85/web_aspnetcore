﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Digipolis.Errors.Exceptions;
using Digipolis.Web.Api;
using Digipolis.Web.SampleApi.Data.Entiteiten;
using ValueType = Digipolis.Web.SampleApi.Data.Entiteiten.ValueType;

namespace Digipolis.Web.SampleApi.Data
{
    public class ValueRepository : IValueRepository
    {
        private static readonly List<Value> Values = new List<Value>(new[]
        {
            new Value {Id = 1, Name = "value 1", ValueType = ValueType.Store, CreationDate = DateTime.UtcNow},
            new Value {Id = 2, Name = "value 2", ValueType = ValueType.Unknown, CreationDate = DateTime.UtcNow},
            new Value {Id = 3, Name = "value 3", ValueType = ValueType.Store, CreationDate = DateTime.UtcNow},
            new Value {Id = 4, Name = "value 4", ValueType = ValueType.Unknown, CreationDate = DateTime.UtcNow},
            new Value {Id = 5, Name = "value 5", ValueType = ValueType.Store, CreationDate = DateTime.UtcNow}
        });

        private static IQueryable<Value> Table => Values.AsQueryable();


        public IEnumerable<TSelect> GetAll<TSelect>(PageOptions queryOptions, out int total) where TSelect : class, new()
        {
            total = Table.Count();
            var query = Table
                .ProjectTo<TSelect>()
                .Skip((queryOptions.Page - 1) * queryOptions.PageSize)
                .Take(queryOptions.PageSize);
            return query;
        }

        public IEnumerable<dynamic> GetAll<TSelect>(PageOptions queryOptions, out int total, params string[] fields) where TSelect : class, new()
        {
            total = Table.Count();
            var query = Table
                .ProjectTo<TSelect>()
                .SelectPartially(fields)
                .Skip((queryOptions.Page - 1) * queryOptions.PageSize)
                .Take(queryOptions.PageSize);
            return query;
        }

        public TSelect GetById<TSelect>(int id) where TSelect : class, new()
        {
            if (Table.All(x => x.Id != id)) throw new NotFoundException();
            return Table.Where(x => x.Id == id).ProjectTo<TSelect>().FirstOrDefault();
        }

        public dynamic GetById<TSelect>(int id, params string[] fields) where TSelect : class, new()
        {
            if (Table.All(x => x.Id != id)) throw new NotFoundException();
            return Table.Where(x => x.Id == id).ProjectTo<TSelect>().SelectPartially(fields).FirstOrDefault();
        }

        public Value Add(Value value)
        {
            //Mimic DB exception thrown by Unique Constraint
            if (Table.Any(x => x.Name.Equals(value.Name, StringComparison.CurrentCultureIgnoreCase)))
                throw new InvalidOperationException();

            value.Id = Values.Max(x => x.Id) + 1;
            value.CreationDate = DateTime.UtcNow;
            Values.Add(value);
            return value;
        }

        public Value Update(int id, Value value)
        {
            //Mimic DB exception thrown by no record found
            if (Table.All(x => x.Id != id))
                throw new NotFoundException();

            //Mimic DB exception thrown by Unique Constraint
            if (Table.Any(x => x.Name.Equals(value.Name, StringComparison.CurrentCultureIgnoreCase) && x.Id != id))
                throw new InvalidOperationException();

            var dbValue = Values.Find(x => x.Id == id);
            dbValue.Name = value.Name;
            return dbValue;
        }

        public void Delete(int id)
        {
            //Mimic DB exception thrown by no record found
            if (Table.All(x => x.Id != id))
                throw new NotFoundException();

            Values.Remove(Values.Find(x => x.Id == id));
        }

        public void Delete(Value entity)
        {
            if(entity == null) throw new ArgumentNullException(nameof(entity));
        }
    }
}