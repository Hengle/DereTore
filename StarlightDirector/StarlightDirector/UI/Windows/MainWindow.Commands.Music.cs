﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using StarlightDirector.Extensions;
using AudioOut = NAudio.Wave.WasapiOut;

namespace StarlightDirector.UI.Windows {
    partial class MainWindow {

        public static readonly ICommand CmdMusicSelectWaveFile = CommandHelper.RegisterCommand();
        public static readonly ICommand CmdMusicPlay = CommandHelper.RegisterCommand();
        public static readonly ICommand CmdMusicStop = CommandHelper.RegisterCommand();

        private void CmdMusicSelectWaveFile_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Editor.Project != null;
        }

        private void CmdMusicSelectWaveFile_Executed(object sender, ExecutedRoutedEventArgs e) {
            var openDialog = new OpenFileDialog();
            openDialog.CheckPathExists = true;
            openDialog.Multiselect = false;
            openDialog.ShowReadOnly = false;
            openDialog.ReadOnlyChecked = false;
            openDialog.ValidateNames = true;
            openDialog.Filter = Application.Current.FindResource<string>(App.ResourceKeys.WaveFileFilter);
            var dialogResult = openDialog.ShowDialog();
            if (dialogResult ?? false) {
                Editor.Project.MusicFileName = openDialog.FileName;
                CmdMusicSelectWaveFile.RaiseCanExecuteChanged();
                NotifyProjectChanged();
            }
        }

        private void CmdMusicPlay_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            var r = Editor.Project?.HasMusic ?? false;
            if (r) {
                if (_selectedWaveOut != null) {
                    r = _selectedWaveOut.PlaybackState != PlaybackState.Playing;
                } else {
                    r = Project.HasMusic;
                }
            }
            e.CanExecute = r;
        }

        private void CmdMusicPlay_Executed(object sender, ExecutedRoutedEventArgs e) {
            PlayMusic(0);
        }

        private void CmdMusicStop_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            var r = Editor.Project?.HasMusic ?? false;
            if (r) {
                r = _selectedWaveOut != null && _selectedWaveOut.PlaybackState == PlaybackState.Playing;
            }
            e.CanExecute = r;
        }

        private void CmdMusicStop_Executed(object sender, ExecutedRoutedEventArgs e) {
            StopMusic();
        }

        private void SelectedWaveOut_PlaybackStopped(object sender, EventArgs e) {
            var waveOut = _selectedWaveOut;
            if (waveOut != null) {
                waveOut.PlaybackStopped -= SelectedWaveOut_PlaybackStopped;
                waveOut.Dispose();
                _selectedWaveOut = null;
            }
            _waveReader?.Dispose();
            _waveReader = null;
        }

        internal bool MusicLoaded => Editor.Project?.HasMusic ?? false;

        internal void PlayMusic(double startOffsetInMillis) {
            var o = _selectedWaveOut;
            if (o == null) {
                o = new AudioOut(AudioClientShareMode.Shared, 0);
                var fileStream = new FileStream(Project.MusicFileName, FileMode.Open, FileAccess.Read);
                _waveReader = new WaveFileReader(fileStream);
                o.PlaybackStopped += SelectedWaveOut_PlaybackStopped;
                o.Init(_waveReader);
                _selectedWaveOut = o;
            }

            if (o.PlaybackState == PlaybackState.Playing) {
                o.Stop();
            }

            _waveReader.CurrentTime = TimeSpan.FromMilliseconds(startOffsetInMillis);
            o.Play();
            CmdMusicPlay.RaiseCanExecuteChanged();
        }

        internal void StopMusic() {
            var o = _selectedWaveOut;
            o?.Stop();
            SelectedWaveOut_PlaybackStopped(o, EventArgs.Empty);
            CmdMusicStop.RaiseCanExecuteChanged();
        }

        internal TimeSpan? GetCurrentMusicTime() {
            return _waveReader?.CurrentTime;
        }

        internal TimeSpan? GetMusicTotalTime() {
            return _waveReader?.TotalTime;
        }

        private AudioOut _selectedWaveOut;
        private WaveFileReader _waveReader;

    }
}
