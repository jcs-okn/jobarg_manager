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
    /// ジョブコマンド設定テーブルのDAOクラス
    /// </summary>
    public class RunJobCommandDAO : BaseDAO
    {
        #region フィールド


        private string _tableName = "ja_run_job_command_table";

        private string[] _primaryKey = { "inner_job_id", "command_cls" };

        private string _selectSql = "select * from ja_run_job_command_table where 0!=0";

        private string _selectSqlByPk = "select * from ja_run_job_command_table " + 
                                        "where inner_jobnet_id = ? " +
                                        "and command_cls = ?";

        private string _selectSqlByJobnet = "select * from ja_run_job_command_table " +
                        "where inner_jobnet_id = ? ";

        private DBConnect _db = null;

        #endregion

        #region コンストラクタ

        public RunJobCommandDAO(DBConnect db)
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
        /// <param name="inner_job_id">実行用ジョブ内部管理ID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByPk(object inner_job_id, object command_cls)
        {
            _db.CreateSqlConnect();

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_job_id", inner_job_id));
            sqlParams.Add(new ComSqlParam(DbType.String, "@command_cls", command_cls));

            DataTable dt = _db.ExecuteQuery(this._selectSqlByPk, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> 
        /// データの取得.
        /// </summary>
        /// <param name="inner_jobnet_id">実行用ジョブネット内部管理ID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByJobnet(object inner_jobnet_id)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", inner_jobnet_id));

            DataTable dt = _db.ExecuteQuery(_selectSqlByJobnet, sqlParams, TableName);

            return dt;
        }

        #endregion
    }
}
