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
using System.Windows;
using System.Data;
using System;
using jp.co.ftf.jobcontroller.Common;
using System.Collections.Generic;
using System.Windows.Controls;
using jp.co.ftf.jobcontroller.DAO;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2013/06/03 新規作成<BR>                           *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// ReleaseSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class ReleaseSetting : Window
    {
        #region フィールド

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        #endregion

        #region コンストラクタ
        public ReleaseSetting(IRoom room, string jobId)
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
        #endregion

        #region イベント

        /// <summary>登録処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnToroku_Click(object sender, RoutedEventArgs e)
        {
            // DB接続をオープン 
            dbAccess.CreateSqlConnect();

            // 入力チェック 
            if (!InputCheck())
            {
                // DB接続をクローズ 
                dbAccess.CloseSqlConnect();

                return;
            }

            // DB接続をクローズ 
            dbAccess.CloseSqlConnect();

            //処理前現在データで履歴を作成
            ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.Container)_myJob.Container).CreateHistData();

            // 入力されたジョブID 
            string newJobId = txtJobId.Text;
            // 入力されたジョブ名 
            string newJobNm = txtJobName.Text;

            // ジョブ管理テーブルの更新 
            DataRow[] rowJobCon = _myJob.Container.JobControlTable.Select("job_id='" + _oldJobId + "'");
            if (rowJobCon != null && rowJobCon.Length > 0)
            {
                rowJobCon[0]["job_id"] = newJobId;
                rowJobCon[0]["job_name"] = newJobNm;
            }

            // 情報取得アイコン設定テーブルの更新 
            DataRow[] rowRelease = _myJob.Container.IconReleaseTable.Select("job_id='" + _oldJobId + "'");
            if (rowRelease != null && rowRelease.Length > 0)
            {
                // ジョブID 
                rowRelease[0]["job_id"] = newJobId;

                rowRelease[0]["release_job_id"] = txtReleseJobId.Text;

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
            txtReleseJobId.IsEnabled = false;

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

            // 保留解除アイコン設定テーブルのデータを取得 
            DataRow[] rowRelease;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowRelease = _myJob.Container.IconReleaseTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowRelease = _myJob.Container.IconReleaseTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowRelease != null && rowRelease.Length > 0)
            {
                txtReleseJobId.Text = Convert.ToString(rowRelease[0]["release_job_id"]);
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

            // 保留解除ジョブID
            string releaseJobIdForChange = Properties.Resources.err_message_release_job_id;
            string releaseJobId = txtReleseJobId.Text;
            // 未入力の場合 
            if (CheckUtil.IsNullOrEmpty(releaseJobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { releaseJobIdForChange });
                return false;
            }
            // バイト数チェック 
            if (CheckUtil.IsLenOver(releaseJobId, 1024))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { releaseJobIdForChange, "1024" });
                return false;
            }
            // 半角英数字とハイフン、アンダーバー、スラッシュチェック 
            if (!CheckUtil.IsHankakuStrAndHyphenUnderbarAndSlash(releaseJobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_016,
                    new string[] { releaseJobIdForChange });
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
