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
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.Common;
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
    /// ExtSettting.xaml の相互作用ロジック
    /// </summary>
    public partial class ExtJobSetting : Window
    {
        #region コンストラクタ
        public ExtJobSetting(IRoom room, string jobId)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

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

        /// <summary>拡張ジョブ定義テーブル</summary>
        private DataTable _dtDefine;
        public DataTable DtDefine
        {
            get
            {
                return _dtDefine;
            }
            set
            {
                _dtDefine = value;
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

            // 拡張ジョブアイコン設定テーブルの更新 
            DataRow[] rowExtJob = _myJob.Container.IconExtjobTable.Select("job_id='" + OldJobId + "'");
            if (rowExtJob != null && rowExtJob.Length > 0)
            {
                rowExtJob[0]["job_id"] = newJobId;
                rowExtJob[0]["command_id"] = Convert.ToString(combExtJob.SelectedValue);
                rowExtJob[0]["value"] = txtParameter.Text;
            }

            // ジョブIDが変更された場合、フロー管理テーブルを更新 
            if (!_oldJobId.Equals(newJobId))
                CommonUtil.UpdateFlowForJobId(_myJob.Container.FlowControlTable, _oldJobId, newJobId);

            // 画面再表示 
            _myJob.Container.JobItems.Remove(_oldJobId);
            _myJob.Container.JobItems.Add(newJobId, _myJob);
            _myJob.JobId = newJobId;
            _myJob.JobName = newJobNm;
            _myJob.Container.SetedJobIds[_myJob.JobId] = "1";
            this.Close();
        }

        /// <summary>拡張ジョブを変える</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void combExtJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string commandId = Convert.ToString(combExtJob.SelectedValue);
            DataRow[] rowd = this._dtDefine.Select("command_id='" + commandId + "'");
            if (rowd != null && rowd.Length > 0)
            {
                tbMemo.Text = Convert.ToString(rowd[0]["memo"]);
            }
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
            combExtJob.IsEnabled = false;
            txtParameter.IsReadOnly = true;

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

            // 拡張ジョブ定義テーブルのデータを取得 
            DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
            dbAccess.CreateSqlConnect();
            DefineExtJobDAO _defineExtJobDAO = new DefineExtJobDAO(dbAccess);
            _dtDefine = _defineExtJobDAO.GetEntityByLang(LoginSetting.Lang);

            dbAccess.CloseSqlConnect();
            combExtJob.Items.Clear();
            combExtJob.ItemsSource = _dtDefine.DefaultView;
            combExtJob.DisplayMemberPath = Convert.ToString(_dtDefine.Columns["command_name"]);
            combExtJob.SelectedValuePath = Convert.ToString(_dtDefine.Columns["command_id"]);

            // 拡張ジョブアイコン設定テーブルのデータを取得 
            string cmdId = "";
            DataRow[] rowExtJob;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowExtJob = _myJob.Container.IconExtjobTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowExtJob = _myJob.Container.IconExtjobTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowExtJob != null && rowExtJob.Length > 0)
            {
                cmdId = Convert.ToString(rowExtJob[0]["command_id"]);
                combExtJob.SelectedValue = cmdId;
                if (!CheckUtil.IsNullOrEmpty(cmdId))
                {
                    DataRow[] rowDefine = _dtDefine.Select("command_id='" + cmdId + "'");
                    if (rowDefine != null && rowDefine.Length > 0)
                    {
                        tbMemo.Text = Convert.ToString(rowDefine[0]["memo"]);
                    }
                }
                txtParameter.Text = Convert.ToString(rowExtJob[0]["value"]);
            }
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
                if (!jobId.Equals(_oldJobId))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                        new string[] { jobIdForChange });
                    return false;
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

            // 拡張ジョブ 
            string extJobForChange = Properties.Resources.err_message_extend_job;
            string extJob = Convert.ToString(combExtJob.SelectedValue);
            // 半角数字チェック 
            if (CheckUtil.IsNullOrEmpty(extJob))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { extJobForChange });
                return false;
            }

            // パラメータ 
            string paramForChange = Properties.Resources.err_message_parameter;
            string param = Convert.ToString(txtParameter.Text);
            // バイト数チェック 
            if (CheckUtil.IsLenOver(param, 4000))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { param, "4000" });
                return false;
            }
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
