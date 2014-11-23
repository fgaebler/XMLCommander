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
using System.Xml;
using Microsoft.Win32;

namespace XMLCommander
{
    class IO
    {
        internal static bool FileToXMLDocument(ref XmlDocument Document)
        {
            try 
   	        {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".xml";
                dlg.Filter = "XML documents (.xml)|*.xml";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == false) return false;
                string filename = dlg.FileName;

                XmlReaderSettings ReaderSettings = new XmlReaderSettings();
                ReaderSettings.IgnoreWhitespace = true;

                XmlReader reader = XmlReader.Create(filename, ReaderSettings);
                Document.Load(reader);
                return true;
	        }
	        catch (Exception ex)
	        {
                MessageBox.Show(ex.Message);
	        }
            return false;
        }
    }
}
