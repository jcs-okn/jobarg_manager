/*
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC �� �� 2012/10/20 �V�K�쐬<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// �����N���X�i��󂠂�j
    /// </summary>
	public sealed class Arrow : Shape
	{
		#region �ˑ��v���p�e�B��`

        /// <summary>�J�n�_��X���W </summary>
		public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1",typeof(double), 
            typeof(Arrow),new FrameworkPropertyMetadata(0.0,FrameworkPropertyMetadataOptions.AffectsRender 
                | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>�J�n�_��Y���W </summary>
		public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), 
            typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>�I�_��X���W </summary>
		public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), 
            typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                | FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        /// <summary>�I�_��Y���W </summary>
		public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), 
            typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>���̕� </summary>
		public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", 
            typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>���̒��� </summary>
		public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", 
            typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                | FrameworkPropertyMetadataOptions.AffectsMeasure)); 

		#endregion

		#region CLR�v���p�e�B

        /// <summary>�N�_�̂w���W</summary>
		[TypeConverter(typeof(LengthConverter))]
		public double X1
		{
			get { return (double)base.GetValue(X1Property); }
			set { base.SetValue(X1Property, value); }
		}

        /// <summary>�N�_��Y���W</summary>
		[TypeConverter(typeof(LengthConverter))]
		public double Y1
		{
			get { return (double)base.GetValue(Y1Property); }
			set { base.SetValue(Y1Property, value); }
		}

        /// <summary>�I�_��X���W</summary>
		[TypeConverter(typeof(LengthConverter))]
		public double X2
		{
			get { return (double)base.GetValue(X2Property); }
			set { base.SetValue(X2Property, value); }
		}

        /// <summary>�I�_��Y���W</summary>
		[TypeConverter(typeof(LengthConverter))]
		public double Y2
		{
			get { return (double)base.GetValue(Y2Property); }
			set { base.SetValue(Y2Property, value); }
		}

        /// <summary>���̕�</summary>
		[TypeConverter(typeof(LengthConverter))]
		public double HeadWidth
		{
			get { return (double)base.GetValue(HeadWidthProperty); }
			set { base.SetValue(HeadWidthProperty, value); }
		}

        /// <summary>���̍���</summary>
		[TypeConverter(typeof(LengthConverter))]
		public double HeadHeight
		{
			get { return (double)base.GetValue(HeadHeightProperty); }
			set { base.SetValue(HeadHeightProperty, value); }
		}

		#endregion

        #region Overrides
        /// <summary>�􉽒�`</summary>
		protected override Geometry DefiningGeometry
		{
			get
			{
				// Create a StreamGeometry for describing the shape
				StreamGeometry geometry = new StreamGeometry();
				geometry.FillRule = FillRule.EvenOdd;

				using (StreamGeometryContext context = geometry.Open())
				{
					InternalDrawArrowGeometry(context);
				}

				// Freeze the geometry for performance benefits
				geometry.Freeze();

				return geometry;
			}
		}		

		#endregion

		#region Privates ���b�\�h

        /// <summary>�����̕`��</summary>
		private void InternalDrawArrowGeometry(StreamGeometryContext context)
		{
			double theta = Math.Atan2(Y1 - Y2, X1 - X2);
			double sint = Math.Sin(theta);
			double cost = Math.Cos(theta);

			Point pt1 = new Point(X1, this.Y1);
			Point pt2 = new Point(X2, this.Y2);

			Point pt3 = new Point(
				X2 + (HeadWidth * cost - HeadHeight * sint),
				Y2 + (HeadWidth * sint + HeadHeight * cost));

			Point pt4 = new Point(
				X2 + (HeadWidth * cost + HeadHeight * sint),
				Y2 - (HeadHeight * cost - HeadWidth * sint));

            context.BeginFigure(pt1, true, false);

            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);
		}
		
		#endregion
    }
}