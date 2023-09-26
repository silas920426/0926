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

namespace _0926
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Dictionary<string, int> drinks = new Dictionary<string, int>();
        Dictionary<string, int> orders = new Dictionary<string, int>();
        public MainWindow()
        {
            InitializeComponent();
            //顯示所有飲料品項
            AddNewDrink(drinks);

            //顯示所有飲料品項
            DisplayDrinkMenu(drinks);
        }

        private void DisplayDrinkMenu(Dictionary<string, int> myDrinks)
        {
            foreach(var drink in myDrinks) 
            {              
                
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;


                CheckBox cb = new CheckBox();
                cb.Content = $"{drink.Key}";
                cb.Width = 120;
                cb.FontSize = 18;
                cb.FontWeight = FontWeights.Bold;
                cb.Foreground = Brushes.Blue;
                cb.Width = 200;
                cb.Margin = new Thickness(5);
                cb.VerticalAlignment = VerticalAlignment.Center;
                
                Label lb_price = new Label();
                lb_price.Content = $"{drink.Value} 元";
                lb_price.FontSize = 18;
                lb_price.Margin = new Thickness(5);
                lb_price.Foreground = Brushes.Red;
                lb_price.VerticalAlignment = VerticalAlignment.Center;

                Slider sl = new Slider();
                sl.Width = 100;
                sl.Value = 0;
                sl.Minimum = 0;
                sl.Maximum = 10;
                sl.VerticalAlignment = VerticalAlignment.Center;
                sl.IsSnapToTickEnabled = true;

                Label lb = new Label();
                lb.Content = 0;
                lb.FontSize = 18;
                lb.Margin = new Thickness(5);
                lb.Foreground = Brushes.Green;
                lb.VerticalAlignment = VerticalAlignment.Center;

                sp.Children.Add(cb);
                sp.Children.Add(lb_price);
                sp.Children.Add(sl);
                sp.Children.Add(lb);

                Blinding myBlinding = new Blinding("Value");
                myBlinding.Source = sl;
                lb.SetBinding(ContentProperty.myBlinding);



                stackpanel_DrinkMenu.Children.Add(sp);
            }
        }

        private void AddNewDrink(Dictionary<string, int> myDrinks)
        {
            myDrinks.Add("紅茶大杯", 40);
            myDrinks.Add("紅茶小杯", 20);
            myDrinks.Add("綠茶大杯", 40);
            myDrinks.Add("綠茶小杯", 20);
            myDrinks.Add("咖啡大杯", 60);
            myDrinks.Add("咖啡小杯", 40);
        }

        private void textbox_textchange(object sender, TextChangedEventArgs e)
        {
            var targetTextBox = sender as TextBox;

            bool success = int.TryParse(targetTextBox.Text, out int quantity);
            if (!success)
            {
                MessageBox.Show("請重新輸入", "輸入錯誤");
            }
            else if (quantity <= 0)
            {
                MessageBox.Show("請輸入一個正整數");
            }
            else
            {
                var targetStackPanel = targetTextBox.Parent as StackPanel;
                var targetLabel = targetStackPanel.Children[0] as Label;
                string drinkName = targetLabel.Content.ToString();
                if (orders.ContainsKey(drinkName)) orders.Remove(drinkName);
                orders.Add(drinkName, quantity);
            }
        }

        private void button_click(object sender, RoutedEventArgs e)
        {
            double total = 0.0;
            double sellPrice = 0.0;
            String discountString = "";
            string displayString = "訂購清單如下：\n";

            foreach (var item in orders)
            {
                string drinkName = item.Key;
                int quantity = item.Value;
                int price = drinks[drinkName];
                total += price * quantity;
                displayString += $"{drinkName} X {quantity}杯, 每杯{price}元 , 總共{price * quantity} 元。\n";
            }

            if (total >= 500)
            {
                discountString = "訂購滿500元以上者8折";
                sellPrice = total * 0.8;
            }
            else if (total >= 300)
            {
                discountString = "訂購滿300元以上者85折";
                sellPrice = total * 0.85;
            }
            else if (total >= 200)
            {
                discountString = "訂購滿200元以上者9折";
                sellPrice = total * 0.9;
            }
            else
            {
                discountString = "訂購未滿200元以上者不打折";
                sellPrice = total;
            }
            displayString += $"本次訂購{orders.Count}項，{discountString} ，售價{sellPrice}元。";
            textblock.Text = displayString;
        }
    }
}   

