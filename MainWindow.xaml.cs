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

        public class kkk
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public override string ToString()
            {
                return this.Key + ", " + this.Value + " years old";
            }
        }

        public List<kkk> liste = new List<kkk>();
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 13; i++)
			{
                //ListViewItem lvi = new ListViewItem();
                //lvi.Content = i.ToString();
                kkk lll = new kkk();
                lll.Key = "aaa";
                lll.Value = "bbb";
                liste.Add(lll);
                //ListView1.Items.Add(lll);
			}
            ListView1.ItemsSource = liste;
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {

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
                b.Content = "o";
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
                b.Content = "O";
            }
        }

        private void MaximizeColumnValue()
        {
            double Height_ListView = ListView1.ActualHeight - 24;
            double Height_Items = ListView1.Items.Count * 20.88;
            if (Height_Items < Height_ListView)
                Column_Value.Width = ListView1.ActualWidth - Column_Key.ActualWidth - 9;
            else
                Column_Value.Width = ListView1.ActualWidth - Column_Key.ActualWidth - 26;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MaximizeColumnValue();
        }
    }
}
