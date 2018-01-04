using System;
using System.Collections.Generic;
using System.Linq;
using Mertin.SignatureRecognition.WPF;
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

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int attempt = 0;
        Signature original;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void status(object sender, SignatureCanvasStatusChangedEventArgs e)
        {
            if (e.Status == SignatureCanvasStatus.Done)
            {
                if (attempt == 0)
                    original = canvas.Signature.Value;
                else
                {
                    SignatureComparisonResult result = Signature.Compare(original, canvas.Signature.Value);
                    string text;
                    if (result.IsMatch)
                        text = "Signatures match.";
                    else
                    {
                        text = "Signatures do not match.";
                        for (int i = 0; i < result.Failures.Count; ++i)
                            text += string.Format("Error {0}{1}: {2}", i, result.Failures[i].SegmentIndex == null ? "" : " - Part " + result.Failures[i].SegmentIndex, result.Failures[i].Code.ToString());
                    }
                    new Message(text).ShowDialog();
                    ++attempt;
                }
            }
        }
    }
}
