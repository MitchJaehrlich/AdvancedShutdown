using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdvancedShutdown
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // Global Variables
        string buttonClickedText = "Done. Click to cancel shutdown";
        string buttonUnclickedText = "Schedule Shutdown";
        public MainWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Button to schedule the shutown
        /// </summary>
        private void ScheduleShutdownClickedwnClicked(object sender, RoutedEventArgs e)
        {
            // is shutdown to be scheduled?
            // if true, shutdown is to be scheduled
            if (btnScheduleShutdown.Content.ToString() != buttonClickedText)
            {
                StartShutdown();
            }
            // false, shutdown is to be cancelled
            else
            {
                CancelShutdown();
            }
            
        }

        /// <summary>
        /// Start the shutdown schedule based on the specified conditions
        /// </summary>
        void StartShutdown()
        {
            
            btnScheduleShutdown.Content = buttonClickedText;

            if (chkStudownTime.IsChecked == true)
            {
                string inputTime = txtTime.Text;
                DateTime shutdownTime = new DateTime();

                if (DateTime.TryParse(inputTime, out shutdownTime))
                {
                    TimeSpan secondsUntilShutdown = ParseDateTimeLikeAHuman(shutdownTime);

                    string cmdArgument = $"shutdown -s -t {(int)secondsUntilShutdown.TotalSeconds}";

                    RunCommand(cmdArgument);
                }
                else
                {
                    CancelShutdown();
                }
            }
        }

        /// <summary>
        /// Stops the scheduled shutdown
        /// </summary>
        void CancelShutdown()
        {
            btnScheduleShutdown.Content = buttonUnclickedText;

            if (chkStudownTime.IsChecked == true)
            {
                RunCommand("shutdown -a");
            }
        }

        void RunCommand(string arguments)
        {
            System.Diagnostics.Process cmd = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {arguments}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            cmd.Start();
            txtOutput.Text = arguments;
        }

        /// <summary>
        /// Attempts to solve for date time formatted without specifying am or pm
        /// </summary>
        TimeSpan ParseDateTimeLikeAHuman(DateTime input)
        {
            TimeSpan output = new TimeSpan();

            output = input.Subtract(DateTime.Now);

            // check to see if it was able to parse a time in the future
            if (output.TotalMilliseconds < 0)
            {
                // try adding 12 hours incase military time wasn't used
                output = output.Add(TimeSpan.FromHours(12));

                if (output.TotalMilliseconds < 0)
                {
                    CancelShutdown();
                    return TimeSpan.MaxValue;
                }
            }

            return output;
        }
    }
}
