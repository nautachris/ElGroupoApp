using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ElGroupo.Domain.Data.Configurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            ToTable("User", "dbo");
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            Property(x => x.Email).HasColumnName("EmailAddress");
        }
    }
}
