﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xenon.Data;

#nullable disable

namespace Xenon.Migrations
{
    [DbContext(typeof(ModHelperContext))]
    partial class ModHelperContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("Xenon.Data.UserChannelChange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("JoinOrLeaveDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("JoinedChannelID")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("LeftChannelID")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ChannelChanges");
                });

            modelBuilder.Entity("Xenon.Data.UserMute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("AppliedRoleID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Expiry")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("Moderator")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Mutes");
                });
#pragma warning restore 612, 618
        }
    }
}
