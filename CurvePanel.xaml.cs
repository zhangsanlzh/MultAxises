using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RyChart
{
    /// <summary>
    /// CurvePanel.xaml 的交互逻辑
    /// </summary>
    public partial class CurvePanel : UserControl
    {
        public CurvePanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重置此控件
        /// </summary>
        public void CurvePnl_Reset()
        {
            //重置各个Y轴
            foreach (var item in YListPnl.Children)
            {
                (item as Y).ResetY();
                (item as Y).Visibility = Visibility.Collapsed;
            }

            (xGrid.Children[0] as X).ResetX();//重置X轴
            ClearCanvas();
        }

        /// <summary>
        /// 将若干个点连接成线，此方法用于控件初绘时绘制图像。若要在拖动时绘线，请调用<see cref="DragRefresh"/>
        /// </summary>
        /// <param name="dic">记录在库的点集</param>
        /// <param name="iCanvas">某个画布</param>
        /// <param name="color">曲线与 Y轴的颜色</param>
        /// <param name="yMin">Y轴表示的最小值</param>
        /// <param name="yMax">Y轴表示的最大值</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void DrawPointsData(Dictionary<string,double> dic,int iCanvas,SolidColorBrush color,double yMin, double yMax,DateTime startTime,DateTime endTime)
        {
            Y_InitializeComponent(10, 4, 8);
            X_InitializeComponent(10, 4);
            SetYVisible(iCanvas);
            RefreshGrid(dic.Count);
            SetYColor(iCanvas, color);
            SetYMaxValue(iCanvas, yMax);
            SetYMinValue(iCanvas, yMin);
            SetYChapterText(iCanvas);
            SetXChapterText(startTime,endTime);
            LBDrawPointsData(ConvertToPointCollection(dic, startTime, endTime), iCanvas);
        }

        /// <summary>
        /// 按间隔取好的点构成的字典
        /// </summary>
        public Dictionary<int, double> dicByInterval = new Dictionary<int, double>();

        /// <summary>
        /// 相邻两点的间隔
        /// </summary>
        private double intervalX = 0;

        /// <summary>
        /// 根据字典的 Key 值计算出此点的 X 坐标
        /// </summary>
        public double GetX(int key)
        {
            return key * intervalX;
        }

        /// <summary>
        /// 根据字典的 Value 值和对应的 Y 轴编号计算出相应的 Y 坐标
        /// </summary>
        public double GetY(double value, int yIndex)
        {
            //待返回的 y 值
            double y = 0;

            if (yIndex >= 0 && yIndex <= YListPnl.Children.Count - 1)
            {
                double yHeight = ActualHeight;//Y轴高
                double yMaxValue = (YListPnl.Children[yIndex] as Y).YMaxValue;//Y轴最大值
                double yMinValue = (YListPnl.Children[yIndex] as Y).YMinValue;//Y轴最小值

                if (value == 0)
                    y = 0;
                else
                    y = (value - yMinValue) * yHeight / (yMaxValue - yMinValue);
            }

            return y;
        }

        /// <summary>
        /// 根据传入的点集返回构造好的字典
        /// </summary>
        public Dictionary<int, double> GetPointDictionary(Dictionary<string, double> dic, DateTime startTime, DateTime endTime)
        {
            oxy = OXY.LeftBottom;
            ConvertToPointCollection(dic, startTime, endTime);

            return dicByInterval;
        }

        /// <summary>
        /// 把字典解析成（x,y）构成的点集。该点集可直接通过方法 <see cref="DrawLine"/> 显示到某层画布上
        /// </summary>
        public PointCollection Parse(Dictionary<int, double> dic, int iCanvas)
        {
            PointCollection pc = new PointCollection();

            foreach (var item in dic)
            {
                double x = GetX(item.Key);
                double y = GetY(item.Value, iCanvas);
                pc.Add(new Point(x, y));
            }

            return pc;
        }

        /// <summary>
        /// 计算某层画布上的线对应的坐标点集，但不绘出
        /// </summary>
        public PointCollection GetPointCollection(Dictionary<string, double> dic, int iCanvas, DateTime startTime, DateTime endTime)
        {
            oxy = OXY.LeftBottom;

            return VPTGP(ConvertToPointCollection(dic, startTime, endTime), iCanvas);
        }

        /// <summary>
        /// 初始化整个控件
        /// </summary>
        /// <param name="iCanvas">某层画布</param>
        /// <param name="color">Y轴和线条的颜色</param>
        /// <param name="yMin">Y轴最小值</param>
        /// <param name="yMax">Y轴最大值</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        public void CurvePnl_Init(int iCanvas, SolidColorBrush color, double yMin, double yMax, DateTime startTime, DateTime endTime)
        {
            Y_InitializeComponent(10, 4, 8);
            X_InitializeComponent(10, 4);
            SetYVisible(iCanvas);
            RefreshGrid(20);
            SetYColor(iCanvas, color);
            SetYMaxValue(iCanvas, yMax);
            SetYMinValue(iCanvas, yMin);
            SetYChapterText(iCanvas);
            SetXChapterText(startTime, endTime);
        }

        /// <summary>
        /// 将线的点集绘制在某层画布上
        /// </summary>
        public void DrawLine(PointCollection pc, int iCanvas)
        {
            if (curveLineExit(iCanvas))
            {
                path = DragDrawPointsData(pc, iCanvas);
                RefreshPathData(path, iCanvas);
            }
            else
            {
                DrawPointsData(pc, iCanvas);
            }

        }

        /// <summary>
        /// 指示某个画布上是否有曲线
        /// </summary>
        private bool curveLineExit(int iCanvas)
        {
            Canvas c = GetCanvas(iCanvas);
            if (c is Canvas)
            {
                for (int i = 0; i < c.Children.Count; i++)
                {
                    if ((c.Children[i] as System.Windows.Shapes.Path).Name == "curveLine")
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 此方法用于拖动时得到新的图像
        /// </summary>
        public void DragRefresh(Dictionary<string, double> dic, int iCanvas, DateTime startTime, DateTime endTime)
        {
            if (iCanvas == 0)//只执行一次,以提高效率
            {
                RefreshGrid(dic.Count);
                SetXChapterText(startTime, endTime);
            }

            path = LBDragDrawPointsData(ConvertToPointCollection(dic, startTime, endTime), iCanvas);
            RefreshPathData(path, iCanvas);
        }

        /// <summary>
        /// 获取取点间隔
        /// </summary>
        public int GetDataInterval(DateTime startTime, DateTime endTime)
        {
            int interval = (int)Math.Round((endTime - startTime).TotalSeconds / (Width - 15));

            int[] intervalArr = new int[12] { 1, 2, 3, 4, 5, 6, 10, 12, 15, 20, 30, 60 };//合法的 60*(1/k) 间隔, k∈{60,30,20,15,12,10,6,5,4,3,2,1}

            if (interval==0)
            {
                interval = 1;
            }
            else if (interval<60 && 60%interval !=0)
            {
                int index = DivSearch(interval, 0, 11, intervalArr);
                int a = interval - intervalArr[index];
                int b = interval - intervalArr[index + 1];

                interval = a < b ? intervalArr[index] : intervalArr[index + 1];
            }
            else if(interval>60)
            {
                int n = interval / 60;
                interval = 60 * n;
            }

            //间隔2时拖动延迟很大，若为2采用3
            if (interval==2)
            {
                interval = 3;
            }

            return interval;
        }

    }
}