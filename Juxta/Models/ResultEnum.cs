using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Juxta.Models
{
    public struct ResultEnum
    {
        private readonly string name;
        private readonly int id;

        public static ResultEnum None => new ResultEnum(-1, string.Empty);
        public static ResultEnum Abide => new ResultEnum(0, "Соблюдает карантин");
        public static ResultEnum Closed => new ResultEnum(1, "Дверь не открыли");
        public static ResultEnum Hospital => new ResultEnum(2, "Госпитализация");
        public static ResultEnum NoLiving => new ResultEnum(3, "Место жительства устанавливается");

        private ResultEnum(int id, string name)
        {
            this.name = name;
            this.id = id;
        }

        public override string ToString() => name;
        
        public static IEnumerable<string> GetNames() => GetValues().Select(x => x.name);
        
        public static ResultEnum GetValue(int id) => GetValues().First(x => x.id == id);
        
        public static ResultEnum GetValue(string name) => GetValues().First(x => x.name == name);

        public static IReadOnlyList<ResultEnum> GetValues()
        {
            return typeof(ResultEnum).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(property => (ResultEnum)property.GetValue(null))
                .ToList();
        }

        public static explicit operator int(ResultEnum x) => x.id;
        public static explicit operator ResultEnum(int id) => GetValue(id);
    }
}
