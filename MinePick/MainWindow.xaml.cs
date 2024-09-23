using Microsoft.Win32;
using System.IO;
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

namespace MinePick
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            All_Ini();
        }

        private void btm_Open_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Configure open folder dialog box
            Microsoft.Win32.OpenFolderDialog dialog = new();

            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            // Show open folder dialog box
            bool? result = dialog.ShowDialog();

            // Process open folder dialog box results
            if (result == true)
            {
                // Get the selected folder
                string fullPathToFolder = dialog.FolderName;
                string folderNameOnly = dialog.SafeFolderName;
                ipt_Path.Text = fullPathToFolder;
                ipt_Path.IsReadOnly = true;

                Find_files();
            }
        }

        private void btm_Clear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ipt_Path.Text = string.Empty;
            ipt_Path.IsReadOnly = false;
        }

        private void ipt_Path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Directory.Exists(ipt_Path.Text))
                {
                    ipt_Path.IsReadOnly = true;

                    Find_files();
                }
            }
        }













        private void Find_files()
        {
            string path = ipt_Path.Text;
            if (path != null & Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                FileInfo[] files = directory.GetFiles();

                ipt_List.Items.Clear();
                foreach (FileInfo file in files) 
                {
                    if (file.Extension ==".xlsx")
                    {
                        ipt_List.Items.Add(file.Name);
                    }
                }
            }

        }



        private void All_Ini()
        {
            ipt_Path.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }




















    }
}