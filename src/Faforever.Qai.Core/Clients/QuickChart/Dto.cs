using Newtonsoft.Json;
using System;

namespace Faforever.Qai.Core.Clients.QuickChart;

public class QuickChartRequest
{
    public long Version { get; set; }
    public string? BackgroundColor { get; set; }
    public long Width { get; set; }
    public long Height { get; set; }
    public double? DevicePixelRatio { get; set; }
    public string? Format { get; set; }
    public ChartDefinition? Chart { get; set; }
}

public class ChartDefinition
{
    public ChartOptions? Options { get; set; }
    public ChartData? Data { get; set; }
}

public class ChartData
{
    public ChartDataSet[] Datasets { get; set; } = Array.Empty<ChartDataSet>();
}

public class ChartDataSet
{
    public string? Type { get; set; }
    public string? Label { get; set; }
    [JsonProperty("yAxisID")]
    public string? YAxisId { get; set; }
    public string? BackgroundColor { get; set; }
    public string? BorderColor { get; set; }
    public long? BorderWidth { get; set; }
    public long? Order { get; set; }
    public DataPoint[] Data { get; set; } = Array.Empty<DataPoint>();
}

public class LineChartDataSet : ChartDataSet
{
    public double? PointRadius { get; internal set; }
    public int PointHoverRadius { get; internal set; }
    public int PointHitRadius { get; internal set; }
}

public class BarChartDataSet : ChartDataSet
{
    
    public double? BarPercentage { get; set; }
    public long? BarThickness { get; set; }
    public long? MaxBarThickness { get; set; }
    //public new CandleStickDataPoint[] Data { get; set; } = Array.Empty<CandleStickDataPoint>();
}

public class CandleStickChartDataSet : ChartDataSet
{
    public CandleStickColor? Color { get; set; }
    public CandleStickColor? BorderColor { get; set; }
    //public CandleStickDataPoint[] Data { get; set; } = Array.Empty<CandleStickDataPoint>();
    public string? BackgroundColor { get; set; }
}

public class CandleStickColor
{
    public string? Up { get; set; }
    public string? Down { get; set; }
    public string? Unchanged { get; set; }
}

public class CandleStickDataPoint : DataPoint
{
    [JsonProperty("o")]
    public decimal? Open { get; set; }
    [JsonProperty("h")]
    public decimal? High { get; set; }
    [JsonProperty("l")]
    public decimal? Low { get; set; }
    [JsonProperty("c")]
    public decimal? Close { get; set; }
}

public class LineDataPoint : DataPoint
{
}

public class DataPoint
{
    [JsonProperty("x")]
    public long Timestamp { get; set; }
    [JsonProperty("y")]
    public decimal? Value { get; set; }
}

public class ChartOptions
{
    public bool Parsing { get; set; }
    public bool SpanGaps { get; set; }
    public bool Animation { get; set; }
    public long PointRadius { get; set; }
    public Plugins? Plugins { get; set; }
    public bool Responsive { get; set; }
    public bool MaintainAspectRatio { get; set; }
    public Scales? Scales { get; set; }
    public Interaction? Interaction { get; set; }
}

public class Interaction
{
    public bool Intersect { get; set; }
    public string? Mode { get; set; }
}

public class Plugins
{
    public Title? Title { get; set; }
    public Legend Legend { get; internal set; }
}

public class Title
{
    public bool Display { get; set; }
    public string? Text { get; set; }
}

public class Legend
{
    public bool Display { get; set; }
}

public class Scales
{
    public Scale? X { get; set; }
    public Scale? Y { get; set; }
    public Volume? Volume { get; set; }
}

public class Volume
{
    public string Type { get; set; }
    public bool BeginAtZero { get; set; }
    public string? Position { get; set; }
    public decimal? Max { get; set; }
    public Grid? Grid { get; set; }
    public Ticks? Ticks { get; set; }
}

public class Grid
{
    public bool Display { get; set; }
}

public class Ticks
{
    public bool Display { get; set; }
    public long? Precision { get; set; }
    public TickFormat? Format { get; set; }
    public class TickFormat
    {
        public int MinimumFractionDigits { get; set; }
        public int MaximumFractionDigits { get; set; }
    }
}

public class Scale
{
    public string? Type { get; set; }
}
