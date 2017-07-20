﻿using Captura.Models;
using Captura.Properties;
using Ookii.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Captura
{
    public static class FFMpegService
    {
        public static bool FFMpegExists
        {
            get
            {
                var exePath = FFMpegExePath;

                if (File.Exists(exePath))
                    return true;

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = "-version",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    return true;
                }
                catch { return false; }
            }
        }

        public static string FFMpegExePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Settings.Instance.FFMpegFolder))
                    return "ffmpeg.exe";

                return Path.Combine(Settings.Instance.FFMpegFolder, "ffmpeg.exe");
            }
        }

        public static void SelectFFMpegFolder()
        {
            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Settings.Instance.FFMpegFolder,
                UseDescriptionForTitle = true,
                Description = Resources.SelectFFMpegFolder
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    Settings.Instance.FFMpegFolder = dlg.SelectedPath;
            }
        }

        public static Action FFMpegDownloader { get; set; }

        public static Process StartFFMpeg(string Arguments)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = FFMpegExePath,
                    Arguments = Arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                },
                EnableRaisingEvents = true
            };
                        
            process.ErrorDataReceived += (s, e) => FFMpegLog.Instance.Write(e.Data);

            process.Start();

            process.BeginErrorReadLine();
            
            return process;
        }
    }
}