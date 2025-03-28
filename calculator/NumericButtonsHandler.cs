using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace calculator
{
    public class NumericButtonsHandler
    {
        private readonly MainWindow mainWindow;
        private readonly TextBox textBlock;
        private bool isChecked; 

        private readonly Button zeroBtn;
        private readonly Button oneBtn;
        private readonly Button twoBtn;
        private readonly Button threeBtn;
        private readonly Button fourBtn;
        private readonly Button fiveBtn;
        private readonly Button sixBtn;
        private readonly Button sevenBtn;
        private readonly Button eightBtn;
        private readonly Button nineBtn;

        private readonly Button aBtn;
        private readonly Button bBtn;
        private readonly Button cBtn;
        private readonly Button dBtn;
        private readonly Button eBtn;
        private readonly Button fBtn;

        private int currentBase = 10;

        public NumericButtonsHandler(
            MainWindow window,
            TextBox text,
            bool digitGroupingChecked,
            Button zeroButton,
            Button oneButton,
            Button twoButton,
            Button threeButton,
            Button fourButton,
            Button fiveButton,
            Button sixButton,
            Button sevenButton,
            Button eightButton,
            Button nineButton,
            Button aButton,
            Button bButton,
            Button cButton,
            Button dButton,
            Button eButton,
            Button fButton
        )
        {
            mainWindow = window;
            textBlock = text;
            isChecked = digitGroupingChecked;

            zeroBtn = zeroButton;
            oneBtn = oneButton;
            twoBtn = twoButton;
            threeBtn = threeButton;
            fourBtn = fourButton;
            fiveBtn = fiveButton;
            sixBtn = sixButton;
            sevenBtn = sevenButton;
            eightBtn = eightButton;
            nineBtn = nineButton;
            aBtn = aButton;
            bBtn = bButton;
            cBtn = cButton;
            dBtn = dButton;
            eBtn = eButton;
            fBtn = fButton;
        }

        public void SetDigitGroupingChecked(bool value)
        {
            isChecked = value;
        }

        public void Zero_Click(object sender, RoutedEventArgs e) => AddDigit("0");
        public void One_Click(object sender, RoutedEventArgs e) => AddDigit("1");
        public void Two_Click(object sender, RoutedEventArgs e) => AddDigit("2");
        public void Three_Click(object sender, RoutedEventArgs e) => AddDigit("3");
        public void Four_Click(object sender, RoutedEventArgs e) => AddDigit("4");
        public void Five_Click(object sender, RoutedEventArgs e) => AddDigit("5");
        public void Six_Click(object sender, RoutedEventArgs e) => AddDigit("6");
        public void Seven_Click(object sender, RoutedEventArgs e) => AddDigit("7");
        public void Eight_Click(object sender, RoutedEventArgs e) => AddDigit("8");
        public void Nine_Click(object sender, RoutedEventArgs e) => AddDigit("9");
        

        public void A_Click(object sender, RoutedEventArgs e) => AddDigit("A");
        public void B_Click(object sender, RoutedEventArgs e) => AddDigit("B");
        public void C_Click(object sender, RoutedEventArgs e) => AddDigit("C");
        public void D_Click(object sender, RoutedEventArgs e) => AddDigit("D");
        public void E_Click(object sender, RoutedEventArgs e) => AddDigit("E");
        public void F_Click(object sender, RoutedEventArgs e) => AddDigit("F");

        private void AddDigit(string digit)
        {
            if (mainWindow.CurrentBase == 2 && digit != "0" && digit != "1")
            {
                return;
            }

            if (textBlock.Text == "0" || mainWindow.OperationFlag || mainWindow.MemoryFlag)
            {
                textBlock.Text = digit;
                mainWindow.OperationFlag = false;
                mainWindow.MemoryFlag = false;
            }
            else
            {
                textBlock.Text += digit;
            }

            mainWindow.AppendToExpression(digit);

            FormatWithGroupingIfEnabled();
        }


        public void Comma_Click(object sender, RoutedEventArgs e)
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (!textBlock.Text.Contains(separator))
            {
                textBlock.Text += separator;
                mainWindow.AppendToExpression(separator);

            }
        }

        private void FormatWithGroupingIfEnabled()
        {
            if (isChecked && currentBase == 10)
            {
                if (string.IsNullOrWhiteSpace(textBlock.Text) || textBlock.Text == "0")
                    return;

                string decSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                int decimals = 0;
                int sepIndex = textBlock.Text.IndexOf(decSeparator);
                if (sepIndex >= 0)
                {
                    decimals = textBlock.Text.Length - sepIndex - 1;
                }
                else
                {
                    decimals = 0;
                }

                string format = "N" + decimals;

                if (decimal.TryParse(textBlock.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal val))
                {
                    string formatted = val.ToString(format, CultureInfo.CurrentCulture);
                    if (formatted != textBlock.Text)
                    {
                        textBlock.Text = formatted;
                        textBlock.SelectionStart = textBlock.Text.Length;
                    }
                }
            }
        }

        public void Binary_Click(object sender, RoutedEventArgs e)
        {
            SwitchBase(2, new bool[]
            {
                true,  
                true,  
                false,false,false,false,false,false,false,false, 
                false,false,false,false,false,false 
            });
            System.IO.File.WriteAllText("calcBase.txt", "2");
        }

        public void Octal_Click(object sender, RoutedEventArgs e)
        {
            SwitchBase(8, new bool[]
            {
                true,true,true,true,true,true,true,true, 
                false,false, 
                false,false,false,false,false,false 
            });
            System.IO.File.WriteAllText("calcBase.txt", "8");
        }

        public void Decimal_Click(object sender, RoutedEventArgs e)
        {
            SwitchBase(10, new bool[]
            {
                true,true,true,true,true,true,true,true,true,true, 
                false,false,false,false,false,false 
            });
            System.IO.File.WriteAllText("calcBase.txt", "10");
        }

        public void Hex_Click(object sender, RoutedEventArgs e)
        {
            SwitchBase(16, new bool[]
            {
                true,true,true,true,true,true,true,true,true,true, 
                true,true,true,true,true,true 
            });
            System.IO.File.WriteAllText("calcBase.txt", "16");
        }

        private void SwitchBase(int newBase, bool[] statesForNewBase)
        {
            try
            {
                int value = Convert.ToInt32(textBlock.Text, currentBase);

                string converted = Convert.ToString(value, newBase).ToUpper();

                textBlock.Text = converted;

                SetButtonStates(statesForNewBase);

                currentBase = newBase;
                mainWindow.CurrentBase = newBase;

            }
            catch (FormatException)
            {
                textBlock.Text = "Conversion Error!";
                mainWindow.Exception = true;
            }
            catch (OverflowException)
            {
                textBlock.Text = "Overflow!";
                mainWindow.Exception = true;
            }
        }
     
        private void SetButtonStates(bool[] states)
        {
            Button[] allButtons =
            {
                zeroBtn,  oneBtn,  twoBtn,  threeBtn, fourBtn,
                fiveBtn,  sixBtn,  sevenBtn,eightBtn,nineBtn,
                aBtn,     bBtn,    cBtn,    dBtn,     eBtn, fBtn
            };

            for (int i = 0; i < allButtons.Length && i < states.Length; i++)
            {
                allButtons[i].IsEnabled = states[i];
            }
        }
    }
}
