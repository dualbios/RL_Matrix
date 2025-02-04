using OxyPlot;
using OxyPlot.Series;
using RLMatrix;

namespace Road.WinForms;

public class FormDrawingService : IRLChartService {
    private readonly OxyPlot.WindowsForms.PlotView _rewardPlotView;

    public FormDrawingService(OxyPlot.WindowsForms.PlotView rewardPlotView) {
        _rewardPlotView = rewardPlotView;
    }

    public void CreateOrUpdateChart(List<double> episodeRewards) {
        LineSeries series = _rewardPlotView.Model.Series[0] as LineSeries ?? throw new InvalidOperationException();
        foreach (double reward in episodeRewards) {
            series.Points.Add(new DataPoint(series.Points.Count, reward));
        }
    }
}
