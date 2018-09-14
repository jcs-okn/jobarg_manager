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
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/11/05 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// ジョブデータクラス（ジョブアイコンをドラッグする時利用）

    /// </summary>
    public class JobData
    {
        #region プロパティ
        /// <summary>ジョブタイプ </summary>
        private ElementType _jobType;
        public ElementType JobType
        {
            get
            {
                return _jobType;
            }
            set
            {
                _jobType = value;
            }
        }

        /// <summary>別のデータ（例：ジョブネットID） </summary>
        private object _data;
        public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        #endregion

    }
}
