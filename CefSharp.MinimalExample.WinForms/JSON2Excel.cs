using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace CefSharp.MinimalExample.WinForms
{
    /// <summary>
    /// c#json转excel
    /// 步骤:
    /// 1、将json文件 转成  DataTable类型
    /// 2、将DataTable类型 数据  导出成Excel 表格
    /// </summary>

    class JSON2Excel
    {
        /// 1、c# 调用方法
        //json  转 DataTable  类型   然后导出  excel
        //参数1 String json json数据 参数2 tabName导出成Excel后的名称
        public String jsonToDataTable(String json, String tabName)
        {
            String meg = "fail";
            //System.Windows.Forms.MessageBox.Show(json);
            //json数据转成  DataTable   类型
            DataTable dataTab = toDataTableTwo(json);
            //调用另存为  系统文件窗口
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //设置文件类型
            saveFileDialog.FileName = tabName;  //导出excel的默认名称
            saveFileDialog.Filter = "Excel files(*.xls)|*.xls|All files(*.*)|*.*";
            //保存对话框是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;
            //String DialogResult = saveFileDialog.ShowDialog();
            //点了保存按钮进入
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //获得文件路径
                String localFilePath = saveFileDialog.FileName.ToString();
                //System.Windows.Forms.MessageBox.Show(localFilePath);
                dataTableToCsv(dataTab, localFilePath);
                meg = "success";
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("导出失败");
            }
            return meg;
        }
        ///2、将json文件 转成  DataTable类型方法
        //json  转 DataTable
        public DataTable toDataTableTwo(string json)
        {
            DataTable dataTable = new DataTable(); //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        //Columns
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        //Rows
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {

                            dataRow[current] = dictionary[current];
                        }
                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                        //Console.WriteLine(dataTable.Rows[0].ToString());
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }
        ///3、将DataTable类型 数据  导出成Excel 表格
        //导出Excel
        public void dataTableToCsv(DataTable table, string file)
        {
            string title = "";
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                //Console.WriteLine(table.Columns[i].ColumnName);
                //title += table.Columns[i].ColumnName + "\t"; //栏位：自动跳到下一单元格
                title += table.Columns[i].ColumnName + "\t"; //栏位：自动跳到下一单元格
            }

            title = title.Substring(0, title.Length - 1) + "\n";
            sw.Write(title);
            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += row[i].ToString().Trim() + "\t"; //内容：自动跳到下一单元格
                }
                line = line.Substring(0, line.Length - 1) + "\n";
                sw.Write(line);
            }
            sw.Close();
            fs.Close();
        }
    }

}
