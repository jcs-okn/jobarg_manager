﻿/*
** Job Arranger Manager
** Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
** Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
**
**
** Licensed to the Apache Software Foundation (ASF) under one or more 
** contributor license agreements. See the NOTICE file distributed with
** this work for additional information regarding copyright ownership. 
** The ASF licenses this file to you under the Apache License, Version 2.0
** (the "License"); you may not use this file except in compliance with 
** the License. You may obtain a copy of the License at
**
** http://www.apache.org/licenses/LICENSE-2.0
**
** Unless required by applicable law or agreed to in writing, software
** distributed under the License is distributed on an "AS IS" BASIS,
** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
** See the License for the specific language governing permissions and
** limitations under the License.
**
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;

namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    /// <summary>
    /// JobnetExecControlAllPage.xaml の相互作用ロジック
    /// </summary>
    public partial class JobnetExecControlAllPage : BaseUserControl
    {
        #region コンストラクタ
        public JobnetExecControlAllPage(JobnetExecControlPage parent)
        {
            InitializeComponent();
            _parent = parent;
            DataContext = this;
            _allSelectedInnerJobnetId = "";
        }
        #endregion

        #region プロパティ
        /// <summary>ジョブネット一覧</summary>
        public ObservableCollection<JobnetExecInfo> JobnetExecList { get; set; }

        /// <summary>実行管理画面</summary>
        private JobnetExecControlPage _parent;
        public JobnetExecControlPage Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "JobnetExecControlAllPage";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_300;
            }
        }

        /// <summary>選択された内部実行管理ＩＤ</summary>
        private String _allSelectedInnerJobnetId;
        public String AllSelectedInnerJobnetId
        {
            get
            {
                return _allSelectedInnerJobnetId;
            }
        }
        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>リストPreview右マウスダウン</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ListView_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectItemOnRightClick(e);
            e.Handled = true;
        }

        //*******************************************************************
        /// <summary>リストPreview右マウスUP</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ListView_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectItemOnRightClick(e);
            this.contextMenu.PlacementTarget = sender as UIElement;
            this.contextMenu.IsOpen = true;
        }
        //*******************************************************************
        /// <summary>リスト右マウスダウン</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void FriendsListViewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectItemOnRightClick(e);
        }

        //*******************************************************************
        /// <summary>コンテキストメニューオープン</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void listView_ContextMenuOpening(object sender, RoutedEventArgs e)
        {

            hideContextMenu.IsEnabled = false;
            stopContextMenu.IsEnabled = false;

            //added by YAMA 2014/04/25
            DelayedContextMenu.IsEnabled = false;

            // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
            updtContextMenu.IsEnabled = false;
            reserveContextMenu.IsEnabled = false;
            releaseContextMenu.IsEnabled = false;

            // 何らかのアイテムを選択した状態のとき
            if (listView1.SelectedItems.Count > 0)
            {
                JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)listView1.SelectedItems[0];
                if ((RunJobStatusType)jobnetExecInfo.status == RunJobStatusType.During || (RunJobStatusType)jobnetExecInfo.status == RunJobStatusType.None)
                {
                    stopContextMenu.IsEnabled = true;
                    stopContextMenu.Tag = jobnetExecInfo;
                }

            //added by YAMA 2014/04/25
            // 展開状況が「遅延起動」
            if ((LoadStausType)jobnetExecInfo.load_status == LoadStausType.Delay && (RunJobStatusType)jobnetExecInfo.status == RunJobStatusType.During)
            //if ((LoadStausType)jobnetExecInfo.load_status == LoadStausType.Delay)
            {
                DelayedContextMenu.IsEnabled = true;
                DelayedContextMenu.Tag = jobnetExecInfo;
            }

                // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
                // 「開始予定時刻変更」「起動保留」「起動保留解除」の選択可否を制御
                if (jobnetExecInfo.status == 0)
                {
                    // ステータスが「0：未実行」の場合、「開始予定時刻変更」「起動保留」を選択可能
                    updtContextMenu.IsEnabled = true;
                    updtContextMenu.Tag = jobnetExecInfo;
                    reserveContextMenu.IsEnabled = true;
                    reserveContextMenu.Tag = jobnetExecInfo;
                }
                else
                {
                    updtContextMenu.IsEnabled = false;
                    reserveContextMenu.IsEnabled = false;
                }
                if (jobnetExecInfo.start_pending_flag == 1)
                {
                    // 起動保留フラグが「1：起動保留」の場合、「起動保留解除」を選択可能
                    // added by YAMA 2014/12/03    (V2.1.0 No22)
                    // 「開始予定時刻変更」「起動保留」を選択不可  → 「起動保留」を選択不可
                    /* added by YAMA 2014/12/18    (ステータスが「0：未実行」の場合、「起動保留解除」選択可能) */
                    //releaseContextMenu.IsEnabled = true;
                    //releaseContextMenu.Tag = jobnetExecInfo;
                    if (jobnetExecInfo.status == 0)
                    {
                        releaseContextMenu.IsEnabled = true;
                        releaseContextMenu.Tag = jobnetExecInfo;
                    }

                    // added by YAMA 2014/12/03    (V2.1.0 No22)
                    //updtContextMenu.IsEnabled = false;
                    reserveContextMenu.IsEnabled = false;
                }
                else
                {
                    releaseContextMenu.IsEnabled = false;
                }

            }
            #if VIEWER
                this.contextMenu.Visibility = System.Windows.Visibility.Hidden;
            #endif
        }

        //*******************************************************************
        /// <summary>リスト行選択が変更された場合</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ListView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                JobnetExecInfo selected = (JobnetExecInfo)listView1.SelectedItems[0];
                _allSelectedInnerJobnetId = Convert.ToString(selected.inner_jobnet_id);
            }
        }
        //*******************************************************************
        /// <summary>リスト行データをダブルクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = ((ListViewItem)sender).Content as JobnetExecInfo;
            if (jobnetExecInfo != null) Parent.ViewDetail(jobnetExecInfo.inner_jobnet_id);

        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                listView1.SelectedIndex = listView1.SelectedIndex - 1;
                if (listView1.SelectedIndex < 0)
                    listView1.SelectedIndex = 0;
            }

            if (e.Key == Key.Down)
            {
                listView1.SelectedIndex = listView1.SelectedIndex + 1;
            }
            e.Handled = true;
        }

        #endregion

        #region privateメソッド

        /// <summary>リスト右マウスクリック</summary>
        /// <param name="e">イベント</param>
        private void SelectItemOnRightClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
            {
                Point clickPoint = e.GetPosition(listView1);
                object element = listView1.InputHitTest(clickPoint);
                if (element != null)
                {
                    ListViewItem clickedListViewItem = GetVisualParent<ListViewItem>(element);
                    if (clickedListViewItem != null)
                    {
                        listView1.SelectedItem = clickedListViewItem.Content;
                    }
                    else
                    {
                        listView1.SelectedItem = null;
                    }

                }
            }
        }

        /// <summary>親オブジェクト取得</summary>
        /// <param name="childObject">子オブジェクト</param>
        private T GetVisualParent<T>(object childObject) where T : Visual
        {
            DependencyObject child = childObject as DependencyObject;
            while ((child != null) && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        //*******************************************************************
        /// <summary>開始予定時刻変更</summary>
        /// <param name="sender"></param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ContextUpdateSchedule_Click(object sender, RoutedEventArgs e)
        {
//            int intNum = 0;
            int intNum = 1;

            // 開始ログ
            base.WriteStartLog("ContextUpdateSchedule_Click", Consts.PROCESS_026);

//            // 起動保留 に設定
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)listView1.SelectedItems[0];
//            intNum = DBUtil.Set_Reserve_Jobnet(jobnetExecInfo.inner_jobnet_id);

            // ステータスが「未実行」以外の場合、エラーダイアログを表示
            if (intNum == 1)
            {
                // 開始予定時刻変更画面 を表示
                decimal innerJobnetId = jobnetExecInfo.inner_jobnet_id;
                String jobnetId = jobnetExecInfo.jobnet_id;
                String scheduledTime = jobnetExecInfo.scheduled_time;
                scheduledTime = scheduledTime.Substring(0, 16);
                UpdateScheduleWindow updateWindow = new UpdateScheduleWindow(innerJobnetId, jobnetId, scheduledTime);
                //            updateWindow.Owner = this.Parent;
                updateWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                updateWindow.ShowDialog();

//                // 起動保留 を解除
//                DBUtil.SetReleaseJobnet(jobnetExecInfo.inner_jobnet_id);
            }
            else
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_005);
            }
            // 終了ログ
            base.WriteEndLog("ContextUpdateSchedule_Click", Consts.PROCESS_026);
        }

        #endregion
    }
}
