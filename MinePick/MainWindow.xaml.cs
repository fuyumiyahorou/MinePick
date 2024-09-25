using Microsoft.Win32;
using System.Diagnostics;
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
using System.Linq;
using System.Data;
using unvell.ReoGrid.IO;
using unvell.ReoGrid;
using System.Windows.Forms;




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

        private void ipt_Path_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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


        private void ipt_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ipt_List.SelectedIndex > -1)
            {
                string file_path = ipt_Path.Text + @"\" + ipt_List.SelectedItem;



                Load_Excel(file_path);

            }




        }

        private void Load_Excel(string path)
        {
            ipt_Sheet.Dispose();
            ipt_Sheet.Worksheets.Clear();
            ipt_Sheet.Load(path);

            ipt_Sheet.CurrentWorksheet.SetCols(ipt_Sheet.CurrentWorksheet.MaxContentCol + 1);
            ipt_Sheet.CurrentWorksheet.SetRows(ipt_Sheet.CurrentWorksheet.MaxContentRow + 1);
            ipt_Sheet.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Cell;


            ipt_Sheet.Readonly = true;

            GC.Collect();
        }









        private void Find_files()
        {
            string path = ipt_Path.Text;
            if (Directory.Exists(path))
            {
                if (path != null)
                {
                    DirectoryInfo directory = new DirectoryInfo(path);
                    FileInfo[] files = directory.GetFiles();

                    ipt_List.Items.Clear();
                    foreach (FileInfo file in files)
                    {
                        if (file.Extension == ".xlsx")
                        {

                            ipt_List.Items.Add(file.Name);
                        }
                    }
                }


            }
            if (ipt_List.Items.Count > 0)
            {
                opt_Count.Text = "已加载 " + ipt_List.Items.Count + " 项";
            }
            else
            {
                opt_Count.Text = "未加载";
            }
            ipt_List.SelectedIndex = -1;

        }

















        private void All_Ini()
        {
            ipt_Path.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            ipt_Sheet.CurrentWorksheet.Resize(1, 1);
            opt_Sheet.CurrentWorksheet.Resize(1, 2);
        }

        private void ipt_Sheet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GC.Collect();
            RangePosition rp = ipt_Sheet.CurrentWorksheet.SelectionRange;
            opt_Sheet.CurrentWorksheet.Resize(1, 2);

            opt_Loco.Text = "已选择 " + rp.ToString().Split(":")[0];
                                                                                                                           

            ReoGridControl temp = new ReoGridControl();
            int lo = 0;
            foreach (string li in ipt_List.Items)
            {
                string cf = ipt_Path.Text + "\\" + li;
                temp.Load(cf);
                //temp.CurrentWorksheet.SelectionRange = rp;
                var value = temp.CurrentWorksheet[rp.ToString().Split(":")[0]];
                opt_Sheet.CurrentWorksheet[lo, 0] = li;
                opt_Sheet.CurrentWorksheet[lo, 1] = value;





                lo ++;
                opt_Sheet.CurrentWorksheet.SetRows(lo+1);
            }

        }

        private void ipt_Fresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Directory.Exists(ipt_Path.Text))
            {
                Find_files();
            }
        }

        private void opt_Clear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(System.Windows.Forms.MessageBox.Show("确定清除吗？", "清除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.OK)
            {
                opt_Sheet.CurrentWorksheet.Resize(1, 2);
                opt_Sheet.CurrentWorksheet.DeleteRows(0,0);

                opt_Sheet.CurrentWorksheet.Resize(1, 2);



            }


        }
    }
}