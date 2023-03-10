using JL.Core.Utilities;

namespace JL.Core;

public interface IFrontend
{
    public CoreConfig CoreConfig { get; }

    public void PlayAudio(byte[] sound, float volume = 1);

    public void Alert(AlertLevel alertLevel, string message);

    public bool ShowYesNoDialog(string text, string caption);

    public void ShowOkDialog(string text, string caption);

    public Task CopyFromWebSocket(string text);

    public Task UpdateJL(Uri downloadUrlOfLatestJLRelease);

    public void InvalidateDisplayCache();

    public void ApplyDictOptions();
}
