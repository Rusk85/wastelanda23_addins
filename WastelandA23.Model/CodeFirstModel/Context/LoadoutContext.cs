using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Model.CodeFirstModel
{
    public class LoadoutContext : DbContext
    {
        public LoadoutContext() : base("name=LoadoutContext")
        {

        }

        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerInfo> PlayerInfoes { get; set; }
        public DbSet<CurrentWeapon> CurrentWeapons { get; set; }
        public DbSet<CurrentMode> CurrentModes { get; set; }
        public DbSet<Loadout> Loadouts { get; set; }
        public DbSet<Backpack> Backpacks { get; set; }
        public DbSet<Uniform> Uniforms { get; set; }
        public DbSet<Vest> Vests { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<AssignableItem> AssignableItems { get; set; }
        public DbSet<PrimaryWeapon> PrimaryWeapons { get; set; }
        public DbSet<PrimaryWeaponItem> PrimaryWeaponItems { get; set; }
        public DbSet<SecondaryWeapon> SecondaryWeapons { get; set; }
        public DbSet<SecondaryWeaponItem> SecondaryWeaponItems { get; set; }
        public DbSet<HandgunWeapon> HandgunWeapons { get; set; }
        public DbSet<HandgunWeaponItem> HandgunWeaponItems { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var m = modelBuilder;

            // one : zero or one
            // Parent: Loadout
            m.Entity<PrimaryWeapon>().HasRequired(_ => _.Loadout)
                .WithOptional(_ => _.PrimaryWeapon).WillCascadeOnDelete(true);
            m.Entity<SecondaryWeapon>().HasRequired(_ => _.Loadout)
                .WithOptional(_ => _.SecondaryWeapon).WillCascadeOnDelete(true);
            m.Entity<HandgunWeapon>().HasRequired(_ => _.Loadout)
                .WithOptional(_ => _.HandgunWeapon).WillCascadeOnDelete(true);
            m.Entity<Backpack>().HasRequired(_ => _.Loadout)
                .WithOptional(_ => _.Backpack).WillCascadeOnDelete(true);
            m.Entity<Vest>().HasRequired(_ => _.Loadout)
                .WithOptional(_ => _.Vest).WillCascadeOnDelete(true);
            m.Entity<Uniform>().HasRequired(_ => _.Loadout)
                .WithOptional(_ => _.Uniform).WillCascadeOnDelete(true);
            // Parent: Player
            m.Entity<Loadout>().HasRequired(_ => _.Player)
                .WithOptional(_ => _.Loadout)
                .WillCascadeOnDelete(true);

            // one : one
            // Parent: Player
            m.Entity<PlayerInfo>().HasKey(_ => _.Id)
                .Property(_ => _.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            m.Entity<PlayerInfo>()
                .HasRequired(_ => _.Player)
                .WithRequiredDependent(_ => _.PlayerInfo)
                .WillCascadeOnDelete(true);
            // Parent: Player
            m.Entity<CurrentWeapon>().HasKey(_ => _.Id)
                .Property(_ => _.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            m.Entity<CurrentWeapon>()
                .HasRequired(_ => _.Player)
                .WithRequiredDependent(_ => _.CurrentWeapon)
                .WillCascadeOnDelete(true);
            // Parent: CurrentWeapon
            m.Entity<CurrentMode>().HasKey(_ => _.Id)
                .Property(_ => _.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            m.Entity<CurrentMode>()
                .HasRequired(_ => _.CurrentWeapon)
                .WithRequiredDependent(_ => _.CurrentMode)
                .WillCascadeOnDelete(true);

            m.Entity<Magazine>().HasOptional(_ => _.HandgunWeapon)
                .WithOptionalDependent(_ => _.Magazine)
                .Map(_ =>
                {
                    _.MapKey("HandgunWeaponId");
                })
                .WillCascadeOnDelete(true);
            m.Entity<Magazine>().HasOptional(_ => _.PrimaryWeapon)
                .WithOptionalDependent(_ => _.Magazine)
                .Map(_ =>
                {
                    _.MapKey("PrimaryWeaponId");
                })
                .WillCascadeOnDelete(true);
            m.Entity<Magazine>().HasOptional(_ => _.SecondaryWeapon)
                .WithOptionalDependent(_ => _.Magazine)
                .Map(_ =>
                {
                    _.MapKey("SecondaryWeaponId");
                })
                .WillCascadeOnDelete(true);


            // one : many
            // Parent: Weapons Childs: WeaponItem
            m.Entity<HandgunWeaponItem>().HasRequired(_ => _.HandgunWeapon)
                .WithMany().HasForeignKey(_ => _.HandgunWeaponId)
                .WillCascadeOnDelete(true);
            m.Entity<PrimaryWeaponItem>().HasRequired(_ => _.PrimaryWeapon)
                .WithMany().HasForeignKey(_ => _.PrimaryWeaponId)
                .WillCascadeOnDelete(true);
            m.Entity<SecondaryWeaponItem>().HasRequired(_ => _.SecondaryWeapon)
                .WithMany().HasForeignKey(_ => _.SecondaryWeaponId)
                .WillCascadeOnDelete(true);
            m.Entity<AssignableItem>().HasOptional(_ => _.Loadout)
                .WithMany().HasForeignKey(_ => _.LoadoutId)
                .WillCascadeOnDelete(true);
            m.Entity<Item>().HasOptional(_ => _.Backpack)
                .WithMany().HasForeignKey(_ => _.BackpackId)
                .WillCascadeOnDelete(true);
            m.Entity<Item>().HasOptional(_ => _.Uniform)
                .WithMany().HasForeignKey(_ => _.UniformId)
                .WillCascadeOnDelete(true);
            m.Entity<Item>().HasOptional(_ => _.Vest)
                .WithMany().HasForeignKey(_ => _.VestId)
                .WillCascadeOnDelete(true);
            // many : one 
            // required otherwise we get foreign keys referencing the same table with different delete/update constraints
            m.Entity<HandgunWeapon>().HasMany(_ => _.HandgunWeaponItems)
                .WithRequired(_ => _.HandgunWeapon).HasForeignKey(_ => _.HandgunWeaponId)
                .WillCascadeOnDelete(true);
            m.Entity<PrimaryWeapon>().HasMany(_ => _.PrimaryWeaponItems)
                .WithRequired(_ => _.PrimaryWeapon).HasForeignKey(_ => _.PrimaryWeaponId)
                .WillCascadeOnDelete(true);
            m.Entity<SecondaryWeapon>().HasMany(_ => _.SecondaryWeaponItems)
                .WithRequired(_ => _.SecondaryWeapon).HasForeignKey(_ => _.SecondaryWeaponId)
                .WillCascadeOnDelete(true);
            m.Entity<Loadout>().HasMany(_ => _.AssignableItems)
                .WithOptional().HasForeignKey(_ => _.LoadoutId)
                .WillCascadeOnDelete(true);
            m.Entity<Backpack>().HasMany(_ => _.Items)
                .WithOptional().HasForeignKey(_ => _.BackpackId)
                .WillCascadeOnDelete(true);
            m.Entity<Uniform>().HasMany(_ => _.Items)
                .WithOptional().HasForeignKey(_ => _.UniformId)
                .WillCascadeOnDelete(true);
            m.Entity<Vest>().HasMany(_ => _.Items)
                .WithOptional().HasForeignKey(_ => _.VestId)
                .WillCascadeOnDelete(true);


            // many : many








        }

    }
}
