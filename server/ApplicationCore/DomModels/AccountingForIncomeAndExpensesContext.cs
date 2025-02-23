using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace server.ApplicationCore.DomModels;

public partial class AccountingForIncomeAndExpensesContext : IdentityDbContext<Client>
{
    protected readonly IConfiguration Configuration;

    public AccountingForIncomeAndExpensesContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    public virtual DbSet<CategoryLimit> CategoryLimits { get; set; }

    public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    public virtual DbSet<ExpenseItem> ExpenseItems { get; set; }

    public virtual DbSet<IncomeCategory> IncomeCategories { get; set; }

    public virtual DbSet<IncomeItem> IncomeItems { get; set; }

    public virtual DbSet<Client> Clients { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CategoryLimit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC0765A9EB74");

            entity.HasIndex(e => e.ExpenseCategoryId, "IX_CategoryLimits_ExpenseCategoryId");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LimitAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.ExpenseCategory).WithMany(p => p.CategoryLimits)
                .HasForeignKey(d => d.ExpenseCategoryId)
                .HasConstraintName("FK__CategoryL__Expen__4BAC3F29");
        });

        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpenseC__3214EC077ACD7DB6");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.ExpenseCategories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ExpenseCa__ClientI__3B75D760");
        });

        modelBuilder.Entity<ExpenseItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpenseI__3214EC07BE00CF88");

            entity.HasIndex(e => e.ExpenseCategoryId, "IX_ExpenseItems_ExpenseCategoryId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");

            entity.HasOne(d => d.ExpenseCategory).WithMany(p => p.ExpenseItems)
                .HasForeignKey(d => d.ExpenseCategoryId)
                .HasConstraintName("FK__ExpenseIt__Expen__3F466844");
        });

        modelBuilder.Entity<IncomeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__IncomeCa__3214EC0798FFEC61");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.IncomeCategories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__IncomeCat__ClientI__4316F928");
        });

        modelBuilder.Entity<IncomeItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__IncomeIt__3214EC07A5490B0A");

            entity.HasIndex(e => e.IncomeCategoryId, "IX_IncomeItems_IncomeCategoryId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");

            entity.HasOne(d => d.IncomeCategory).WithMany(p => p.IncomeItems)
                .HasForeignKey(d => d.IncomeCategoryId)
                .HasConstraintName("FK__IncomeIte__Incom__46E78A0C");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clients__3214EC072F24FD09");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        //OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
