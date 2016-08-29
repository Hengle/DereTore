﻿using System;
using DereTore.Application.ScoreEditor.Forms;
using App = System.Windows.Forms.Application;

namespace DereTore.Application.ScoreEditor {
    internal static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main() {
            App.EnableVisualStyles();
            App.SetCompatibleTextRenderingDefault(false);
            App.Run(new FViewer());
        }
    }
}