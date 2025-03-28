using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace calculator
{
    public partial class MainWindow : Window
    {
        public NumericButtonsHandler NumericHandler { get; private set; }
        public OperationsHandler OperationsHandler { get; private set; }
        public MemoryAndMiscHandler MemoryMiscHandler { get; private set; }

        public int CurrentBase { get; set; } 

        public bool Exception { get; set; }
        public bool OperationFlag { get; set; }
        public bool MemoryFlag { get; set; }
        public bool Checked { get; set; }

        public bool IsProgrammerMode { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();

            ResetCalculator();
            LoadDigitGrouping();

            NumericHandler = new NumericButtonsHandler(
                this,text, Checked,
                zero, one, two, three,
                four, five, six, seven,
                eight, nine, hexA, hexB,
                hexC, hexD, hexE, hexF
            );
            OperationsHandler = new OperationsHandler(this, text);
            MemoryMiscHandler = new MemoryAndMiscHandler(this, text, clearMemory, recallMemory);

            LoadCalcMode();
            SetCalculatorMode(IsProgrammerMode);
            LoadCalcBase();

            if (clearMemory != null) clearMemory.IsEnabled = false;
            if (recallMemory != null) recallMemory.IsEnabled = false;
        }

        public void ResetCalculator()
        {
            if (text != null)
            {
                text.Text = "0";
            }
            Exception = false;
            OperationFlag = false;
            MemoryFlag = false;
            OperationsHandler?.ResetOperations();
        }

        private void LoadDigitGrouping()
        {
            try
            {
                string filePath = "digitGrouping.txt";
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "False");
                    Checked = false;
                }
                else
                {
                    Checked = File.ReadAllText(filePath).Trim() == "True";
                }
            }
            catch
            {
                Checked = false;
            }

            if (Digit2 != null)
            {
                Digit2.IsChecked = Checked;
            }
        }

        private void SaveCalcMode()
        {
            File.WriteAllText("calcMode.txt", IsProgrammerMode ? "True" : "False");
        }

        private void LoadCalcMode()
        {
            string filePath = "calcMode.txt";
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "False");
                IsProgrammerMode = false;
            }
            else
            {
                string modeStr = File.ReadAllText(filePath).Trim();
                IsProgrammerMode = (modeStr == "True");
            }
        }

        private void LoadCalcBase()
        {
            try
            {
                string filePath = "calcBase.txt";
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "10");
                }
                string baseStr = File.ReadAllText(filePath).Trim();
                if (int.TryParse(baseStr, out int savedBase))
                {
                    if (savedBase != 10)
                    {
                        IsProgrammerMode = true;
                        modeToggleMenuItem.Header = "Programmer";  

                        switch (savedBase)
                        {
                            case 2:
                                NumericHandler.Binary_Click(null, null);
                                break;
                            case 8:
                                NumericHandler.Octal_Click(null, null);
                                break;
                            case 16:
                                NumericHandler.Hex_Click(null, null);
                                break;
                            default:
                                NumericHandler.Decimal_Click(null, null);
                                break;
                        }
                    }
                    else
                    {
                        string modeFile = "calcMode.txt";
                        if (!File.Exists(modeFile))
                        {
                            File.WriteAllText(modeFile, "False");
                            IsProgrammerMode = false;
                            modeToggleMenuItem.Header = "Standard";
                        }
                        else
                        {
                            string modeStr = File.ReadAllText(modeFile).Trim();
                            IsProgrammerMode = (modeStr == "True");
                            modeToggleMenuItem.Header = IsProgrammerMode ? "Programmer" : "Standard";
                        }
                        NumericHandler.Decimal_Click(null, null);
                    }
                }
                else
                {
                    NumericHandler.Decimal_Click(null, null);
                }
            }
            catch
            {
                NumericHandler.Decimal_Click(null, null);
            }
        }

        public void SetCalculatorMode(bool isProgrammer)
        {
            if (isProgrammer)
            {
                if (Binary != null) Binary.IsEnabled = true;
                if (Octal != null) Octal.IsEnabled = true;
                if (Decimal != null) Decimal.IsEnabled = true;
                if (Hex != null) Hex.IsEnabled = true;
                if (CurrentBase == 16)
                {
                    if (hexA != null) hexA.IsEnabled = true;
                    if (hexB != null) hexB.IsEnabled = true;
                    if (hexC != null) hexC.IsEnabled = true;
                    if (hexD != null) hexD.IsEnabled = true;
                    if (hexE != null) hexE.IsEnabled = true;
                    if (hexF != null) hexF.IsEnabled = true;
                }
                else
                {
                    if (hexA != null) hexA.IsEnabled = false;
                    if (hexB != null) hexB.IsEnabled = false;
                    if (hexC != null) hexC.IsEnabled = false;
                    if (hexD != null) hexD.IsEnabled = false;
                    if (hexE != null) hexE.IsEnabled = false;
                    if (hexF != null) hexF.IsEnabled = false;
                }
            }
            else
            {
                CurrentBase = 10;
                NumericHandler.Decimal_Click(null, null);
                if (Binary != null) Binary.IsEnabled = false;
                if (Octal != null) Octal.IsEnabled = false;
                if (Decimal != null) Decimal.IsEnabled = false;
                if (Hex != null) Hex.IsEnabled = false;
                if (hexA != null) hexA.IsEnabled = false;
                if (hexB != null) hexB.IsEnabled = false;
                if (hexC != null) hexC.IsEnabled = false;
                if (hexD != null) hexD.IsEnabled = false;
                if (hexE != null) hexE.IsEnabled = false;
                if (hexF != null) hexF.IsEnabled = false;
            }
        }

        private void ModeToggleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsProgrammerMode = !IsProgrammerMode;
            modeToggleMenuItem.Header = IsProgrammerMode ? "Programmer" : "Standard";
            SetCalculatorMode(IsProgrammerMode);
            SaveCalcMode();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
                this.ShowInTaskbar = true;
            else if (this.WindowState == WindowState.Normal)
                this.ShowInTaskbar = true;
            base.OnStateChanged(e);
        }

        public TextBox ExpressionDisplay => expressionDisplay;

        public void AppendToExpression(string s)
        {
            if (ExpressionDisplay.Text == "Your expression")
                ExpressionDisplay.Text = "";
            ExpressionDisplay.Text += s;
        }

        public void ClearExpression()
        {
            ExpressionDisplay.Text = "";
        }

        private void One_Click(object sender, RoutedEventArgs e) => NumericHandler.One_Click(sender, e);
        private void Two_Click(object sender, RoutedEventArgs e) => NumericHandler.Two_Click(sender, e);
        private void Three_Click(object sender, RoutedEventArgs e) => NumericHandler.Three_Click(sender, e);
        private void Four_Click(object sender, RoutedEventArgs e) => NumericHandler.Four_Click(sender, e);
        private void Five_Click(object sender, RoutedEventArgs e) => NumericHandler.Five_Click(sender, e);
        private void Six_Click(object sender, RoutedEventArgs e) => NumericHandler.Six_Click(sender, e);
        private void Seven_Click(object sender, RoutedEventArgs e) => NumericHandler.Seven_Click(sender, e);
        private void Eight_Click(object sender, RoutedEventArgs e) => NumericHandler.Eight_Click(sender, e);
        private void Nine_Click(object sender, RoutedEventArgs e) => NumericHandler.Nine_Click(sender, e);
        private void Zero_Click(object sender, RoutedEventArgs e) => NumericHandler.Zero_Click(sender, e);
        private void Comma_Click(object sender, RoutedEventArgs e) => NumericHandler.Comma_Click(sender, e);
        private void A_Click(object sender, RoutedEventArgs e) => NumericHandler.A_Click(sender, e);
        private void B_Click(object sender, RoutedEventArgs e) => NumericHandler.B_Click(sender, e);
        private void C_Click(object sender, RoutedEventArgs e) => NumericHandler.C_Click(sender, e);
        private void D_Click(object sender, RoutedEventArgs e) => NumericHandler.D_Click(sender, e);
        private void E_Click(object sender, RoutedEventArgs e) => NumericHandler.E_Click(sender, e);
        private void F_Click(object sender, RoutedEventArgs e) => NumericHandler.F_Click(sender, e);
        public void Binary_Click(object sender, RoutedEventArgs e) => NumericHandler.Binary_Click(sender, e);
        public void Octal_Click(object sender, RoutedEventArgs e) => NumericHandler.Octal_Click(sender, e);
        public void Decimal_Click(object sender, RoutedEventArgs e) => NumericHandler.Decimal_Click(sender, e);
        public void Hex_Click(object sender, RoutedEventArgs e) => NumericHandler.Hex_Click(sender, e);
        private void Plus_Click(object sender, RoutedEventArgs e) => OperationsHandler.Plus_Click(sender, e);
        private void Minus_Click(object sender, RoutedEventArgs e) => OperationsHandler.Minus_Click(sender, e);
        private void Multiplication_Click(object sender, RoutedEventArgs e) => OperationsHandler.Multiplication_Click(sender, e);
        private void Division_Click(object sender, RoutedEventArgs e) => OperationsHandler.Division_Click(sender, e);
        private void Equal_Click(object sender, RoutedEventArgs e) => OperationsHandler.Equal_Click(sender, e);
        private void PlusOrMinus_Click(object sender, RoutedEventArgs e) => OperationsHandler.PlusOrMinus_Click(sender, e);
        private void Radical_Click(object sender, RoutedEventArgs e) => OperationsHandler.Radical_Click(sender, e);
        private void Square_Click(object sender, RoutedEventArgs e) => OperationsHandler.Square_Click(sender, e);
        private void Percent_Click(object sender, RoutedEventArgs e) => OperationsHandler.Percent_Click(sender, e);
        private void Reverse_Click(object sender, RoutedEventArgs e) => OperationsHandler.Reverse_Click(sender, e);
        private void About_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.About_Click(sender, e);
        private void Copy_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.Copy_Click(sender, e);
        private void Cut_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.Cut_Click(sender, e);
        private void Paste_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.Paste_Click(sender, e);
        private void Digit2_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.Digit2_Click(sender, e);
        private void ClearEntry_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.ClearEntry_Click(sender, e);
        private void Clear_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.Clear_Click(sender, e);
        private void Backspace_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.Backspace_Click(sender, e);
        private void RecallMemory_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.RecallMemory_Click(sender, e);
        private void SaveMemory_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.SaveMemory_Click(sender, e);
        private void ClearMemory_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.ClearMemory_Click(sender, e);
        private void AddToMemory_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.AddToMemory_Click(sender, e);
        private void SubtractFromMemory_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.SubtractFromMemory_Click(sender, e);
        private void ShowMemory_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.ShowMemory_Click(sender, e);
        private void SelectMemIndex_Click(object sender, RoutedEventArgs e) => MemoryMiscHandler.ChooseMemoryIndex();
        private void KeyboardInput(object sender, KeyEventArgs e) => MemoryMiscHandler.KeyboardInput(sender, e);
    }
}
