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
 * NatureFW.MetaControl is free software. You are allowed to download, modify and distribute 
 * the source code in accordance with LGPL 2.1 license, however if you want to use 
 * NatureFW.MetaControl on your site or include it in your commercial software, you must  be registered.
 * http://www.natureFW.com/registered
 */

/* ***********************************************
 * author :  金洋（金色海洋jyk）
 * email  :  jyk0011@live.cn  
 * function: 显示数据列表的控件
 * history:  created by 金洋 
 *           2011-4-11 整理
 *           2012-8-31 检查
 * **********************************************
 */


using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.UI;
using Nature.Common;
using Nature.MetaData.Entity;
using Nature.MetaData.Entity.MetaControl;
using Nature.MetaData.ManagerMeta;
using Nature.UI.WebControl.MetaControl.Form;

namespace Nature.UI.WebControl.MetaControl
{
    /// <summary>
    /// 专门显示数据的控件
    /// </summary>
    [ToolboxData("<Nature:DataGridList runat=server></Nature:DataGridList>")]
    public class DataGridList : MyDataBoundControl 
    {
        /// <summary>
        /// 存放列表用的字段的描述信息，key：字段ID，value：GridColumnMeta  ColumnMeta
        /// </summary>
        public Dictionary<int, IColumn> DicGridCols;//= new Dictionary<int, GridColumnsInfo>();

        #region DataBind()
        /// <summary>
        /// 绑定数据
        /// </summary>
        public override void DataBind()
        {
            ManagerMeta = new ManagerGridMeta
                              {
                                  DalCollection = DalCollection,
                                  PageViewID = PageViewID
                              };

            Create();

            if ((Site != null) && Site.DesignMode)
            {
                //设计模式，退出
                return;
            }

            //base.DataBind();

            #region 加载配置信息

            if (DicGridCols == null)
                DicGridCols = ManagerMeta.GetMetaData(null);

            //没有配置信息，退出
            if (DicGridCols == null)
            {
                Functions.MsgBox("Grid没有设置配置信息", true);
                return;
            }

            #endregion

            #region 判断是否要锁定行列

            bool isLock;


            if (PageViewMeta.LockColumns == 0 && PageViewMeta.LockRows == 0)
                //不锁定
                isLock = false;
            else
                //锁定
                isLock = true;

            #endregion

            var html = new StringBuilder(10000);

            //输出表格
            WriteTable(html, isLock);

            Controls.Add(new LiteralControl(html.ToString()));

        }

        #endregion

        #region 输出主体表格
        /// <summary>
        /// 输出主体表格
        /// </summary>
        /// <param name="html">拼接的HTML</param>
        /// <param name="isLock">是否锁定行列</param>
        private void WriteTable(StringBuilder html, bool isLock)
        {
            var mgrGridMeta = (ManagerGridMeta)ManagerMeta;

            //获取用户可以访问的字段ID
            int[] columnIDs = mgrGridMeta.GetUserListColumns("");//UserOnlineInfo.RoleIDs

            #region 输出<table> 包括锁定设置
            if (isLock)
            {
                //锁定行列，输出DIV
                html.Append("\n\r<div id=\"divMain\" class=\"div_Main\" style=\"overflow:auto;width:100%;\" onscroll=\"myScroll(this)\">");

                //输出table
                html.Append("<table id=\"");  // class=\"css_Grid\"
                html.Append(ClientID);  // class=\"css_Grid\"  style=\"width:1200px;\"
                html.Append("_table\"  ");  //
                html.Append(" style=\"width:");  //根据配置信息设置表格的宽度
                html.Append(PageViewMeta.TableWidth == 0 ? DicGridCols.Count * 120 : PageViewMeta.TableWidth);
                html.Append("px;\" ");  //

            }
            else
            {
                //不锁定行列，输出table
                html.Append("<table id=\"");  // class=\"css_Grid\"
                html.Append(ClientID);  // class=\"css_Grid\"  style=\"width:1200px;\"
                html.Append("_table\" style=\"width:100%;\" ");  //
            }

            html.Append(" rules=\"all\" class=\"tbl_Data\" >");  //
            #endregion

            #region 输出页眉
            html.Append("<tr ID=\"tr0\">");    //class=\"css_GridTR\"

            //foreach (KeyValuePair<int, ColumnsInfo> info in dic_GridCols)
            string canUseCol = CanUseColumns;
            foreach (int id in columnIDs )
            {
                bool canShow = true;
                if (canUseCol.Length != 0)
                    if (!canUseCol.Contains(id.ToString(CultureInfo.InvariantCulture)))
                        canShow = false ;
                
                if (canShow)
                {
                    html.Append("<th>");
                    //html.Append(((GridInfo)info.Value).ColName);
                    html.Append(((ColumnMeta)DicGridCols[id]).ColName);
                    html.Append("</th>");    
                }
                
            }
            html.Append("</tr>");
            #endregion

            #region 输出数据
            if (DataSource != null)
            {
                //输出js脚本
                var js = new StringBuilder();
                js.Append("<script >var myGridDataID = \"" + ClientID + "\"</script>");
                base.Page.Response.Write(js.ToString());

                var dt = DataSource as DataTable;
                if (dt != null)
                {
                    BindbyDataTable(html, dt, columnIDs);
                }

                var dic = DataSource as Dictionary<int,Dictionary<int,object>>;
                if (dic!=null)
                {
                    BindbyDictionary(html, dic, columnIDs);
                }
            }
            #endregion

            html.Append("</table>");

            if (isLock)
            {
                html.Append("</div>");
            }
           
        }
        #endregion

        #region 用DataTable绑定列表

        /// <summary>
        /// 用DataTable绑定列表
        /// </summary>
        /// <param name="html">拼接table的 HTML.</param>
        /// <param name="dt">DataTable格式的数据源</param>
        /// <param name="columnIDs">用户角色允许查看的字段ID（列）</param>
        /// user:jyk
        /// time:2012/9/5 14:10
        private void BindbyDataTable(StringBuilder html,DataTable dt, int[] columnIDs)
        {
            //主键的字段名
            string idColumn = PageViewMeta.PKColumn; 
            
            //定义交替次数
            //int t = 0;

            int i = 1;//自增变量，给TR设置ID用

            string canUseCol = CanUseColumns;
             
            foreach (DataRow dr in dt.Rows)
            {
                
                //获取主键字段的值
                string dataID = dr[idColumn].ToString();     //数据主键值

                html.Append("<tr ID=\"tr");
                html.Append(i); i++;
                html.Append("\" "); //myclass=\"css_TR_c
                //html.Append(t % 2 + 1);
                //html.Append("\" "); //class=\"css_TR_c
                //html.Append(t % 2 + 1); t++;
                html.Append(" onclick=\"Ck(this,'"); //onmouseover=\"Over(this)\" onmouseout=\"Out(this)\"
                html.Append(dataID);
                html.Append("')\" ondblclick=\"DbCK(this,");
                html.Append(dataID);
                html.Append(")\">");

                //foreach (KeyValuePair<int, ColumnsInfo> info in dic_GridCols)
                foreach (int id in columnIDs)
                {
                    var gInfo = (GridColumnMeta)DicGridCols[id];   //循环里面的字段信息

                    if (canUseCol.Length != 0)
                        if (!canUseCol.Contains(id.ToString(CultureInfo.InvariantCulture)))
                            continue;

                    html.Append("<td ");
                    #region align
                    if (gInfo.Align.Length > 0 && gInfo.Align != "left")
                    {
                        html.Append("align=\"");
                        html.Append(gInfo.Align);
                        html.Append("\" ");
                    }
                    #endregion

                    #region width
                    if (gInfo.ColWidth != 0)
                    {
                        html.Append("width=\"");
                        html.Append(gInfo.ColWidth);
                        html.Append("\" ");
                    }
                    #endregion

                    html.Append(">");

                    #region 设置数据的长度和格式
                    string tmpValue = dr[gInfo.ColSysName].ToString();        //循环里的字段值
                    if (gInfo.MaxLength != 0)
                        tmpValue = Functions.StringCut(tmpValue, gInfo.MaxLength);

                    if (gInfo.Format.Length != 0)
                    {
                        //判断数据库的字段类型，然后先转换再格式化。
                        switch (gInfo.ColType)
                        {
                            case "datetime":
                            case "smalldatetime":
                                if (tmpValue.Length > 0)
                                {
                                    var tmpTime = (DateTime)dr[gInfo.ColSysName];
                                    tmpValue = tmpTime == DateTime.Parse("1900-1-1") ? "" : string.Format(gInfo.Format, dr[gInfo.ColSysName]);
                                }
                                break;

                            default:
                                tmpValue = string.Format(gInfo.Format, dr[gInfo.ColSysName]);
                                break;

                        }
                    }
                    #endregion

                    html.Append(tmpValue);
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }
        }
        #endregion

        #region 用 Dictionary 绑定列表
        /// <summary>
        /// Bindbies the dictionary.
        /// </summary>
        /// <param name="html">拼接table的 HTML.</param>
        /// <param name="dic">Dictionary 格式的数据源</param>
        /// <param name="columnIDs">用户角色允许查看的字段ID（列）</param>
        /// user:jyk
        /// time:2012/9/5 14:11
        private void BindbyDictionary(StringBuilder html, Dictionary<int, Dictionary<int, object>> dic, int[] columnIDs)
        {
            
        }
        #endregion

        #region 设计时支持
        /// <summary>
        /// 设计时支持
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            if ((Site != null) && Site.DesignMode)
                output.Write("<table class=\"css_Grid\"><Caption>myGrid数据显示控件</Caption><tr class=\"css_GridTR\"><td>页眉1</td><td>页眉2</td><tr><tr><td>数据绑定</td><td>数据绑定</td><tr></table>");
            else
                base.Render(output);
        }
        #endregion

    }

}
