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
    /// EndSettingxaml.xaml の相互作用ロジック
    /// </summary>
    public partial class EndSetting : Window
    {
        #region コンストラクタ 

        public EndSetting(IRoom room, string jobId)
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

            // 終了アイコン設定テーブルの更新 
            DataRow[] drEndIcon = _myJob.Container.IconEndTable.Select("job_id='" + OldJobId + "'");
            if (drEndIcon != null && drEndIcon.Length > 0)
            {
                drEndIcon[0]["job_id"] = newJobId;
                if (cbJobNetStop.IsChecked == true)
                {
                    drEndIcon[0]["jobnet_stop_flag"] = "1";
                }
                else
                {
                    drEndIcon[0]["jobnet_stop_flag"] = "0";
                }
                if (CheckUtil.IsNullOrEmpty(txtEndCode.Text))
                {
                    drEndIcon[0]["jobnet_stop_code"] = "0";
                }
                else
                {
                    drEndIcon[0]["jobnet_stop_code"] = txtEndCode.Text;
                }
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
            txtEndCode.IsEnabled = false;
            cbJobNetStop.IsEnabled = false;

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

            // 終了アイコン設定テーブルのデータを取得 
            DataRow[] rowIconEnd;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowIconEnd = _myJob.Container.IconEndTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowIconEnd = _myJob.Container.IconEndTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowIconEnd != null && rowIconEnd.Length > 0)
            {
                string jobnetStopFlag = Convert.ToString(rowIconEnd[0]["jobnet_stop_flag"]);
                if ("1".Equals(jobnetStopFlag))
                {
                    cbJobNetStop.IsChecked = true;
                }
                else
                {
                    cbJobNetStop.IsChecked = false;
                }
                txtEndCode.Text = Convert.ToString(rowIconEnd[0]["jobnet_stop_code"]);
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
            DataRow[] rowJob =
                    _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
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
            string jobNameForChange = 
                Properties.Resources.err_message_job_name;
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

            // 終了コード
            string endCodeForChange = Properties.Resources.err_message_exit_code;
            String endCode = txtEndCode.Text;

            //added by YAMA 2014/09/30
            // 未入力チェック
            if (CheckUtil.IsNullOrEmpty(endCode))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { endCodeForChange });
                return false;
            }

            //added by YAMA 2014/09/30
            // 半角英数字、ドル記号、アンダーバー以外はエラー
            if (!CheckUtil.IsHankakuStrAndDollarAndUnderbar(endCode))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                    new string[] { endCodeForChange });
                return false;
            }

            //added by YAMA 2014/09/30
            // １文字目が半角英字の場合、エラー
            if (CheckUtil.IsHankakuLerrer(endCode.Substring(0, 1)))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                    new string[] { endCodeForChange });
                return false;
            }

            //added by YAMA 2014/09/30
            // １文字目が半角数値の場合、全て半角数値以外はエラー
            // 数値の範囲は０～２５５以外はエラー
            if (CheckUtil.IsHankakuNum(endCode.Substring(0, 1)))
            {
                if (CheckUtil.IsHankakuNum(endCode.Substring(1)))
                {
                    Int16 endCodeInt = Convert.ToInt16(endCode);
                    if (endCodeInt < 0 || endCodeInt > 255)
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                            new string[] { endCodeForChange });
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                        new string[] { endCodeForChange });
                    return false;
                }
            }

            //added by YAMA 2014/09/30
            // １文字目が＄の場合
            if (endCode.Substring(0, 1) == "$")
            {
                try
                {
                    // ２文字目は半角英字とアンダーバー以外はエラー、＄のみもエラー
                    if (!CheckUtil.IsHankakuLerrerAndUnderbar(endCode.Substring(1, 1)))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                            new string[] { endCodeForChange });
                        return false;
                    }
                    else
                    {
                        //３文字以降は半角英数字とアンダーバー以外はエラー
                        if (!CheckUtil.IsHankakuStrAndUnderbar(endCode.Substring(2)))
                        {
                            CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                                new string[] { endCodeForChange });
                            return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                        new string[] { endCodeForChange });
                    return false;
                }
            }
            else
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_END_SETTING_001,
                    new string[] { endCodeForChange });
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
