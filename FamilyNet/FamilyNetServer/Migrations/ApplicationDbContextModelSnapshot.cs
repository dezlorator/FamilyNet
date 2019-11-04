﻿// <auto-generated />
using System;
using FamilyNetServer.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FamilyNetServer.Migrations
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

            modelBuilder.Entity("FamilyNetServer.Models.Address", b =>
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

            modelBuilder.Entity("FamilyNetServer.Models.AuctionLot", b =>
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

            modelBuilder.Entity("FamilyNetServer.Models.BaseItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Price");

                    b.HasKey("ID");

                    b.ToTable("BaseItem");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseItem");
                });

            modelBuilder.Entity("FamilyNetServer.Models.BaseItemType", b =>
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

            modelBuilder.Entity("FamilyNetServer.Models.CharityMaker", b =>
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

            modelBuilder.Entity("FamilyNetServer.Models.Donation", b =>
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

                    b.HasIndex("DonationItemID")
                        .IsUnique()
                        .HasFilter("[DonationItemID] IS NOT NULL");

                    b.HasIndex("OrphanageID");

                    b.ToTable("Donations");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Location", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsDeleted");

                    b.Property<float?>("MapCoordX");

                    b.Property<float?>("MapCoordY");

                    b.HasKey("ID");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Orphan", b =>
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

            modelBuilder.Entity("FamilyNetServer.Models.Orphanage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AdressID");

                    b.Property<string>("Avatar");

                    b.Property<bool>("IsDeleted");

                    b.Property<int?>("LocationID");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Rating");

                    b.HasKey("ID");

                    b.HasIndex("AdressID")
                        .IsUnique()
                        .HasFilter("[AdressID] IS NOT NULL");

                    b.HasIndex("LocationID");

                    b.ToTable("Orphanages");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Quest", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<int?>("DonationID");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.Property<int?>("VolunteerID");

                    b.HasKey("ID");

                    b.HasIndex("DonationID");

                    b.HasIndex("VolunteerID");

                    b.ToTable("Quests");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Representative", b =>
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

            modelBuilder.Entity("FamilyNetServer.Models.TypeBaseItem", b =>
                {
                    b.Property<int>("ItemID");

                    b.Property<int>("TypeID");

                    b.HasKey("ItemID", "TypeID");

                    b.HasIndex("TypeID");

                    b.ToTable("TypeBaseItems");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Volunteer", b =>
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

            modelBuilder.Entity("FamilyNetServer.Models.AuctionLotItem", b =>
                {
                    b.HasBaseType("FamilyNetServer.Models.BaseItem");

                    b.HasDiscriminator().HasValue("AuctionLotItem");
                });

            modelBuilder.Entity("FamilyNetServer.Models.DonationItem", b =>
                {
                    b.HasBaseType("FamilyNetServer.Models.BaseItem");

                    b.HasDiscriminator().HasValue("DonationItem");
                });

            modelBuilder.Entity("FamilyNetServer.Models.AuctionLotItemType", b =>
                {
                    b.HasBaseType("FamilyNetServer.Models.BaseItemType");

                    b.Property<int?>("ItemID");

                    b.HasIndex("ItemID");

                    b.HasDiscriminator().HasValue("AuctionLotItemType");
                });

            modelBuilder.Entity("FamilyNetServer.Models.AuctionLot", b =>
                {
                    b.HasOne("FamilyNetServer.Models.AuctionLotItem", "AuctionLotItem")
                        .WithMany()
                        .HasForeignKey("AuctionLotItemID");

                    b.HasOne("FamilyNetServer.Models.Orphan", "Orphan")
                        .WithMany("AuctionLots")
                        .HasForeignKey("OrphanID");
                });

            modelBuilder.Entity("FamilyNetServer.Models.BaseItemType", b =>
                {
                    b.HasOne("FamilyNetServer.Models.BaseItemType", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentID");
                });

            modelBuilder.Entity("FamilyNetServer.Models.CharityMaker", b =>
                {
                    b.HasOne("FamilyNetServer.Models.Address", "Address")
                        .WithOne()
                        .HasForeignKey("FamilyNetServer.Models.CharityMaker", "AddressID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.OwnsOne("FamilyNetServer.Models.FullName", "FullName", b1 =>
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

                            b1.HasOne("FamilyNetServer.Models.CharityMaker")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNetServer.Models.FullName", "CharityMakerID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNetServer.Models.Donation", b =>
                {
                    b.HasOne("FamilyNetServer.Models.CharityMaker", "CharityMaker")
                        .WithMany("Donations")
                        .HasForeignKey("CharityMakerID");

                    b.HasOne("FamilyNetServer.Models.DonationItem", "DonationItem")
                        .WithOne()
                        .HasForeignKey("FamilyNetServer.Models.Donation", "DonationItemID");

                    b.HasOne("FamilyNetServer.Models.Orphanage", "Orphanage")
                        .WithMany("Donations")
                        .HasForeignKey("OrphanageID");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Orphan", b =>
                {
                    b.HasOne("FamilyNetServer.Models.Orphanage", "Orphanage")
                        .WithMany("Orphans")
                        .HasForeignKey("OrphanageID");

                    b.OwnsOne("FamilyNetServer.Models.FullName", "FullName", b1 =>
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

                            b1.HasOne("FamilyNetServer.Models.Orphan")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNetServer.Models.FullName", "OrphanID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNetServer.Models.Orphanage", b =>
                {
                    b.HasOne("FamilyNetServer.Models.Address", "Adress")
                        .WithOne()
                        .HasForeignKey("FamilyNetServer.Models.Orphanage", "AdressID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("FamilyNetServer.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationID");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Quest", b =>
                {
                    b.HasOne("FamilyNetServer.Models.Donation", "Donation")
                        .WithMany()
                        .HasForeignKey("DonationID");

                    b.HasOne("FamilyNetServer.Models.Volunteer", "Volunteer")
                        .WithMany()
                        .HasForeignKey("VolunteerID");
                });

            modelBuilder.Entity("FamilyNetServer.Models.Representative", b =>
                {
                    b.HasOne("FamilyNetServer.Models.Orphanage", "Orphanage")
                        .WithMany("Representatives")
                        .HasForeignKey("OrphanageID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("FamilyNetServer.Models.FullName", "FullName", b1 =>
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

                            b1.HasOne("FamilyNetServer.Models.Representative")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNetServer.Models.FullName", "RepresentativeID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNetServer.Models.TypeBaseItem", b =>
                {
                    b.HasOne("FamilyNetServer.Models.BaseItem", "Item")
                        .WithMany("TypeBaseItem")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FamilyNetServer.Models.BaseItemType", "Type")
                        .WithMany("TypeBaseItem")
                        .HasForeignKey("TypeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FamilyNetServer.Models.Volunteer", b =>
                {
                    b.HasOne("FamilyNetServer.Models.Address", "Address")
                        .WithOne()
                        .HasForeignKey("FamilyNetServer.Models.Volunteer", "AddressID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.OwnsOne("FamilyNetServer.Models.FullName", "FullName", b1 =>
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

                            b1.HasOne("FamilyNetServer.Models.Volunteer")
                                .WithOne("FullName")
                                .HasForeignKey("FamilyNetServer.Models.FullName", "VolunteerID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("FamilyNetServer.Models.AuctionLotItemType", b =>
                {
                    b.HasOne("FamilyNetServer.Models.AuctionLotItem", "Item")
                        .WithMany("AuctionLotItemTypes")
                        .HasForeignKey("ItemID");
                });
#pragma warning restore 612, 618
        }
    }
}
