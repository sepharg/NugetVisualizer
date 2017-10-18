﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using NugetVisualizer.Core.Repositories;
using System;

namespace NugetVisualizer.Core.Migrations
{
    [DbContext(typeof(NugetVisualizerContext))]
    partial class NugetVisualizerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("NugetVisualizer.Core.Domain.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("TargetFramework");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.HasIndex("Name", "Version")
                        .IsUnique();

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("NugetVisualizer.Core.Domain.Project", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Name");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("NugetVisualizer.Core.Domain.ProjectPackage", b =>
                {
                    b.Property<string>("ProjectName");

                    b.Property<int>("PackageId");

                    b.Property<int>("SnapshotVersion");

                    b.HasKey("ProjectName", "PackageId", "SnapshotVersion");

                    b.HasIndex("PackageId");

                    b.ToTable("ProjectPackages");
                });

            modelBuilder.Entity("NugetVisualizer.Core.Domain.Snapshot", b =>
                {
                    b.Property<int>("Version")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Version");

                    b.ToTable("Snapshots");
                });

            modelBuilder.Entity("NugetVisualizer.Core.Domain.ProjectPackage", b =>
                {
                    b.HasOne("NugetVisualizer.Core.Domain.Package", "Package")
                        .WithMany("ProjectPackages")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NugetVisualizer.Core.Domain.Project", "Project")
                        .WithMany("ProjectPackages")
                        .HasForeignKey("ProjectName")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
