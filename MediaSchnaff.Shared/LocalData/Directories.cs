﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaSchnaff.Shared.LocalData
{
    public interface IDirectories
    {
        string StorageDir { get; }
        string LogFile {  get; }
        string ThumbnailDir { get; }
    }

    public class Directories : IDirectories
    {
        public string StorageDir
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "MediaSchnaff");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string ThumbnailDir
        {
            get
            {
                var dir = Path.Combine(
                    StorageDir,
                    "Thumbnails");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string LogDir
        {
            get
            {
                var dir = Path.Combine(
                    StorageDir,
                    "Log");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string LogFile
        {
            get
            {
                return Path.Combine(LogDir, "Log.txt");
            }
        }
    }
}