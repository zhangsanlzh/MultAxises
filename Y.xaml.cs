using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace RyChart
{
    /// <summary>
    /// Y.xaml 的交互逻辑
    /// </summary>
    public partial class Y : UserControl
    {
        public Y()
        {
            InitializeComponent();
        }

        #region YColor-Y轴颜色
        /// <summary>
        /// Y轴颜色
        /// </summary>
        [Browsable(false)]
        internal Brush YColor
        {
            get { return (Brush)GetValue(BgColorProperty); }
            set { SetValue(BgColorProperty, value); }
        }

        internal static readonly DependencyProperty BgColorProperty = DependencyProperty.Register("YColor",
            typeof(Brush), typeof(Y), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#EEEEF2"), (sender, args) =>
            {
                try
                {
                    BrushConverter brushConverter = new BrushConverter();
                    (sender as Y).rect.Fill = (Brush)args.NewValue;
                }
                catch (Exception) { }
            }));

        #endregion

        #region YMaxValue-Y轴最大值
        /// <summary>
        /// Y轴最大值
        /// </summary>
        internal double YMaxValue
        {
            get { return (double)GetValue(YMaxValueProperty); }
            set { SetValue(YMaxValueProperty, value); }
        }

        internal static readonly DependencyProperty YMaxValueProperty = DependencyProperty.Register("YMaxValue",
            typeof(double), typeof(Y), new PropertyMetadata(Double.PositiveInfinity, (sender, args) =>
            {
                try
                {
                    //int newValue = int.Parse(args.NewValue.ToString());
                }
                catch (Exception) { }
            }));

        #endregion

        #region YMinValue-Y轴最小值
        /// <summary>
        /// Y轴最小值
        /// </summary>
        internal double YMinValue
        {
            get { return (double)GetValue(YMinValueProperty); }
            set { SetValue(YMinValueProperty, value); }
        }

        internal static readonly DependencyProperty YMinValueProperty = DependencyProperty.Register("YMinValue",
            typeof(double), typeof(Y), new PropertyMetadata(Double.NegativeInfinity, (sender, args) =>
            {
                try
                {
                    //int newValue = int.Parse(args.NewValue.ToString());
                }
                catch (Exception) { }
            }));
        #endregion

        #region BigYChapterCount-Y轴大刻度数
        /// <summary>
        /// Y轴大刻度数
        /// </summary>
        internal int BigYChapterCount
        {
            get { return (int)GetValue(BigYChapterCountProperty); }
            set { SetValue(BigYChapterCountProperty, value); }
        }

        internal static readonly DependencyProperty BigYChapterCountProperty = DependencyProperty.Register("BigYChapterCount",
            typeof(int), typeof(Y), new PropertyMetadata(-1, (sender, args) =>
            {
                try
                {
                    //int newValue = int.Parse(args.NewValue.ToString());
                }
                catch (Exception) { }
            }));
        #endregion

        #region SmallYChapterCount-Y轴小刻度数
        /// <summary>
        /// Y轴小刻度数
        /// </summary>
        internal int SmallYChapterCount
        {
            get { return (int)GetValue(SmallYChapterCountProperty); }
            set { SetValue(SmallYChapterCountProperty, value); }
        }

        internal static readonly DependencyProperty SmallYChapterCountProperty = DependencyProperty.Register("SmallYChapterCount",
            typeof(int), typeof(Y), new PropertyMetadata(-1, (sender, args) =>
            {
                try
                {
                    //int newValue = int.Parse(args.NewValue.ToString());
                }
                catch (Exception) { }
            }));
        #endregion

    }

    /// <summary>
    /// 刻度类型
    /// </summary>
    public enum ChapterType
    {
        /// <summary>
        /// 大刻度
        /// </summary>
        Big=1,
        /// <summary>
        /// 小刻度
        /// </summary>
        Small
    }

}
