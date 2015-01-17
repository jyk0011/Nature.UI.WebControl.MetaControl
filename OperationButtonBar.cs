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
 * function: 按钮组
 * history:  created by 金洋 
 *           2011-4-11 整理
 * **********************************************
 */


using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Nature.Data;
using Nature.MetaData.Entity;
using Nature.MetaData.Entity.MetaControl;
using Nature.MetaData.Entity.WebPage;
using Nature.MetaData.Enum;
using Nature.MetaData.ManagerMeta;

namespace Nature.UI.WebControl.MetaControl
{
    /// <summary>
    /// 按钮组
    /// </summary>
    [ToolboxData("<Nature:OperationButtonBar runat=server></Nature:OperationButtonBar>")]
    public class OperationButtonBar : System.Web.UI.WebControls.Panel 
    {
        #region 属性
        #region 访问数据库的实例，四个

        /// <summary>
        /// 访问数据库的实例的集合，四个
        /// </summary>
        /// <value>
        /// 访问数据库的实例的集合
        /// </value>
        /// user:jyk
        /// time:2012/9/13 10:52
        public DalCollection DalCollection { set; get; }

        #endregion

        #region 获取或者设置页面视图的元数据
        /// <summary>
        /// 获取或者设置页面视图的元数据
        /// </summary>
        /// <value>
        /// 页面视图的元数据
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:57
        public PageViewMeta PageViewMeta { set; get; }
        #endregion

        #region 模块ID
        /// <summary>
        /// 模块ID
        /// </summary>
        [Bindable(true)]
        [Category("配置信息")]
        [Localizable(true)]
        [Description("模块ID，用于提取配置信息")]
        public int ModuleID
        {
            set { ViewState["_ModuleID"] = value; }
            get
            {
                if (ViewState["_ModuleID"] == null)
                    return 0;
                return int.Parse(ViewState["_ModuleID"].ToString());
            }
        }
        #endregion

        #region 视图ID
        /// <summary>
        /// 视图ID
        /// </summary>
        [Bindable(true)]
        [Category("配置信息")]
        [Localizable(true)]
        [Description("视图ID，用于提取配置信息")]
        public int PageViewID
        {
            set { ViewState["_PageViewID"] = value; }
            get
            {
                if (ViewState["_PageViewID"] == null)
                    return 0;
                return int.Parse(ViewState["_PageViewID"].ToString());
            }
        }
        #endregion

        #region 设置权限，当前用户对于指定的模块可以使用的按钮ID集合
        private string _roleButtonID = "";
        /// <summary>
        /// 设置权限，当前用户对于指定的模块可以使用的按钮ID集合
        /// </summary>
        [Bindable(true)]
        [Category("配置信息")]
        [Localizable(true)]
        [Description("设置权限，当前用户对于指定的模块可以使用的按钮ID集合")]
        public string UserCanUseButtonID
        {
            set { _roleButtonID = value.TrimEnd(','); }
            get { return _roleButtonID; }
        }
        #endregion

        #endregion

        /// <summary>
        /// 加载子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Attributes.Add("align", "left");

            var mgrButton = new ManagerButtonListMeta
                                {DalCollection = DalCollection,
                                    ModuleID = ModuleID, 
                                    RoleColumnID = _roleButtonID
                                };

            var buttonList = mgrButton.GetMetaData(null);
           
            string userButtonID = "," + _roleButtonID + ",";

            //Functions.MsgBox(_roleButtonID + "<br>" + userButtonID + "<br>" + sql, false);

            var btnIDs = new[] { "btnLook", "btnAdd", "btnUpdate", "btnDel", "btnFind", "btnExcel", "btnAccess", "AddUpdateData", "Hyperlinks", "a", "b", "c" };
            var btnIDCount = new[] {0, 0, 0, 0, 0, 0, 0,0,0,0,0,0};

            if (buttonList != null)
            {
                #region 遍历，绘制按钮
                foreach (KeyValuePair<int, IColumn> a in buttonList)
                {
                    var button = (ButtonListMeta)a.Value;
                    var btn = new HtmlInputButton {Value = button.Title };      //按钮
                    btn.Attributes.Add("class", "btn");

                    //判断权限，有权限，创建按钮。
                    if (userButtonID.Contains( "," + button.ButtonID + ",") || userButtonID == ",," || userButtonID == "")
                    {
                        //设置了按钮ID，或者没有设置权限到按钮
                        //设置ID
                        int btnIndex = (int) button.ButonType - 401;
                        string tmpID = btnIDs[btnIndex];

                        btnIDCount[btnIndex]++;
                        if (btnIDCount[btnIndex] > 1)
                            tmpID += "_" + btnIDCount[btnIndex];
                        btn.ID = tmpID;

                        string value;

                        #region 根据按钮类型添加前台事件
                        switch (button.ButonType)
                        {
                            case ButonType.AddData:   // "1"添加 
                            case ButonType.UpdateData:   // "2"修改
                            case ButonType.ViewData:   // "5"查看
                            case ButonType.AddUpdateData:   // "6"添加后修改
                                //                  url, mdId,   mpvid, fpvId, btnId , w , h )
                                value = "btnOpenWeb('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
                                value = string.Format(value, button.URL, button.OpenModuleID,
                                    button.OpenPageViewID ,button.FindPageViewID , button.ButtonID, button.WebWidth, button.WebHeight);

                                btn.Attributes.Add("onclick",value);
                                Controls.Add(btn);
                                break;

                            case ButonType.DeleteData  :        //删除
                                //                    url,  mdId, dpvId, btnId
                                value = "DeleteData('{0}','{1}','{2}','{3}')";
                                value = string.Format(value, button.URL, button.OpenModuleID, button.OpenPageViewID, button.ButtonID);
                                btn.Attributes.Add("onclick", value );
                                Controls.Add(btn);
                                break;

                            case ButonType.FindData :       //查询
                                btn.Attributes.Add("onclick", "btnSearch()");
                                Controls.Add(btn);
                                break;

                            case ButonType.OutputExcel :        //导出到Excel
                                btn.Attributes.Add("onclick", "btnExcel()");
                                Controls.Add(btn);
                                break;

                            case ButonType.OutpuAccess :       //导出到Access
                                btn.Attributes.Add("onclick", "btnAccess()");
                                Controls.Add(btn);
                                break;

                        }
                        #endregion

                        #region 设置需要先选择一条记录的提示
                        if (button.IsNeedSelect)
                        {
                            //需要记录ID
                            btn.Disabled = true;
                            btn.Attributes.Add("title", "请先选择记录，然后在单击按钮。");
                        }
                        #endregion

                        var space = new LiteralControl("&nbsp;") { ID = "s" + button.ButtonID};     //间隔
                        Controls.Add(space);
                    }
                }

                #endregion
            }
        }

        #region 设计时支持
        /// <summary>
        /// 设计时支持
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            if ((Site != null) && Site.DesignMode)
                output.Write("<input type=\"button\" value=\"按钮组\">操作按钮组");
            else
                base.Render(output);
        }
        #endregion
    }
}
