using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helper;
using System.Data;

namespace TeachMgr.Handler
{
    /// <summary>
    /// UserHandler 的摘要说明
    /// </summary>
    public class UserHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strResult = string.Empty;
            try
            {
                string action = context.Request.Form["action"];
                switch (action)
                {
                    case "getUserList":
                        strResult = GetUserList();
                        break;
                    case "addUser":
                        string strUserName = context.Request.Form["name"];
                        int roleId = Convert.ToInt32(context.Request.Form["role"]);
                        strResult = AddUser(strUserName,roleId);
                        break;
                    case "delUser":
                        int delId = Convert.ToInt32(context.Request.Form["id"]);
                        strResult = DeleteUser(delId);
                        break;
                    case "resetPwd":
                        int resetId = Convert.ToInt32(context.Request.Form["id"]);
                        strResult = ResetPwd(resetId);
                        break;
                    default:
                        break;
                }
                strResult = JsonHelper.SuccessReturn(strResult);
            }
            catch
            {
                strResult = JsonHelper.FailReturn("error");
            }
            context.Response.Write(strResult);
        }

        public string GetUserList()
        {
            BLL.UserBLL bll = new BLL.UserBLL();
            DataSet ds = bll.GetList("");
            return ds.Tables[0].ToJson();
        }

        public string AddUser(string strUserName,int RoleId)
        {
            Model.UserModel mod = new Model.UserModel();
            mod.LoginName = strUserName;
            mod.RoleId = RoleId;
            mod.Pwd = "666666"; //初始密码为666666
            BLL.UserBLL bll = new BLL.UserBLL();
            bool b = bll.Add(mod);
            if(b)
            {
                return "添加用户成功".ToJson();
            }
            else
            {
                return "添加用户失败".ToJson();
            }
        }

        public string DeleteUser(int id)
        {
            BLL.UserBLL bll = new BLL.UserBLL();
            bool result = bll.Delete(id);
            if (result)
            {
                return "删除用户成功".ToJson();
            }
            else
            {
                return "删除用户失败！".ToJson();
            }
        }

        public string ResetPwd(int id)
        {
            BLL.UserBLL bll = new BLL.UserBLL();
            bool result = bll.UpdatePwd(id);
            if (result)
            {
                return "重置密码成功".ToJson();
            }
            else
            {
                return "重置密码失败！".ToJson();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}