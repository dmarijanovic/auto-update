using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DamirM.CommonLibrary;

namespace DamirM.AutoUpdate
{
    public class BatchScripts
    {
        private Product product;
        public BatchScripts(Product product)
        {
            this.product = product;
        }
        public void GenerateScript()
        {
            StreamWriter sw = new StreamWriter("replace.bat");
            Log.Write("Generating replace.bat script", this, "GenerateScript", Log.LogType.DEBUG);
            sw.WriteLine("@echo off");
            // Sleep for 3 sec
            sw.WriteLine("ping -n 3 127.0.0.1 > nul");
            foreach (FileStruct fileStruct in product.Files)
            {
                //copy "C:\test\_tmp\93ed9f095279f753a0b5b01ca5b12a6e" "C:\test\Common Library2.dll"
                if (!fileStruct.SaveSuccessfully)
                {
                    sw.WriteLine(string.Format("copy \"{0}\" \"{1}\"", "_tmp\\" + fileStruct.md5Server, fileStruct.name));
                }
            }
            foreach (FileStruct fileStruct in product.Files)
            {
                if (!fileStruct.SaveSuccessfully)
                {
                    sw.WriteLine(string.Format("del \"{0}\"", "_tmp\\" + fileStruct.md5Server));
                }
            }
            sw.WriteLine("start autoupdate.exe");
            sw.WriteLine("rmdir /s /q \"_tmp\"");
            sw.WriteLine("exit");
            sw.Close();
        }
    }
}
