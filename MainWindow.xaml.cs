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
        private bool DocumentChanged;
        private XmlElement RootNode;
        internal static List<MyDataGridItem> MyDataGridElementList;

        internal static DataGrid DataGrid1;

        internal class MyDataGridItem
        {
            public XmlElement _XmlElement { get; set; }
            public MyDataGridItem ParentMyDataGridItem { get; set; }
            public List<MyDataGridItem> ChildMyDataGridItems { get; set; }
            public bool IsExpanded { get; set; }
            public int Level { get; set; }
            public bool IsEditable { get; set; }

            public int Width1 { get; set; }
            public int Width2 { get; set; }
            public int Width3 { get; set; }
            public int Width4 { get; set; }
            public int Width5 { get; set; }
            public int Width6 { get; set; }
            public int Width7 { get; set; }
            public int Width8 { get; set; }
            public int Width9 { get; set; }
            public bool b1Transparent { get; set; }
            public bool b2Transparent { get; set; }
            public bool b3Transparent { get; set; }
            public bool b4Transparent { get; set; }
            public bool b5Transparent { get; set; }
            public bool b6Transparent { get; set; }
            public bool b7Transparent { get; set; }
            public bool b8Transparent { get; set; }
            public bool b9Transparent { get; set; }

            public bool IsLastChild
            {
                get
                {
                    if (this._XmlElement.ParentNode == null) return true;
                    return this._XmlElement.Equals(this._XmlElement.ParentNode.LastChild);
                }
            }
            public string Name
            { 
                get 
                {
                    string rval = string.Empty;
                    //for (int i = 0; i < this.Level; i++)
                    //{
                    //    rval += "-";
                    //}
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

            public MyDataGridItem(XmlElement e, MyDataGridItem _ParentMyDataGridItem, bool _IsExpanded, int _Level)
            {
                _XmlElement = e;
                ParentMyDataGridItem = _ParentMyDataGridItem;
                IsExpanded = _IsExpanded;
                Level = _Level;
                ChildMyDataGridItems = new List<MyDataGridItem>();
                IsEditable = (this._XmlElement.HasChildNodes && (this._XmlElement.ChildNodes[0].GetType() == typeof(XmlText))) ? true : false;
                if (this.ParentMyDataGridItem == null) return;
                Width1 = (this.Level < 1) ? 0 : 16;
                Width2 = (this.Level < 2) ? 0 : 16;
                Width3 = (this.Level < 3) ? 0 : 16;
                Width4 = (this.Level < 4) ? 0 : 16;
                Width5 = (this.Level < 5) ? 0 : 16;
                Width6 = (this.Level < 6) ? 0 : 16;
                Width7 = (this.Level < 7) ? 0 : 16;
                Width8 = (this.Level < 8) ? 0 : 16;
                Width9 = (this.Level < 9) ? 0 : 16;
                b1Transparent = (Level > 0 && GetSiblingFromParent(this, this.Level) == null) ? true : false;
                b2Transparent = (Level > 1 && GetSiblingFromParent(this, this.Level - 1) == null) ? true : false;
                b3Transparent = (Level > 2 && GetSiblingFromParent(this, this.Level - 2) == null) ? true : false;
                b4Transparent = (Level > 3 && GetSiblingFromParent(this, this.Level - 3) == null) ? true : false;
                b5Transparent = (Level > 4 && GetSiblingFromParent(this, this.Level - 4) == null) ? true : false;
                b6Transparent = (Level > 5 && GetSiblingFromParent(this, this.Level - 5) == null) ? true : false;
                b7Transparent = (Level > 6 && GetSiblingFromParent(this, this.Level - 6) == null) ? true : false;
                b8Transparent = (Level > 7 && GetSiblingFromParent(this, this.Level - 7) == null) ? true : false;
                b9Transparent = (Level > 8 && GetSiblingFromParent(this, this.Level - 8) == null) ? true : false;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Document = new XmlDocument();
            MyDataGridElementList = new List<MyDataGridItem>();

            DataGrid1 = new DataGrid();
            DataGrid1.BorderThickness = new Thickness(0);
            DataGrid1.Margin = new Thickness(0);
            DataGrid1.AutoGenerateColumns = false;
            DataGrid1.RowHeaderWidth = 0;
            DataGrid1.CanUserResizeRows = false;
            DataGrid1.RowHeight = 18;
            DataGrid1.GridLinesVisibility = DataGridGridLinesVisibility.None;
            DataGrid1.PreparingCellForEdit += DataGrid1_PreparingCellForEdit;
            DataGrid1.BeginningEdit += DataGrid1_BeginningEdit;
            DataGrid1.CellEditEnding += DataGrid1_CellEditEnding;

            
            System.Windows.Media.Brush br = (System.Windows.Media.Brush)Application.Current.Resources["Transparent"];
            DataGrid1.Background = br;
            DataGrid1.MouseDoubleClick += DataGrid2_MouseDoubleClick;

            DataGrid1.RowBackground = (System.Windows.Media.Brush)Application.Current.Resources["Transparent"];

            DataGridTemplateColumn col1 = new DataGridTemplateColumn();
            col1.Header = "Knoten/Schlüssel";
            col1.HeaderStyle = (Style)Application.Current.Resources["MyHeaderStyle"];
            col1.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            col1.IsReadOnly = true;
            col1.CanUserSort = false;
            col1.CellStyle = (Style)Application.Current.Resources["KeyCellStyle"];

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Wert";
            col2.HeaderStyle = (Style)Application.Current.Resources["MyHeaderStyle"];
            col2.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            col2.CanUserSort = false;
            col2.CellStyle = (Style)Application.Current.Resources["ValueCellStyle"];

            Binding myBinding = new Binding("Value");
            col2.Binding = myBinding;

            DataGrid1.Columns.Add(col1);
            DataGrid1.Columns.Add(col2);
            Grid.SetColumn(DataGrid1, 0);
            Grid.SetRow(DataGrid1, 1);

            Grid1.Children.Add(DataGrid1);
        }

        private void DataGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid1_MouseDoubleClick(sender, e);
        }

        private void Load()
        {
            try
            {
                if (DocumentChanged && MessageBox.Show("Das aktuelle Dokument wurde geändert.\nÄnderungen verwerfen?", "Frage", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
                XmlDocument doc = new XmlDocument();
                if (IO.FileToXMLDocument(ref doc))
                {
                    ClearDatagrid();
                    Document = doc.Clone() as XmlDocument;
                    RootNode = Document.DocumentElement;

                    MyDataGridElementList.Add(new MyDataGridItem(RootNode, null, false, 0));
                    DataGrid1.ItemsSource = MyDataGridElementList;
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
            ClearDatagrid();
        }

        private void ClearDatagrid()
        {
            MyDataGridElementList.Clear();
            Document = null;
            RootNode = null;
            DataGrid1.ItemsSource = null;
            DocumentChanged = false;
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
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
                if (e.RightButton == MouseButtonState.Pressed) return;
                MyDataGridItem CurrentMyDataGridItem = DataGrid1.SelectedItem as MyDataGridItem;
                if (CurrentMyDataGridItem == null) return;
                TextBlock test = e.OriginalSource as TextBlock;
                if (test == null) return;
                ExpandOrCollaps(CurrentMyDataGridItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal static void ExpandOrCollaps(MyDataGridItem DGI)
        {
            if (DGI == null) return;
            if (!DGI._XmlElement.HasChildNodes) return;
            if (DGI._XmlElement.LastChild.GetType() != typeof(XmlElement)) return;

            int CurrentIndex = MyDataGridElementList.IndexOf(DGI);
            if (DGI.IsExpanded)
                CollapseDataGridElement(DGI);
            else
                ExpandDataGridElement(DGI, ref CurrentIndex);
            DGI.IsExpanded = !DGI.IsExpanded;
            DataGrid1.ItemsSource = null;
            DataGrid1.ItemsSource = MyDataGridElementList;
        }

        private static void ExpandDataGridElement(MyDataGridItem DGI, ref int Index)
        {
            if (DGI._XmlElement.ChildNodes == null) return;
            if (DGI.ChildMyDataGridItems.Count != 0)
            {
                AddChildMyDataGridItems(DGI, ref Index);
            }
            else
            {
                for (int i = 0; i < DGI._XmlElement.ChildNodes.Count; i++)
                {
                    if (DGI._XmlElement.ChildNodes[i].HasChildNodes)
                    {
                        MyDataGridItem cn = new MyDataGridItem(DGI._XmlElement.ChildNodes[i] as XmlElement, DGI, false, DGI.Level + 1);
                        Index++;
                        MyDataGridElementList.Insert(Index, cn);
                        DGI.ChildMyDataGridItems.Add(cn);
                    }
                }
            }
        }

        private static void AddChildMyDataGridItems(MyDataGridItem DGI, ref int Index)
        {
            for (int i = 0; i < DGI.ChildMyDataGridItems.Count; i++)
            {
                Index++;
                MyDataGridElementList.Insert(Index, DGI.ChildMyDataGridItems[i]);
                if (!DGI.ChildMyDataGridItems[i].IsExpanded) continue;
                if (DGI.ChildMyDataGridItems[i].ChildMyDataGridItems != null)
                {
                    AddChildMyDataGridItems(DGI.ChildMyDataGridItems[i], ref Index);
                }
            }
        }

        private static void CollapseDataGridElement(MyDataGridItem DGI)
        {
            int index = MyDataGridElementList.IndexOf(DGI);
            bool weiter = true;
            while (weiter && (index + 1 < MyDataGridElementList.Count))
            {
                if (MyDataGridElementList[index + 1].Level > DGI.Level)
                    MyDataGridElementList.RemoveAt(index + 1);
                else
                    weiter = false;
            }
        }

        private void DataGrid1_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // hier gehts zuerst rein
            if (!MyDataGridElementList[e.Row.GetIndex()]._XmlElement.HasChildNodes) { e.Cancel = true; }
            else if (MyDataGridElementList[e.Row.GetIndex()]._XmlElement.ChildNodes[0].GetType() != typeof(XmlText)) { e.Cancel = true; }
        }
        private string OldValue;
        private void DataGrid1_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            // hier gehts nach DataGrid1_BeginningEdit rein
            TextBox tb = e.EditingElement as TextBox;
            OldValue = tb.Text;
        }
        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            TextBox tb = e.EditingElement as TextBox;
            if (OldValue != tb.Text)
                DocumentChanged = true;
        }

        private static XmlNode GetSiblingFromParent(MyDataGridItem DGI, int heigth)
        {
            if (heigth == 0) return DGI._XmlElement.NextSibling;
            return GetSiblingFromParent(DGI.ParentMyDataGridItem, heigth - 1);
        }
    }

    public partial class StyleEvents
    {
        public void NodeButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            MainWindow.ExpandOrCollaps(b.DataContext as MainWindow.MyDataGridItem);
        }
    }

    //public class ArtistNameConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType,
    //        object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        try
    //        {
    //            return (value.ToString() == "1");
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType,
    //        object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
