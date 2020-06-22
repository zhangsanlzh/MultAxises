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
    public partial class X
    {
        /// <summary>
        /// 重置X轴
        /// </summary>
        internal void ResetX()
        {
            tbContainer.Children.Clear();
            canvasChapters.Children.Clear();
        }

        /// <summary>
        /// 给 X轴添加若干大刻度
        /// </summary>
        /// <param name="cCount">数量</param>
        public void AddXBigChapters(int cCount)
        {
            XBigChapterCount = cCount;

            GeometryGroup group = new GeometryGroup();
            Path path = new Path();

            for (int j = 0; j <= cCount; j++)
            {
                double interval = (Width - 15) / cCount;//减15是因为绘图区域并非整个控件的宽
                double loc1 = interval * j;

                if (j==cCount)
                {
                    loc1 = interval * j-1;//让最后一个刻度向左偏移1，这样更加明显
                }

                LineGeometry line = new LineGeometry();
                line.StartPoint = new Point(loc1, 0);
                line.EndPoint = new Point(loc1, 10);

                group.Children.Add(line);
            }

            path.Stroke = new SolidColorBrush(Colors.Black);
            path.StrokeThickness = 0.8;
            path.Data = group;
            canvasChapters.Children.Add(path);
        }

        /// <summary>
        /// 给 X轴添加若干小刻度
        /// </summary>
        /// <param name="cCount">刻度总数</param>
        public void AddXSmallChapters(int cCount)
        {
            XSmallChapterCount = cCount;

            //若未设置大刻度
            if (XBigChapterCount == -1)
            {
                return;
            }

            GeometryGroup group = new GeometryGroup();
            Path path = new Path();

            double bInterval = (Width-15) / XBigChapterCount;
            double sInterval = bInterval / cCount;//小刻度的间隔

            for (int j =0; j < XBigChapterCount; j++)
            {
                double sloc = bInterval * j;

                for (int i = 0; i < XSmallChapterCount; i++)
                {
                    double sloc1 = sloc + i * sInterval;

                    LineGeometry line = new LineGeometry();
                    line.StartPoint = new Point(sloc1, 0);
                    line.EndPoint = new Point(sloc1, 5);

                    group.Children.Add(line);
                }
            }

            path.Stroke = new SolidColorBrush(Colors.Black);
            path.StrokeThickness = 0.8;
            path.Data = group;

            canvasChapters.Children.Add(path);
        }

        /// <summary>
        /// 从当前控件开始，查找指定类型名的控件
        /// </summary>
        /// <param name="obj">当前控件</param>
        /// <param name="systemTypeName">指定控件的类型名</param>
        /// <returns></returns>
        private object FindControl(DependencyObject obj, string systemTypeName)
        {
            if (obj.DependencyObjectType.SystemType.Name == systemTypeName)
            {
                return obj;
            }

            return FindControl(VisualTreeHelper.GetParent(obj), systemTypeName);
        }

        /// <summary>
        /// 设置 X轴大刻度旁的文字,
        /// 此种方法用于第一个数据点和最后一个数据点恰好是起始时间和结束时间的情况,其间数据可空
        /// 称此情况为全有效点情况
        /// </summary>
        /// <param name="dic">一系列时间字符组成的字典</param>
        internal void SetXChapterText(Dictionary<int,string> dic)
        {
            if (XBigChapterCount==0)
            {
                return;
            }

            tbContainer.Children.Clear();//清空文本

            double interval = (ActualWidth-15) / XBigChapterCount;//减15是因为绘图区域并非整个控件的宽
            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = new TextBlock();
                tb.FontSize = 8;
                tb.FontWeight = FontWeights.DemiBold;
                tb.Foreground = new SolidColorBrush(Colors.Black);
                tb.Text = dic[dic.Count / 10 * i];
                double loc = interval * i;
                tb.Margin = new Thickness(loc, 0, 0, 0);
                tbContainer.Children.Add(tb);
            }
        }

        /// <summary>
        /// 设置 X轴大刻度旁的文字,
        /// 此种方法用于第一个数据点和最后一个数据点不是起始时间和结束时间的情况,首尾点两端数据为空,其间数据可空
        /// 称此情况为非全有效点情况
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        internal void SetXChapterText(DateTime startTime,DateTime endTime)
        {
            if (XBigChapterCount == 0)
            {
                return;
            }

            double seconds = (endTime - startTime).TotalSeconds;
            double secInterval = seconds / 10;

            tbContainer.Children.Clear();//清空文本

            double interval = (ActualWidth-15) / XBigChapterCount;//减15是因为绘图区域并非整个控件的宽
            for (int i = 0; i <= 10; i++)
            {
                TextBlock tb = new TextBlock();
                tb.FontSize = 8;
                tb.FontWeight = FontWeights.DemiBold;
                tb.Foreground = new SolidColorBrush(Colors.Black);
                tb.Text = startTime.AddSeconds(i * secInterval).ToString("yyyy.M.dd\nHH:mm:ss");

                double loc = interval * i;
                if (i > 0)
                {
                    loc = interval * i - 22;//减44是在textBlock FontSize为8的基础上为将文本与大刻度线居中对齐所取的估计值
                }
                if (i==10)
                {
                    loc = interval * i-44;//减44是在textBlock FontSize为8的基础上为将文本与大刻度线右对齐所取的估计值
                }

                tb.Margin = new Thickness(loc, 0, 0, 0);

                tbContainer.Children.Add(tb);
            }
        }

    }
}
