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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using jp.co.ftf.jobcontroller.Common;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/04 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// JobNet.xaml の相互作用ロジック
    /// </summary>
    public partial class JobNet : UserControl, IElement
    {
        #region コンストラクタ
        public JobNet()
        {
            InitializeComponent();
            this.DataContext = new IconViewData();
        }

        public JobNet(RunJobMethodType methodType)
        {
            InitializeComponent();
            _methodType = methodType;
            this.DataContext = new IconViewData();
        }

        public JobNet(SolidColorBrush color)
        {
            InitializeComponent();
            picJobnet.Fill = color;
            this.DataContext = new IconViewData();
        }

        //added by YAMA 2014/07/01
        public JobNet(SolidColorBrush color, SolidColorBrush characterColor)
        {
            InitializeComponent();
            picJobnet.Fill = color;
            tbJobId.Foreground = characterColor;		// 文字の色 設定
            tbJobName.Foreground = characterColor;		// 文字の色 設定
            this.DataContext = new IconViewData();
        }

        #endregion

        #region プロパティ
        // コンテナ
        IContainer _container;
        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        /// <summary>ジョブId</summary>
        private string _jobId;
        public string JobId
        {
            get
            {
                return _jobId;
            }
            set
            {
                _jobId = value;

                // 表示文字をセット
                tbJobId.Text = CommonUtil.GetOmitString(value, SystemConst.LEN_JOBID_JOBNET);
                IconViewData data = (IconViewData)this.DataContext;
                data.JobId = value;
            }
        }

        /// <summary>処理フラグ</summary>
        private RunJobMethodType _methodType;
        public RunJobMethodType MethodType
        {
            get
            {
                return _methodType;
            }
            set
            {
                _methodType = value;

            }
        }

        /// <summary>内部ジョブId</summary>
        public string InnerJobId { get; set; }

        /// <summary>ジョブ名</summary>
        private string _jobName;
        public string JobName
        {
            get
            {
                return _jobName;
            }
            set
            {
                _jobName = value;

                // 表示文字をセット
                tbJobName.Text = CommonUtil.GetOmitString(value, SystemConst.LEN_JOBNAME_JOBNET);
                IconViewData data = (IconViewData)this.DataContext;
                data.JobName = value;

            }
        }

        /// <summary>ZIndex</summary>
        public int ZIndex
        {
            get
            {
                return (int)this.GetValue(Canvas.ZIndexProperty);

            }
            set
            {
                this.SetValue(Canvas.ZIndexProperty, value);
            }

        }

        /// <summary>幅</summary>
        public double PicWidth
        {
            get
            {
                return tbJobId.Width;
            }
        }

        /// <summary>高さ</summary>
        public double PicHeight
        {
            get
            {
                return tbJobId.Height + tbJobName.Height + 4;
            }
        }

        /// <summary>アイコンの状態</summary>
        private IElementState _state = IElementState.Focus;
        public IElementState State
        {
            get { return _state; }
        }

        /// <summary>連接点：Top</summary>
        public Point TopConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotTop.GetValue(Canvas.LeftProperty) + HotspotTop.Width / 2;
                p.Y = (double)HotspotTop.GetValue(Canvas.TopProperty) + HotspotTop.Height / 2;

                return p;
            }
        }

        /// <summary>連接点：Bottom</summary>
        public Point BottomConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotBottom.GetValue(Canvas.LeftProperty) + HotspotBottom.Width / 2;
                p.Y = (double)HotspotBottom.GetValue(Canvas.TopProperty) + HotspotBottom.Height / 2;
                return p;
            }
        }

        /// <summary>連接点：Left</summary>
        public Point LeftConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotLeft.GetValue(Canvas.LeftProperty) + HotspotLeft.Width / 2;
                p.Y = (double)HotspotLeft.GetValue(Canvas.TopProperty) + HotspotLeft.Height / 2;
                return p;
            }
        }

        /// <summary>連接点：Right</summary>
        public Point RightConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotRight.GetValue(Canvas.LeftProperty) + HotspotRight.Width / 2;
                p.Y = (double)HotspotRight.GetValue(Canvas.TopProperty) + HotspotRight.Height / 2;
                return p;
            }
        }

        /// <summary>画面項目：Top点</summary>
        public Rectangle TopSpot
        {
            get
            {
                return HotspotTop;
            }
        }

        /// <summary>画面項目：Bottom点</summary>
        public Rectangle BottomSpot
        {
            get
            {
                return BottomSpot;
            }
        }

        /// <summary>画面項目：Left点</summary>
        public Rectangle LeftSpot
        {
            get
            {
                return HotspotLeft;
            }
        }

        /// <summary>画面項目：Right点</summary>
        public Rectangle RightSpot
        {
            get
            {
                return HotspotRight;
            }
        }

        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>JobId変更時イベント</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void textBlockJobId_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            bool IsTextTrimmed = false;
            var textBlock = sender as TextBlock;
            if (textBlock != null && textBlock.Tag != null && CheckUtil.IsLenOver(textBlock.Tag.ToString(), GetJobIdTrimLimitLength()))
            {
                IsTextTrimmed = true;
            }
            TextBlockService.SetIsTextTrimmed(textBlock, IsTextTrimmed);
        }

        //*******************************************************************
        /// <summary>JobName変更時イベント</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void textBlockJobName_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            bool IsTextTrimmed = false;
            var textBlock = sender as TextBlock;
            if (textBlock != null && textBlock.Tag != null
                    && CheckUtil.IsLenOver(textBlock.Tag.ToString(), GetJobNameTrimLimitLength()))
            {
                IsTextTrimmed = true;
            }
            TextBlockService.SetIsTextTrimmed(textBlock, IsTextTrimmed);
        }

        #endregion

        #region privateメッソド

        /// <summary>連接点の色をセット</summary>
        private void SetHotspotStyle(Color color, double opacity)
        {
            HotspotLeft.Fill = new SolidColorBrush(color);
            HotspotLeft.Opacity = opacity;
            HotspotTop.Fill = new SolidColorBrush(color);
            HotspotTop.Opacity = opacity;
            HotspotRight.Fill = new SolidColorBrush(color);
            HotspotRight.Opacity = opacity;
            HotspotBottom.Fill = new SolidColorBrush(color);
            HotspotBottom.Opacity = opacity;
        }

        /// <summary>JobID表示文字数</summary>//
        private int GetJobIdTrimLimitLength()
        {
            return SystemConst.LEN_JOBID_JOBNET - 3;
        }

        /// <summary>Job名表示文字数</summary>/// 
        private int GetJobNameTrimLimitLength()
        {
            return SystemConst.LEN_JOBNAME_JOBNET - 3;
        }

        #endregion

        #region public メッソド

        /// <summary>自分を削除</summary>
        public void Delete()
        {
            _container.RemoveItem(this);
            this.Delete();
        }

        /// <summary>フォーカスする</summary>
        public void SetFocus()
        {
            if (this._state != IElementState.Focus)
            {
                SetHotspotStyle(Colors.Blue, 1.0);
                //HotspotLeft.Visibility = Visibility.Visible;
                //HotspotTop.Visibility = Visibility.Visible;
                //HotspotRight.Visibility = Visibility.Visible;
                //HotspotBottom.Visibility = Visibility.Visible;

                this._state = IElementState.Focus;
            }
        }

        /// <summary>フォーカスを釈放</summary>
        public void SetUnFocus()
        {
            if (this._state != IElementState.UnFocus)
            {
                //HotspotLeft.Visibility = Visibility.Collapsed;
                //HotspotTop.Visibility = Visibility.Collapsed;
                //HotspotRight.Visibility = Visibility.Collapsed;
                //HotspotBottom.Visibility = Visibility.Collapsed;

                this._state = IElementState.UnFocus;
            }
        }

        /// <summary>選択の色をセット</summary>
        public void SetSelectedColor()
        {
            picJobnet.Fill = SystemConst.ColorConst.SelectedColor;
        }

        /// <summary>色のリセット</summary>
        public void ResetInitColor()
        {
            Brush color;
            switch (MethodType)
            {
                case RunJobMethodType.HOLD:
                    color = SystemConst.ColorConst.HoldColor;
                    break;
                case RunJobMethodType.SKIP:
                    color = SystemConst.ColorConst.SkipColor;

                    //added by YAMA 2014/07/01
                    // スキップ設定時は白文字を設定
                    tbJobId.Foreground = SystemConst.ColorConst.WhiteColor;
                    tbJobName.Foreground = SystemConst.ColorConst.WhiteColor;

                    break;
                default:
                    color = SystemConst.ColorConst.JobColor;

                    //added by YAMA 2014/07/01
                    // スキップ設定以外は黒文字を設定
                    tbJobId.Foreground = SystemConst.ColorConst.BlackColor;
                    tbJobName.Foreground = SystemConst.ColorConst.BlackColor;

                    break;
            }
            picJobnet.Fill = color;
        }

        /// <summary>部品欄のアイコン色をセット</summary>
        public void InitSampleColor()
        {
            picJobnet.Fill = SystemConst.ColorConst.SampleColor;
        }

        //added by YAMA 2014/07/01
        /// <summary>部品欄の文字色をセット</summary>
        public void SetStatusCharacterColor(SolidColorBrush color)
        {
            tbJobId.Foreground = color;
            tbJobName.Foreground = color;
        }

        /// <summary>実行状態によりアイコン色をセット</summary>
        public void SetStatusColor(SolidColorBrush color)
        {
            picJobnet.Fill = color;
        }

        /// <summary>部品欄のアイコン選択状態をセット</summary>
        public void SetSelected()
        {
            if (this._state != IElementState.Selected)
            {
                SetHotspotStyle(Colors.Red, 0.5);
                //HotspotLeft.Visibility = Visibility.Visible;
                //HotspotTop.Visibility = Visibility.Visible;
                //HotspotRight.Visibility = Visibility.Visible;
                //HotspotBottom.Visibility = Visibility.Visible;

                this._state = IElementState.Selected;
            }
        }

        /// <summary>ToolTip表示内容設定</summary>/// 
        public void SetToolTip(){
            StringBuilder sb = new StringBuilder();
            sb.Append(Properties.Resources.job_id_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(_jobId);
            sb.Append("\n");
            sb.Append(Properties.Resources.job_name_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(_jobName);
            picToolTip.ToolTip = sb.ToString();
        }

        /// <summary>ToolTip表示内容設定</summary>/// 
        public void SetToolTip(string jobId, string jobName){
            StringBuilder sb = new StringBuilder();
            sb.Append(Properties.Resources.job_id_label_text);
            sb.Append(jobId);
            sb.Append("\n");
            sb.Append(Properties.Resources.job_name_label_text);
            sb.Append(jobName);
            picToolTip.ToolTip = sb.ToString();
        }

        /// <summary>ToolTip表示内容リセット</summary>///
        public void ResetToolTip(string toolTip)
        {
            picToolTip.ToolTip = toolTip;
        }

        #endregion

    }
}
