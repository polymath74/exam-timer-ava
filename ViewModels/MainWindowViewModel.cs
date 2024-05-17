using ReactiveUI.Fody.Helpers;

namespace exam_timer_ava.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive]
    public bool ShowSettings { get; set; } = true;

    public void ShowHideSettings() => ShowSettings = !ShowSettings;
}
