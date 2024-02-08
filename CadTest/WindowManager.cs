using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CadTest
{
    public class WindowManager
    {
        private static readonly Dictionary<string, Window> dicWindows = new Dictionary<string, Window>();

        public static void ShowWindow(Window window)
        {
            Application.ShowModelessWindow(Application.MainWindow.Handle, window, false);
        }
        public static void ShowWindow(string key, Func<Window> createWindow)
        {
            if (!dicWindows.ContainsKey(key))
            {
                dicWindows[key] = createWindow();
                dicWindows[key].Closed += (s, e) => dicWindows.Remove(key);
                ShowWindow(dicWindows[key]);
            }
            else
            {
                try
                {
                    dicWindows[key].Visibility = Visibility.Visible;
                    dicWindows[key].WindowState = WindowState.Normal;
                    dicWindows[key].Focus();
                }
                catch (Exception)
                {
                    dicWindows.Remove(key);
                    ShowWindow(key, createWindow);
                }
            }
        }
        public static void ShowWindow<T>(Func<Window> creatWindow) where T : Window
        {
            string key = typeof(T).FullName;
            ShowWindow(key, creatWindow);
        }
        public static void ShowWindow<T>(params object[] args) where T : Window
        {
            Window creatWindow() => Activator.CreateInstance(typeof(T), args) as T;
            ShowWindow<T>(creatWindow);
        }

        public static bool? ShowDialogWindow(Window window)
        {
            return Application.ShowModalWindow(Application.MainWindow.Handle, window, false);
        }
        public static bool? ShowDialogWindow(string key, Func<Window> creatWindow)
        {
            dicWindows[key] = creatWindow();
            dicWindows[key].Closed += (s, e) => dicWindows.Remove(key);
            return ShowDialogWindow(dicWindows[key]);
        }
        public static bool? ShowDialogWindow<T>(Func<Window> creatWindow) where T : Window
        {
            string key = typeof(T).FullName;
            return ShowDialogWindow(key, creatWindow);
        }
        public static bool? ShowDialogWindow<T>(params object[] args) where T : Window
        {
            Window creatWindow() => Activator.CreateInstance(typeof(T), args) as T;
            return ShowDialogWindow<T>(creatWindow);
        }

        public static void CloseWindow(string key)
        {
            if (dicWindows.ContainsKey(key))
            {
                dicWindows[key].Close();
            }
        }
        public static void CloseWindow<T>()
        {
            string key = typeof(T).FullName;
            CloseWindow(key);
        }

        public static void ShowAlertDialog(string message)
        {
            Application.ShowAlertDialog(message);
        }
        public static void ShowException(Exception ex)
        {
            ShowAlertDialog(ex.Message);
            //log
        }
        public static bool ShowYesNoDialog(string title, string message)
        {
            var result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public static bool ShowSaveFileDialog(string title, string defaultPath, out string filePath)
        {
            string ext = System.IO.Path.GetExtension(defaultPath).TrimStart('.');
            string defaultName = System.IO.Path.ChangeExtension(defaultPath, null);
            var dialog = new Autodesk.AutoCAD.Windows.SaveFileDialog(title, defaultName, ext, title, default);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.Filename;
                return true;
            }
            filePath = default;
            return false;
        }
        public static bool ShowOpenFileOrFolderDialog(string title, string defaultPath, bool onlyFolders, out string filePath)
        {
            string ext = System.IO.Path.GetExtension(defaultPath).TrimStart('.');
            string defaultName = System.IO.Path.ChangeExtension(defaultPath, null);
            Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags flags = default;
            if (onlyFolders)
            {
                flags |= Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.AllowFoldersOnly;
            }
            var dialog = new Autodesk.AutoCAD.Windows.OpenFileOrFolderDialog(title, defaultName, ext, title, flags);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.FileOrFoldername;
                return true;
            }
            filePath = default;
            return false;
        }
    }
}
