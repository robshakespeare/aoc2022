using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AoC.MAUI;

public partial class ResultDisplayView
{
    public static readonly BindableProperty PartNumProperty = BindableProperty.Create(nameof(PartNum), typeof(int), typeof(ResultDisplayView), 0);

    public int PartNum
    {
        get => (int) GetValue(PartNumProperty);
        set => SetValue(PartNumProperty, value);
    }

    public static readonly BindableProperty ResultProperty = BindableProperty.Create(nameof(Result), typeof(Result), typeof(ResultDisplayView), new Result());

    public Result Result
    {
        get => (Result) GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }

    public ResultDisplayView()
    {
        InitializeComponent();
    }

    private async void CopyResultEventAsync(object? sender, TappedEventArgs e)
    {
        await Clipboard.Default.SetTextAsync($"{Result.Value}");
        var toast = Toast.Make($"✔️ Part {PartNum} copied to clipboard", ToastDuration.Short, 18);
        await toast.Show();
    }
}
