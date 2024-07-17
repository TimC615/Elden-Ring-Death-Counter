﻿using Microsoft.VisualBasic.Logging;
using OBSWebsocketDotNet.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Resources.ResXFileRef;

namespace Elden_Ring_Death_Counter
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        bool updateIncrementKey = false;
        bool updateKeyTest = false;
        Key tempKey;

        public SettingsWindow()
        {
            InitializeComponent();

            //preload currently saved filepath
            FilePathTextBox.Text = Properties.Settings.Default.SaveFileLocation;

            //preselect dropdown item to saved modifier
            switch (Properties.Settings.Default.IncrementModifier)
            {
                //none selected
                case "":
                    ModifierKeyDropDown.SelectedValue = "None";
                    break;
                //alt
                case "Alt":
                    ModifierKeyDropDown.SelectedValue = "Alt";
                    break;
                //control
                case "Ctrl":
                    ModifierKeyDropDown.SelectedValue = "Ctrl";
                    break;
                //shift
                case "Shift":
                    ModifierKeyDropDown.SelectedValue = "Shift";
                    break;
                //windows
                case "Windows":
                    ModifierKeyDropDown.SelectedValue = "Windows";
                    break;
            }
            IncrementKeyTextBox.Text = Properties.Settings.Default.IncrementKey;
        }

        private void UpdateFilePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "txt files (*.txt)|*.txt";
            fileDialog.RestoreDirectory = true;
            DialogResult success = fileDialog.ShowDialog();

            //if user successfully chooses a .txt file
            if(success == System.Windows.Forms.DialogResult.OK || success == System.Windows.Forms.DialogResult.Yes)
            {
                /*
                string path = fileDialog.FileName;

                Properties.Settings.Default.SaveFileLocation = path;

                Properties.Settings.Default.Save();
                */

                FilePathTextBox.Text = fileDialog.FileName;
            }
        }

        //intercept keydown event before textbox is updated
        void IncrementKeyTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.LeftAlt && e.Key != Key.LeftCtrl && e.Key != Key.LeftShift && e.Key != Key.LWin &&
                e.Key != Key.RightAlt && e.Key != Key.RightCtrl && e.Key != Key.RightShift && e.Key != Key.RWin
                && e.Key != Key.System)
            {
                IncrementKeyTextBox.Text = e.Key.ToString();

                //tells UI that keydown has been handled, meaning there is no need to write keydown character to text box
                e.Handled = true;
            }
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            //update saved key
            //keypress has already been comverted into a Key object approved string so dorectly saving is ok
            Properties.Settings.Default.IncrementKey = IncrementKeyTextBox.Text;


            //update saved modifier
            string newModifier = ModifierKeyDropDown.SelectedValue.ToString();

            //change "none" string to a variable that is acceptable to be modified into ModifierKeys
            if (newModifier == "None")
                newModifier = "";
            Properties.Settings.Default.IncrementModifier = newModifier;


            //update saved file path
            Properties.Settings.Default.SaveFileLocation = FilePathTextBox.Text;

            //save changes
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
