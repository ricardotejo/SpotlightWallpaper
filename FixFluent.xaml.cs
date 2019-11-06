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
using System.Windows.Shapes;

namespace SpotlightWallpaper
{
    /// <summary>
    /// Interaction logic for FixFluent.xaml
    /// </summary>
    public partial class FixFluent : Window
    {
        public FixFluent()
        {
            InitializeComponent();

            // IMPORTANT!
            // This window is needed due a bug on FixFluent when an application doesn't have a MainWindow 
            // because runs on background at startup.
            Hide();
        }
    }
}
