using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;


namespace _0926
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Dictionary<string, int> drinks = new Dictionary<string, int>();
        Dictionary<string, int> orders = new Dictionary<string, int>();
        String takeout = "";
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
            foreach (var drink in myDrinks)
            {

                var sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };


                var cb = new CheckBox
                {
                    Content = $"{drink.Key}",
                    Width = 120,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Blue,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center
                };

                Label lb_price = new Label
                {
                    Content = $"{drink.Value} 元",
                    FontSize = 18,
                    Margin = new Thickness(3),
                    Foreground = Brushes.Red,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Slider sl = new Slider
                {
                    Width = 100,
                    Value = 0,
                    Minimum = 0,
                    Maximum = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsSnapToTickEnabled = true
                };

                Label lb = new Label
                {
                    Content = 0,
                    FontSize = 18,
                    Margin = new Thickness(3),
                    Foreground = Brushes.Green,
                    VerticalAlignment = VerticalAlignment.Center
                };

                sp.Children.Add(cb);
                sp.Children.Add(lb_price);
                sp.Children.Add(sl);
                sp.Children.Add(lb);

                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);

                stackpanel_DrinkMenu.Children.Add(sp);
            }
        }

        private void AddNewDrink(Dictionary<string, int> myDrinks)
        {
            //myDrinks.Add("紅茶大杯", 40);
            //myDrinks.Add("紅茶小杯", 20);
            //myDrinks.Add("綠茶大杯", 40);
            //myDrinks.Add("綠茶小杯", 20);
            //myDrinks.Add("咖啡大杯", 60);
            //myDrinks.Add("咖啡小杯", 40);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV檔案|*.csv|文字檔案|*.txt|所有檔案|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    string drinkName = tokens[0];
                    int price = Convert.ToInt32(tokens[1]);
                    myDrinks.Add(drinkName, price);
                }
            }
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
            //將訂購的飲料加入訂單
            PlaceOrder(orders);

            //顯示訂單明細
            DisplayOrderDetail(orders);

        }

        private void DisplayOrderDetail(Dictionary<string, int> myOrders)
        {
            textblock.Inlines.Clear();

            Run titleString = new Run
            {
                Text = "訂購清單如下：",
                FontSize = 16,
                Foreground = Brushes.Blue
            };

            Run takeoutString = new Run
            {
                Text = $"{takeout}",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Foreground = Brushes.Brown
            };

            textblock.Inlines.Add(titleString);
            textblock.Inlines.Add(takeoutString);
            textblock.Inlines.Add(new Run("\n訂單明細如下：\n"));

            double total = 0.0;
            double sellPrice = 0.0;
            String discountString = "";

            int i = 1;
            foreach (var item in myOrders)
            {
                string drinkName = item.Key;
                int quantity = item.Value;
                int price = drinks[drinkName];
                total += price * quantity;
                Run itemstring = new Run
                {
                    Text = $"{drinkName} X {quantity}杯, 每杯{price}元 , 總共{price * quantity} 元。\n",
                    FontSize = 16,
                    Foreground = Brushes.Green
                };
                textblock.Inlines.Add(itemstring);
                i++;
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

            Italic displayString = new Italic(new Run
            {
                Foreground = Brushes.Red,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Text = $"本次訂購{myOrders.Count}項，{discountString} ，售價{sellPrice}元。"
            });

                textblock.Inlines.Add(displayString);
        }
        private void PlaceOrder(Dictionary<string, int> myOrders)
        {
            myOrders.Clear();
            for (int i=0; i < stackpanel_DrinkMenu.Children.Count; i++)
            {
                var sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var sl = sp.Children[2] as Slider;
                string drinkName = cb.Content.ToString();
                int quantity = Convert.ToInt32(sl.Value);

                if(cb.IsChecked == true && quantity != 0) 
                { 
                    myOrders.Add(drinkName,quantity);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            takeout = targetRadioButton.Content.ToString();
            //MessageBox.Show($"您選擇的是{targetRadioButton.Content}", "選擇結果");
        }
    }
}   

