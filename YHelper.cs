using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RyChart
{
    public partial class Y
    {
        /// <summary>
        /// 重置Y轴
        /// </summary>
        internal void ResetY()
        {
            tbContainer.Children.Clear();
            gridChapters.Children.Clear();
        }

        /// <summary>
        /// 给 Y轴添加若干大刻度
        /// </summary>
        /// <param name="cCount">刻度总数</param>
        /// <param name="yType">Y轴类型</param>
        internal void AddBigYChapters(int cCount)
        {
            BigYChapterCount = cCount;

            GeometryGroup group = new GeometryGroup();
            Path path = new Path();

            double ctrlHeight = (FindParentCtrl(this, "CurvePanel") as CurvePanel).Height;
            for (int i = 0; i < cCount; i++)
            {
                double loc = ctrlHeight - (ctrlHeight / cCount) * i;

                LineGeometry line = new LineGeometry();
                line.StartPoint = new Point(0, loc);
                line.EndPoint = new Point(10, loc);

                group.Children.Add(line);
            }
            Binding bd = new Binding();
            bd.Path = new PropertyPath("Fill");
            bd.ElementName = "rect";

            path.SetBinding(Path.StrokeProperty, bd);
            path.StrokeThickness = 1;
            path.Data = group;
            gridChapters.Children.Add(path);
        }

        /// <summary>
        /// 设置Y轴每两个相邻大刻度间的小间隔数
        /// </summary>
        /// <param name="cCount">间隔总数</param>
        internal void SetYSmallIntervalCount(int cCount)
        {
            SmallYChapterCount = cCount;

            //若未设置大刻度
            if (BigYChapterCount == -1)
            {
                return;
            }

            double ctrlHeight = (FindParentCtrl(this, "CurvePanel") as CurvePanel).Height;

            //相邻大刻度的间隔，也就是小刻度的总长
            double bigInterval = ctrlHeight / BigYChapterCount;
            //相邻小刻度的间隔
            double smallInterval = bigInterval / cCount;

            GeometryGroup group = new GeometryGroup();
            Path path = new Path();

            for (int i = 1; i <= BigYChapterCount; i++)
            {
                double bloc = ctrlHeight - (ctrlHeight / BigYChapterCount) * i;
                for (int j = 1; j < SmallYChapterCount; j++)
                {
                    double sloc = bloc + j * smallInterval;

                    LineGeometry line = new LineGeometry();
                    line.StartPoint = new Point(0, sloc);
                    line.EndPoint = new Point(5, sloc);

                    group.Children.Add(line);
                }
            }

            Binding bd = new Binding();
            bd.Path = new PropertyPath("Fill");
            bd.ElementName = "rect";

            path.SetBinding(Path.StrokeProperty, bd);
            path.StrokeThickness = 0.5;
            path.Data = group;
            gridChapters.Children.Add(path);
        }

        /// <summary>
        /// 获取Y轴的高度
        /// </summary>
        private double GetYActualHeight()
        {
            double ctrlHeight = (FindParentCtrl(this, "CurvePanel") as CurvePanel).Height;
            double xHeight = (FindParentCtrlByName(this, "xGrid") as Grid).ActualHeight;

            ctrlHeight -= xHeight;

            return ctrlHeight;
        }

        /// <summary>
        /// 在刻度旁添加一个文本内容
        /// </summary>
        internal void AddYChapterText(double offset, string text)
        {
            TextBlock tb = new TextBlock();
            tb.FontSize = 9;
            tb.Text = text;
            tb.FontWeight = FontWeights.Black;
            tb.Margin = new Thickness(5, offset, 0, 0);
            tb.TextAlignment = TextAlignment.Right;
            tb.Foreground = YColor;

            //RotateTransform rt = new RotateTransform(-90);
            //tb.LayoutTransform = rt;

            tbContainer.Children.Add(tb);
        }

        /// <summary>
        /// 从当前控件开始，查找指定类型名的控件
        /// </summary>
        /// <param name="obj">当前控件</param>
        /// <param name="systemTypeName">指定控件的类型名</param>
        /// <returns></returns>
        internal object FindParentCtrl(DependencyObject obj, string systemTypeName)
        {
            if (obj.DependencyObjectType.SystemType.Name == systemTypeName)
            {
                return obj;
            }

            return FindParentCtrl(VisualTreeHelper.GetParent(obj), systemTypeName);
        }

        /// <summary>
        /// 从当前控件开始，查找指定名称的控件
        /// </summary>
        /// <param name="obj">当前控件</param>
        /// <param name="ctrlName">控件名</param>
        /// <returns></returns>
        private object FindParentCtrlByName(DependencyObject obj, string ctrlName)
        {
            if (obj.DependencyObjectType.Name == "CurvePanel")
            {
                return GetChildObject<Grid>(obj, ctrlName);
            }

            return FindParentCtrlByName(VisualTreeHelper.GetParent(obj), ctrlName);
        }

        /// <summary>
        /// 查找子控件
        /// </summary>
        /// <typeparam name="T">子控件的类型</typeparam>
        /// <param name="obj">要找的是obj的子控件</param>
        /// <param name="name">想找的子控件的Name属性</param>
        /// <returns>目标子控件</returns>
        private static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    // 在下一级中没有找到指定名字的子控件，就再往下一级找
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }

            return null;
        }

    }
}
