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

namespace Project1 {
    /// <summary>
    /// Interaction logic for SetUp.xaml
    /// </summary>
    public partial class SetUp : Window {
        public SetUp() {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            int rows = Int32.Parse(trows.Text);
            int cols = Int32.Parse(tcols.Text);
            int soldiers = Int32.Parse(soldiercount.Text);
            int archers = Int32.Parse(archercount.Text);
            MainWindow mw = new MainWindow(rows,cols,soldiers,archers);
            mw.Show();
			this.Close();
        }
    }
}
