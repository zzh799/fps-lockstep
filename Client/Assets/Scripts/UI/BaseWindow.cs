using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEditor.UI;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// time:2019/3/24
    /// author:Sun
    /// description:UI窗口基类
    /// </summary>
    public abstract class BaseWindow : Window
    {
        /// <summary>
        /// 当前窗口名字
        /// </summary>
        public abstract string resName { get; }

        /// <summary>
        /// 当前窗口所属Panel
        /// </summary>
        public abstract string packageName { get; }

        /// <summary>
        /// 默认关闭按钮
        /// </summary>
        protected string CloseBtnName = "btnClose";

        /// <summary>
        /// 拖拽区域
        /// </summary>
        protected string DragAreaName = "dragArea";

        public Action OnBtnCloseClicked;
        /// <summary>
        /// 窗口初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            //从页面内创建窗口
            GObject windObj = UIPackage.CreateObject(packageName, resName);
            if (windObj == null)
            {
                throw new System.Exception($"Create Window:{resName} failed");
            }

            contentPane = windObj.asCom;
            container.cachedTransform.position = Vector3.zero;
            container.cachedTransform.localScale = Vector3.one;
            contentPane.SetSize(GRoot.inst.width, GRoot.inst.height);

            closeButton = GetChild(CloseBtnName);
            if (closeButton != null)
            {
                closeButton.onClick.Set(onClick_btnClose);
            }

            //页面所有组件
            for (int i = 0; i < contentPane.numChildren; i++)
            {
                GObject gObject = contentPane.GetChildAt(i);
                if (gObject.name == CloseBtnName)
                {
                    closeButton = gObject;
                }

                if (gObject.name == DragAreaName)
                {
                    dragArea = gObject;
                }
            }

            pivot = new Vector2(0.5f, 0.5f);
        }


        /// <summary>
        /// 显示页面动画,可重写
        /// </summary>
        protected override void DoShowAnimation()
        {
            scale = Vector2.one;
            OnShown();
        }

        /// <summary>
        /// 隐藏页面动画，可重写
        /// </summary>
        protected override void DoHideAnimation()
        {
            base.HideImmediately();
        }

        public override void Dispose()
        {
            OnDestroy();
            base.Dispose();
        }

        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 重写的显示界面
        /// </summary>
        protected override void OnShown()
        {
            base.OnShown();
            this.visible = true;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        private void onClick_btnClose()
        {
            if (OnBtnCloseClicked!=null)
            {
                OnBtnCloseClicked.Invoke();
            }
        }
    }
}