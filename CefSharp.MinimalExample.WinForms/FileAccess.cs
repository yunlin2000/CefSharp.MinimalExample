using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefSharp.MinimalExample.WinForms
{
   public class FileAccess
    {
        public string smtp;//smtp.qq.com  
        public string mail;//
        public string pwd;//
        public int jiange;//分钟
        public int 报警值;//
        public void Save()
        {
            try
            {
                System.IO.File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(this));
            }
            catch (Exception ex)
            {

            }
        }

        public static FileAccess Read()
        {
            try
            {

                string json = System.IO.File.ReadAllText("config.json");
                FileAccess cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<FileAccess>(json);
                if (cfg != null)
                {
                    return cfg;
                }
            }
            catch (Exception ex)
            {

            }

            return new FileAccess();
        }
    }
}
