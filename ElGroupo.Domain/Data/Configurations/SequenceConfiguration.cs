using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElGroupo.Domain.Data.Configurations
{
    public class SequenceConfiguration : IBuildModels
    {
        public SequenceConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("IdGenerator", "dbo").StartsAt(1)
                .IncrementsBy(1);
            modelBuilder.ForSqlServerUseSequenceHiLo("IdGenerator", "dbo");
        }
    }
}
