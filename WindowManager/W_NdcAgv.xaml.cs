﻿using ModuleManager;
using ModuleManager.NDC;
using NdcManager.DataGrid;
using NdcManager.DataGrid.Models;
using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskManager;
using WindowManager.Datagrid;

namespace WindowManager
{
    /// <summary>
    /// W_NdcAgv.xaml 的交互逻辑
    /// </summary>
    public partial class W_NdcAgv : UserControl, ITabWin
    {

        #region Property

        NdcAgvDataGrid taskDataGrid;

        #endregion

        public W_NdcAgv()
        {
            InitializeComponent();

            taskDataGrid = new NdcAgvDataGrid();

            DataContext = taskDataGrid;
            DataControl._mNDCControl.NoticeUpdate += _mNDCControl_TaskListUpdate;
            DataControl._mNDCControl.NoticeDelete += _mNDCControl_TaskListDelete;
            DataControl._mNDCControl.NoticeRedirect += _mNDCControl_NoticeRedirect;
            DataControl._mNDCControl.NoticeMsg += _mNDCControl_NoticeMsg;
        }

        private void _mNDCControl_NoticeMsg(string msg)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notice.Show(msg, "Notice", 10, MessageBoxIcon.Error);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 关闭窗口的时候执行释放的动作
        /// </summary>
        public void Close()
        {
            DataControl._mNDCControl.NoticeUpdate -= _mNDCControl_TaskListUpdate;
            DataControl._mNDCControl.NoticeDelete -= _mNDCControl_TaskListDelete;
            DataControl._mNDCControl.NoticeRedirect -= _mNDCControl_NoticeRedirect;
            DataControl._mNDCControl.NoticeMsg -= _mNDCControl_NoticeMsg;
        }
        private void _mNDCControl_NoticeRedirect(NDCItem model)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notice.Show("任务ID:" + model._mTask.TASKID + "\nOrder:" + model._mTask.NDCINDEX + "\nIkey:" + model._mTask.IKEY + "\n需要重定向!!", "Notice", 10, MessageBoxIcon.Info);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void _mNDCControl_TaskListDelete(NDCItem model)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    taskDataGrid.DeleteTask(model);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void _mNDCControl_TaskListUpdate(NDCItem item)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    taskDataGrid.UpdateTaskInList(item);
                });
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(taskId.Text, out int taskid))
            {
                Notice.Show("任务ID必须是整型数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }

            if (!WindowCommon.ConfirmAction("是否请求[AGV任务]！！"))
            {
                return;
            }

            if (!DataControl._mNDCControl.AddNDCTask(taskid, loadSite.Text, unloadSite.Text, out string result))
            {
                Notice.Show(result, "错误", 3, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 缺货
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadAgvBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(taskId.Text, out int taskid))
            {
                Notice.Show("任务ID必须是整型数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(agvName.Text, out int agvid))
            {

                Notice.Show("AGVID必须是整型数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }

            if (!WindowCommon.ConfirmAction("是否进行[取货任务]！！"))
            {
                return;
            }

            if (!DataControl._mNDCControl.DoLoad(taskid, agvid, out string result))
            {
                Notice.Show(result, "错误", 3, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 卸货
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnloadAgvBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(taskId.Text, out int taskid))
            {
                Notice.Show("任务ID必须是数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(agvName.Text, out int agvid))
            {
                Notice.Show("AGVID必须是整型数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }

            if (!WindowCommon.ConfirmAction("是否进行[卸货任务]！！"))
            {
                return;
            }

            if (!DataControl._mNDCControl.DoUnLoad(taskid, agvid, out string result))
            {
                Notice.Show(result, "错误", 3, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 进行重定向
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedirectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(taskId.Text, out int taskid))
            {
                Notice.Show("任务ID必须是整型数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }

            int orderint = -1;
            if (order.Text != "" && !int.TryParse(order.Text,out orderint))
            {
                Notice.Show("Order必须是数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }

            if (!WindowCommon.ConfirmAction("是否进行[重定向任务]！！"))
            {
                return;
            }

            if (!DataControl._mNDCControl.DoReDerect(taskid, redirectArea.Text, out string result, orderint))
            {

                Notice.Show(result, "错误", 3, MessageBoxIcon.Error);
            }
        }

        private void DgCustom_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row != null)
            {

            }
        }

        private void deleteorder_Click(object sender, RoutedEventArgs e)
        {

            int i = -1;
            if (index.Text != "" && !int.TryParse(index.Text, out i))
            {
                Notice.Show("Index必须是数字", "错误", 3, MessageBoxIcon.Error);
                return;
            }

            if (!WindowCommon.ConfirmAction("确定需要[取消任务]！！"))
            {
                return;
            }

            if (!DataControl._mNDCControl.DoCancelIndex(i, out string result))
            {

                Notice.Show(result, "错误", 3, MessageBoxIcon.Error);
            }
        }
    }

}
