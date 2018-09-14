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
using System.Data;
using System.Text;
using jp.co.ftf.jobcontroller.Common;

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/19 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.DAO
{
    /// <summary>
    /// タスクアイコン設定テーブルのDAOクラス
    /// </summary>
    public class ScheduleJobnetDAO : BaseDAO
    {
        #region フィールド


        private string _tableName = "ja_schedule_jobnet_table";

        private string[] _primaryKey = { "schedule_id", "jobnet_id", "update_date" };

        private string _selectSql = "select * from ja_schedule_jobnet_table where 0!=0";

        private string _selectSqlByPk = "select * from ja_schedule_jobnet_table " +
                                        "where schedule_id = ? " +
                                        "and jobnet_id= ? " +
                                        "and update_date = ?";
        
        private string _selectIdSqlBySchedule = "select * from ja_schedule_jobnet_table " +
                                "where schedule_id = ? " +
                                "and update_date = ?";

        private string _selectSqlBySchedule = "select js.*, jj.jobnet_name from ja_schedule_jobnet_table js, " +
                        "(SELECT A.jobnet_id,A.jobnet_name " +
                        "FROM  ja_jobnet_control_table  A " +
                        "WHERE " +
                            "(EXISTS ( " +
                                "SELECT * " +
                                "FROM ja_jobnet_control_table " +
                                "WHERE jobnet_id=A.jobnet_id " +
                                "AND valid_flag=1) and A.valid_flag=1)" +
                        "OR ( " +
                                "NOT EXISTS ( " +
                                "SELECT  * " +
                                "FROM ja_jobnet_control_table " +
                                "WHERE jobnet_id=A.jobnet_id " +
                                "AND valid_flag=1) " +
                            "and " +
                                "NOT EXISTS ( " +
                                "SELECT  * " +
                                "FROM ja_jobnet_control_table " +
                                "WHERE jobnet_id=A.jobnet_id " +
                                "AND update_date>A.update_date) " +
                        ")) jj " +
                        "where js.schedule_id = ? and js.update_date = ? " +
                        "and js.jobnet_id = jj.jobnet_id ";


        private string _selectSqlByScheduleEmpty = "select js.*, jj1.jobnet_name " +
                        "from ja_schedule_jobnet_table js,ja_jobnet_control_table jj1 " +
                        "where 0!=0 " +
                        "and js.jobnet_id = jj1.jobnet_id " +
                        "and jj1.update_date = (select max(jj2.update_date) from ja_jobnet_control_table jj2 group by jj2.jobnet_id having jj1.jobnet_id = jj2.jobnet_id)";

        private DBConnect _db = null;

        #endregion

        #region コンストラクタ

        public ScheduleJobnetDAO(DBConnect db)
        {
            _db = db;
        }
        #endregion

        #region プロパティ

        /// <summary>　テーブル名前 </summary>
        public override string TableName
        {
            get
            {
                return _tableName;
            }
        }

        /// <summary>　キー </summary>
        public override string[] PrimaryKey
        {
            get
            {
                return _primaryKey;
            }
        }

        /// <summary> 検索用のSQL </summary>
        public override string SelectSql
        {
            get
            {
                return _selectSql;
            }
        }

        /// <summary> 検索条件を指定したSQL文 </summary>
        public override string SelectSqlByPk
        {
            get
            {
                return _selectSqlByPk;
            }
        }

        #endregion

        #region publicメソッド

        //************************************************************************
        /// <summary> テーブルの構築</summary>
        /// <return>テーブルの結構</return>
        //************************************************************************
        public DataTable GetEmptyTable()
        {
            _db.CreateSqlConnect();

            DataTable dt = _db.ExecuteQuery(SelectSql);

            return dt;

        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="value_name">ジョブコントローラ変数名</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByPk(object schedule_id, object jobnet_id, object update_date)
        {
            _db.CreateSqlConnect();

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "schedule_id", schedule_id));
            sqlParams.Add(new ComSqlParam(DbType.Int64, "jobnet_id", jobnet_id));
            sqlParams.Add(new ComSqlParam(DbType.Int64, "update_date", update_date));

            DataTable dt = _db.ExecuteQuery(this._selectSqlByPk, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> 
        /// データの取得.
        /// </summary>
        /// <param name="schedule_id">スケジュールＩＤ</param>
        /// <param name="update_date">更新日</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityBySchedule(object schedule_id, object update_date)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "schedule_id", schedule_id));
            sqlParams.Add(new ComSqlParam(DbType.Int64, "update_date", update_date));

            DataTable dt = _db.ExecuteQuery(_selectSqlBySchedule, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> 
        /// データの取得.
        /// </summary>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByScheduleEmpty()
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            DataTable dt = _db.ExecuteQuery(_selectSqlByScheduleEmpty, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> 
        /// データの取得.
        /// </summary>
        /// <param name="schedule_id">スケジュールId</param>
        /// <param name="update_date">更新日</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetIdEntityBySchedule(object schedule_id, object update_date)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "schedule_id", schedule_id));
            sqlParams.Add(new ComSqlParam(DbType.Int64, "update_date", update_date));

            DataTable dt = _db.ExecuteQuery(_selectIdSqlBySchedule, sqlParams, TableName);

            return dt;
        }

        #endregion
    }
}
