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

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Region")
                        .IsRequired();

                    b.Property<string>("Street")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("City");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLot", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuctionLotItemID");

                    b.Property<DateTime>("Date");

                    b.Property<bool>("IsDeleted");

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

                    b.Property<bool>("IsDeleted");

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

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("ParentID");

                    b.HasKey("ID");

                    b.HasIndex("ParentID");

                    b.ToTable("BaseItemType");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseItemType");
                });

            modelBuilder.Entity("FamilyNet.Models.CharityMaker", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressID");

                    b.Property<string>("Avatar");

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("EmailID");

                    b.Property<bool>("IsDeleted");

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AddressID")
                        .IsUnique()
                        .HasFilter("[AddressID] IS NOT NULL");

                    b.ToTable("CharityMakers");
                });

            modelBuilder.Entity("FamilyNet.Models.Donation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CharityMakerID");

                    b.Property<int?>("DonationItemID");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsRequest");

                    b.Property<DateTime>("LastDateWhenStatusChanged");

                    b.Property<int?>("OrphanageID");

                    b.Property<int>("Status");

                    b.HasKey("ID");

                    b.HasIndex("CharityMakerID");

                    b.HasIndex("DonationItemID");

                    b.HasIndex("OrphanageID");

                    b.ToTable("Donations");
                });

            modelBuilder.Entity("FamilyNet.Models.DonationItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Price");

                    b.HasKey("ID");

                    b.ToTable("DonationItem");
                });

            modelBuilder.Entity("FamilyNet.Models.Orphan", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Avatar");

                    b.Property<DateTime>("Birthday");

                    b.Property<bool>("ChildInOrphanage");

                    b.Property<bool>("Confirmation");

                    b.Property<int>("EmailID");

                    b.Property<bool>("IsDeleted");

                    b.Property<int?>("OrphanageID")
                        .IsRequired();

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("OrphanageID");

                    b.ToTable("Orphans");
                });

            modelBuilder.Entity("FamilyNet.Models.Orphanage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AdressID");

                    b.Property<string>("Avatar");

                    b.Property<bool>("IsDeleted");

                    b.Property<float?>("MapCoordX");

                    b.Property<float?>("MapCoordY");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AdressID")
                        .IsUnique()
                        .HasFilter("[AdressID] IS NOT NULL");

                    b.ToTable("Orphanages");
                });

            modelBuilder.Entity("FamilyNet.Models.Representative", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Avatar");

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("EmailID");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("OrphanageID");

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("OrphanageID");

                    b.ToTable("Representatives");
                });

            modelBuilder.Entity("FamilyNet.Models.Volunteer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressID");

                    b.Property<string>("Avatar");

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("EmailID");

                    b.Property<bool>("IsDeleted");

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AddressID")
                        .IsUnique()
                        .HasFilter("[AddressID] IS NOT NULL");

                    b.ToTable("Volunteers");
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLotItemType", b =>
                {
                    b.HasBaseType("FamilyNet.Models.BaseItemType");

                    b.Property<int?>("ItemID");

                    b.HasIndex("ItemID");

                    b.HasDiscriminator().HasValue("AuctionLotItemType");
                });

            modelBuilder.Entity("FamilyNet.Models.DonationItemType", b =>
                {
                    b.HasBaseType("FamilyNet.Models.BaseItemType");

                    b.Property<int?>("ItemID")
                        .HasColumnName("DonationItemType_ItemID");

                    b.HasIndex("ItemID");

                    b.HasDiscriminator().HasValue("DonationItemType");
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLot", b =>
                {
                    b.HasOne("FamilyNet.Models.AuctionLotItem", "AuctionLotItem")
                        .WithMany()
                        .HasForeignKey("AuctionLotItemID");

                    b.HasOne("FamilyNet.Models.Orphan", "Orphan")
                        .WithMany("AuctionLots")
                        .HasForeignKey("OrphanID");
                });

            modelBuilder.Entity("FamilyNet.Models.BaseItemType", b =>
                {
                    b.HasOne("FamilyNet.Models.BaseItemType", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentID");
                });

            modelBuilder.Entity("FamilyNet.Models.CharityMaker", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Address")
                        .WithOne()
                        .HasForeignKey("FamilyNet.Models.CharityMaker", "AddressID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.OwnsOne("FamilyNet.Models.FullName", "FullName", b1 =>
                        {
                            b1.Property<int>("CharityMakerID")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Name")
                                .IsRequired();

                            b1.Property<string>("Patronymic")
                                .IsRequired();

                            b1.Property<string>("Surname")
                                .IsRequired();

                            b1.HasKey("CharityMakerID");

                            b1.ToTable("CharityMakers");

                            b1.HasOne("FamilyNet.Models.CharityMaker")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNet.Models.FullName", "CharityMakerID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNet.Models.Donation", b =>
                {
                    b.HasOne("FamilyNet.Models.CharityMaker", "CharityMaker")
                        .WithMany("Donations")
                        .HasForeignKey("CharityMakerID");

                    b.HasOne("FamilyNet.Models.DonationItem", "DonationItem")
                        .WithMany()
                        .HasForeignKey("DonationItemID");

                    b.HasOne("FamilyNet.Models.Orphanage", "Orphanage")
                        .WithMany("Donations")
                        .HasForeignKey("OrphanageID");
                });

            modelBuilder.Entity("FamilyNet.Models.Orphan", b =>
                {
                    b.HasOne("FamilyNet.Models.Orphanage", "Orphanage")
                        .WithMany("Orphans")
                        .HasForeignKey("OrphanageID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("FamilyNet.Models.FullName", "FullName", b1 =>
                        {
                            b1.Property<int>("OrphanID")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Name")
                                .IsRequired();

                            b1.Property<string>("Patronymic")
                                .IsRequired();

                            b1.Property<string>("Surname")
                                .IsRequired();

                            b1.HasKey("OrphanID");

                            b1.ToTable("Orphans");

                            b1.HasOne("FamilyNet.Models.Orphan")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNet.Models.FullName", "OrphanID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNet.Models.Orphanage", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Adress")
                        .WithOne()
                        .HasForeignKey("FamilyNet.Models.Orphanage", "AdressID")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("FamilyNet.Models.Representative", b =>
                {
                    b.HasOne("FamilyNet.Models.Orphanage", "Orphanage")
                        .WithMany("Representatives")
                        .HasForeignKey("OrphanageID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("FamilyNet.Models.FullName", "FullName", b1 =>
                        {
                            b1.Property<int>("RepresentativeID")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Name")
                                .IsRequired();

                            b1.Property<string>("Patronymic")
                                .IsRequired();

                            b1.Property<string>("Surname")
                                .IsRequired();

                            b1.HasKey("RepresentativeID");

                            b1.ToTable("Representatives");

                            b1.HasOne("FamilyNet.Models.Representative")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNet.Models.FullName", "RepresentativeID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNet.Models.Volunteer", b =>
                {
                    b.HasOne("FamilyNet.Models.Address", "Address")
                        .WithOne()
                        .HasForeignKey("FamilyNet.Models.Volunteer", "AddressID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.OwnsOne("FamilyNet.Models.FullName", "FullName", b1 =>
                        {
                            b1.Property<int>("VolunteerID")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Name")
                                .IsRequired();

                            b1.Property<string>("Patronymic")
                                .IsRequired();

                            b1.Property<string>("Surname")
                                .IsRequired();

                            b1.HasKey("VolunteerID");

                            b1.ToTable("Volunteers");

                            b1.HasOne("FamilyNet.Models.Volunteer")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNet.Models.FullName", "VolunteerID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNet.Models.AuctionLotItemType", b =>
                {
                    b.HasOne("FamilyNet.Models.AuctionLotItem", "Item")
                        .WithMany("AuctionLotItemTypes")
                        .HasForeignKey("ItemID");
                });

            modelBuilder.Entity("FamilyNet.Models.DonationItemType", b =>
                {
                    b.HasOne("FamilyNet.Models.DonationItem", "Item")
                        .WithMany("DonationItemTypes")
                        .HasForeignKey("ItemID");
                });
#pragma warning restore 612, 618
        }
    }
}
