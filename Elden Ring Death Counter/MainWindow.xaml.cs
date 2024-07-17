using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.IO;
using static System.Windows.Forms.LinkLabel;
using System.ComponentModel;

namespace Elden_Ring_Death_Counter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //global variables
        int deathCounter = 0;
        GlobalHotKey currentHotKey = null;
        bool enableIncrementFlag = false;
        int counterValue = 0;

        public MainWindow()
        {
            InitializeComponent();

            HotkeysManager.SetupSystemHook();

            Closing += MainWindow_Closing;
            this.SizeChanged += MainWindow_Resized;
        }

        //----------------------UI Interaction Methods----------------------
        private void ToggleIncrementer_Click(object sender, RoutedEventArgs e)
        {
            //stop incrementer and remove hotkey
            if (enableIncrementFlag)
            {
                enableIncrementFlag = false;
                ToggleIncrementer.Content = "Enable Death Counter";

                HotkeysManager.RemoveHotKey(currentHotKey);
            }
            //start incrementer and add hotkey
            else
            {
                enableIncrementFlag = true;
                ToggleIncrementer.Content = "Disable Death Counter";

                UpdateHotKey();

                string userHelpMessage = "";
                if (currentHotKey.Modifier != ModifierKeys.None)
                {
                    userHelpMessage = currentHotKey.Modifier.ToString() + " + ";
                }
                userHelpMessage += currentHotKey.Key.ToString();

                Log($"Trigger counter by using hotkey: {userHelpMessage}");

                HotkeysManager.AddHotKey(currentHotKey);
            }
        }

        //open settings window
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        //enables consolelog to resize with the main window
        private void MainWindow_Resized(object sender, SizeChangedEventArgs e)
        {
            ConsoleLog.Width = e.NewSize.Width - 20;
            ConsoleLog.Height = e.NewSize.Height - 160;
        }

        //make sure system hook is ended when closing application
        protected void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            HotkeysManager.ShutdownSystemHook();

        }
        //----------------------End of UI Interaction Methods----------------------

        //read current counter value from user-defined text file, increment, and write to file
        public void HotKeyTriggered()
        {
            if (enableIncrementFlag)
            {
                //Log($"Hotkey Triggered \tKey: {Properties.Settings.Default.IncrementKey}\tModifier:{Properties.Settings.Default.IncrementModifier}");
                //Log($"Hotkey");
                string filePath = Properties.Settings.Default.SaveFileLocation;
                string fileName = System.IO.Path.GetFileName(filePath);
                string[] fileInput;

                //handles if provided file path is accessable or not
                if (!File.Exists(filePath))
                {
                    Log($"There was an issue with the selected file. Please make sure file path is set to a readable text file.");
                }
                else
                {
                    try
                    {
                        fileInput = File.ReadAllLines(filePath);
                    }
                    catch (Exception e)
                    {
                        Log($"Read File Error: {e.Message}");
                        return;
                    }

                    //handles when user tries to use a file with nothing inside it
                    if (fileInput.Length == 0)
                    {
                        counterValue = 0;
                    }
                    else
                    {
                        try
                        {
                            counterValue = Int32.Parse(fileInput[0]);
                        }
                        catch (ArgumentNullException e)
                        {
                            Log(e.Message);
                        }
                        catch (FormatException)
                        {
                            Log($"{fileName} can only contain a singular whole number without commas and no whitespace beforehand.");
                            return;
                        }
                        catch (OverflowException)
                        {
                            Log($"The number stored in {fileName} is too big.");
                            return;
                        }
                    }

                    counterValue++;

                    Log($"New Death. Counter now at {counterValue}");

                    try
                    {
                        File.WriteAllText(filePath, counterValue.ToString());
                    }
                    catch (Exception e)
                    {
                        Log($"Write to {fileName} error: {e.Message}");
                    }
                }
            }
        }

        //updates global currentHotKey object to the user's currently saved key and modifier preferences
        private void UpdateHotKey()
        {
            KeyConverter keyConverter = new KeyConverter();
            ModifierKeysConverter modifierConverter = new ModifierKeysConverter();

            Key newKey = (Key)keyConverter.ConvertFromString(Properties.Settings.Default.IncrementKey);
            ModifierKeys newModifier = (ModifierKeys)modifierConverter.ConvertFromString(Properties.Settings.Default.IncrementModifier);

            GlobalHotKey newHotKey = new GlobalHotKey(newModifier, newKey, HotKeyTriggered);
            
            currentHotKey = newHotKey;
        }

        //write string to a user-facing text box and internal development output window
        public void Log(string printMessage)
        {
            Action writeToConsoleLog = () => {
                ConsoleLog.AppendText("\n" + printMessage);
                ConsoleLog.ScrollToEnd();
            };

            Dispatcher.BeginInvoke(writeToConsoleLog);

            //not using Console.WriteLine() as WPF doesn't have a console window
            //writes to 'Output' window during debug instead
            Trace.WriteLine(printMessage);
        }
    }
}