using System.Runtime.CompilerServices;

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
}
