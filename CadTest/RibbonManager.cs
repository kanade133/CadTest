using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CadTest
{
    internal static class RibbonManager
    {
        private static readonly Dictionary<string, RibbonTab> _dicRibbon = new Dictionary<string, RibbonTab>();
        public static RibbonTab GetRibbonTab(string tabName)
        {
            var ribbonControl = ComponentManager.Ribbon;
            RibbonTab ribbonTab = null;
            foreach (var tab in ribbonControl.Tabs)
            {
                if (tab.AutomationName == tabName)
                {
                    ribbonTab = tab;
                    break;
                }
            }
            if (ribbonTab == null)
            {
                if (_dicRibbon.TryGetValue(tabName, out ribbonTab))
                {
                    ribbonControl.Tabs.Add(ribbonTab);
                }
                else
                {
                    ribbonTab = new RibbonTab();
                    ribbonTab.Title = tabName;
                    ribbonTab.Id = $"{tabName}Tab";//no Id will dialog error while civil3D closing
                    ribbonControl.Tabs.Add(ribbonTab);
                    _dicRibbon[tabName] = ribbonTab;
                }
            }
            else if (ribbonTab.IsVisible == false)
            {
                ribbonTab.IsVisible = true;
            }
            return ribbonTab;
        }
        public static RibbonPanel GetRibbonPanel(string tabName, string panelName)
        {
            var ribbonTab = RibbonManager.GetRibbonTab(tabName);
            RibbonPanel ribbonPanel = null;
            foreach (var panel in ribbonTab.Panels)
            {
                if (panel.Source.AutomationName == panelName)
                {
                    ribbonPanel = panel;
                    break;
                }
            }
            if (ribbonPanel == null)
            {
                var ribbonPanelSource = new RibbonPanelSource();
                ribbonPanelSource.Title = panelName;
                ribbonPanelSource.Id = $"{panelName}PanelSource";
                ribbonPanel = new RibbonPanel();
                ribbonPanel.Source = ribbonPanelSource;
                ribbonPanel.Id = $"{panelName}Panel";
                ribbonTab.Panels.Add(ribbonPanel);
            }
            return ribbonPanel;
        }
        public static RibbonRowPanel AddRibbonRowPanel(this RibbonPanel ribbonPanel)
        {
            var rowPanel = new RibbonRowPanel();
            rowPanel.IsTopJustified = true;//顶部对齐
            rowPanel.Source = new RibbonSubPanelSource();
            ribbonPanel.Source.Items.Add(rowPanel);
            return rowPanel;
        }
        public static RibbonRowPanel AddRibbonRowPanel(this RibbonFlowPanel flowPanel)
        {
            var rowPanel = new RibbonRowPanel();
            rowPanel.IsTopJustified = true;//顶部对齐
            rowPanel.Source = new RibbonSubPanelSource();
            flowPanel.Source.Items.Add(rowPanel);
            return rowPanel;
        }
        public static RibbonFlowPanel AddRibbonFlowPanel(this RibbonPanel ribbonPanel)
        {
            var flowPanel = new RibbonFlowPanel();
            flowPanel.IsTopJustified = true;//顶部对齐
            flowPanel.Source = new RibbonSubPanelSource();
            ribbonPanel.Source.Items.Add(flowPanel);
            return flowPanel;
        }
        public static T Init<T>(this T ribbonItem, string text, RibbonItemSize size, string imageRelativePath) where T : RibbonItem
        {
            ribbonItem.Text = text;
            ribbonItem.ShowText = true;
            ribbonItem.ShowImage = true;
            ribbonItem.Size = size;
            if (size == RibbonItemSize.Standard)
            {
                ribbonItem.Image = ImageHelper.GetImageSourceInternal(imageRelativePath);
                if (ribbonItem is RibbonButton button)
                {
                    button.Orientation = System.Windows.Controls.Orientation.Horizontal;
                }
            }
            else if (size == RibbonItemSize.Large)
            {
                ribbonItem.LargeImage = ImageHelper.GetImageSourceInternal(imageRelativePath);
                if (ribbonItem is RibbonButton button)
                {
                    button.Orientation = System.Windows.Controls.Orientation.Vertical;
                }
            }
            return ribbonItem;
        }
        public static T Init<T>(this T ribbonItem, string text, RibbonItemSize size, string imageRelativePath, string toolTip) where T : RibbonItem
        {
            ribbonItem.Text = text;
            ribbonItem.ShowText = true;
            ribbonItem.ShowImage = true;
            ribbonItem.Size = size;
            if (size == RibbonItemSize.Standard)
            {
                ribbonItem.Image = ImageHelper.GetImageSourceInternal(imageRelativePath);
                if (ribbonItem is RibbonButton button)
                {
                    button.Orientation = System.Windows.Controls.Orientation.Horizontal;
                }
            }
            else if (size == RibbonItemSize.Large)
            {
                ribbonItem.LargeImage = ImageHelper.GetImageSourceInternal(imageRelativePath);
                if (ribbonItem is RibbonButton button)
                {
                    button.Orientation = System.Windows.Controls.Orientation.Vertical;
                }
            }
            ribbonItem.ToolTip = new RibbonToolTip() { Title = text, Content = toolTip };
            return ribbonItem;
        }
        public static T SetCommand<T>(this T ribbonCommandItem, Action action) where T : RibbonCommandItem
        {
            ribbonCommandItem.CommandHandler = new RibbonCommandHandle(action);
            return ribbonCommandItem;
        }
        public static T SetCommand<T>(this T ribbonCommandItem, string command) where T : RibbonCommandItem
        {
            ribbonCommandItem.CommandHandler = new RibbonCommandHandle(command);
            return ribbonCommandItem;
        }

        private class RibbonCommandHandle : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private readonly Action action;
            private string command;

            public RibbonCommandHandle() { }
            public RibbonCommandHandle(Action action)
            {
                this.action = action;
            }
            public RibbonCommandHandle(string command)
            {
                this.command = command;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }
            public void Execute(object parameter)
            {
                if (action != null)
                {
                    action();
                }
                else
                {
                    if (string.IsNullOrEmpty(command) && parameter is RibbonCommandItem rci)
                    {
                        command = rci.CommandParameter?.ToString();
                    }
                    if (!string.IsNullOrEmpty(command))
                    {
                        if (!command.EndsWith(" ") && !command.EndsWith("\n"))
                        {
                            command += "\n";
                        }
                        Application.DocumentManager.MdiActiveDocument.SendStringToExecute(command, true, false, true);
                    }
                }
            }
        }
    }
}
