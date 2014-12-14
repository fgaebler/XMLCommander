using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Xml;
using System.Drawing;
using System.IO;

namespace XMLCommander
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Window Drag & Resize
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        //Attach this to the MouseDown event of your drag control to move the window in place of the title bar
        private void WindowDrag(object sender, MouseButtonEventArgs e) // MouseDown
        {
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle,
                0xA1, (IntPtr)0x2, (IntPtr)0);
        }

        //Attach this to the PreviewMousLeftButtonDown event of the grip control in the lower right corner of the form to resize the window
        private void WindowResize(object sender, MouseButtonEventArgs e) //PreviewMousLeftButtonDown
        {
            HwndSource hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            SendMessage(hwndSource.Handle, 0x112, (IntPtr)61448, IntPtr.Zero);
        }
        #endregion

        private XmlDocument Document;
        private XmlElement RootNode;
        private List<MyDataGridItem> MyDataGridElementList;

        private class MyDataGridItem
        {
            public XmlElement _XmlElement { get; set; }
            public MyDataGridItem ParentMyDataGridItem { get; set; }
            public List<MyDataGridItem> ChildMyDataGridItems { get; set; }
            public bool IsExpanded { get; set; }
            public int Level { get; set; }
            public string IsEditable { get; set; }

            public string Name
            { 
                get 
                {
                    string rval = string.Empty;
                    for (int i = 0; i < this.Level; i++)
                    {
                        rval += "-";
                    }
                    return rval + this._XmlElement.Name; 
                } 
            }

            public string Value
            {
                get 
                {
                    XmlText textnode;
                    if (!this._XmlElement.HasChildNodes) return string.Empty;
                    else if ((textnode = this._XmlElement.ChildNodes[0] as XmlText) == null) return string.Empty;
                    else
                        return textnode.Value;
                }
                set
                {
                    XmlText textnode;
                    if (!this._XmlElement.HasChildNodes) { }
                    else if ((textnode = this._XmlElement.ChildNodes[0] as XmlText) == null) { }
                    else
                        textnode.Value = value;
                }
            }

            public MyDataGridItem(XmlElement e, MyDataGridItem p, bool ie, int l)
            {
                _XmlElement = e;
                ParentMyDataGridItem = p;
                IsExpanded = ie;
                Level = l;
                ChildMyDataGridItems = new List<MyDataGridItem>();
                IsEditable = (this._XmlElement.HasChildNodes && (this._XmlElement.ChildNodes[0].GetType() == typeof(XmlText))) ? "1" : "0";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Document = new XmlDocument();
            MyDataGridElementList = new List<MyDataGridItem>();
        }

        private void Load()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (IO.FileToXMLDocument(ref doc))
                {
                    Document = doc.Clone() as XmlDocument;
                    RootNode = Document.DocumentElement;

                    MyDataGridElementList.Clear();
                    MyDataGridElementList.Add(new MyDataGridItem(RootNode, null, false, 0));
                    DataGrid1.ItemsSource = MyDataGridElementList;
                    //TreeViewItem tvi = new TreeViewItem();
                    //tvi.Header = RootNode.Name;
                    //tvi.Tag = new TreeViewItemTagObject(RootNode, true);
                    //if (AddPossibleChildsToTreeViewItem(RootNode, ref tvi))
                    //{
                    //    TreeView1.Items.Clear();
                    //    TreeView1.Items.Add(tvi);
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_MaxMin_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (this.WindowState != System.Windows.WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
                b.Style = FindResource("MyNormalSizeButtonStyle") as Style;
                b.Content = "o";
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
                b.Style = FindResource("MyMaximizeButtonStyle") as Style;
                b.Content = "O";
            }
        }

        //private void MaximizeColumnValue()
        //{
        //    double Height_ListView = ListView1.ActualHeight - 24;
        //    double Height_Items = ListView1.Items.Count * 20.88;
        //    if (Height_Items < Height_ListView)
        //        Column_Value.Width = ListView1.ActualWidth - Column_Key.ActualWidth - 9;
        //    else
        //        Column_Value.Width = ListView1.ActualWidth - Column_Key.ActualWidth - 26;
        //}

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //MaximizeColumnValue();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void DataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyDataGridItem CurrentMyDataGridItem = DataGrid1.SelectedItem as MyDataGridItem;
                if (CurrentMyDataGridItem == null) return;
                Border test = e.OriginalSource as Border;
                DataGridCell test2 = test.TemplatedParent as DataGridCell;
                if (test2.Column.DisplayIndex != 0) return;
                int CurrentIndex = MyDataGridElementList.IndexOf(CurrentMyDataGridItem);
                if (CurrentMyDataGridItem.IsExpanded) 
                    CollapseDataGridElement(CurrentMyDataGridItem);
                else
                    ExpandDataGridElement(CurrentMyDataGridItem, ref CurrentIndex);
                CurrentMyDataGridItem.IsExpanded = !CurrentMyDataGridItem.IsExpanded;
                DataGrid1.ItemsSource = null;
                DataGrid1.ItemsSource = MyDataGridElementList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExpandDataGridElement(MyDataGridItem mdgi, ref int CurrentIndex)
        {
            if (mdgi._XmlElement.ChildNodes == null) return;
            if (mdgi.ChildMyDataGridItems.Count != 0)
            {
                AddChildMyDataGridItems(mdgi, ref CurrentIndex);
            }
            else
            {
                for (int i = 0; i < mdgi._XmlElement.ChildNodes.Count; i++)
                {
                    if (mdgi._XmlElement.ChildNodes[i].HasChildNodes)// && HasXmlNodeChildNodesWithElements(mdgi._XmlElement.ChildNodes[i]))
                    {
                        MyDataGridItem cn = new MyDataGridItem(mdgi._XmlElement.ChildNodes[i] as XmlElement, mdgi, false, mdgi.Level + 1);
                        CurrentIndex++;
                        MyDataGridElementList.Insert(CurrentIndex, cn);
                        mdgi.ChildMyDataGridItems.Add(cn);
                    }
                }
            }
        }

        private void AddChildMyDataGridItems(MyDataGridItem mdgi, ref int CurrentIndex)
        {
            for (int i = 0; i < mdgi.ChildMyDataGridItems.Count; i++)
            {
                CurrentIndex++;
                MyDataGridElementList.Insert(CurrentIndex, mdgi.ChildMyDataGridItems[i]);
                if (!mdgi.ChildMyDataGridItems[i].IsExpanded) continue;
                if (mdgi.ChildMyDataGridItems[i].ChildMyDataGridItems != null)
                {
                    AddChildMyDataGridItems(mdgi.ChildMyDataGridItems[i], ref CurrentIndex);
                }
            }
        }

        private void CollapseDataGridElement(MyDataGridItem mdgi)
        {
            int index = MyDataGridElementList.IndexOf(mdgi);
            bool weiter = true;
            while (weiter && (index + 1 < MyDataGridElementList.Count))
            {
                if (MyDataGridElementList[index + 1].Level > mdgi.Level)
                    MyDataGridElementList.RemoveAt(index + 1);
                else
                    weiter = false;
            }
        }

        private void DataGrid1_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            int i = 0;
        }

        private void DataGrid1_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (!MyDataGridElementList[e.Row.GetIndex()]._XmlElement.HasChildNodes) { e.Cancel = true; }
            else if (MyDataGridElementList[e.Row.GetIndex()]._XmlElement.ChildNodes[0].GetType() != typeof(XmlText)) { e.Cancel = true; }
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
    }
    
    public class ArtistNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return (value.ToString() == "1");
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
