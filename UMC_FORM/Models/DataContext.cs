namespace UMC_FORM.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataContext : DbContext
    {
        public DataContext()
            : base("name=FormConnection")
        {
        }

        public virtual DbSet<Form_Process> Form_Process { get; set; }
        public virtual DbSet<Form_Stations> Form_Stations { get; set; }
        public virtual DbSet<Form_User> Form_User { get; set; }
        public virtual DbSet<PR_ACC_F06> PR_ACC_F06 { get; set; }
        public virtual DbSet<Form_Comment> Form_Comment { get; set; }
        public virtual DbSet<Form_Summary> Form_Summary { get; set; }
        public virtual DbSet<Form_Dept> Form_Depts { get; set; }
        public virtual DbSet<Form_ProcessName> Form_ProcessNames { get; set; }
        public virtual DbSet<Form_Role> Form_Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Form_Process>()
                .Property(e => e.RETURN_STATION_NO)
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
