using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace exam_timer_ava.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive]
    public bool ShowSettings { get; set; } = true;

    public void ShowHideSettings() => ShowSettings = !ShowSettings;

    [Reactive]
    public bool HasPerusal { get; set; } = true;
    [Reactive]
    public bool HasPlanning { get; set; } = false;
    [Reactive]
    public bool HasNoPrep { get; set; } = false;

    [ObservableAsProperty]
    public bool CanSetPreparation { get; }

    [Reactive]
    public int? PreparationMinutes { get; set; } = 5;

    [Reactive]
    public int? WorkingMinutes { get; set; } = 60;

    [ObservableAsProperty]
    public bool HasEffectivePreparation { get; }

    [ObservableAsProperty]
    public string PreparationName { get; } = "Perusal";

    [Reactive]
    public TimeSpan PreparationStartTime { get; set; }
    [Reactive]
    public TimeSpan WorkingStartTime { get; set; }
    [Reactive]
    public TimeSpan FinishTime { get; set; }

    [Reactive]
    public string CurrentActivity { get; } = string.Empty;
    [Reactive]
    public string RemainingTime { get; } = string.Empty;

    public MainWindowViewModel()
    {
        var now = DateTime.Now.TimeOfDay;
        PreparationStartTime = TimeSpan.FromMinutes((now.Hours * 60 + now.Minutes) / 5 * 5 + 5);
        WorkingStartTime = PreparationStartTime.Add(TimeSpan.FromMinutes(5));
        FinishTime = WorkingStartTime.Add(TimeSpan.FromMinutes(60));

        this.WhenAnyValue(x => x.HasPerusal, x => x.HasPlanning, (perusal, planning) => perusal || planning)
            .ToPropertyEx(this, x => x.CanSetPreparation);
        this.WhenAnyValue(x => x.HasPerusal, x => x.HasPlanning, x => x.PreparationMinutes, (perusal, planning, minutes) => (perusal || planning) && PreparationMinutes.HasValue && PreparationMinutes.Value > 0)
            .ToPropertyEx(this, x => x.HasEffectivePreparation);
        this.WhenAnyValue(x => x.HasPerusal, x => x.HasPlanning, (perusal, planning) => perusal ? "Perusal" : planning ? "Planning" : "No Prep")
            .ToPropertyEx(this, x => x.PreparationName);

        this.WhenAnyValue(x => x.PreparationStartTime, x => x.PreparationMinutes)
            .Subscribe(_ => WorkingStartTime = PreparationStartTime.Add(TimeSpan.FromMinutes(PreparationMinutes ?? 0)));
        this.WhenAnyValue(x => x.WorkingStartTime, x => x.WorkingMinutes)
            .Subscribe(_ => {
                PreparationStartTime = WorkingStartTime.Subtract(TimeSpan.FromMinutes(PreparationMinutes ?? 0));
                FinishTime = WorkingStartTime.Add(TimeSpan.FromMinutes(WorkingMinutes ?? 0));
            });
        this.WhenAnyValue(x => x.FinishTime)
            .Subscribe(_ => WorkingStartTime = FinishTime.Subtract(TimeSpan.FromMinutes(WorkingMinutes ?? 0)));
    }
}
