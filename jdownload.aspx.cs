using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace TeachMgr
{
    public partial class jdownload : System.Web.UI.Page
    {
        string action = string.Empty;
        string path = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            action = Request.QueryString["action"];
            path = Server.UrlDecode(Request.QueryString["path"]);

            if (!String.IsNullOrEmpty(path))
            {
                switch (action)
                {
                    case "download":
                        GetFile(path);
                        break;

                    case "info":
                        GetInfo(path);
                        break;
                }
            }
            Response.Write(GetJson(new MyFileInfo() { Error = "error" }));
            Response.End();
        }

        private void GetFile(string path)
        {
            // get file info
            FileInfo fi = new FileInfo(Server.MapPath(path));
            string fileName = fi.Name;//客户端保存的文件名
            string filePath = Server.MapPath(path);//路径
            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        private void GetInfo(string path)
        {
            // get file info
            FileInfo fi = new FileInfo(Server.MapPath(path));
            MyFileInfo myFileInfo = new MyFileInfo()
            {
                FileName = fi.Name,
                FileType = fi.Extension,
                FileSize = (fi.Length / long.Parse("1024")).ToString(),
                Error = "null"
            };
            Response.Write(GetJson(myFileInfo));
            Response.End();
        }


        public static string GetJson<T>(T obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                json.WriteObject(ms, obj);
                string szJson = Encoding.UTF8.GetString(ms.ToArray());
                return szJson;
            }
        }

        [DataContract]
        class MyFileInfo
        {
            [DataMember]
            public string FileName { get; set; }
            [DataMember]
            public string FileType { get; set; }
            [DataMember]
            public string FileSize { get; set; }
            [DataMember]
            public string Error { get; set; }
        }
    }
}