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

using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Nature.MetaData.Entity;
using Nature.MetaData.Entity.WebPage;
using Nature.MetaData.Manager;
using Nature.MetaData.ManagerMeta;
using Nature.User;
using Nature.Data;

namespace Nature.UI.WebControl.MetaControl.Form
{
    /// <summary>
    /// 表单控件、查询控件和数据列表控件的基类
    /// </summary>
    public abstract class MyDataBoundControl : DataBoundControl, INamingContainer
    {

        //属性

        #region 模块ID

        /// <summary>
        /// 获取或者设置模块ID
        /// </summary>
        /// <value>
        /// 模块ID
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:18
        public int ModuleID
        {
            set { ViewState["ModuleID"] = value; }
            get { return int.Parse(ViewState["ModuleID"].ToString()); }
        }

        #endregion

        #region 页面视图ID

        /// <summary>
        /// 获取或者设置页面视图ID
        /// </summary>
        /// <value>
        /// 模块ID
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:18
        public int PageViewID
        {
            set { ViewState["PageViewID"] = value; }
            get { return int.Parse(ViewState["PageViewID"].ToString()); }
        }

        #endregion

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

        #region 元数据——页面视图

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

        #region 元数据——页面里的字段

        /// <summary>
        /// 页面里的字段的元数据的集合
        /// </summary>
        /// <value>
        /// 字段的元数据的集合
        /// </value>
        /// user:jyk
        /// time:2012/9/12 14:42
        public Dictionary<int, ColumnMeta> PageViewColumnMeta { set; get; }

        #endregion

        #region ManagerMeta

        /// <summary>
        /// 管理元数据，加载列表、表单元数据的实体类
        /// </summary>
        /// <value>
        /// 模块ID
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:18
        public ManagerMeta ManagerMeta { get; set; }

        #endregion

        #region ManagerData

        /// <summary>
        /// 表单控件需要的添加、修改、提取数据用的
        /// </summary>
        /// <value>
        /// 模块ID
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:18
        public ManagerData ManagerData { set; get; }

        #endregion

        #region 可以使用的字段，1,2,3 的形式

        /// <summary>
        /// 可以使用的字段，1,2,3 的形式
        /// </summary>
        /// <value>
        /// 可以使用的字段，1,2,3 的形式
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:18
        public string CanUseColumns { get; set; }

        #endregion

        #region 当前登录人的信息

        /// <summary>
        /// 当前登录人的信息
        /// </summary>
        /// <value>
        /// 用户在线信息
        /// </value>
        /// user:jyk
        /// time:2012/9/5 10:18
        public UserOnlineInfo UserOnlineInfo { get; set; }

        #endregion

        /// <summary>
        /// 根据属性，实例化需要的数据
        /// </summary>
        /// user:jyk
        /// time:2012/9/12 14:46
        public void Create()
        {
            if (PageViewMeta == null)
            {
                //获取页面视图
                var mgrPVM = new ManagerPageViewMeta
                                 {
                                     DalCollection = DalCollection,
                                     PageViewID = PageViewID
                                 };

                PageViewMeta = mgrPVM.GetPageViewMeta(null);
            }

            if (ManagerData == null)
            {
                ManagerData = new ManagerData
                                  {
                                      Dal = DalCollection.DalCustomer,
                                      DictFormColumnMeta = ManagerMeta.GetMetaData(null),
                                      PageViewMeta = PageViewMeta 
                                  };
            }
        }


    }

}
