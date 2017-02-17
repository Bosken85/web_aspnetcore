using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Digipolis.Web.Api
{
    public static class DynamicTypeBuilder
    {
        private static readonly AssemblyName AssemblyName = new AssemblyName() { Name = "DynamicLinqTypes" };
        private static readonly ModuleBuilder ModuleBuilder = null;
        private static readonly ConcurrentDictionary<string, Tuple<string, Type>> BuiltTypes = new ConcurrentDictionary<string, Tuple<string, Type>>();

        static DynamicTypeBuilder()
        {
            ModuleBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> fields)
        {
            return fields.OrderBy(v => v.Key).ThenBy(v => v.Value.Name).Aggregate(string.Empty, (current, field) => current + (field.Key + ";" + field.Value.Name + ";"));
        }

        public static Type GetDynamicType(Dictionary<string, Type> fields, Type basetype, Type[] interfaces)
        {
            if (null == fields) throw new ArgumentNullException(nameof(fields));
            if (0 == fields.Count) throw new ArgumentOutOfRangeException(nameof(fields), "fields must have at least 1 field definition");

            try
            {
                Tuple<string, Type> result;
                Monitor.Enter(BuiltTypes);
                string typeKey = GetTypeKey(fields);

                if (BuiltTypes.ContainsKey(typeKey))
                {
                    if(!BuiltTypes.TryGetValue(typeKey, out result)) throw new ArgumentOutOfRangeException();
                    return result.Item2;
                }

                string typeName = $"DynamicLinqType{BuiltTypes.Count}";
                TypeBuilder typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, null, Type.EmptyTypes);

                foreach (var field in fields)
                    typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);

                result = new Tuple<string, Type>(typeName, typeBuilder.CreateTypeInfo().AsType());
                BuiltTypes.TryAdd(typeKey, result); 
                return result.Item2;
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(BuiltTypes);
            }

        }

    }
}