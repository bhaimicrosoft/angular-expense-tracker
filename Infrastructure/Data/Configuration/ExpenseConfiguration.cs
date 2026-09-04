using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id); // Primary key
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);

        // Map the Money Value object to flat columns in the Expense table
        builder.ComplexProperty(e => e.Amount, amountBuilder =>
        {
            amountBuilder.Property(m => m.Amount).HasColumnName("Amount").IsRequired();

            amountBuilder.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });
        
        // Relations
        builder.HasOne<User>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Category>().WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}