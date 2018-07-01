using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HidLibrary;

namespace ColorControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Communicator _communicator;
        public MainWindow()
        {
            InitializeComponent();
            _communicator = new Communicator();
            ColorCanvas.SelectedColor = Colors.DarkMagenta;
        }


        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytes = new byte[65];
            bytes[1] = 200;
            _communicator.SendMessage(bytes);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            var color = ColorCanvas.SelectedColor;
            var messages = PackageGenerator.Create(Strip.Text.ToInt())
                .SetRange(FirstIndex.Text.ToInt(), LastIndex.Text.ToInt())
                .SetColor(ColorCanvas.SelectedColor.Value)
                .BuildPackets();

            var result = _communicator.SendMessages(messages.ToArray());
            MessageCountBlock.Text = $"{result.Attempts}";
            MessageSuccessBlock.Text = $"{result.Successes}";
        }
    }
}
