using System.Windows;
using System.Windows.Controls;

namespace calculator
{
    public class OperationsHandler
    {
        private readonly MainWindow mainWindow;
        private readonly TextBox textBlock;
        private double number;
        private string operation;

        public OperationsHandler(MainWindow window, TextBox text)
        {
            mainWindow = window;
            textBlock = text;
            operation = "";
        }

        public void ResetOperations()
        {
            operation = "";
        }

        private double GetValue()
        {
            int currentBase = mainWindow.CurrentBase;
            if (currentBase == 10)
            {
                return double.Parse(textBlock.Text);
            }
            else
            {
                return (double)Convert.ToInt32(textBlock.Text, currentBase);
            }
        }

        private string DisplayValue(double result)
        {
            int currentBase = mainWindow.CurrentBase;
            if (currentBase == 10)
            {
                return result.ToString();
            }
            else
            {
                return Convert.ToString((int)result, currentBase).ToUpper();
            }
        }

        public void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (operation != "" && operation != "=")
            {
                Equal_Click(sender, e, false); 
            }
            number = GetValue();
            operation = "+";
            mainWindow.OperationFlag = true;
            mainWindow.AppendToExpression(" + ");
        }

        public void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (operation != "" && operation != "=")
            {
                Equal_Click(sender, e, false);
            }
            number = GetValue();
            operation = "-";
            mainWindow.OperationFlag = true;
            mainWindow.AppendToExpression(" - ");
        }

        public void Multiplication_Click(object sender, RoutedEventArgs e)
        {
            if (operation != "" && operation != "=")
            {
                Equal_Click(sender, e, false);
            }
            number = GetValue();
            operation = "×";
            mainWindow.OperationFlag = true;
            mainWindow.AppendToExpression(" × ");
        }

        public void Division_Click(object sender, RoutedEventArgs e)
        {
            if (operation != "" && operation != "=")
            {
                Equal_Click(sender, e, false);
            }
            number = GetValue();
            operation = "÷";
            mainWindow.OperationFlag = true;
            mainWindow.AppendToExpression(" ÷ ");
        }

       
        public void Equal_Click(object sender, RoutedEventArgs e, bool final)
        {
            if (operation == "" || operation == "=")
                return;

            double rightValue = GetValue();
            double result = 0;

            if (mainWindow.Exception)
            {
                mainWindow.ResetCalculator();
                return;
            }

            switch (operation)
            {
                case "+":
                    result = number + rightValue;
                    break;
                case "-":
                    result = number - rightValue;
                    break;
                case "×":
                    result = number * rightValue;
                    break;
                case "÷":
                    if (rightValue == 0)
                    {
                        textBlock.Text = "Operatie nepermisa!";
                        mainWindow.Exception = true;
                        return;
                    }
                    result = number / rightValue;
                    break;
            }

            textBlock.Text = DisplayValue(result);
            operation = "="; 
            mainWindow.OperationFlag = true;

            if (final)
            {
                mainWindow.ClearExpression();
            }
        }

        public void Equal_Click(object sender, RoutedEventArgs e)
        {
            Equal_Click(sender, e, true);
        }

        public void PlusOrMinus_Click(object sender, RoutedEventArgs e)
        {
            double value = GetValue();
            value = -value;
            textBlock.Text = DisplayValue(value);
        }

        public void Radical_Click(object sender, RoutedEventArgs e)
        {
            double value = GetValue();
            if (value >= 0)
            {
                double result = Math.Sqrt(value);
                textBlock.Text = DisplayValue(result);
            }
            else
            {
                textBlock.Text = "Radacina negativa";
                mainWindow.Exception = true;
            }
            mainWindow.ClearExpression();
        }

        public void Square_Click(object sender, RoutedEventArgs e)
        {
            double value = GetValue();
            double result = Math.Pow(value, 2);
            textBlock.Text = DisplayValue(result);
            mainWindow.ClearExpression();
        }

        public void Percent_Click(object sender, RoutedEventArgs e)
        {
            double currentValue = GetValue();
            double result = 0;

            switch (operation)
            {
                case "+":
                case "-":
                    result = number * (currentValue / 100);
                    break;
                case "×":
                case "÷":
                    result = currentValue / 100;
                    break;
                default:
                    result = currentValue / 100;
                    break;
            }
            textBlock.Text = DisplayValue(result);
            mainWindow.ClearExpression();
        }

        public void Reverse_Click(object sender, RoutedEventArgs e)
        {
            double currentValue = GetValue();
            if (currentValue != 0)
            {
                double result = 1 / currentValue;
                textBlock.Text = DisplayValue(result);
            }
            else
            {
                textBlock.Text = "Operatie nepermisa";
                mainWindow.Exception = true;
            }
            mainWindow.ClearExpression();
        }
    }
}
