using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ElGroupo.Domain.Data.Configurations
{
    public static class DbContextBuilderExtensions
    {
        public static ModelBuilder AddConfiguration<TModelConfigurator>(this ModelBuilder modelBuilder)
        {
            var constructor =
                typeof(TModelConfigurator)
                    .GetConstructors(
                        )
                    .FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(ModelBuilder));
            if (constructor == null) throw new InvalidOperationException($"{typeof(TModelConfigurator)} must have a public constructor taking a ModelBuilder");

            var conf = constructor.Invoke(new[] { modelBuilder });
            return modelBuilder;
        }
    }
    public interface IBuildModels { }
}
