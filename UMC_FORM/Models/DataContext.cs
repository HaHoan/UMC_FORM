namespace UMC_FORM.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using UMC_FORM.Models.GA;

    public partial class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataConnection")
        {
        }

        public virtual DbSet<Form_Comment> Form_Comment { get; set; }
        public virtual DbSet<Form_Dept> Form_Depts { get; set; }
        public virtual DbSet<Form_Procedures> Form_Procedures { get; set; }
        public virtual DbSet<Form_Process> Form_Process { get; set; }
        public virtual DbSet<Form_ProcessName> Form_ProcessNames { get; set; }
        public virtual DbSet<Form_Reject> Form_Reject { get; set; }
        public virtual DbSet<Form_Role> Form_Roles { get; set; }
        public virtual DbSet<Form_Stations> Form_Stations { get; set; }
        public virtual DbSet<Form_User> Form_User { get; set; }
        public virtual DbSet<LCA_FILE> LCA_FILE { get; set; }
        public virtual DbSet<LCA_FORM_01> LCA_FORM_01 { get; set; }
        public virtual DbSet<LCA_PERMISSION> LCA_PERMISSION { get; set; }
        public virtual DbSet<LCA_QUOTE> LCA_QUOTE { get; set; }
        public virtual DbSet<PR_ACC_F06> PR_ACC_F06 { get; set; }
        public virtual DbSet<Form_Summary> Form_Summary { get; set; }
        public virtual DbSet<Form_Log> Form_Logs { get; set; }
        public virtual DbSet<GA_LEAVE_FORM> GA_LEAVE_FORM { get; set; }
        public virtual DbSet<GA_LEAVE_FORM_ITEM> GA_LEAVE_FORM_ITEM { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Form_User>()
                .Property(e => e.CODE)
                .IsUnicode(false);

            modelBuilder.Entity<Form_User>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<Form_User>()
                .Property(e => e.SIGNATURE)
                .IsUnicode(false);

            modelBuilder.Entity<Form_User>()
                .Property(e => e.DEPT)
                .IsUnicode(false);


            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.TICKET)
                .IsUnicode(false);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_1)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_2)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_3)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_4)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_5)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_6)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_7)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_8)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_9)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PR_ACC_F06>()
                .Property(e => e.AMOUNT_10)
                .HasPrecision(18, 0);
        }
    }
}
