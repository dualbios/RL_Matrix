using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Series;
using RLMatrix;

namespace Road.Wpf;

public class CanvasDrawingService : IRLChartService {
    private readonly PlotModel _rewardPlotView;

    public CanvasDrawingService(PlotModel rewardPlotView) {
        _rewardPlotView = rewardPlotView;
    }

    public void CreateOrUpdateChart(List<double> episodeRewards) {
        LineSeries series = _rewardPlotView.Series[0] as LineSeries ?? throw new InvalidOperationException();
        foreach (double reward in episodeRewards) {
            series.Points.Add(new DataPoint(series.Points.Count, reward));
        }
    }
}
