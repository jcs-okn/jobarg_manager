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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.IO;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// relatedObjectList.xaml の相互作用ロジック
    /// </summary>
    public partial class relatedObjectList : BaseWindow
    {
        #region フィールド


        /// <summary>データ</summary>
        private DataTable _dt;

        //オブジェクト関連チェック added by YAMA 2014/10/17
        private const string RELATEDOBJECT_CALENDAR = "CALENDAR";
        private const string RELATEDOBJECT_FILTER = "FILTER";
        private const string RELATEDOBJECT_SCHEDULE = "SCHEDULE";
        private const string RELATEDOBJECT_JOBNET = "JOBNET";


        #endregion

        #region コンストラクタ
        public relatedObjectList(DataTable dt, String mode)
        {
            InitializeComponent();
            _dt = dt;
            int cnt;

            string objectType;

            List<string> RelatedObjectList = new List<string>();

            objectType = _dt.Rows[0]["ObjectType"].ToString();

            for (cnt = 0; cnt < _dt.Rows.Count; cnt++)
            {
                if (objectType == _dt.Rows[cnt]["ObjectType"].ToString())
                {
                    RelatedObjectList.Add(_dt.Rows[cnt]["RelatedObject"].ToString());
                }
                else 
                {
                    IEnumerable<string> distinctList = RelatedObjectList.Distinct();
                    objectType = _dt.Rows[cnt]["RelatedObject"].ToString();
                    foreach (string Obj in distinctList)
                    {
                        this.RelatedObject.Text += getRelatedObjectType(Obj) + Environment.NewLine;
                    }
                    RelatedObjectList.Clear();
                    RelatedObjectList.Add(objectType);
                }

            }

            if (cnt == _dt.Rows.Count)
            {
                IEnumerable<string> distinctList = RelatedObjectList.Distinct();
                foreach (string Obj in distinctList)
                {
                    this.RelatedObject.Text += getRelatedObjectType(Obj) + Environment.NewLine;
                }
                RelatedObjectList.Clear();
            }

            
            this.ErrMessage.Content = _dt.Rows[0]["ErrMessage"];
            if (mode == "DelObject")
            {
                this.force_button.IsEnabled = false;
            }

        }
        #endregion

        #region プロパティ
        /// <summary>強制実行フラグ</summary>
        private bool _forceRun;
        public bool ForceRun
        {
            get
            {
                return _forceRun;
            }
            set
            {
                _forceRun = value;
            }
        }

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "relatedObjectList";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_260;
            }
        }
        #endregion

        #region イベント

        //*******************************************************************
        /// <summary>ＯＫボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void close_button_Click(object sender, EventArgs e)
        {
            ForceRun = false;
            Close();
        }


        //*******************************************************************
        /// <summary>強制実行ボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void force_button_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = CommonDialog.ForceRunDialog();
            if (result == MessageBoxResult.Yes)
            {
                ForceRun = true;
            }
            Close();
        }

        #endregion

        #region privateメソッド

        private string getRelatedObjectType(string objtype)
        {
            string chgstr = "";

            switch (objtype)
            {
                case RELATEDOBJECT_CALENDAR:
                    chgstr = Properties.Resources.relatedobject_calendar;
                    break;

                case RELATEDOBJECT_FILTER:
                    chgstr = Properties.Resources.relatedobject_filter;
                    break;

                case RELATEDOBJECT_SCHEDULE:
                    chgstr = Properties.Resources.relatedobject_schedule;
                    break;

                case RELATEDOBJECT_JOBNET:
                    chgstr = Properties.Resources.relatedobject_jobnet;
                    break;
                default:
                    chgstr = objtype;
                    break;
            }

            return chgstr;
        }
        #endregion
    }
}
