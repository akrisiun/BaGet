﻿using System;
using System.IO;
using System.Diagnostics;
using BaGet.Core.Configuration;

namespace BaGet.Core.Extensions
{
    public static class OptionsExtensions
    {
        public static DatabaseOptions EnsureValid(this DatabaseOptions options)
        {
            if (options == null) 
            {
                if (Debugger.IsAttached) {
                    options = new DatabaseOptions { 
                        Type = DatabaseType.Sqlite,
                        ConnectionString = "Data Source=baget.db"
                    };
                } else {
                    ThrowMissingConfiguration(nameof(BaGetOptions.Database));
                }
            }

            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                ThrowMissingConfiguration(
                    nameof(BaGetOptions.Database),
                    nameof(DatabaseOptions.ConnectionString));
            }
            
            return options;
        }

        public static void EnsureValid(this StorageOptions options)
        {
            if (options == null) ThrowMissingConfiguration(nameof(BaGetOptions.Storage));
        }

        public static void EnsureValid(this FileSystemStorageOptions options)
        {
            if (options == null) ThrowMissingConfiguration(nameof(BaGetOptions.Storage));

            options.Path = string.IsNullOrEmpty(options.Path)
                ? Path.Combine(Directory.GetCurrentDirectory(), "Packages")
                : options.Path;

            // Ensure the package storage directory exists
            Directory.CreateDirectory(options.Path);
        }

        public static void EnsureValid(this SearchOptions options)
        {
            if (options == null) ThrowMissingConfiguration(nameof(BaGetOptions.Search));
        }

        public static void EnsureValid(this MirrorOptions options)
        {
            if (options == null)
            {
                ThrowMissingConfiguration(nameof(BaGetOptions.Mirror));
            }

            if (!options.Enabled) return;

            if (options.PackageSource == null)
            {
                ThrowMissingConfiguration(
                    nameof(BaGetOptions.Mirror),
                    nameof(MirrorOptions.PackageSource));
            }

            if (options.PackageDownloadTimeoutSeconds <= 0)
            {
                options.PackageDownloadTimeoutSeconds = 600;
            }
        }

        public static void ThrowMissingConfiguration(params string[] segments)
        {
            var name = string.Join(":", segments);

            throw new InvalidOperationException($"The '{name}' configuration is missing");
        }
    }
}
