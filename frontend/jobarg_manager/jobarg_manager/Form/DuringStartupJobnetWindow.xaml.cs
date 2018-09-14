﻿/*
** Job Arranger for ZABBIX
** Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
** Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
**
** This program is free software; you can redistribute it and/or modify
** it under the terms of the GNU General Public License as published by
** the Free Software Foundation; either version 2 of the License, or
** (at your option) any later version.
**
** This program is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License
** along with this program; if not, write to the Free Software
** Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using jp.co.ftf.jobcontroller.JobController.Form.JobManager;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.Common;

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// DuringStartupJobnetWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DuringStartupJobnetWindow : BaseWindow
    {
        #region フィールド
        public DispatcherTimer dispatcherTimer;
        public JobArrangerWindow _jobArrangerWindow;
        private String _innerJobnetId;
        private DateTime startTime;
        /// <summary>監視時間秒で指定</summary>
        private static int TimeOut = 300;

        //added by YAMA 2014/07/10 即時起動時の展開完了チェック
        // 一度展開完了検出後、再度展開完了チェックし２秒、間を空ける
        public int checkInvoid_counter = 0;


        #endregion

        #region コンストラクタ
        public DuringStartupJobnetWindow(String innerJobnetId, JobArrangerWindow jobArrangerWindow)
        {
            InitializeComponent();
            _innerJobnetId = innerJobnetId;
            _jobArrangerWindow = jobArrangerWindow;
            DataContext = this;
        }
        #endregion

        #region プロパティ

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "DuringStartupJobnetWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_400;
            }
        }
        #endregion

        /// <summary>展開チェック開始</summary>
        public void startCheck()
        {
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(checkInvoid);
            dispatcherTimer.Start();
            startTime = DateTime.Now;
        }

        /// <summary>展開チェック</summary>
        private void checkInvoid(Object sender, EventArgs e)
        {
            DataTable dt = DBUtil.GetRunJobnetSummary(_innerJobnetId);

            if (dt.Rows.Count > 0)
            {
                //added by YAMA 2014/07/09
                int load_status = (Int32)dt.Rows[0]["load_status"];

                if (load_status == 0)
                {
                    //added by YAMA 2014/07/11
                    int multiple_start_up = (Int32)dt.Rows[0]["multiple_start_up"];
                    // 1：スキップ、2：待ち合せ の時だけ待つ
                    if (multiple_start_up != 0)
                    {
                        //added by YAMA 2014/07/10
                        if (checkInvoid_counter < 1)
                        {
                            checkInvoid_counter++;
                            return;
                        }
                    }

                    checkInvoid_counter = 0;

                    dispatcherTimer.Stop();
                    this.Close();
                    JobnetExecDetail detail = new JobnetExecDetail(_innerJobnetId, false);
                    detail.Topmost = true;
                    detail.Show();
                    return;
                }
                // 遅延起動
                else if (load_status == 2)
                {
                    dispatcherTimer.Stop();
                    this.Close();
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBNET_LOAD_002);
                    return;
                }
                // 実行スキップ
                else if (load_status == 3)
                {
                    dispatcherTimer.Stop();
                    this.Close();
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBNET_LOAD_003);
                    return;
                }
                //added by YAMA 2014/07/10
                else
                {
                    dispatcherTimer.Stop();
                    this.Close();
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBNET_LOAD_001);
                    return;
                }
            }
            DateTime now = DateTime.Now;
            TimeSpan duration;
            duration = now - startTime;
            if (duration.TotalSeconds > TimeOut)
            {
                dispatcherTimer.Stop();
                this.Close();
            }

        }
    }
}
