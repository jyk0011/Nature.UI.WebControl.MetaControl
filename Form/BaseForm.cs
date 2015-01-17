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
 * function: 表单控件和查询控件的基类
 * history:  created by 金洋 
 *           2011-4-11 整理
 * **********************************************
 */

using System;
using System.Collections.Generic;
using System.Web.UI;
using Nature.Common;
using Nature.Data;
using Nature.MetaData.ControlExtend;
using Nature.MetaData.Entity;
using Nature.MetaData.Entity.MetaControl;
using Nature.MetaData.Enum;
using Nature.MetaData.Manager;
using Nature.MetaData.ManagerMeta;
using Nature.UI.WebControl.BaseControl.Dictionary;

namespace Nature.UI.WebControl.MetaControl.Form
{
    /// <summary>
    /// 表单控件和查询控件的基类控件
    /// 表单布局部分
    /// </summary>
    public abstract partial class BaseForm : MyDataBoundControl
    {
        private static readonly Dictionary<ControlType, Func<Control>> DicFormControls = DictionaryControl.GetDicFormControls();
        private static readonly Dictionary<ControlType, Func<Control>> DicFindControls = DictionaryControl.GetDicFindControls();

        /// <summary>
        /// 加载子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if ((Site != null) && Site.DesignMode)
            {
                //设计状态，退出
                return;
            }

          
        }

        #region 显示表单

        /// <summary>
        /// 显示表单
        /// </summary>
        public bool ShowForm()
        {
            //True：表单控件；False：查询控件
            //查询控件

            bool isForm = ControlKind != PageViewType.FindForm ;

            #region 加载配置信息

            if (DicBaseCols == null)
                DicBaseCols = ManagerMeta.GetMetaData(null);

            //没有配置信息，退出
            if (DicBaseCols == null)
            {
                if (!isForm) //查询控件没有设置配置信息
                    Functions.MsgBox("", false);
                else
                    Functions.MsgBox("表单控件没有设置配置信息", true);
                return false;
            }

            //创建字段值的容器
            DicColumnsValue = ManagerMeta.CreateDicColumnValue(DicBaseCols);

            #endregion

            #region 加载控件类型

            //if (dic_FormControls == null)
            //    dic_FormControls = ColumnsInfoMgr.LoadFormControl();

            //if (dic_FindControls == null)
            //    dic_FindControls = ColumnsInfoMgr.LoadFindControl();

            #endregion

            //定义接口，通过接口操作子控件

            //开始绘制表格
            Controls.Add(isForm
                             ? new LiteralControl("<table rules=\"all\" class=\"tablem tableClass02\">")
                             : new LiteralControl("<table class=\"tablem tableClass02\">"));

            Int32 index = 0; //用于多列的设置

            #region 循环配置信息

            foreach (KeyValuePair<int, IColumn> info in DicBaseCols)
            {
                var bInfo = (FormColumnMeta) info.Value;

                //判断是不是级联下拉列表框
                var uniteListExtend = bInfo.ControlExtend as UniteListExtend;
                if (uniteListExtend != null)
                {
                    if (uniteListExtend.IsFristList == false)
                    {
                        //不是第一个下拉列表框，略过
                        continue;
                    }
                }

                #region 加载需要的子控件

                Control tmpControl = isForm ? DicFormControls[bInfo.ControlKind]() : DicFindControls[bInfo.ControlKind]();

                #endregion

                var iControl = (IControlHelp) tmpControl;
                tmpControl.ID = "ctrl_" + bInfo.ColumnID; //设置ID

                //iControl.ShowMe(bInfo,dal);

                #region 显示字段

                SetStartTr(index, bInfo); //判断是否显示<TR>

                //添加到表单控件里
                SetStartTd(bInfo); //设置开始的TD
                Controls.Add(new LiteralControl(bInfo.ColName));

                index += SetTDcolspan(bInfo); //设置中间的TD，一个字段占用几个TD
                Controls.Add(tmpControl); //添加控件

                //设置第一个子控件的ClinetID
                if (_firstChildControlClinetID.Length == 0)
                    _firstChildControlClinetID = tmpControl.ClientID;


                //设置帮助信息
                if (bInfo is ModColumnMeta)
                    if (bInfo.ControlHelpStation == 3)
                        Controls.Add(new LiteralControl("&nbsp;" + bInfo.ControlColHelp));


                SetEndTd(bInfo); //设置结束的TD

                SetEndTr(index, bInfo); //判断是否显示</TR> ，有了</TR>就相当于换行了

                #endregion

                //Add 后才会调用Onit函数

                #region 替换当前登录人的一些信息

                //bInfo.ControlInfo = bInfo.ControlInfo.Replace("{PersonID}", base.User.PersonID);
                //bInfo.ControlInfo = bInfo.ControlInfo.Replace("{UserID}", base.User.PersonID);

                #endregion

                if (!base.Page.IsPostBack)
                {
                    //第一次访问
                    iControl.ShowMe(bInfo, ManagerMeta.DalCollection.DalCustomer, isForm); //让子控件自己描绘自己
                    
                    if (ManagerMeta is ManagerFormMeta)
                        if (new ManagerData().TypeOperationData  ==  ButonType.ViewData)
                            //设置只读
                            iControl.SetControlState("2");
                }
                else
                {
                    //回发访问
                    if (!isForm) //查询控件
                        iControl.ShowMe(bInfo, ManagerMeta.DalCollection.DalCustomer, isForm); //让子控件自己描绘自己

                    //有些控件在表单回发的时候也需要再次ShowMe
                    switch (bInfo.ControlKind)
                    {
                        case ControlType.SelectRecords: //选择记录的控件，需要设置前台的onclick事件
                        case ControlType.UniteList: //联动下拉列表框
                            iControl.ShowMe(bInfo, ManagerMeta.DalCollection.DalCustomer, isForm); //让子控件自己描绘自己
                            break;


                    }
                }
                index++;

                if (index >= _repeatColumns) index = 0;

            }

            #endregion

            #region 表格不满需要补充TD

            if (index != 0 && index <= _repeatColumns - 1)
            {
                //表格不满需要补充TD
                for (Int32 i = index; i < _repeatColumns; i++)
                {
                    Controls.Add(new LiteralControl("<TD>&nbsp;</TD>"));
                    Controls.Add(new LiteralControl("<TD>&nbsp;</TD>"));
                }
                Controls.Add(new LiteralControl("</TR>"));
            }

            #endregion

            Controls.Add(new LiteralControl("</Table>"));

            return true;
        }

        #endregion

        #region 设置table

        #region 设置开始的TD

        /// <summary>
        /// 设置开始的TD
        /// </summary>
        /// <param name="formColumnMeta">字段的描述</param>
        private void SetStartTd(IColumn formColumnMeta)
        {
            var fInfo = (FormColumnMeta) formColumnMeta;
            if (fInfo.TdStart > 0)
            {
                //需要合并到上一个TD里面，不显示中间的TD
                string s = "&nbsp;";
                for (int i = 1; i < fInfo.TdStart; i++)
                    s += "&nbsp;";

                Controls.Add(new LiteralControl(s));
            }
            else
            {
                Controls.Add(new LiteralControl("<TD align=\"right\" >"));
            }

        }

        #endregion

        #region 设置结束的TD

        /// <summary>
        /// 设置一个字段占用几个TD
        /// </summary>
        /// <param name="bInfo">字段的描述</param>
        private void SetEndTd(FormColumnMeta bInfo)
        {
            if (bInfo.TdEnd)
            {
                //需要合并下面的TD，不显示结束的TD
            }
            else
            {
                //不需要合并下面的TD，显示结束的TD
                Controls.Add(new LiteralControl("</TD>"));
            }

        }

        #endregion

        #region 设置一个字段占用几个TD

        /// <summary>
        /// 设置一个字段占用几个TD
        /// </summary>
        /// <param name="bInfo">字段的描述</param>
        private Int32 SetTDcolspan(FormColumnMeta bInfo)
        {
            if (bInfo.TdStart > 0)
            {
                //需要合并到上一个TD里面，不显示中间的TD
                Controls.Add(new LiteralControl("："));
                return -1;
            }

            //不合并到上一个TD，显示中间的TD
            Controls.Add(new LiteralControl("：</TD>"));
            if (bInfo.TdColspan >= 2)
            {
                //判断一个字段需要占用几个TD
                Controls.Add(
                    new LiteralControl(string.Format("<TD align=\"left\" colspan=\"{0}\">", (bInfo.TdColspan*2 - 1))));
                return bInfo.TdColspan - 1;
            }

            Controls.Add(new LiteralControl("<TD align=\"left\">"));
            return 0;
        }

        #endregion

        #region 设置是否显示开始的 TR

        /// <summary>
        /// 设置是否显示开始的 TR
        /// </summary>
        /// <param name="index">循环的序号</param>
        /// <param name="bInfo">字段的描述</param>
        private void SetStartTr(Int32 index, FormColumnMeta bInfo)
        {
            if (bInfo.TdStart > 0)
            {
                //合并到上一个TD，不显示TR
            }
            else
            {
                if (index == 0)
                {
                    Controls.Add(bInfo.ControlState == "4"
                                     ? new LiteralControl("<TR id='tr" + bInfo.ColumnID + "' style='display:none;'>")
                                     : new LiteralControl("<TR id='tr" + bInfo.ColumnID + "'>"));
                }
            }
        }

        #endregion

        #region 设置是否显示结束的 TR

        /// <summary>
        /// 设置是否显示结束的 TR
        /// </summary>
        /// <param name="index">循环的序号</param>
        /// <param name="bInfo">字段的描述</param>
        private void SetEndTr(Int32 index, FormColumnMeta bInfo)
        {
            if (bInfo.TdEnd)
            {
                //合并下一个TD，不显示TR
            }
            else
            {
                if (index == _repeatColumns - 1)
                    Controls.Add(new LiteralControl("</TR>"));
            }
        }

        #endregion

        #endregion

        #region 控制设计时的页面

        /// <summary>
        /// 控制设计时的页面。
        /// </summary>
        /// <param name="output"></param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            if ((Site != null) && Site.DesignMode)
            {
                //设计时
                output.Write(ControlKind == PageViewType.FindForm 
                                 ? "<table class=\"css_Form\" id=myFind width=500 height=80><TR><TD colspan=\"2\" align=\"center\">查询控件</TD><TD></TR><TR><TD algin=\"right\">字段1：</TD><TD><input type=\"text\"></TD><TD>字段2：</TD><TD><SELECT><OPTION selected>下拉列表框</OPTION></SELECT></TD></TR></table>"
                                 : "<table class=\"css_Form\" id=myForm width=500 height=80><TR><TD colspan=\"2\" align=\"center\">表单控件</TD><TD></TR><TR><TD algin=\"right\">字段1：</TD><TD><input type=\"text\"></TD></TR><TR><TD>字段2：</TD><TD><SELECT><OPTION selected>下拉列表框</OPTION></SELECT></TD></TR></table>");
            }
            else
            {
                //运行时
                base.RenderContents(output);
                //output.Write(output);
            }

        }

        #endregion

    }
}
