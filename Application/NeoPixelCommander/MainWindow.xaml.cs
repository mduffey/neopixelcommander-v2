using NeoPixelCommander.Library;
using NeoPixelCommander.Library.ColorManagers;
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

namespace NeoPixelCommander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IColorManager ActiveManager;
        public MainWindow()
        {
            InitializeComponent();
            //ColorCanvas.SelectedColor = Colors.DarkMagenta;
        }


        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            LEDs.SendUniversal(Colors.Black);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ActiveManager?.Stop();
            var moodlight = new Moodlight();
            moodlight.ChangeRate = Convert.ToInt32(Moodlight.ChangeRate.Value);
            moodlight.Intensity = Convert.ToInt32(Moodlight.Intensity.Value);
            moodlight.IsDynamic = true;
            moodlight.Start();
            ActiveManager = moodlight;

            
            //var color = ColorCanvas.SelectedColor;
            //var messages = PackageGenerator.Create(Strip.Text.ToInt())
            //    .SetRange(FirstIndex.Text.ToInt(), LastIndex.Text.ToInt())
            //    .SetColor(ColorCanvas.SelectedColor.Value)
            //    .BuildPackets();

            //var result = Communicator.Instance.SendMessages(messages.ToArray());
            //MessageCountBlock.Text = $"{result.Attempts}";
            //MessageSuccessBlock.Text = $"{result.Successes}";
        }
    }
}
