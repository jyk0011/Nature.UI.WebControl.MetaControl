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
 * function: 查询控件
 * history:  created by 金洋 
 *           2011-4-11 整理
 *           2012-9-10 输出查询的格式，参数化SQL
 * **********************************************
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using Nature.MetaData.Entity;
using Nature.MetaData.Entity.MetaControl;
using Nature.MetaData.Enum;
using Nature.MetaData.Manager;
using Nature.MetaData.ManagerMeta;
using Nature.UI.WebControl.MetaControl.Form;

namespace Nature.UI.WebControl.MetaControl
{
    /// <summary>
    /// 查询控件
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<Nature:FindForm runat=server></Nature:FindForm>")]
    public class FindForm : BaseForm
    {
        #region 设置查询条件的格式

        /// <summary>
        /// 查询条件的格式：SQL语句，参数化SQL语句
        /// </summary>
        [Bindable(true), Category("配置信息"), Localizable(true), Description("查询条件的格式：SQL语句，参数化SQL语句")]
        public string ResultsKind { get; set; }

        #endregion
         
        /// <summary>
        /// 设置标志
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ControlKind = PageViewType.FindForm ;

        }

        /// <summary>
        /// 加载子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            ManagerMeta = new ManagerFindMeta
                              {
                                  DalCollection = DalCollection,
                                  PageViewID = PageViewID
                              };

            Create();

            RepeatColumns = PageViewMeta.ColumnCount;  //设置列数


            ShowForm(); //绘制控件和表格

           

            //ShowData();
        }

        #region 获取查询条件
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        public string GetSearchWhere()
        {
            var query = new StringBuilder(1000);
            
            //提取用户输入的信息，返回信息是否安全
            string msg = GetInputValue();

            if (msg.Length != 0)
            {
                //输入的信息格式不正确，不能继续
                return msg;    // "<BR>填写的信息格式不正确<BR>" + msg;
            }

            string tableName;
            ManagerFind.SetQuery(DicBaseCols, DicColumnsValue, query,DalCollection.DalCustomer ,out tableName );

            return query.ToString();

        }
        #endregion

     
    }
}
