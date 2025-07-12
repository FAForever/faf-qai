using System;

namespace Faforever.Qai.Core.Clients.QuickChart;

public static class ChartTemplate
{
    public static QuickChartRequest CreateRatingChartRequest(string title)
    {
        var req = new QuickChartRequest
        {
            Version = 4,
            BackgroundColor = "transparent",
            Width = 1200,
            Height = 600,
            DevicePixelRatio = 1.0,
            Format = "png",
            Chart = new ChartDefinition
            {
                Options = new ChartOptions
                {
                    Parsing = false,
                    SpanGaps = true,
                    Animation = false,
                    PointRadius = 0,
                    Plugins = new Plugins
                    {
                        Title = new Title
                        {
                            Display = true,
                            Text = title
                        },
                        Legend = new Legend
                        {
                            Display = false
                        }
                    },
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Scales = new Scales
                    {
                        X = new Scale { Type = "timeseries" },
                        Y = new Scale { Type = "linear" },
                        Volume = new Volume
                        {
                            Type = "linear",
                            BeginAtZero = true,
                            Position = "right",
                            Max = 1000,
                            Grid = new Grid { Display = true },
                            Ticks = new Ticks
                            {
                                Display = true,
                                Format = new Ticks.TickFormat { MinimumFractionDigits = 0, MaximumFractionDigits = 0 }
                            }
                        }
                    },
                    Interaction = new Interaction
                    {
                        Intersect = false,
                        Mode = "index"
                    }
                },
                Data = new ChartData
                {
                    Datasets = [
                            new CandleStickChartDataSet {
                                Type = "candlestick",
                                //Label = "Rating",
                                YAxisId = "y",
                                Order = 2,
                                Color = new CandleStickColor {
                                    Up = "#3CB36E",
                                    Down = "#F74F4E",
                                    Unchanged = "#999"
                                },
                                BorderColor = new CandleStickColor {
                                    Up = "#317F50",
                                    Down = "#BA4040",
                                    Unchanged = "#999"
                                },
                                Data = Array.Empty<CandleStickDataPoint>()
                            },
                            new BarChartDataSet {
                                Type = "bar",
                                Label = "# games",
                                YAxisId = "volume",
                                Order = 12,
                                BackgroundColor = "#4789D5",
                                BorderColor = "#31547C",
                                BorderWidth = 1,
                                BarPercentage = 0.5,
                                BarThickness = 6,
                                MaxBarThickness = 8,
                                Data = Array.Empty<DataPoint>()
                            },
                            new LineChartDataSet
                            {
                                Type = "line",
                                Label = "Average",
                                YAxisId = "y",
                                Order = 0,
                                BorderColor = "#F7B84B",
                                BorderWidth = 2,
                                PointRadius = 2,
                                PointHoverRadius = 3,
                                PointHitRadius = 5,
                                Data = Array.Empty<DataPoint>()
                            }
                        ]
                }
            }
        };

        return req;
    }
}