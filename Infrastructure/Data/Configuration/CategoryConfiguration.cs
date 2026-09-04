using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);   // Primary key
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.HexColor).IsRequired().HasMaxLength(7); // #FFFFFF;
        
        // Relations : A single user can have many categories. If user is deleted, delete all the categories using cascade
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}