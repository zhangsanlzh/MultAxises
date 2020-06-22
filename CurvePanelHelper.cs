using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RyChart
{
    public partial class CurvePanel
    {
        /// <summary>
        /// 坐标原点的位置
        /// </summary>
        private OXY oxy;
        /// <summary>
        /// 网格的竖线数
        /// </summary>
        private int vlCount;
        /// <summary>
        /// 网格的横线数
        /// </summary>
        private int hlCount;
        /// <summary>
        /// 某个画布上的曲线
        /// </summary>
        private System.Windows.Shapes.Path path = null;

        #region Visibility-控件可见性
        [Description("指示该控件是否可见"), DisplayName("可见性"), Category("CurvePanel")]
        /// <summary>
        /// 控件可见性
        /// </summary>
        public Visibility VisibilityCtrl
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("VisibilityCtrl",
            typeof(Visibility), typeof(CurvePanel), new PropertyMetadata(Visibility.Visible, (sender, args) =>
            {
                try
                {
                    (sender as CurvePanel).Visibility = (Visibility)args.NewValue;
                }
                catch (Exception) { }
            }));

        #endregion

        #region YCount-Y轴数量
        [Description("设置Y轴数量"), DisplayName("Y轴数量"), Browsable(false)]
        /// <summary>
        /// Y轴数量
        /// </summary>
        private int YCount
        {
            get { return (int)GetValue(YCountProperty); }
            set { SetValue(YCountProperty, value); DrawGrid(vlCount, hlCount); }
        }

        private static readonly DependencyProperty YCountProperty = DependencyProperty.Register("YCount",
            typeof(int), typeof(CurvePanel), new PropertyMetadata(0, (sender, args) =>
            {
                try
                {
                    int newValue = int.Parse(args.NewValue.ToString());

                    AddY(sender, newValue);
                    AddCanvas(sender, newValue);
                }
                catch (Exception) { }
            }));

        #endregion

        #region BgColor-背景颜色
        [Description("设置控件的背景色"), DisplayName("背景色"), Category("CurvePanel")]
        /// <summary>
        /// 背景颜色
        /// </summary>
        public Brush BgColor
        {
            get { return (Brush)GetValue(BgColorProperty); }
            set { SetValue(BgColorProperty, value); }
        }

        public static readonly DependencyProperty BgColorProperty = DependencyProperty.Register("BgColor",
            typeof(Brush), typeof(CurvePanel), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#FFF7F7F7"), (sender, args) =>
            {
                try
                {
                    BrushConverter brushConverter = new BrushConverter();
                    (sender as CurvePanel).border.Background = (Brush)args.NewValue;
                }
                catch (Exception) { }
            }));

        #endregion

        #region BigYChapterCount-Y轴大刻度数
        /// <summary>
        /// Y轴大刻度数
        /// </summary>
        [Description("Y轴大刻度数"), DisplayName("Y轴大刻度数"), Browsable(false)]
        public int YBigChapterCount
        {
            get { return (int)GetValue(YBigChapterCountCountProperty); }
            set { SetValue(YBigChapterCountCountProperty, value); }
        }

        public static readonly DependencyProperty YBigChapterCountCountProperty = DependencyProperty.Register("YBigChapterCount",
            typeof(int), typeof(CurvePanel), new PropertyMetadata(0));
        #endregion

        #region YSmallChapterCount-Y轴小刻度数
        /// <summary>
        /// Y轴小刻度数
        /// </summary> 
        [Description("Y轴小刻度数"), DisplayName("Y轴小刻度数"), Browsable(false)]
        public int YSmallChapterCount
        {
            get { return (int)GetValue(YSmallChapterCountProperty); }
            set { SetValue(YSmallChapterCountProperty, value); }
        }

        public static readonly DependencyProperty YSmallChapterCountProperty = DependencyProperty.Register("YSmallChapterCount",
            typeof(int), typeof(CurvePanel), new PropertyMetadata(0));
        #endregion

        /// <summary>
        /// 初始化Y轴
        /// </summary>
        /// <param name="YBigChapterCount">Y轴大刻度数</param>
        /// <param name="YSmallIntervalCount">Y轴相邻两个大刻度间的小间隔数</param>
        /// <param name="YCount">Y轴个数</param>
        private void Y_InitializeComponent(int YBigChapterCount, int YSmallIntervalCount, int yCount = 1)
        {
            YCount = yCount;

            for (int i = 0; i < yCount; i++)
            {
                AddBigYChapters(i, YBigChapterCount);
                SetYSmallIntervalCount(i, YSmallIntervalCount);
            }

            //设置 X轴位置
            xGrid.Width = Width;
        }

        /// <summary>
        /// 初始化X轴
        /// </summary>
        /// <param name="xBigChapterCount">X轴大刻度数</param>
        /// <param name="xSmallIntervalCount">X轴相邻两个大刻度间的小间隔数</param>
        private void X_InitializeComponent(int xBigChapterCount, int xSmallIntervalCount)
        {
            AddXBigChapters(xBigChapterCount);
            SetXSmallIntervalCount(xSmallIntervalCount);
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="vlCount">竖线数</param>
        /// <param name="hlCount">横线数</param>
        private System.Windows.Shapes.Path DrawGrid(int vlCount, int hlCount)
        {
            this.vlCount = vlCount;
            this.hlCount = hlCount;

            Point StartPoint = new Point(0, 0);//起点
            Point EndPoint = new Point(0, 0);//终点

            GeometryGroup group = new GeometryGroup();

            #region 绘制竖线
            StartPoint.Y = 0;
            for (int j = 0; j <= vlCount; j++)
            {
                double interval = (Width - 15) / vlCount; //减15是因为绘图区域并非整个控件的宽
                StartPoint.X = j * interval;

                if (StartPoint.X > Width - 15)//减15是因为绘图区域并非整个控件的宽
                {
                    break;
                }

                EndPoint.X = StartPoint.X;
                EndPoint.Y = Height;//减10是因为绘图区域并非整个控件的高

                LineGeometry line = new LineGeometry();
                line.StartPoint = StartPoint;
                line.EndPoint = EndPoint;
                group.Children.Add(line);
            }
            #endregion

            #region 绘制横线
            StartPoint.X = 0;
            for (int j = 0; j <= hlCount; j++)
            {
                double interval = Height / hlCount;
                StartPoint.Y = j * interval;

                if (StartPoint.Y > Height)
                {
                    break;
                }

                EndPoint.X = Width - 15;//减15是因为绘图区域并非整个控件的宽
                EndPoint.Y = StartPoint.Y;

                LineGeometry line = new LineGeometry();
                line.StartPoint = StartPoint;
                line.EndPoint = EndPoint;
                group.Children.Add(line);
            }
            #endregion

            ////设置线型为虚线
            //DoubleCollection dCollection = new DoubleCollection();
            //dCollection.Add(1);
            //dCollection.Add(3);

            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Stroke = (Brush)new BrushConverter().ConvertFromString("#EEEEF2");
            path.StrokeThickness = 0.4;
            path.Data = group;
            //glPath.StrokeDashArray = dCollection;

            Canvas c = GetCanvas(0);
            if (c is Canvas)
            {
                if (c.Children.Count == 0)
                    c.Children.Add(path);
            }

            return path;
        }

        /// <summary>
        /// 设置某个Y轴可见性
        /// </summary>
        private void SetYVisibility(int yIndex, Visibility visibility)
        {
            if (yIndex < 0 || yIndex > YListPnl.Children.Count - 1)
            {
                return;
            }

            YListPnl.Children[yIndex].Visibility = visibility;
        }

        /// <summary>
        /// 使某个Y轴不可见
        /// </summary>
        private void SetYCollapsed(int yIndex)
        {
            if (yIndex < 0 || yIndex > YListPnl.Children.Count - 1)
            {
                return;
            }

            YListPnl.Children[yIndex].Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 使某个Y轴可见
        /// </summary>
        private void SetYVisible(int yIndex)
        {
            if (yIndex < 0 || yIndex > YListPnl.Children.Count - 1)
            {
                return;
            }

            YListPnl.Children[yIndex].Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 添加画布
        /// </summary>
        /// <param name="canvasCount">画布数量</param>
        private static void AddCanvas(object sender, int canvasCount)
        {
            (sender as CurvePanel).CanvasListPnl.Children.Clear();

            for (int i = 0; i < canvasCount; i++)
            {
                Canvas c = new Canvas();
                Canvas.SetZIndex(c, i);
                c.Background = new SolidColorBrush(Colors.Transparent);

                (sender as CurvePanel).CanvasListPnl.Children.Add(c);
            }
        }

        /// <summary>
        /// 添加Y轴
        /// </summary>
        /// <param name="ycount">Y轴数量</param>
        private static void AddY(object sender, int ycount)
        {
            (sender as CurvePanel).YListPnl.Children.Clear();

            for (int i = 0; i < ycount; i++)
            {
                Y y = new Y();
                y.YColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                Binding bding = new Binding();
                bding.Path = new PropertyPath(HeightProperty);
                bding.ElementName = "CurvePnl";

                y.SetBinding(HeightProperty, bding);

                (sender as CurvePanel).YListPnl.Children.Add(y);
            }
        }

        /// <summary>
        /// 尺寸改变为新值,这样可保证在设计器中拖动改变并保持其大小
        /// </summary>
        private void CurvePnl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = e.NewSize.Height;
        }



        /// <summary>
        /// 将存有数据的字典转为指定日期范围内的点集
        /// </summary>
        /// <param name="dic">要转换的字典</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        private PointCollection ConvertToPointCollection(Dictionary<string, double> dic, DateTime startTime, DateTime endTime)
        {
            int interval = GetDataInterval(startTime, endTime);

            //合乎需要的点集
            PointCollection pc = new PointCollection();

            try
            {
                string pdtStart = dic.First().Key;//开始点的时间

                if (pdtStart == "")//没有数据直接返回
                    return null;

                string pdtEnd = dic.Last().Key;//结束点的时间

                #region 只构造出有值的部分的数据
                Dictionary<int, double> dicByInterval = new Dictionary<int, double>();//按间隔取好的点
                int totalPointsCount = 0;//按当前间隔取点，最终需要构造的点数

                //构造出只包含有值时间点的字典
                for (DateTime dt = startTime; dt < endTime; dt = dt.AddSeconds(interval))
                {
                    if (dt >= DateTime.Parse(pdtStart) && dt <= DateTime.Parse(pdtEnd))//这样可保证构造出的点尽可能得少
                    {
                        if (startTime.Second % interval != 0)//只取初始曲线所在的序列
                            continue;

                        //查询dic中是否已有K-V对
                        string key = dt.ToString();
                        if (!dic.ContainsKey(key))
                            dicByInterval.Add(totalPointsCount, double.NaN);
                        else
                            dicByInterval.Add(totalPointsCount, dic[key]);
                    }

                    totalPointsCount++;
                }

                //记录字典
                this.dicByInterval = dicByInterval;

                //按当前间隔取点，每两个相邻的点之间的水平距离
                double intervalX = (Width - 15) / totalPointsCount;//减15是因为绘图区域并非整个控件的宽

                //记录相邻两点的间隔
                this.intervalX = intervalX;

                foreach (var item in dicByInterval)
                {
                    pc.Add(new Point(intervalX * item.Key, item.Value));
                }
                #endregion

            }
            catch (Exception) { }

            return pc;
        }

        /// <summary>
        /// 根据点数刷新网格数量
        /// </summary>
        private void RefreshGrid(int count)
        {
            int r = count % 50;//余数

            if (r >= 0 && r <= 10)
            {
                ResetGrid(10, 10);
            }
            else if (r <= 20)
            {
                ResetGrid(20, 20);
            }
            else if (r <= 30)
            {
                ResetGrid(30, 30);
            }
            else if (r <= 40)
            {
                ResetGrid(40, 40);
            }
            else
            {
                ResetGrid(50, 50);
            }
        }

        /// <summary>
        /// 给 X轴刻度下方添加文字,
        /// </summary>
        private void SetXChapterText(DateTime startTime, DateTime endTime)
        {
            if (YCount == 0)//当未调用控件初始化方法时
            {
                return;
            }
            (xGrid.Children[0] as X).SetXChapterText(startTime, endTime);
        }

        /// <summary>
        /// 给 X轴刻度下方添加文字,
        /// </summary>
        private void SetXChapterText(Dictionary<int, string> dic)
        {
            if (YCount == 0)//当未调用控件初始化方法时
            {
                return;
            }
            (xGrid.Children[0] as X).SetXChapterText(dic);
        }

        /// <summary>
        /// 重设网格
        /// </summary>
        /// <param name="vlCount">竖线数</param>
        /// <param name="hlCount">横线数</param>
        private void ResetGrid(int vlCount, int hlCount)
        {
            if (CanvasListPnl.Children.Count == 0)
            {
                return;
            }

            Canvas c0 = GetCanvas(0);//第一个Canvas
            if (c0.Children.Count == 0)
            {
                return;
            }

            //更新网格对应的 Path元素
            System.Windows.Shapes.Path path = DrawGrid(vlCount, hlCount);
            for (int i = 0; i < c0.Children.Count; i++)
            {
                if ((c0.Children[i] as System.Windows.Shapes.Path).Name != "curveLine")
                {
                    (c0.Children[i] as System.Windows.Shapes.Path).Data = path.Data;
                }
            }
        }

        /// <summary>
        /// 根据变量的值计算出图上坐标
        /// </summary>
        /// <param name="pc">传入的点集</param>
        /// <param name="yIndex">某个y轴</param>
        /// <returns></returns>
        private PointCollection VPTGP(PointCollection pc, int yIndex)
        {
            if (yIndex < 0 || yIndex > YListPnl.Children.Count - 1 || pc == null)
            {
                return null;
            }

            double yHeight = ActualHeight;//Y轴高
            double yMaxValue = (YListPnl.Children[yIndex] as Y).YMaxValue;//Y轴最大值
            double yMinValue = (YListPnl.Children[yIndex] as Y).YMinValue;//Y轴最小值

            PointCollection pc0 = new PointCollection();

            int i = 0;
            foreach (var item in pc)
            {
                Point p = new Point();

                switch (oxy)
                {
                    case OXY.LeftBottom:
                        p.X = pc[i].X;
                        break;
                    case OXY.RightBottom:
                        p.X = xGrid.Width - pc[i].X;
                        break;
                    default:
                        break;
                }

                if (item.Y == 0)
                    p.Y = 0;
                else
                    p.Y = ((item.Y - yMinValue) * yHeight / (yMaxValue - yMinValue));

                pc0.Add(p);

                i++;
            }

            return pc0;
        }

        ///// 此方法有问题，暂时注释
        ///// <summary>
        ///// 绘点集，该方法会以左上角为坐标原点将点集绘出
        ///// </summary>
        ///// <param name="pc">传入的点集</param>
        ///// <param name="cIndex">某个Canvas</param>
        //public void LTDrawPointsData(PointCollection pc,int cIndex)
        //{
        //    oxy = OXY.LeftTop;
        //    DrawPointsData(pc, cIndex);
        //}


        /// <summary>
        /// 获取某个Y轴的颜色
        /// </summary>
        private Brush GetYColor(int yIndex)
        {
            if (yIndex < 0 || yIndex > YListPnl.Children.Count - 1)
            {
                return null;
            }

            return (YListPnl.Children[yIndex] as Y).YColor;
        }

        /// <summary>
        /// 获取某个画布
        /// </summary>
        /// <param name="index">层次号，最下层的画布层次号为0</param>
        /// <returns></returns>
        private Canvas GetCanvas(int index)
        {
            if (index < 0 || index > CanvasListPnl.Children.Count - 1)
                return null;

            return CanvasListPnl.Children[index] as Canvas;
        }

        /// <summary>
        /// 以刻度形式显示最后一个点在X轴上的位置,并标明对应信息
        /// </summary>
        /// <param name="lstDateTime">结束时间</param>
        /// <param name="lstPloc">最后一个点在X轴的结束位置</param>
        /// <param name="iCanvas">某个画布</param>
        private void ShowXLastChapter(DateTime? lstDateTime, double lstPloc, int iCanvas)
        {
            GeometryGroup group = new GeometryGroup();

            LineGeometry line = new LineGeometry();
            line.StartPoint = new Point(lstPloc, 0);
            line.EndPoint = new Point(lstPloc, 15);

            group.Children.Add(line);

            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = group;
            path.Name = "XDataEnd" + iCanvas.ToString();
            path.ToolTip = ((DateTime)lstDateTime).ToString("END TI\\ME:\nyyyy.M.dd \nHH:mm:ss");
            path.StrokeThickness = 5;
            path.Stroke = GetYColor(iCanvas);

            (xGrid.Children[0] as X).canvasChapters.Children.Add(path);
        }

        /// <summary>
        /// 清空所有Canvas上的Path元素
        /// </summary>
        private void ClearCanvas()
        {
            for (int i = 0; i < 8; i++)
            {
                Canvas c = GetCanvas(i);
                if (c != null)
                {
                    if (c.Children.Count > 0)
                    {
                        foreach (var item in c.Children)
                            (item as System.Windows.Shapes.Path).Data = null;
                    }
                }
            }
        }

        /// <summary>
        /// 绘点集，该方法会以右下角为坐标原点将点集绘出
        /// </summary>
        /// <param name="pc">传入的点集</param>
        /// <param name="iCanvas">某个Canvas</param>
        private void RBDrawPointsData(PointCollection pc, int iCanvas)
        {
            oxy = OXY.RightBottom;

            //通过计算方式实现坐标转换
            //DrawPointsData(TTBTransPoints(VPTGP(pc, iCanvas), iCanvas), iCanvas);

            DrawPointsData(VPTGP(pc, iCanvas), iCanvas);//方法内使用了翻转效果，减少一次坐标转换过程
        }

        /// <summary>
        /// 绘点集，该方法会以左下角为坐标原点将点集绘出
        /// </summary>
        /// <param name="pc">传入的点集</param>
        /// <param name="iCanvas">某个Canvas</param>
        private void LBDrawPointsData(PointCollection pc, int iCanvas)
        {
            oxy = OXY.LeftBottom;

            //通过计算方式实现坐标转换
            //DrawPointsData(TTBTransPoints(VPTGP(pc, iCanvas), iCanvas), iCanvas);

            DrawPointsData(VPTGP(pc, iCanvas), iCanvas);//方法内使用了翻转效果，减少一次坐标转换过程
        }

        /// <summary>
        /// 更新某个画布上的曲线。此方法仅在拖动时调用
        /// </summary>
        /// <param name="pc">传入的点集</param>
        /// <param name="iCanvas">某层画布</param>
        /// <returns></returns>
        private Path DragDrawPointsData(PointCollection pc, int iCanvas)
        {
            if (pc == null)
            {
                return null;
            }
            if (YListPnl.Children.Count <= 0 || YListPnl.Children.Count - 1 < iCanvas)
            {
                return null;
            }

            GeometryGroup group = new GeometryGroup();

            for (int i = 0; i < pc.Count - 1; i++)
            {
                Point p0 = pc[i];//起点
                Point p1 = pc[i + 1];//终点

                if (!double.IsNaN(p0.Y) && !double.IsNaN(p1.Y))
                {
                    LineGeometry line = new LineGeometry();
                    line.StartPoint = p0;
                    line.EndPoint = p1;
                    group.Children.Add(line);
                }
            }

            Path path = new Path();
            path.Stroke = (YListPnl.Children[iCanvas] as Y).YColor;
            path.StrokeThickness = 1;
            path.Data = group;
            path.Name = "curveLine";

            Canvas c = GetCanvas(iCanvas);
            if (c is Canvas)
            {
                //使用翻转效果，减少一次坐标转换过程
                ScaleTransform st = new ScaleTransform();
                st.ScaleY = -1;
                c.LayoutTransform = st;
            }

            return path;

        }
        /// <summary>
        /// 拖动时将点集绘到指定画布
        /// </summary>
        /// <param name="pc">点集</param>
        /// <param name="iCanvas">画布层次，默认为最下层画布</param>
        private Path LBDragDrawPointsData(PointCollection pc, int cIndex)
        {
            oxy = OXY.LeftBottom;

            //通过计算方式实现坐标转换
            //DrawPointsData(TTBTransPoints(VPTGP(pc, cIndex), cIndex), cIndex);

            return DragDrawPointsData(VPTGP(pc, cIndex), cIndex);//方法内使用了翻转效果，减少一次坐标转换过程
        }

        /// <summary>
        /// 用一条曲线更新某个画布上的曲线
        /// </summary>
        private void RefreshPathData(System.Windows.Shapes.Path path, int iCanvas)
        {
            if (path == null)
                return;

            Canvas c = GetCanvas(iCanvas);
            if (c is Canvas)
            {
                for (int i = 0; i < c.Children.Count; i++)
                {
                    if ((c.Children[i] as System.Windows.Shapes.Path).Name == "curveLine")
                        (c.Children[i] as System.Windows.Shapes.Path).Data = path.Data;
                }
            }
        }

        // TTBTransPoints() 通过计算方式实现坐标转换，现已改用翻转效果，故暂时舍弃
        ///// <summary>
        ///// 转换点坐标,从左上角为坐标原点转为左下角为坐标原点
        ///// </summary>
        ///// <param name="pc0">传入的点集</param>
        ///// <param name="yMaxValue">变量最大值</param>
        ///// <param name="yMinValue">变量最小值</param>
        ///// <param name="yIndex">某个Y轴</param>
        //private PointCollection TTBTransPoints(PointCollection pc0, int yIndex)
        //{
        //    if (pc0==null)
        //    {
        //        return null;
        //    }
        //    if (yIndex < 0 || yIndex > YListPnl.Children.Count - 1)
        //    {
        //        return null;
        //    }

        //    PointCollection pc1 = new PointCollection();
        //    for (int i = 0; i < pc0.Count; i++)
        //    {
        //        Point p = new Point();
        //        p.X = pc0[i].X;

        //        Y y = YListPnl.Children[yIndex] as Y;
        //        //正确的表达式
        //        //p.Y = -pc0[i].Y + YListPnl.ActualHeight;

        //        //用于此控件修正的表达式，1.3无实际意义，只是为了让曲线恰好与Y轴刻度对齐
        //        p.Y = -pc0[i].Y + YListPnl.ActualHeight + 1.3;

        //        pc1.Add(p);
        //    }

        //    return pc1;
        //}

        /// <summary>
        /// 为 X轴添加若干大刻度
        /// </summary>
        /// <param name="xChapterCount">刻度数</param>
        private void AddXBigChapters(int xChapterCount)
        {
            (xGrid.Children[0] as X).AddXBigChapters(xChapterCount);
        }

        /// <summary>
        /// 设置小刻度的间隔数
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        /// <param name="ccount">小刻度间隔数</param>
        private void SetYSmallIntervalCount(int iY, int ccount)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return;
            }

            (YListPnl.Children[iY] as Y).SetYSmallIntervalCount(ccount);
        }

        /// <summary>
        /// 设置X轴每两个相邻大刻度间的小间隔数
        /// </summary>
        /// <param name="xChapterCount">间隔总数</param>
        public void SetXSmallIntervalCount(int xChapterCount)
        {
            (xGrid.Children[0] as X).AddXSmallChapters(xChapterCount);
        }

        /// <summary>
        /// 将点集绘到指定画布
        /// </summary>
        /// <param name="pc">点集</param>
        /// <param name="iCanvas">画布层次，默认为最下层画布</param>
        private void DrawPointsData(PointCollection pc, int iCanvas)
        {
            if (pc==null)
            {
                return;
            }
            if (YListPnl.Children.Count<=0 || YListPnl.Children.Count-1<iCanvas)
            {
                return;
            }

            GeometryGroup group = new GeometryGroup();

            for (int i = 0; i < pc.Count-1; i++)
            {
                Point p0 = pc[i];//起点
                Point p1 = pc[i + 1];//终点

                if (!double.IsNaN(p0.Y) && !double.IsNaN(p1.Y))
                {
                    LineGeometry line = new LineGeometry();
                    line.StartPoint = p0;
                    line.EndPoint = p1;
                    group.Children.Add(line);
                }
            }

            Path path = new Path();
            path.Stroke= (YListPnl.Children[iCanvas] as Y).YColor;
            path.StrokeThickness = 2;
            path.Data = group;
            path.Name = "curveLine";

            Canvas c = GetCanvas(iCanvas);
            if (c is Canvas)
            {
                c.Children.Add(path);

                //使用翻转效果，减少一次坐标转换过程
                ScaleTransform st = new ScaleTransform();
                st.ScaleY = -1;
                c.LayoutTransform = st;
            }
        }

        /// <summary>
        /// 给某个 Y轴添加若干刻度
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        /// <param name="chapterCount">刻度数</param>
        private void AddBigYChapters(int iY, int chapterCount)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return;
            }

            (YListPnl.Children[iY] as Y).AddBigYChapters(chapterCount);
        }

        /// <summary>
        /// 设置刻度旁文字
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        private void SetYChapterText(int iY)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return;
            }

            (YListPnl.Children[iY] as Y).tbContainer.Children.Clear();//先清空文字

            double yMax = GetYMaxValue(iY);
            double yMin = GetYMinValue(iY);
            if (yMax == double.PositiveInfinity || yMax == double.NaN)
            {
                return;
            }
            if (yMin == double.NegativeInfinity || yMin == double.NaN)
            {
                return;
            }
            int yBigChapterCount = GetYBigChapterCount(iY);//大刻度数      

            if (yBigChapterCount == -1)
            {
                return;
            }

            double range = yMax - yMin;
            double text_Interval = range / yBigChapterCount;    //内容间隔
            double offset_Interval = (int)Height / yBigChapterCount;    //偏移量间隔
            double yHeight = ActualHeight;
            double ctrlHeight = ((YListPnl.Children[iY] as Y).FindParentCtrl(this, "CurvePanel") as CurvePanel).Height;//控件高

            for (int i = 0; i < yBigChapterCount; i++)
            {
                double loc = ctrlHeight - (ctrlHeight / yBigChapterCount) * i;

                string text = "";
                if (text_Interval.ToString().Contains('.'))//若是小数
                {
                    text = (text_Interval * i + yMin).ToString("F2");
                }
                else
                {
                    text = (text_Interval * i + yMin).ToString();
                }

                (YListPnl.Children[iY] as Y).AddYChapterText(loc, text);
            }
        }

        /// <summary>
        /// 获取某个 Y轴的大刻度数
        /// </summary>
        /// <param name="iY"></param>
        private int GetYBigChapterCount(int iY)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return -1;
            }

            if ((YListPnl.Children[iY] as Y).BigYChapterCount == -1)
            {
                return -1;
            }

            return (YListPnl.Children[iY] as Y).BigYChapterCount;
        }

        /// <summary>
        /// 设置某个 Y轴的颜色
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        /// <param name="scb">颜色</param>
        private void SetYColor(int iY, SolidColorBrush scb)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return;
            }

            (YListPnl.Children[iY] as Y).YColor = scb;
        }

        /// <summary>
        /// 获取某个 Y轴的最小值
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        private double GetYMinValue(int iY)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return Double.NaN;
            }
            if ((YListPnl.Children[iY] as Y).YMinValue == Double.NegativeInfinity)
            {
                return Double.NegativeInfinity;
            }

            return (YListPnl.Children[iY] as Y).YMinValue;
        }

        /// <summary>
        /// 获取某个 Y轴的最大值
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        private double GetYMaxValue(int iY)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return Double.NaN;
            }
            if ((YListPnl.Children[iY] as Y).YMaxValue == Double.PositiveInfinity)
            {
                return Double.PositiveInfinity;
            }

            return (YListPnl.Children[iY] as Y).YMaxValue;
        }

        /// <summary>
        /// 设置某个 Y轴的最小值
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        /// <param name="value">值</param>
        private void SetYMinValue(int iY, double value)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return;
            }

            (YListPnl.Children[iY] as Y).YMinValue = value;
        }

        /// <summary>
        /// 设置某个 Y轴的最大值
        /// </summary>
        /// <param name="iY">Y轴编号</param>
        /// <param name="value">值</param>
        private void SetYMaxValue(int iY, double value)
        {
            if (iY > YListPnl.Children.Count - 1 || iY < 0)
            {
                return;
            }

            (YListPnl.Children[iY] as Y).YMaxValue = value;
        }

        /// <summary>
        /// 二分查找法
        /// </summary>
        private int DivSearch(int num, int low, int high, int[] arr)
        {
            int middle = (low + high) / 2;
            while (low <= high)
            {
                if (num < arr[middle])
                {
                    return DivSearch(num, low, middle - 1, arr);
                }
                else if (num > arr[middle])
                {
                    return DivSearch(num, middle + 1, high, arr);
                }
                else
                {
                    return middle;
                }
            }
            return middle;
        }

        // 不清空图像数据而直接将原图像数据覆盖，因此弃用此方法
        ///// <summary>
        ///// 拖动操作时的重置控件方法
        ///// </summary>
        //private void Drag_Reset()
        //{
        //    ClearCanvas();
        //}

    }

    /// <summary>
    /// 坐标原点的位置
    /// </summary>
    public enum OXY
    {
        LeftTop = 1,
        LeftBottom,
        RightBottom
    }

}
