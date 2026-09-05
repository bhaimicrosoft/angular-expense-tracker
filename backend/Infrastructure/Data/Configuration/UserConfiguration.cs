using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey( param => param.Id  );   // Sets the Primary Key
        
        // Restrict column lengths for database security
        builder.Property(param => param.Email).IsRequired().HasMaxLength(255);
        builder.Property(param => param.FullName).IsRequired().HasMaxLength(200);
        
        // Ensure emails are unique at the DB level
        builder.HasIndex(param => param.Email).IsUnique();
    }
}
