using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XMLCommander.Templates
{
    public partial class StyleEvents : ResourceDictionary
    {
        public void NodeButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            MainWindow.ExpandOrCollaps(b.DataContext as MainWindow.MyDataGridItem);
        }
    }
}
