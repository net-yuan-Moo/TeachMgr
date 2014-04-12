using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helper;
using Model;
using BLL;
using System.Data;
using System.Web.SessionState;

namespace TeachMgr.TeacherInfoMgr
{
    /// <summary>
    /// TeacherHandler 的摘要说明
    /// </summary>
    public class TeacherHandler : IHttpHandler, IRequiresSessionState 
    {

        public void ProcessRequest(HttpContext context)
        {
            string strResult = string.Empty;
            try
            {
                string action = context.Request.Form["action"];
                switch (action)
                {
                    case "getTeacherList":
                        strResult = GetTeacherList();
                        break;
                    case "addteacher":
                        TeacherModel mod = new TeacherModel();
                        mod.TeacherName = context.Request.Form["name"];
                        mod.Sex = context.Request.Form["sex"];
                        mod.Age = Convert.ToInt32(context.Request.Form["age"]);
                        mod.Position = context.Request.Form["position"];
                        mod.Post = context.Request.Form["duty"];
                        mod.Education = context.Request.Form["education"];
                        mod.WorkYear = Convert.ToInt32(context.Request.Form["workyear"]);
                        mod.Remark = context.Request.Form["remark"];
                        strResult = AddTeacher(mod);
                        break;
                    case "editteacher":
                        TeacherModel modEdit = new TeacherModel();
                        modEdit.TeacherName = context.Request.Form["name"];
                        modEdit.Sex = context.Request.Form["sex"];
                        modEdit.Age = Convert.ToInt32(context.Request.Form["age"]);
                        modEdit.Position = context.Request.Form["position"];
                        modEdit.Post = context.Request.Form["duty"];
                        modEdit.Education = context.Request.Form["education"];
                        modEdit.WorkYear = Convert.ToInt32(context.Request.Form["workyear"]);
                        modEdit.Remark = context.Request.Form["remark"];
                        modEdit.Id = Convert.ToInt32(context.Request.Form["id"]);
                        strResult = EditTeacher(modEdit);
                        break;
                    case "showEditInfo":
                        int editId = Convert.ToInt32(context.Request.Form["id"]);
                        strResult = ShowEditInfo(editId);
                        break;
                    case "delteacher":
                        int id = Convert.ToInt32(context.Request.Form["id"]);
                        strResult = DeleteTeacher(id);
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

        public string AddTeacher(TeacherModel mod)
        {
            TeacherBLL bll = new TeacherBLL();
            bool result = bll.Add(mod);
            if (result)
            {
                return "添加教师信息成功！".ToJson();
            }
            else
            {
                return "添加教师信息失败！".ToJson();
            }
        }

        public string EditTeacher(TeacherModel mod)
        {
            TeacherBLL bll = new TeacherBLL();
            bool result = bll.Update(mod);
            if (result)
            {
                return "修改教师信息成功！".ToJson();
            }
            else
            {
                return "修改教师信息失败！".ToJson();
            }
        }

        public string ShowEditInfo(int id)
        {
            TeacherBLL bll = new TeacherBLL();
            TeacherModel mod =  bll.GetModel(id);
            return mod.ToJson();
        }

        public string DeleteTeacher(int id)
        {
            TeacherBLL bll = new TeacherBLL();
            bool result = bll.Delete(id);
            if (result)
            {
                return "删除教师信息成功！".ToJson();
            }
            else
            {
                return "删除教师信息失败！".ToJson();
            }
        }

        public string GetTeacherList()
        {
            TeacherBLL bll = new TeacherBLL();
            DataSet ds = bll.GetAllList();
            return ds.Tables[0].ToJson();
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