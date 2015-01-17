/**
 * 自然框架之元数据控件
 * http://www.natureFW.com/
 *
 * @author
 * 金洋（金色海洋jyk）
 * 
 * @copyright
 * Copyright (C) 2005-2013 金洋.
 *
 * Licensed under a GNU Lesser General Public License.
 * http://creativecommons.org/licenses/LGPL/2.1/
 *
 * 自然框架之元数据控件 is free software. You are allowed to download, modify and distribute 
 * the source code in accordance with LGPL 2.1 license, however if you want to use 
 * 自然框架之元数据控件 on your site or include it in your commercial software, you must  be registered.
 * http://www.natureFW.com/registered
 */

/* ***********************************************
 * author :  金洋（金色海洋jyk）
 * email  :  jyk0011@live.cn  
 * function: 表单控件
 * history:  created by 金洋 
 *           2011-4-11 整理
 * **********************************************
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Nature.DebugWatch;
using Nature.MetaData.ControlExtend;
using Nature.MetaData.Entity;
using Nature.MetaData.Entity.MetaControl;
using Nature.MetaData.Enum;
using Nature.MetaData.Manager;
using Nature.MetaData.ManagerMeta;
using Nature.UI.WebControl.MetaControl.Form;

namespace Nature.UI.WebControl.MetaControl
{
    /// <summary>
    /// 表单控件
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<Nature:MyForm runat=server></Nature:MyForm>")]
    public class DataForm : BaseForm
    {
        #region  属性 —— 打开页面视图的按钮的类型
        /// <summary>
        /// 打开页面视图的按钮的类型
        /// 1：查看；2：添加数据；3：修改数据；4：添加后修改；5：查询；11：超链接
        /// </summary>
        /// <value>
        ///  1：查看；2：添加数据；3：修改数据；4：添加后修改；5：查询；11：超链接
        /// </value>
        /// user:jyk
        /// time:2012/9/12 15:04
        public ButonType OpenButonType { get; set; }
        #endregion

        /// <summary>
        /// 创建子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            ManagerMeta = new ManagerFormMeta
                              {
                                  DalCollection = DalCollection ,
                                  PageViewID = PageViewID
                              };

            //初始化
            Create();

            ShowForm(); //绘制控件和表格

            if (!base.Page.IsPostBack)
            {
                switch (OpenButonType)
                {
                    case ButonType.ViewData :
                    case ButonType.UpdateData:
                        //加载数据，用于显示和修改
                        LoadData();
                        break;
                }
                
            }

            //调用外部事件
            OnFormBinded(this, new EventArgs());
        }


        #region 保存数据
        /// <summary>
        /// 保存数据。如果保存成功则返回空字符串，如果不成功，返回说明信息。
        /// 如果是添加数据，成功的话，可以使用 DataID 获得新纪录的主键值（限于SQL数据库、自增ID）
        /// </summary>
        /// <returns></returns>
        public string SaveData(ManagerLogOperate operateLog,ManagerLogDataChange managerLogDataChange)
        {
            var debugInfo = new NatureDebugInfo { Title = "webform的保存数据" };
          
            //提取用户输入的信息，检查信息是否安全
            string msg = GetInputValue();

            if (msg.Length != 0)
            {
                //输入的信息格式不正确，不能继续
                return "<BR>填写的信息格式不正确<BR>" + msg;
            }
            
            ManagerData.DataID = DataID;
            
            ManagerData.DictFormColumnMeta = DicBaseCols;
            ManagerData.DictColumnsValue = DicColumnsValue;
            ManagerData.TypeOperationData = OpenButonType;

            ManagerData.ManagerLogDataChange = managerLogDataChange;

            string err = ManagerData.SaveData(operateLog, debugInfo.DetailList);

            return err;

        }
        #endregion

        #region 填充数据，准备修改和显示
        /// <summary>
        /// 填充数据，准备修改和显示
        /// </summary>
        /// <returns></returns>
        public string LoadData()
        {
            #region 加载配置信息
            if (DicBaseCols == null)
                return "";

            #endregion

            //定义接口，通过接口操作子控件

            //从数据库里提取记录
            ManagerData.DataID = DataID;
            ManagerData.LoadDataFillColumnsValue( DicBaseCols,DicColumnsValue) ;

            foreach (KeyValuePair<int, IColumn> info in DicBaseCols)
            {
                var bInfo = (FormColumnMeta)info.Value;
                var iControl = (IControlHelp)FindControl("ctrl_" + bInfo.ColumnID);
                //iControl.ControlValue = bInfo.ColValue;

                if (bInfo.ControlExtend is UniteListExtend)
                {
                    //联动下拉列表框，特殊处理
                    var uInfo = (UniteListExtend)bInfo.ControlExtend;

                    if (uInfo.IsFristList)
                    {
                        string tmpValue = DicColumnsValue[bInfo.ColumnID] + ",";
                        foreach (int columnID in uInfo.ListOtherColumnIDs)
                        {
                            tmpValue += DicColumnsValue[columnID] + ",";
                        }
                        iControl.ControlValue = tmpValue.TrimEnd(',');
                    }
                }
                else
                {
                    //其他控件直接赋值
                    if (DicColumnsValue[bInfo.ColumnID] == null)
                        iControl.ControlValue = "null";
                    else
                        iControl.ControlValue = DicColumnsValue[bInfo.ColumnID].ToString();

                }
            }

            return "";
        }
        #endregion

        #region 清空记录
        /// <summary>
        /// 重置表单控件里子控件的内容
        /// </summary>
        public void Reset()
        {
            #region 加载配置信息
            if (DicBaseCols == null)
                return ;

            #endregion

            //定义接口，通过接口操作子控件

            foreach (KeyValuePair<int, IColumn> info in DicBaseCols)
            {
                var formColMeta = (FormColumnMeta)info.Value;
                var iControl = (IControlHelp)FindControl("ctrl_" + formColMeta.ColumnID);
                //iControl.ControlValue = bInfo.ColValue;

                switch (formColMeta.ControlKind)
                {
                    case ControlType.SingleTextBox : //单行文本框
                    case ControlType.MultipleTextBox : //多行文本框
                    case ControlType.PasswordTextBox : //密码框
                    case ControlType.DateTimeTextBox: //日期格式

                    case ControlType.FckEditor: //HTML_FCK
                    case ControlType.UpdateFile: //上传文件
                        if (formColMeta.ControlState == "1")
                        {
                            //正常状态的文本框，设置为空字符串，其他状态的保持原值。
                            iControl.ControlValue = "";
                        }
                        break;
                }
                
            }

        }
        #endregion

    }
}
