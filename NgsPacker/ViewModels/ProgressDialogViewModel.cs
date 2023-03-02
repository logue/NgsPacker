using NgsPacker.Events;
using NgsPacker.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace NgsPacker.ViewModels;

public class ProgressDialogViewModel : BindableBase, IDialogAware
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgressDialogViewModel" /> class.
    /// </summary>
    public ProgressDialogViewModel(IEventAggregator eventAggregator)
    {
        eventAggregator
            .GetEvent<ProgressEvent>()
            .Subscribe(ProgressEventHandle);
    }

    /// <summary>
    ///     メッセージ
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     進捗
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    ///     進捗リングの表示制御
    /// </summary>
    public Visibility ProgressRingVisibility { get; set; }

    /// <summary>
    ///     進捗バーの表示制御
    /// </summary>
    public Visibility ProgressBarVisibility { get; set; }

    /// <summary>
    ///     タイトル
    /// </summary>
    public string Title { get; set; }

    public event Action<IDialogResult> RequestClose;

    /// <inheritdoc />
    public bool CanCloseDialog()
    {
        return false;
    }

    /// <inheritdoc />
    public void OnDialogClosed()
    {
    }

    /// <inheritdoc />
    public void OnDialogOpened(IDialogParameters parameters)
    {
    }

    /// <summary>
    ///     進捗モデルのイベントハンドラ
    /// </summary>
    /// <param name="model">進捗モデル</param>
    private void ProgressEventHandle(ProgressEventModel model)
    {
        Title = model.Title;
        Message = model.Message;


        if (model.IsIntermediate)
        {
            ProgressRingVisibility = Visibility.Visible;
            ProgressBarVisibility = Visibility.Hidden;
        }
        else
        {
            ProgressRingVisibility = Visibility.Hidden;
            ProgressBarVisibility = Visibility.Visible;
        }
    }
}
