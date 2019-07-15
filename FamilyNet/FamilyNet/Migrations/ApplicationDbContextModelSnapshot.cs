﻿// <auto-generated />
using System;
using FamilyNet.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FamilyNet.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FamilyNet.Models.Address", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("Country")
                        .IsRequired();

                    b.Property<string>("House")
                        .IsRequired();

                    b.Property<string>("Region")
                        .IsRequired();

                    b.Property<string>("Street")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLot", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuctionLotItemID");

                    b.Property<DateTime>("Date");

                    b.Property<int?>("OrphanID");

                    b.HasKey("ID");

                    b.HasIndex("AuctionLotItemID");

                    b.HasIndex("OrphanID");

                    b.ToTable("AuctionLot");
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLotItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Price");

                    b.HasKey("ID");

                    b.ToTable("AuctionLotItem");
                });

            modelBuilder.Entity("FamilyNet.Models.BaseItemType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuctionLotItemTypeID");

                    b.Property<int?>("AuctionLotItemTypeID1");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int?>("DonationItemTypeID");

                    b.Property<int?>("DonationItemTypeID1");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("AuctionLotItemTypeID");

                    b.HasIndex("AuctionLotItemTypeID1");

                    b.HasIndex("DonationItemTypeID");

                    b.HasIndex("DonationItemTypeID1");

                    b.ToTable("BaseItemType");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseItemType");
                });

            modelBuilder.Entity("FamilyNet.Models.CharityMaker", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressID");

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("ContactsID");

                    b.Property<int>("FullNameID");

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AddressID");

                    b.HasIndex("ContactsID");

                    b.HasIndex("FullNameID");

                    b.ToTable("CharityMakers");
                });

            modelBuilder.Entity("FamilyNet.Models.Contacts", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Phone");

                    b.HasKey("ID");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("FamilyNet.Models.Donation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CharityMakerID");

                    b.Property<DateTime>("Date");

                    b.Property<int?>("DonationItemID");

                    b.HasKey("ID");

                    b.HasIndex("CharityMakerID");

                    b.HasIndex("DonationItemID");

                    b.ToTable("Donations");
                });

            modelBuilder.Entity("FamilyNet.Models.DonationItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Price");

                    b.HasKey("ID");

                    b.ToTable("DonationItem");
                });

            modelBuilder.Entity("FamilyNet.Models.FullName", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Patronymic");

                    b.Property<string>("Surname");

                    b.HasKey("ID");

                    b.ToTable("FullName");
                });

            modelBuilder.Entity("FamilyNet.Models.Orphan", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressID");

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("ContactsID");

                    b.Property<int>("FullNameID");

                    b.Property<int>("OrphanageID");

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AddressID");

                    b.HasIndex("ContactsID");

                    b.HasIndex("FullNameID");

                    b.HasIndex("OrphanageID");

                    b.ToTable("Orphans");
                });

            modelBuilder.Entity("FamilyNet.Models.Orphanage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AddressID");

                    b.Property<string>("Avatar");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AddressID");

                    b.ToTable("Orphanages");
                });

            modelBuilder.Entity("FamilyNet.Models.Representative", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressID");

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("ContactsID");

                    b.Property<int>("FullNameID");

                    b.Property<int>("OrphanageID");

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AddressID");

                    b.HasIndex("ContactsID");

                    b.HasIndex("FullNameID");

                    b.HasIndex("OrphanageID");

                    b.ToTable("Representatives");
                });

            modelBuilder.Entity("FamilyNet.Models.Volunteer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressID");

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("ContactsID");

                    b.Property<int>("FullNameID");

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AddressID");

                    b.HasIndex("ContactsID");

                    b.HasIndex("FullNameID");

                    b.ToTable("Volunteers");
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLotItemType", b =>
                {
                    b.HasBaseType("FamilyNet.Models.BaseItemType");

                    b.Property<int?>("AuctionLotItemID");

                    b.HasIndex("AuctionLotItemID");

                    b.HasDiscriminator().HasValue("AuctionLotItemType");
                });

            modelBuilder.Entity("FamilyNet.Models.DonationItemType", b =>
                {
                    b.HasBaseType("FamilyNet.Models.BaseItemType");

                    b.Property<int?>("DonationItemID");

                    b.HasIndex("DonationItemID");

                    b.HasDiscriminator().HasValue("DonationItemType");
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLot", b =>
                {
                    b.HasOne("FamilyNet.Models.AuctionLotItem", "AuctionLotItem")
                        .WithMany()
                        .HasForeignKey("AuctionLotItemID");

                    b.HasOne("FamilyNet.Models.Orphan")
                        .WithMany("AuctionLots")
                        .HasForeignKey("OrphanID");
                });

            modelBuilder.Entity("FamilyNet.Models.BaseItemType", b =>
                {
                    b.HasOne("FamilyNet.Models.AuctionLotItemType")
                        .WithMany("Childs")
                        .HasForeignKey("AuctionLotItemTypeID");

                    b.HasOne("FamilyNet.Models.AuctionLotItemType")
                        .WithMany("Parent")
                        .HasForeignKey("AuctionLotItemTypeID1");

                    b.HasOne("FamilyNet.Models.DonationItemType")
                        .WithMany("Childs")
                        .HasForeignKey("DonationItemTypeID");

                    b.HasOne("FamilyNet.Models.DonationItemType")
                        .WithMany("Parent")
                        .HasForeignKey("DonationItemTypeID1");
                });

            modelBuilder.Entity("FamilyNet.Models.CharityMaker", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID");

                    b.HasOne("FamilyNet.Models.Contacts", "Contacts")
                        .WithMany()
                        .HasForeignKey("ContactsID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FamilyNet.Models.FullName", "FullName")
                        .WithMany()
                        .HasForeignKey("FullNameID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FamilyNet.Models.Donation", b =>
                {
                    b.HasOne("FamilyNet.Models.CharityMaker")
                        .WithMany("Donations")
                        .HasForeignKey("CharityMakerID");

                    b.HasOne("FamilyNet.Models.DonationItem", "DonationItem")
                        .WithMany()
                        .HasForeignKey("DonationItemID");
                });

            modelBuilder.Entity("FamilyNet.Models.Orphan", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID");

                    b.HasOne("FamilyNet.Models.Contacts", "Contacts")
                        .WithMany()
                        .HasForeignKey("ContactsID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FamilyNet.Models.FullName", "FullName")
                        .WithMany()
                        .HasForeignKey("FullNameID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FamilyNet.Models.Orphanage", "Orphanage")
                        .WithMany("OrphansIds")
                        .HasForeignKey("OrphanageID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FamilyNet.Models.Orphanage", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FamilyNet.Models.Representative", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID");

                    b.HasOne("FamilyNet.Models.Contacts", "Contacts")
                        .WithMany()
                        .HasForeignKey("ContactsID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FamilyNet.Models.FullName", "FullName")
                        .WithMany()
                        .HasForeignKey("FullNameID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FamilyNet.Models.Orphanage", "Orphanage")
                        .WithMany("Representatives")
                        .HasForeignKey("OrphanageID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FamilyNet.Models.Volunteer", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID");

                    b.HasOne("FamilyNet.Models.Contacts", "Contacts")
                        .WithMany()
                        .HasForeignKey("ContactsID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FamilyNet.Models.FullName", "FullName")
                        .WithMany()
                        .HasForeignKey("FullNameID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLotItemType", b =>
                {
                    b.HasOne("FamilyNet.Models.AuctionLotItem")
                        .WithMany("AuctionLotItemType")
                        .HasForeignKey("AuctionLotItemID");
                });

            modelBuilder.Entity("FamilyNet.Models.DonationItemType", b =>
                {
                    b.HasOne("FamilyNet.Models.DonationItem")
                        .WithMany("DonationItemType")
                        .HasForeignKey("DonationItemID");
                });
#pragma warning restore 612, 618
        }
    }
}
