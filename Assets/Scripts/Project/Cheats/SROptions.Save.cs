using System;
using System.ComponentModel;


public partial class SROptions
{
    private const string SaveCategory = "Save";

    public event Action AutoRequested;
    public event Action ManualRequested;
    public event Action SaveRequested;
    public event Action CopySaveFileRequested;
    public event Action CopySaveFileClipboardRequested;


    [Category(SaveCategory)]
    public void Auto() => AutoRequested?.Invoke();


    [Category(SaveCategory)]
    public void Manual() => ManualRequested?.Invoke();


    [Category(SaveCategory)]
    public void Save() => SaveRequested?.Invoke();


    [Category(SaveCategory)]
    public void CopySaveFile() => CopySaveFileRequested?.Invoke();


    [Category(SaveCategory)]
    public void CopySaveFileClipboard() => CopySaveFileClipboardRequested?.Invoke();
}
