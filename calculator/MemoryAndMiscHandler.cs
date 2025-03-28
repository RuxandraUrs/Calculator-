using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Globalization;
using System.Text;
using Microsoft.VisualBasic;


namespace calculator
{
    public class MemoryAndMiscHandler
    {
        private readonly MainWindow mainWindow;
        private readonly TextBox textBlock;
        private List<double> memoryList = new List<double>();
        public int CurrentMemoryIndex { get; set; } = -1;

        private readonly Button clearMemoryButton;
        private readonly Button recallMemoryButton;

        public MemoryAndMiscHandler(MainWindow window, TextBox text, Button clearMemory, Button recallMemory)
        {
            mainWindow = window;
            textBlock = text;
            clearMemoryButton = clearMemory;
            recallMemoryButton = recallMemory;
        }

        public void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Urs Ruxandra-Mihaela\nGrupa: 10LF333", "Programator", MessageBoxButton.OK);
        }

        public void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBlock.Text);
        }

        public void Cut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBlock.Text);
            mainWindow.ResetCalculator();
        }

        public void Paste_Click(object sender, RoutedEventArgs e)
        {
            string textVerify = Clipboard.GetText().Trim();
            int currentBase = mainWindow.CurrentBase;
            if (currentBase == 10)
            {
                if (double.TryParse(textVerify, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    textBlock.Text = textVerify;
                }
                else
                {
                    textBlock.Text = "Lipirea textului nepermisa!";
                    mainWindow.Exception = true;
                }
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(textVerify, currentBase);
                    textBlock.Text = Convert.ToString(value, currentBase).ToUpper();
                }
                catch
                {
                    textBlock.Text = "Lipirea textului nepermisa!";
                    mainWindow.Exception = true;
                }
            }
        }

        public void Digit2_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                bool isChecked = menuItem.IsChecked;

                File.WriteAllText("digitGrouping.txt", isChecked ? "True" : "False");

                mainWindow.Checked = isChecked;
                mainWindow.NumericHandler.SetDigitGroupingChecked(isChecked);
                return;
            }

            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;
                File.WriteAllText("digitGrouping.txt", isChecked ? "True" : "False");

                mainWindow.Checked = isChecked;
                mainWindow.NumericHandler.SetDigitGroupingChecked(isChecked);
            }
        }



        public void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "0";

            string expr = mainWindow.ExpressionDisplay.Text;
            if (!string.IsNullOrWhiteSpace(expr) && expr != "Expresie...")
            {
                string[] tokens = expr.Trim().Split(' ');
                if (tokens.Length > 0)
                {
                    string newExpr = string.Join(" ", tokens, 0, tokens.Length - 1);
                    mainWindow.ExpressionDisplay.Text = newExpr;
                }
            }
        }


        public void Clear_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ResetCalculator();
            mainWindow.ClearExpression();
        }

        public void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.Exception)
            {
                mainWindow.ResetCalculator();
                mainWindow.Exception = false;
            }
            else if (textBlock.Text.Length > 0)
            {
                textBlock.Text = textBlock.Text.Remove(textBlock.Text.Length - 1);

                if (textBlock.Text.Length == 0 || textBlock.Text == "-")
                {
                    textBlock.Text = "0";
                }

                if (!string.IsNullOrEmpty(mainWindow.ExpressionDisplay.Text) &&
                    mainWindow.ExpressionDisplay.Text != "Your expression")
                {
                    string expr = mainWindow.ExpressionDisplay.Text;
                    if (expr.Length > 0)
                    {
                        expr = expr.Remove(expr.Length - 1);
                        mainWindow.ExpressionDisplay.Text = expr;
                    }
                }
            }
        }


        private void UpdateMemoryButtonsState()
        {
            bool hasMemory = memoryList.Count > 0;
            clearMemoryButton.IsEnabled = hasMemory;
            recallMemoryButton.IsEnabled = hasMemory;
        }


        public void SaveMemory_Click(object sender, RoutedEventArgs e)
        {
            int currentBase = mainWindow.CurrentBase;
            double currentValue = 0;

            if (currentBase == 10)
            {
                if (double.TryParse(textBlock.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out currentValue))
                {
                    memoryList.Add(currentValue);
                    CurrentMemoryIndex = memoryList.Count - 1;
                    mainWindow.MemoryFlag = true;
                    UpdateMemoryButtonsState();
                }
                else
                {
                    textBlock.Text = "Salvarea în memorie eșuată!";
                    mainWindow.Exception = true;
                }
            }
            else
            {
                try
                {
                    currentValue = ParseDoubleFromBase(textBlock.Text, currentBase);
                    memoryList.Add(currentValue);
                    CurrentMemoryIndex = memoryList.Count - 1;
                    mainWindow.MemoryFlag = true;
                    UpdateMemoryButtonsState();
                }
                catch
                {
                    textBlock.Text = "Salvarea în memorie eșuată!";
                    mainWindow.Exception = true;
                }
            }
        }

        public static double ParseDoubleFromBase(string input, int numberBase)
        {
            input = input.Trim();
            bool isNegative = false;
            if (input.StartsWith("-"))
            {
                isNegative = true;
                input = input.Substring(1);
            }

            char decimalSeparator = input.Contains('.') ? '.' : (input.Contains(',') ? ',' : '\0');
            double result = 0;

            if (decimalSeparator == '\0')
            {
                result = Convert.ToInt32(input, numberBase);
            }
            else
            {
                string[] parts = input.Split(decimalSeparator);
                int integerPart = 0;
                if (!string.IsNullOrEmpty(parts[0]))
                    integerPart = Convert.ToInt32(parts[0], numberBase);
                result = integerPart;

                string fractionalPart = parts.Length > 1 ? parts[1] : "";
                double fractionalValue = 0;

                for (int i = 0; i < fractionalPart.Length; i++)
                {
                    char c = fractionalPart[i];
                    int digitValue;
                    if (char.IsDigit(c))
                        digitValue = c - '0';
                    else
                        digitValue = char.ToUpper(c) - 'A' + 10;

                    if (digitValue >= numberBase)
                        throw new FormatException("Cifra '" + c + "' nu este validă pentru baza " + numberBase);

                    fractionalValue += digitValue / Math.Pow(numberBase, i + 1);
                }
                result += fractionalValue;
            }

            if (isNegative)
                result = -result;
            return result;
        }
        public void RecallMemory_Click(object sender, RoutedEventArgs e)
        {
            int currentBase = mainWindow.CurrentBase;
            if (memoryList.Count > 0)
            {
                if (CurrentMemoryIndex < 0 || CurrentMemoryIndex >= memoryList.Count)
                    CurrentMemoryIndex = memoryList.Count - 1;

                double memVal = memoryList[CurrentMemoryIndex];
                if (currentBase == 10)
                {
                    textBlock.Text = memVal.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    int intVal = (int)memVal;
                    textBlock.Text = Convert.ToString(intVal, currentBase).ToUpper();
                }
                mainWindow.OperationFlag = true;
            }
            else
            {
                textBlock.Text = "Nu exista memorie";
                mainWindow.Exception = true;
            }
        }

        public void AddToMemory_Click(object sender, RoutedEventArgs e)
        {
            if (memoryList.Count > 0 && double.TryParse(textBlock.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double currentValue))
            {
                if (CurrentMemoryIndex < 0 || CurrentMemoryIndex >= memoryList.Count)
                    CurrentMemoryIndex = memoryList.Count - 1;
                memoryList[CurrentMemoryIndex] += currentValue;
                UpdateMemoryButtonsState();
            }
            else if (double.TryParse(textBlock.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                memoryList.Add(value);
                CurrentMemoryIndex = memoryList.Count - 1;
                UpdateMemoryButtonsState();
            }
        }

        public void SubtractFromMemory_Click(object sender, RoutedEventArgs e)
        {
            if (memoryList.Count > 0 && double.TryParse(textBlock.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double currentValue))
            {
                if (CurrentMemoryIndex < 0 || CurrentMemoryIndex >= memoryList.Count)
                    CurrentMemoryIndex = memoryList.Count - 1;
                memoryList[CurrentMemoryIndex] -= currentValue;
                UpdateMemoryButtonsState();
            }
            else if (double.TryParse(textBlock.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                memoryList.Add(-value);
                CurrentMemoryIndex = memoryList.Count - 1;
                UpdateMemoryButtonsState();
            }
        }

        public void ClearMemory_Click(object sender, RoutedEventArgs e)
        {
            memoryList.Clear();
            textBlock.Text = "Memorie stearsa";
            mainWindow.Exception = true;
            UpdateMemoryButtonsState();
        }

        public void ShowMemory_Click(object sender, RoutedEventArgs e)
        {
            if (memoryList.Count > 0)
            {
                StringBuilder memoryContents = new StringBuilder("Valori salvate in memorie:\n");
                for (int i = 0; i < memoryList.Count; i++)
                {
                    memoryContents.AppendLine("Mem[" + i + "]: " + memoryList[i].ToString(CultureInfo.InvariantCulture));
                }
                MessageBox.Show(memoryContents.ToString(), "Memory Stack", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Memorie goala!", "Avertizare", MessageBoxButton.OK);
                mainWindow.Exception = true;
            }
        }
        public void ChooseMemoryIndex()
        {
            if (memoryList.Count == 0)
            {
                textBlock.Text = "Memorie goala!";
                mainWindow.Exception = true;
                return;
            }

            string prompt = "Alege indexul memoriei (0 .. " + (memoryList.Count - 1) + "):";
            string input = Interaction.InputBox(prompt, "Selecteaza index memorie", "0");

            if (int.TryParse(input, out int idx))
            {
                if (idx >= 0 && idx < memoryList.Count)
                {
                    CurrentMemoryIndex = idx;
                    textBlock.Text = $"Mem[{idx}] selectat!";
                }
                else
                {
                    textBlock.Text = "Index invalid!";
                    mainWindow.Exception = true;
                }
            }
            else
            {
                textBlock.Text = "Index invalid!";
                mainWindow.Exception = true;
            }
        }
        public void KeyboardInput(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.C)
                {
                    Copy_Click(sender, e);
                    return;
                }
                else if (e.Key == Key.V)
                {
                    Paste_Click(sender, e);
                    return;
                }
                else if (e.Key == Key.X)
                {
                    Cut_Click(sender, e);
                    return;
                }
            }

            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    mainWindow.NumericHandler.Zero_Click(sender, null);
                    break;
                case Key.D1:
                case Key.NumPad1:
                    mainWindow.NumericHandler.One_Click(sender, null);
                    break;
                case Key.D2:
                case Key.NumPad2:
                    mainWindow.NumericHandler.Two_Click(sender, null);
                    break;
                case Key.D3:
                case Key.NumPad3:
                    mainWindow.NumericHandler.Three_Click(sender, null);
                    break;
                case Key.D4:
                case Key.NumPad4:
                    mainWindow.NumericHandler.Four_Click(sender, null);
                    break;
                case Key.D5:
                case Key.NumPad5:
                    mainWindow.NumericHandler.Five_Click(sender, null);
                    break;
                case Key.D6:
                case Key.NumPad6:
                    mainWindow.NumericHandler.Six_Click(sender, null);
                    break;
                case Key.D7:
                case Key.NumPad7:
                    mainWindow.NumericHandler.Seven_Click(sender, null);
                    break;
                case Key.D8:
                case Key.NumPad8:
                    mainWindow.NumericHandler.Eight_Click(sender, null);
                    break;
                case Key.D9:
                case Key.NumPad9:
                    mainWindow.NumericHandler.Nine_Click(sender, null);
                    break;

                case Key.OemComma:
                case Key.Decimal:
                    mainWindow.NumericHandler.Comma_Click(sender, null);
                    break;

                case Key.Add:
                    mainWindow.OperationsHandler.Plus_Click(sender, null);
                    break;
                case Key.Subtract:
                    mainWindow.OperationsHandler.Minus_Click(sender, null);
                    break;
                case Key.Multiply:
                    mainWindow.OperationsHandler.Multiplication_Click(sender, null);
                    break;
                case Key.Divide:
                    mainWindow.OperationsHandler.Division_Click(sender, null);
                    break;

                case Key.Enter:
                    mainWindow.OperationsHandler.Equal_Click(sender, null);
                    break;

                case Key.Back:
                    Backspace_Click(sender, null);
                    break;

                case Key.Escape:
                    Clear_Click(sender, null);
                    break;

                case Key.A:
                    if (mainWindow.CurrentBase == 16)
                        mainWindow.NumericHandler.A_Click(sender, null);
                    break;
                case Key.B:
                    if (mainWindow.CurrentBase == 16)
                        mainWindow.NumericHandler.B_Click(sender, null);
                    break;
                case Key.C:
                    if (mainWindow.CurrentBase == 16)
                        mainWindow.NumericHandler.C_Click(sender, null);
                    break;
                case Key.D:
                    if (mainWindow.CurrentBase == 16)
                        mainWindow.NumericHandler.D_Click(sender, null);
                    break;
                case Key.E:
                    if (mainWindow.CurrentBase == 16)
                        mainWindow.NumericHandler.E_Click(sender, null);
                    break;
                case Key.F:
                    if (mainWindow.CurrentBase == 16)
                        mainWindow.NumericHandler.F_Click(sender, null);
                    break;
            }
        }
    }
}
