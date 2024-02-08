using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CadTest
{
    public class RibbonView
    {
        public static RibbonView Instance => _instance ?? (_instance = new RibbonView());
        private static RibbonView _instance;
        private static readonly Guid _guidManhole = new Guid("32DDFFD8-DC4E-4D11-A695-A991C86CFBF7");
        private const string TabName = "MyTab";
        private bool _hasCreated;
        private PaletteSet _paletteSet;
        private RibbonToggleButton _toggleButtonPaletteSet;
        private UserControl1 _uc;
        private RibbonButton _btnAdd;
        private RibbonButton _btnDelete;
        private RibbonButton _btnUpdate;
        private RibbonButton _btnCalculate;

        private RibbonView()
        {
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;
        }
        public void ShowRibbonTab(Document doc)
        {
            if (!_hasCreated)
            {
                CreateRibbonTab();
                _hasCreated = true;
            }
            if (doc != null)
            {
                RibbonManager.GetRibbonTab(TabName);
                BindViewModel(doc);
            }
        }
        private void CreateRibbonTab()
        {
            CreateTogglePanel();
            CreateOperatePanel();
            CreateCalculatePanel();
        }
        private void CreateTogglePanel()
        {
            var ribbonPanel = RibbonManager.GetRibbonPanel(TabName, "Panel1");
            _toggleButtonPaletteSet = new RibbonToggleButton().Init("Palette\nSet", RibbonItemSize.Large, "Resources\\Icon.png");
            _toggleButtonPaletteSet.Orientation = System.Windows.Controls.Orientation.Vertical;
            //创建PaletteSet面板
            _paletteSet = new PaletteSet("Panel1", _guidManhole);//有guid时Civil3D会记住面板位置
            _uc = new UserControl1();
            _paletteSet.AddVisual("My UC", _uc, true);
            _paletteSet.Activate(0);
            _paletteSet.Icon = ImageHelper.GetIconInternal("Resources\\Icon.ico");
            _paletteSet.StateChanged += (s, e) =>
            {
                if (e.NewState == StateEventIndex.Hide)
                {
                    _toggleButtonPaletteSet.IsChecked = false;
                }
                else if (e.NewState == StateEventIndex.Show)
                {
                    _toggleButtonPaletteSet.IsChecked = true;
                }
            };
            _toggleButtonPaletteSet.SetCommand(() =>
            {
                if (_toggleButtonPaletteSet.IsChecked)
                {
                    _paletteSet.Visible = true;
                    _paletteSet.Activate(0);
                    _paletteSet.Size = new System.Drawing.Size(1200, 270);
                    _paletteSet.Dock = DockSides.Bottom;
                }
                else
                {
                    _paletteSet.Visible = false;
                }
            });
            ribbonPanel.Source.Items.Add(_toggleButtonPaletteSet);
        }
        private void CreateOperatePanel()
        {
            var ribbonPanel = RibbonManager.GetRibbonPanel(TabName, "Operate");
            var flowPanel = ribbonPanel.AddRibbonFlowPanel();
            string toolTip = "toolTip";
            flowPanel.Source.Items.Add(_btnAdd = new RibbonButton().Init("Add", RibbonItemSize.Standard, "Resources\\Add.png", toolTip));
            flowPanel.Source.Items.Add(_btnDelete = new RibbonButton().Init("Delete", RibbonItemSize.Standard, "Resources\\Delete.png", toolTip));
            flowPanel.Source.Items.Add(_btnUpdate = new RibbonButton().Init("Update", RibbonItemSize.Standard, "Resources\\Update.png", toolTip));
        }
        private void CreateCalculatePanel()
        {
            var ribbonPanel = RibbonManager.GetRibbonPanel(TabName, "Calculate");
            ribbonPanel.Source.Items.Add(_btnCalculate = new RibbonButton().Init("Calculate", RibbonItemSize.Large, "Resources\\calculate.png"));
        }

        private void BindViewModel(Document doc)
        {
            ViewModel viewModel = new ViewModel();
            _uc.BindViewModel(viewModel);
            _btnAdd.SetCommand(viewModel.Add);
            _btnDelete.SetCommand(viewModel.Add);
            _btnUpdate.SetCommand(viewModel.Add);
            _btnCalculate.SetCommand(viewModel.Add);
        }

        private class ViewModel
        {
            internal void Add()
            {
                throw new NotImplementedException();
            }
        }

        private void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            try
            {
                if (e.Document != null)
                {
                    ShowRibbonTab(e.Document);
                }
                else if (_paletteSet != null)
                {
                    _paletteSet.Visible = false;
                    _toggleButtonPaletteSet.IsChecked = false;
                }
            }
            catch (System.Exception ex)
            {
                WindowManager.ShowException(ex);
            }
        }
        private void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            try
            {
                //RemoveViewModelMediator(e.FileName);
            }
            catch (System.Exception ex)
            {
                WindowManager.ShowException(ex);
            }
        }
    }
}
