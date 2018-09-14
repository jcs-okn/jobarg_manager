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
using System.Windows;
using System.Data;
using System;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using System.Windows.Controls;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 旭 2012/11/05 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// TaskSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class TaskSetting : Window
    {
        #region フィールド

        /// <summary>DBConnect</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>ジョブネット管理のDAO</summary>
        private JobnetControlDAO _jobnetControlDAO;

        /// <summary>ジョブネット管理テーブル</summary>
        private DataTable _jobNetTable;


        #endregion

        #region コンストラクタ
        public TaskSetting(IRoom room, string jobId)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

            _jobnetControlDAO = new JobnetControlDAO(dbAccess);

            SetValues(jobId);

            if (_myJob.ContentItem.InnerJobId != null)
            {
                ChangeButton4DetailRef();
            }
        }
        #endregion

        #region プロパティ
        /// <summary>ジョブ</summary>
        private IRoom _myJob;
        public IRoom MyJob
        {
            get
            {
                return _myJob;
            }
            set
            {
                _myJob = value;
            }
        }

        /// <summary>ジョブID</summary>
        private string _oldJobId;
        public string OldJobId
        {
            get
            {
                return _oldJobId;
            }
            set
            {
                _oldJobId = value;
            }
        }
        #endregion

        #region イベント
        /// <summary> 登録処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnToroku_Click(object sender, RoutedEventArgs e)
        {
            // 入力チェック 
            if (!InputCheck())
            {
                return;
            }

            //処理前現在データで履歴を作成
            ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.Container)_myJob.Container).CreateHistData();

            // 入力されたジョブID 
            string newJobId = txtJobId.Text;
            // 入力されたジョブ名 
            string newJobNm = txtJobName.Text;

            // ジョブ管理テーブルの更新 
            DataRow[] drJobCon = _myJob.Container.JobControlTable.Select("job_id='" + OldJobId + "'");
            if (drJobCon != null && drJobCon.Length > 0)
            {
                drJobCon[0]["job_id"] = newJobId;
                drJobCon[0]["job_name"] = newJobNm;
            }

            // タスクアイコン設定テーブルの更新 
            DataRow[] drTask = _myJob.Container.IconTaskTable.Select("job_id='" + OldJobId + "'");
            if (drTask != null && drTask.Length > 0)
            {
                drTask[0]["job_id"] = newJobId;
                drTask[0]["submit_jobnet_id"] = Convert.ToString(combJobNetId.SelectedValue);
            }

            // ジョブIDが変更された場合、フロー管理テーブルを更新 
            if (!_oldJobId.Equals(newJobId))
                CommonUtil.UpdateFlowForJobId(_myJob.Container.FlowControlTable, _oldJobId, newJobId);

            // 画面再表示 
            _myJob.Container.JobItems.Remove(_oldJobId);
            _myJob.Container.JobItems.Add(newJobId, _myJob);
            _myJob.JobId = newJobId;
            _myJob.JobName = newJobNm;
            _myJob.Container.SetedJobIds[_myJob.JobId]="1";
            this.Close();
        }

        /// <summary>ジョブネットIDを変える</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void combJobNetId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string jobNetId = Convert.ToString(combJobNetId.SelectedValue);
            dbAccess.CreateSqlConnect();
            DataTable dtJobNet = _jobnetControlDAO.GetValidORMaxUpdateDateEntityById(jobNetId);
            if (dtJobNet != null && dtJobNet.Rows.Count > 0)
            {
                tbJobNetName.Text = Convert.ToString(dtJobNet.Rows[0]["jobnet_name"]);
            }
            dbAccess.CloseSqlConnect();
        }

        /// <summary>キャンセルをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //*******************************************************************
        /// <summary>画面を閉める</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_myJob.ItemEditType == Consts.EditType.READ && _myJob.Container.ParantWindow is JobEdit)
                _myJob.ResetInitColor();
        }
        #endregion

        #region publicメッソド
        public void SetDisable()
        {
            txtJobId.IsEnabled = false;
            txtJobName.IsEnabled = false;
            btnToroku.IsEnabled = false;
            combJobNetId.IsEnabled = false;

        }
        #endregion

        #region privateメッソド

        /// <summary> 値のセットと表示処理</summary>
        /// <param name="sender">源</param>
        private void SetValues(string jobId)
        {
            // ジョブ管理テーブルのデータを取得 
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                txtJobId.Text = jobId;
                txtJobName.Text = Convert.ToString(rowJob[0]["job_name"]);
            }

            dbAccess.CreateSqlConnect();

            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                _jobNetTable = _jobnetControlDAO.GetInfoByUserIdSuper();
            }
            else
            {
                _jobNetTable = _jobnetControlDAO.GetInfoByUserId(LoginSetting.UserID);
            }

            combJobNetId.ItemsSource = _jobNetTable.DefaultView;
            combJobNetId.DisplayMemberPath = Convert.ToString(_jobNetTable.Columns["jobnet_id"]);
            combJobNetId.SelectedValuePath = Convert.ToString(_jobNetTable.Columns["jobnet_id"]);

            // タスクアイコン設定テーブルのデータを取得 
            DataRow[] rowTask;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowTask = _myJob.Container.IconTaskTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowTask = _myJob.Container.IconTaskTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowTask != null && rowTask.Length > 0)
            {
                string jobNetId = Convert.ToString(rowTask[0]["submit_jobnet_id"]);
                combJobNetId.SelectedValue = jobNetId;

                DataTable dtJobNet = _jobnetControlDAO.GetValidORMaxUpdateDateEntityById(jobNetId);
                if (dtJobNet != null && dtJobNet.Rows.Count > 0)
                {
                    tbJobNetName.Text = Convert.ToString(dtJobNet.Rows[0]["jobnet_name"]);
                }
            }
            dbAccess.CloseSqlConnect();
        }

        /// <summary> 各項目のチェック処理</summary>
        private bool InputCheck()
        {
            // ジョブID 
            string jobIdForChange = Properties.Resources.err_message_job_id;
            String jobId = txtJobId.Text;
            // 未入力の場合 
            if (CheckUtil.IsNullOrEmpty(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { jobIdForChange });
                return false;
            }
            // 桁数チェック 
            if (CheckUtil.IsLenOver(jobId, 32))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { jobIdForChange, "32" });
                return false;
            }
            // 半角英数値、「-」、「_」チェック 
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                    new string[] { jobIdForChange });
                return false;
            }
            // 予約語（START）チェック 
            if (CheckUtil.IsHoldStrSTART(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_001);
                return false;
            }
            // すでに登録済みの場合 
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                foreach (DataRow row in rowJob)
                {
                    if (!jobId.Equals(_oldJobId) && jobId.Equals(row["job_id"]))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                            new string[] { jobIdForChange });
                        return false;
                    }
                }
            }

            // ジョブ名 
            string jobNameForChange = Properties.Resources.err_message_job_name;
            String jobName = txtJobName.Text;
            // バイト数チェック 
            if (CheckUtil.IsLenOver(jobName, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { jobNameForChange, "64" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(jobName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { jobNameForChange });
                return false;
            }

            // ジョブネットID 
            string jobNetIdForChange = Properties.Resources.err_message_jobnet_id;
            String jobNetId = Convert.ToString(combJobNetId.SelectedValue);
            // 未入力の場合 
            if (CheckUtil.IsNullOrEmpty(jobNetId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { jobNetIdForChange });
                return false;
            }

            // 自ジョブネットIDチェック 
            if (_myJob.Container.JobnetId.Equals(jobNetId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_011);
                return false;
            }

            // ジョブネットIDの存在チェック 
            dbAccess.CreateSqlConnect();
            string count = _jobnetControlDAO.GetCountByJobNetId(jobNetId);
            if ("0".Equals(count))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_002);
                return false;
            }
            dbAccess.CloseSqlConnect();

            return true;
        }

        /// <summary> 詳細画面からの参照時のボタンの切り替え</summary>
        private void ChangeButton4DetailRef()
        {
            GridSetting.Children.Remove(btnToroku);
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            btnCancel.Content = Properties.Resources.close_button_text;
            btnCancel.IsDefault = true;
        }
        #endregion
    }
}
