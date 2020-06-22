using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RyChart
{
    /// <summary>
    /// X.xaml 的交互逻辑
    /// </summary>
    public partial class X : UserControl
    {
        public X()
        {
            InitializeComponent();
        }

        //在以时间为X轴的项目中无用,故注释
        //#region XMaxValue-X轴最大值
        ///// <summary>
        ///// X轴最大值
        ///// </summary>
        //[Browsable(false)]
        //public double XMaxValue
        //{
        //    get { return (double)GetValue(XMaxValueProperty); }
        //    set { SetValue(XMaxValueProperty, value); }
        //}

        //public static readonly DependencyProperty XMaxValueProperty = DependencyProperty.Register("XMaxValue",
        //    typeof(double), typeof(X), new PropertyMetadata(Double.PositiveInfinity, (sender, args) =>
        //    {
        //        try
        //        {
        //            //int newValue = int.Parse(args.NewValue.ToString());
        //        }
        //        catch (Exception) { }
        //    }));

        //#endregion

        //#region XMinValue-X轴最小值
        ///// <summary>
        ///// X轴最小值
        ///// </summary>
        //[Browsable(false)]
        //public double XMinValue
        //{
        //    get { return (double)GetValue(XMinValueProperty); }
        //    set { SetValue(XMinValueProperty, value); }
        //}

        //public static readonly DependencyProperty XMinValueProperty = DependencyProperty.Register("XMinValue",
        //    typeof(double), typeof(X), new PropertyMetadata(Double.NegativeInfinity, (sender, args) =>
        //    {
        //        try
        //        {
        //            //int newValue = int.Parse(args.NewValue.ToString());
        //        }
        //        catch (Exception) { }
        //    }));
        //#endregion

        #region XBigChapterCount-X轴大刻度数
        /// <summary>
        /// X轴大刻度数
        /// </summary>
        [Browsable(false)]
        public int XBigChapterCount
        {
            get { return (int)GetValue(XBigChapterCountProperty); }
            set { SetValue(XBigChapterCountProperty, value); }
        }

        public static readonly DependencyProperty XBigChapterCountProperty = DependencyProperty.Register("XBigChapterCount",
            typeof(int), typeof(X), new PropertyMetadata(-1, (sender, args) =>
            {
                try
                {
                    //int newValue = int.Parse(args.NewValue.ToString());
                }
                catch (Exception) { }
            }));
        #endregion

        #region XSmallChapterCount-X轴小刻度数
        /// <summary>
        /// X轴小刻度数
        /// </summary>
        [Browsable(false)]
        public int XSmallChapterCount
        {
            get { return (int)GetValue(XSmallChapterCountProperty); }
            set { SetValue(XSmallChapterCountProperty, value); }
        }

        public static readonly DependencyProperty XSmallChapterCountProperty = DependencyProperty.Register("XSmallChapterCount",
            typeof(int), typeof(X), new PropertyMetadata(-1, (sender, args) =>
            {
                try
                {
                    //int newValue = int.Parse(args.NewValue.ToString());
                }
                catch (Exception) { }
            }));
        #endregion

    }
}
