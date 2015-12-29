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

namespace Project1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow(int rows, int cols,int soldiercount,int archercount) {
            InitializeComponent();
            init(rows,cols);
            soldierCount.Content = soldiercount;
            archerCount.Content = archercount;
        }
        private List<List<Label>> labels;
        private Button lastButton = null;
        private Label lastLabel = null;
        //private Label lastclick = null;

        private void init(int rows, int cols) {
            grid.Width = cols * 75;
            grid.Height = rows * 100;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.ShowGridLines = true;
            grid.Background = new SolidColorBrush(Colors.LightSteelBlue);

            //grid.RowDefinitions.Clear();
            for (int i = 0; i < rows; i++) {
                RowDefinition gridRow1 = new RowDefinition();
                gridRow1.Height = new GridLength(100);
                grid.RowDefinitions.Add(gridRow1);
            }
            //grid.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++) {
                ColumnDefinition gridCol1 = new ColumnDefinition();
                gridCol1.Width = new GridLength(75);
                grid.ColumnDefinitions.Add(gridCol1);
            }
            labels = new List<List<Label>>();
            for (int i = 0; i < rows; i++) {
                List<Label> rowlabels = new List<Label>();
                for (int j = 0; j < cols; j++) {
                    Label l1 = new Label();
                    {
                        l1.Width = 30;
                        l1.Height = 25;
                        l1.Foreground = new SolidColorBrush(Colors.White);
                        l1.Content = i.ToString() + ", " + j.ToString();
                        l1.Margin = new Thickness(0, 0, 0, 0);
                        l1.VerticalAlignment = VerticalAlignment.Top;
                        l1.HorizontalAlignment = HorizontalAlignment.Left;

                    }
                    Grid.SetRow(l1, i);
                    Grid.SetColumn(l1, j);
                    grid.Children.Add(l1);
                    Label l2 = new Label();
                    {
                        l2.Width = 40;
                        l2.Height = 25;
                        l2.Foreground = new SolidColorBrush(Colors.White);
                        l2.Content = "blank";
                        l2.Margin = new Thickness(0, 0, 0, 0);
                        l2.VerticalAlignment = VerticalAlignment.Center;
                        l2.HorizontalAlignment = HorizontalAlignment.Center;

                    }
                    rowlabels.Add(l2);
                    Grid.SetRow(l2, i);
                    Grid.SetColumn(l2, j);
                    grid.Children.Add(l2);
                }
                labels.Add(rowlabels);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            lastButton = (Button)sender;
            //string text = b.Content.ToString();
            lastLabel = soldierCount;
        }
        private void button2_Click(object sender, RoutedEventArgs e) {
            lastButton = (Button)sender;
            lastLabel = archerCount;
        }
        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.ClickCount == 2) // for double-click, remove this condition if only want single click
                {
                var point = Mouse.GetPosition(grid);

                int row = 0;
                int col = 0;
                double accumulatedHeight = 0.0;
                double accumulatedWidth = 0.0;

                // calc row mouse was over
                foreach (var rowDefinition in grid.RowDefinitions) {
                    accumulatedHeight += rowDefinition.ActualHeight;
                    if (accumulatedHeight >= point.Y)
                        break;
                    row++;
                }

                // calc col mouse was over
                foreach (var columnDefinition in grid.ColumnDefinitions) {
                    accumulatedWidth += columnDefinition.ActualWidth;
                    if (accumulatedWidth >= point.X)
                        break;
                    col++;
                }
                MessageBox.Show(string.Format("Grid clicked at row {0}, column {1}", row, col));
                // row and col now correspond Grid's RowDefinition and ColumnDefinition mouse was over when double clicked!
                if (lastButton != null) {
                    setguy(row, col);
                }
            }
        }
        private void setguy(int row,int col) {
            Label label = labels[row][col];
            label.Content = lastButton.Content.ToString();
            int count = Int32.Parse(lastLabel.Content.ToString());
            lastLabel.Content = count - 1;

        }
        //private void init() {
        //    int rows = 4;
        //    int cols = 3;
        //    grid.Width = cols * 75;
        //    grid.Height = rows * 100;
        //    grid.HorizontalAlignment = HorizontalAlignment.Left;
        //    grid.VerticalAlignment = VerticalAlignment.Top;
        //    grid.ShowGridLines = true;
        //    grid.Background = new SolidColorBrush(Colors.LightSteelBlue);
        //}
        //private void button_Click(object sender, RoutedEventArgs e) {
        //    int rows = Int32.Parse(trows.Text);
        //    int cols = Int32.Parse(tcols.Text);
        //    grid.Width = cols * 75;
        //    grid.Height = rows * 100;
        //    grid.RowDefinitions.Clear();
        //    for (int i = 0; i < rows; i++) {
        //        RowDefinition gridRow1 = new RowDefinition();
        //        gridRow1.Height = new GridLength(100);
        //        grid.RowDefinitions.Add(gridRow1);
        //    }
        //    grid.ColumnDefinitions.Clear();
        //    for (int i = 0; i < cols; i++) {
        //        ColumnDefinition gridCol1 = new ColumnDefinition();
        //        gridCol1.Width = new GridLength(75);
        //        grid.ColumnDefinitions.Add(gridCol1);
        //    }
        //    int num = 1;
        //    labels = new List<List<Label>>();
        //    for (int i = 0; i < rows; i++) {
        //        List<Label> rowlabels = new List<Label>();
        //        for (int j = 0; j < cols; j++) {
        //            Label l1 = new Label();
        //            {
        //                //l1.Name = "lbl";
        //                l1.Width = 30;
        //                l1.Height = 25;
        //                l1.Foreground = new SolidColorBrush(Colors.White);
        //                l1.Content = i.ToString()+", "+j.ToString();
        //                //l1.Tag = ;
        //                l1.Margin = new Thickness(0, 0, 0, 0);
        //                l1.VerticalAlignment = VerticalAlignment.Top;
        //                l1.HorizontalAlignment = HorizontalAlignment.Left;
        //                //l1.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

        //            }
        //            Grid.SetRow(l1, i);
        //            Grid.SetColumn(l1, j);
        //            //grid.Children.Add(l1);
        //            //Label l2 = new Label();
        //            //{
        //            //    //l1.Name = "lbl";
        //            //    l2.Width = 30;
        //            //    l2.Height = 25;
        //            //    l2.Foreground = new SolidColorBrush(Colors.White);
        //            //    l2.Content = num.ToString();
        //            //    //l1.Tag = ;
        //            //    l2.Margin = new Thickness(0, 0, 0, 0);
        //            //    l2.VerticalAlignment = VerticalAlignment.Center;
        //            //    l2.HorizontalAlignment = HorizontalAlignment.Center;
        //            //    //l1.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

        //            //}
        //            //rowlabels.Add(l2);
        //            //num++;
        //            //Grid.SetRow(l2, i);
        //            //Grid.SetColumn(l2, j);
        //            //grid.Children.Add(l2);
        //        }
        //        labels.Add(rowlabels);
        //    }
        //}
        //private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
        //    if (e.ClickCount == 2) // for double-click, remove this condition if only want single click
        //        {
        //        var point = Mouse.GetPosition(grid);

        //        int row = 0;
        //        int col = 0;
        //        double accumulatedHeight = 0.0;
        //        double accumulatedWidth = 0.0;

        //        // calc row mouse was over
        //        foreach (var rowDefinition in grid.RowDefinitions) {
        //            accumulatedHeight += rowDefinition.ActualHeight;
        //            if (accumulatedHeight >= point.Y)
        //                break;
        //            row++;
        //        }

        //        // calc col mouse was over
        //        foreach (var columnDefinition in grid.ColumnDefinitions) {
        //            accumulatedWidth += columnDefinition.ActualWidth;
        //            if (accumulatedWidth >= point.X)
        //                break;
        //            col++;
        //        }
        //        MessageBox.Show(string.Format("Grid clicked at row {0}, column {1}", row, col));
        //        // row and col now correspond Grid's RowDefinition and ColumnDefinition mouse was over when double clicked!
        //        if (lastclick != null) {
        //            perfmove(row, col);
        //        }
        //        lastclick = labels[row][col];
        //    }
        //}
        //private void perfmove(int row,int col) {
        //    Label newclick = labels[row][col];
        //    string text1 = lastclick.Content.ToString();
        //    string text2 = newclick.Content.ToString();
        //    lastclick.Content = text2;
        //    newclick.Content = text1;

        //}
    }
}

        //private void button_Click(object sender, RoutedEventArgs e) {

        //    label.Content = "hello";

        //    Button btn = new Button();{
        //        btn.Name = "mybutton";
        //        btn.Height = 20;
        //        btn.Width = 50;
        //        btn.Foreground = new SolidColorBrush(Colors.White);
        //        btn.Content = "btnnum" + num.ToString();
        //        btn.Tag = num;
        //        btn.Content = "Browse-" + num.ToString();
        //        btn.Margin = new Thickness(0, 0, 0, 0);
        //        //btn.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
        //    }
        //    Grid.SetRow(btn, 0);
        //    Grid.SetColumn(btn, num);
        //    grid.Children.Add(btn);
        //    btn.Click += btn_Click;
        //    num++;
        //}    
        //void btn_Click(object sender, RoutedEventArgs e) {
        //    label.Content = "works?";
        //}
