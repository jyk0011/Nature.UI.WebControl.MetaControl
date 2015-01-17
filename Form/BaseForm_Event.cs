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
 * function: 表单控件和查询控件的基类，事件部分
 * history:  created by 金洋 
 *           2011-4-11 整理
 * **********************************************
 */

using System;
using System.Web.UI;

namespace Nature.UI.WebControl.MetaControl.Form
{
    /// <summary>
    /// 表单控件和查询控件的基类，事件部分
    /// </summary>
    public abstract partial class BaseForm :  IPostBackEventHandler
    {
        /// <summary>
        /// 事件用
        /// </summary>
        protected static readonly object EventFormBinded = new object();

        #region 定义事件
        /// <summary>
        /// 用户单击页号后，触发的事件，在绑定显示数据的控件之前触发
        /// </summary>
        ///[Description("表单的里控件，绑定数据之后触发。修改模式有效")]
        public event EventHandler FormBinded
        {
            add
            {
                Events.AddHandler(EventFormBinded, value);
            }

            remove
            {
                Events.RemoveHandler(EventFormBinded, value);
            }
        }
        #endregion

        #region 调用外部事件
        /// <summary>
        /// 用户单击页号后，触发的事件，在绑定显示数据的控件之前触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnFormBinded(object sender, EventArgs e)
        {
            var hd = (EventHandler)Events[EventFormBinded];
            if (hd != null)
                hd(sender, e);
        }
        #endregion

        #region 实现接口
        /// <summary>
        /// 实现接口 
        /// </summary>
        /// <param name="s"></param>
        public void RaisePostBackEvent(string s)
        {
            
        }
        #endregion

    }
}
