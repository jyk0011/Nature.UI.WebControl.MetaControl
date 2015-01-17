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
 * function: 表单控件和查询控件的基类，属性部分和函数部分
 * history:  created by 金洋 2009-8-18 17:03:18 
 *           2011-4-11 整理
 * **********************************************
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Nature.Attributes;
using Nature.MetaData;
using Nature.MetaData.ControlExtend;
using Nature.MetaData.Entity;
using Nature.MetaData.Entity.MetaControl;
using Nature.MetaData.Enum;
using Nature.MetaData.ManagerMeta;

namespace Nature.UI.WebControl.MetaControl.Form
{
    /// <summary>
    /// 表单控件和查询控件的基类控件
    /// 属性和外部函数部分
    /// 定义字典
    /// </summary>
    public abstract partial class BaseForm 
    {
        #region 表单用的Meta
        /// <summary>
        ///  表单用的Meta
        /// </summary>
        public ManagerFormMeta  ManagerFormColumnMeta
        {
            set { ManagerMeta  = value; }
            get { return (ManagerFormMeta)ManagerMeta; }
        }
        #endregion

        #region 属性
        #region 设置是表单控件还是查询控件
        /// <summary>
        /// 控件的状态，表单控件or查询控件
        /// </summary>
        [Bindable(true)]
        [Category("配置信息")]
        [Localizable(true)]
        [Description("表单控件or查询控件。")]
        public PageViewType ControlKind
        {
            set { ViewState["ControlKind"] = value; }
            get
            {
                if (ViewState["ControlKind"] == null)
                    return PageViewType.DataForm;

                return (PageViewType)ViewState["ControlKind"];
            }
        }
        #endregion

        #region 设置显示几列
        private Int32 _repeatColumns = 1;
        /// <summary>
        /// 显示几列。默认一列。
        /// 相当于DataList的 RepeatColumns 属性
        /// </summary>
        [Bindable(true)]
        [Category("配置信息")]
        [Localizable(true)]
        [Description("表单的列数。默认一列。相当于DataList的 RepeatColumns 属性")]
        public Int32 RepeatColumns
        {
            set { _repeatColumns = value; }
            get { return _repeatColumns; }
        }
        #endregion

        #region 是否已经获取了用户输入的信息

        /// <summary>
        /// 是否已经获取过用户输入的信息
        /// </summary>
        public bool IsGetCustomerData { get; set; }

        #endregion
       
        #region 第一个子控件的ClinetID
        private string _firstChildControlClinetID = "";
        /// <summary>
        /// 第一个子控件的ClinetID
        /// </summary>
        public string FirstChildControlClinetID
        {
            get { return _firstChildControlClinetID; }
            
        }
        #endregion

        #region 获取新添加记录的主键ID值
        private string _newDataID = "";
       
        /// <summary>
        /// 获取新添加记录的主键ID值
        /// </summary>
        public string NewDataID
        {
            get { return _newDataID; }

        }
        #endregion

        #region 获取新添加记录的主键ID值
       
        /// <summary>
        /// 记录的主键ID值
        /// </summary>
        public string DataID { get ; set; }
        #endregion

        #endregion

        #region 定义字典，存放字段信息、控件类型等
        /// <summary>
        /// 存放列表用的字段的描述信息，key：字段ID，value：GridColumnsInfo
        /// </summary>
        public Dictionary<int, IColumn> DicBaseCols;//= new Dictionary<int, GridColumnsInfo>();

        /// <summary>
        /// 记录字段的值。key：字段ID，value：字段值
        /// </summary>
        public Dictionary<int, object> DicColumnsValue;

        /// <summary>
        /// 
        /// </summary>
        protected BaseForm()
        {
            IsGetCustomerData = false;
        }

        #endregion

        #region 外部函数
        //外部取值部分
        #region 通过ColumnID修改控件值
        /// <summary>
        /// 通过ColumnID修改控件值
        /// </summary>
        /// <param name="columnID">字段ID</param>
        /// <param name="value">字段值</param>
        public void SetControlValue(string columnID, string value)
        {
            //通过ColumnID修改控件值
            if (FindControl("ctrl_" + columnID) != null)
            {
                //给控件赋值
                ((IControlHelp)FindControl("ctrl_" + columnID)).ControlValue = value;
                //设置集合的值
                DicColumnsValue[Int32.Parse(columnID)] = value;
              
            }
        }
        #endregion

        #region 通过ColumnID获取控件值
        /// <summary>
        /// 通过ColumnID获取控件值
        /// </summary>
        /// <param name="columnID">字段ID</param>
        public string GetControlValue(string columnID)
        {
            // 通过ColumnID获取控件值
            if (FindControl("ctrl_" + columnID) != null)
                return ((IControlHelp) FindControl("ctrl_" + columnID)).ControlValue;

            return "";
        }

        #endregion

        #region 通过ColumnID获取控件实例
        /// <summary>
        /// 通过ColumnID获取控件实例
        /// </summary>
        /// <param name="columnID">字段ID</param>
        public System.Web.UI.Control GetControl(string columnID)
        {
            return FindControl("ctrl_" + columnID);   //查找功能节点ID
        }
        #endregion

        #region 通过ColumnID获取控件实例
        ///// <summary>
        ///// 通过ColumnID获取控件实例
        ///// </summary>
        ///// <param name="columnID">字段ID</param>
        //public T GetControl<T>(string columnID)
        //{
        //    T re =  (T)Convert.ChangeType( this.FindControl("ctrl_" + columnID),typeof(T));   // 
        //    return re; 
        //}
        #endregion


        //实体类部分，以后会分离出去
        #region 填充实体类
        /// <summary>
        /// 自动给实体类的属性赋值
        /// </summary>
        /// <typeparam name="T">实体类的类型</typeparam>
        /// <param name="t">实例</param>
        public void ControlToEntity<T>(T t)
        {
            //获取用户输入的数据
            GetInputValue();

            //获取类里面的属性
            PropertyInfo[] properties = t.GetType().GetProperties();   // typeof(obj).GetProperties();

            #region 遍历属性
            foreach (PropertyInfo p in properties)
            {
                
                //获取属性里的ColumnIDAttribute的值
                var columnID = (ColumnIDAttribute)p.GetCustomAttributes(typeof(ColumnIDAttribute), false)[0];

                if (columnID.ColumnID == 0)
                    continue;

                if (DicBaseCols.ContainsKey(columnID.ColumnID))
                {
                    //tmpValue = dic_BaseCols[columnID.ColumnID].ColValue;
                    string tmpValue = DicColumnsValue[columnID.ColumnID].ToString();
                    p.SetValue(t, Convert.ChangeType(tmpValue, p.PropertyType), null);  //赋值
                }
            }
            #endregion

        }
        #endregion

        #region 把实体类的属性值设置给控件
        /// <summary>
        /// 把实体类的属性值设置给控件
        /// </summary>
        /// <typeparam name="T">实体类的类型</typeparam>
        /// <param name="t">实例</param>
        public void EntityToControl<T>(T t)
        {
            //获取类里面的属性 
            PropertyInfo[] properties = t.GetType().GetProperties(); 

            #region 遍历属性

            foreach (PropertyInfo p in properties)
            {
                //获取属性里的ColumnIDAttribute的值
                var columnID = (ColumnIDAttribute)p.GetCustomAttributes(typeof(ColumnIDAttribute), false)[0];

                if (columnID.ColumnID == 0)
                    continue;

                if (DicBaseCols.ContainsKey(columnID.ColumnID))
                {
                    string tmpValue = p.GetValue(t, null) == null ? "" : p.GetValue(t, null).ToString();

                    //dic_BaseCols[columnID.ColumnID].ColValue = tmpValue;
                    DicColumnsValue[columnID.ColumnID] = tmpValue;

                    //绑定控件
                    SetControlValue(columnID.ColumnID.ToString(CultureInfo.InvariantCulture), tmpValue);
                }
            }
            #endregion

        }
        #endregion

        //
        #region 获取用户输入的数据
        /// <summary>
        /// 获取用户输入的数据，用 dic_ColumnsValue来装载
        /// </summary>
        public string  GetInputValue()
        {
            if (IsGetCustomerData)      //已经获取过了，退出
                return "";

            //定义取值方式
            string getValueKind = "";
            if (ManagerMeta is ManagerFormMeta)
            {
                if (PageViewMeta.PageViewType == PageViewType.DataForm)
                {
                    switch (PageViewMeta.SQLType)
                    {
                        case SQLType.SQL:
                            getValueKind = "1";
                            break;
                        case SQLType.ParameterSQL:
                        case SQLType.StoredProcedure:
                            getValueKind = "2";
                            break;
                    }
                }
                else
                {
                    getValueKind = "3";
                }

            }

            foreach (KeyValuePair<int, IColumn> info in DicBaseCols)
            {
                var bInfo = (FormColumnMeta)info.Value;
                
                var controlValue = (IControlHelp)FindControl("ctrl_" + bInfo.ColumnID);   //取值的接口
               
                //Page.Response.Write(bInfo.ColSysName);
                //Page.Response.Write(bInfo.ControlCheckKind + "<BR>");

                //if (bInfo.ControlCheckKind == 101)
                //{
                //    //该字段设置为不验证，所以就不用验证了。
                //    //bInfo.ColValue = tmpDataValue;
                //    dic_ColumnsValue[bInfo.ColumnID] = tmpDataValue;
                //    continue;
                //}

                //获取控件的个性属性，
                ControlExt ctrlInfo = bInfo.ControlExtend;
                string tmpDataValue ;   //临时存放数据
                if (ctrlInfo is UniteListExtend)
                {
                    //联动下拉列表框，特殊处理取值
                    var uInfo = (UniteListExtend)ctrlInfo;
                    if (uInfo.IsFristList)
                    {
                        //是第一个下拉列表框，取值
                        tmpDataValue = controlValue.GetControlValue(getValueKind);
                        string[] listValue = tmpDataValue.Split(',');

                        DicColumnsValue[bInfo.ColumnID] = listValue[0];

                        //获取其他列表框，并且取值，赋给字段值的容器
                        int i = 1;
                        foreach (int columnID in uInfo.ListOtherColumnIDs)
                        {
                            DicColumnsValue[columnID] = listValue[i];
                            i++;
                        }
                    }

                }
                else
                {
                    tmpDataValue = controlValue.GetControlValue(getValueKind);

                    #region 验证数据是否符合字段的要求

                    string[] tmpArrValue;  //取值的临时变量，用 `拆分
                    switch (bInfo.ColType)
                    {
                        case "bigint":
                            #region 验证 long
                            long tmplong;
                            if (!Int64.TryParse(tmpDataValue, out tmplong))
                                return InputError(ControlKind, bInfo, tmpDataValue);
                            #endregion
                            break;

                        case "tinyint":
                            #region 验证 tinyint
                            Int16 tmpInt16;
                            if (!Int16.TryParse(tmpDataValue, out tmpInt16))
                                return InputError(ControlKind, bInfo, tmpDataValue);

                            if (tmpInt16 < 0 || tmpInt16 > 255)
                                return InputError(ControlKind, bInfo, tmpDataValue);

                            #endregion

                            break;

                        case "smallint":
                            #region 验证 smallint
                            //Int16 tmpInt16;
                            if (!Int16.TryParse(tmpDataValue, out tmpInt16))
                                return InputError(ControlKind, bInfo, tmpDataValue);

                            #endregion

                            break;

                        case "int":
                            #region 验证 int

                            tmpArrValue = tmpDataValue.Split('`');
                            foreach (string a in tmpArrValue)
                            {
                                if (getValueKind == "3" || !string.IsNullOrEmpty(a)) continue;
                                Int32 tmpInt32;
                                if (!Int32.TryParse(a, out tmpInt32))
                                    return InputError(ControlKind, bInfo, a);
                            }

                            #endregion
                            break;


                        case "numeric":
                        case "smallmoney":
                        case "money":
                        case "decimal":
                            #region 验证 decimal
                            if (getValueKind != "3" && !string.IsNullOrEmpty(tmpDataValue))
                            {
                                decimal tmpdecimal;
                                if (!decimal.TryParse(tmpDataValue, out tmpdecimal))
                                    return InputError(ControlKind, bInfo, tmpDataValue);
                            }

                            #endregion
                            break;

                        case "real":
                        case "float":
                            #region 验证 float
                            float tmpfloat;
                            if (!float.TryParse(tmpDataValue, out tmpfloat))
                                return InputError(ControlKind, bInfo, tmpDataValue);

                            #endregion
                            break;

                        case "uniqueidentifier":
                        case "char":
                        case "nchar":
                        case "varchar":
                        case "nvarchar":
                            break;

                        case "text":
                        case "ntext":
                            break;

                        case "smalldatetime":
                        case "datetime":
                            if (tmpDataValue.Length == 0)
                            {
                                //没有值，不做验证
                            }
                            else
                            {
                                //有值
                                tmpArrValue = tmpDataValue.Split('`');
                                foreach (string a in tmpArrValue)
                                {
                                    if (a.Length > 0)
                                    {
                                        DateTime tmpDateTime;
                                        if (!DateTime.TryParse(a, out tmpDateTime))
                                            return InputError(ControlKind, bInfo, a);
                                    }
                                }
                            }
                            break;

                        case "bit":
                            break;
                    }
                    #endregion

                    DicColumnsValue[bInfo.ColumnID] = tmpDataValue;
                }
                //bInfo.ColValue = tmpDataValue;
            }

            IsGetCustomerData = true;
            return "";
        }

        private static string InputError(PageViewType controlKind, ColumnMeta bInfo, string tmpDataValue)
        {
            //检查是否是查询控件
            if (controlKind == PageViewType.FindForm)
            {
                //查询，判断是否未填，未填写不判断
                if (tmpDataValue.Length == 0)
                    return "";
            }

            //信息格式不正确，即和数据库的字段的格式不符合。
            string msg = "【" + bInfo.ColName + "】的格式不正确，请重新填写！" + tmpDataValue;
            
            //Functions.MsgBox(msg + tmpDataValue, false);

            return msg;

        }
        #endregion

        #endregion

      

    }
}
