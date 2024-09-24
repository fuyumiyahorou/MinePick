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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using System.Data;



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


        private void ipt_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ipt_Sheet.ItemsSource = null;
            ipt_Sheet.Items.Clear();
            if (ipt_List.SelectedIndex>0)
            {
                string file_path = ipt_Path.Text + @"\" + ipt_List.SelectedItem;


                DataTable dt = Get_excel(file_path);



                ipt_Sheet.ItemsSource = (System.Collections.IEnumerable)dt;



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

        private DataTable Get_excel(string file_path)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(file_path, false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                Sheet sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);


                DataTable dataTable = new DataTable();


                while (reader.Read())
                {
                    if (reader.ElementType == typeof(Row))
                    {
                        List<string> ls = new List<string>();
                        Row row = (Row)reader.LoadCurrentElement();
                        foreach (Cell cell in row.Elements<Cell>())
                        {
                            string cellValue = GetCellValue(doc, cell);

                            ls.Add(cellValue);
                        }
                        dataTable.Rows.Add(ls);

                    }
                }
                return dataTable;
            }
        }
        private static string GetCellValue(SpreadsheetDocument doc, Cell cell)
        {
            SharedStringTablePart stringTablePart = doc.WorkbookPart.SharedStringTablePart;
            string value = "";
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerXml;
            }




            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }



















        private void All_Ini()
        {
            ipt_Path.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }


    }








}