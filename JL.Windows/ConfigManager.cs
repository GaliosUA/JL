using System.Configuration;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml;
using HandyControl.Data;
using JL.Core;
using JL.Core.Utilities;
using JL.Windows.GUI;
using JL.Windows.Utilities;

namespace JL.Windows;

internal sealed class ConfigManager : CoreConfig
{
    private static ConfigManager? s_instance;

    public static ConfigManager Instance => s_instance ??= new ConfigManager();

    #region General

    private static readonly List<ComboBoxItem> s_japaneseFonts =
        WindowsUtils.FindJapaneseFonts().OrderByDescending(static f => f.Foreground.ToString(CultureInfo.InvariantCulture)).ThenBy(static font => font.Content)
            .ToList();

    private static readonly List<ComboBoxItem> s_popupJapaneseFonts =
        s_japaneseFonts.ConvertAll(static f => new ComboBoxItem
        {
            Content = f.Content,
            FontFamily = f.FontFamily,
            Foreground = f.Foreground
        });

    public static bool InactiveLookupMode { get; set; } = false; // todo checkbox?
    public static bool InvisibleMode { get; set; } = false;
    public static Brush HighlightColor { get; private set; } = Brushes.AliceBlue;
    public static bool RequireLookupKeyPress { get; private set; } = false;
    public static bool LookupOnSelectOnly { get; private set; } = false;
    public static bool LookupOnLeftClickOnly { get; private set; } = false;
    public static bool AutoAdjustFontSizesOnResolutionChange { get; private set; } = true;

    public static KeyGesture LookupKeyKeyGesture { get; private set; } = new(Key.LeftShift, ModifierKeys.None);
    public static bool HighlightLongestMatch { get; private set; } = false;
    public static bool AutoPlayAudio { get; private set; } = false;
    public static bool CheckForJLUpdatesOnStartUp { get; private set; } = true;
    public static bool DisableHotkeys { get; set; } = false;
    public static bool Focusable { get; private set; } = true;

    #endregion

    #region MainWindow

    public static double MainWindowWidth { get; set; } = 800;
    public static double MainWindowHeight { get; set; } = 200;
    public static bool MainWindowDynamicHeight { get; private set; } = false;
    public static bool MainWindowDynamicWidth { get; private set; } = false;
    public static double MainWindowMaxDynamicWidth { get; private set; } = 800;
    public static double MainWindowMaxDynamicHeight { get; private set; } = 269;
    public static Brush MainWindowTextColor { get; private set; } = Brushes.White;
    public static Brush MainWindowBacklogTextColor { get; private set; } = Brushes.Bisque;
    public static bool AlwaysOnTop { get; set; } = true;
    public static bool TextOnlyVisibleOnHover { get; set; } = false;
    public static bool ChangeMainWindowBackgroundOpacityOnUnhover { get; private set; } = false;
    public static double MainWindowBackgroundOpacityOnUnhover { get; private set; } = 0.2; // 0.2-100
    public static bool TextBoxTrimWhiteSpaceCharacters { get; private set; } = true;
    public static bool TextBoxRemoveNewlines { get; private set; } = false;
    public static bool TextBoxIsReadOnly { get; set; } = true;
    private static bool TextBoxApplyDropShadowEffect { get; set; } = true;
    public static bool CaptureTextFromClipboard { get; set; } = true;
    public static bool OnlyCaptureTextWithJapaneseChars { get; private set; } = true;
    public static bool DisableLookupsForNonJapaneseCharsInMainWindow { get; private set; } = false;
    public static bool MainWindowFocusOnHover { get; private set; } = false;
    public static bool SteppedBacklogWithMouseWheel { get; private set; } = false;
    public static bool CaptureTextFromWebSocket { get; set; } = false;
    public static Uri WebSocketUri { get; private set; } = new("ws://127.0.0.1:6677");

    #endregion

    #region Popup

    public static FontFamily PopupFont { get; private set; } = new("Meiryo");
    public static int PopupMaxWidth { get; set; } = 700;
    public static int PopupMaxHeight { get; set; } = 520;
    public static bool PopupDynamicHeight { get; private set; } = true;
    public static bool PopupDynamicWidth { get; private set; } = true;
    public static bool FixedPopupPositioning { get; private set; } = false;
    public static int FixedPopupXPosition { get; set; } = 0;
    public static int FixedPopupYPosition { get; set; } = 0;
    public static bool PopupFocusOnLookup { get; private set; } = false;
    public static bool ShowMiningModeReminder { get; private set; } = true;
    public static bool DisableLookupsForNonJapaneseCharsInPopups { get; private set; } = true;
    public static Brush PopupBackgroundColor { get; private set; } = new SolidColorBrush(Color.FromRgb(0, 0, 0)) { Opacity = 0.7 };
    public static int PopupXOffset { get; set; } = 10;
    public static int PopupYOffset { get; set; } = 20;
    public static bool PopupFlipX { get; private set; } = true;
    public static bool PopupFlipY { get; private set; } = true;
    public static Brush PrimarySpellingColor { get; private set; } = Brushes.Chocolate;
    public static int PrimarySpellingFontSize { get; set; } = 21;
    public static Brush ReadingsColor { get; private set; } = Brushes.Goldenrod;
    public static int ReadingsFontSize { get; set; } = 19;
    public static Brush AlternativeSpellingsColor { get; private set; } = Brushes.White;
    public static int AlternativeSpellingsFontSize { get; set; } = 17;
    public static Brush DefinitionsColor { get; private set; } = Brushes.White;
    public static int DefinitionsFontSize { get; set; } = 17;
    public static Brush FrequencyColor { get; private set; } = Brushes.White;
    public static int FrequencyFontSize { get; set; } = 17;
    public static Brush DeconjugationInfoColor { get; private set; } = Brushes.White;
    public static int DeconjugationInfoFontSize { get; set; } = 17;
    public static Brush DictTypeColor { get; private set; } = Brushes.LightBlue;
    public static int DictTypeFontSize { get; set; } = 15;
    public static Brush SeparatorColor { get; private set; } = Brushes.White;
    public static bool HideDictButtonsWithNoResults { get; private set; } = false;
    public static bool AutoHidePopupIfMouseIsNotOverIt { get; private set; } = false;
    public static int AutoHidePopupIfMouseIsNotOverItDelayInMilliseconds { get; private set; } = 3000;

    #endregion

    #region Anki
    public static bool AnkiIntegration { get; set; } = false;
    #endregion

    #region Hotkeys

    public static KeyGesture DisableHotkeysKeyGesture { get; private set; } = new(Key.Pause, ModifierKeys.Windows);
    public static KeyGesture MiningModeKeyGesture { get; private set; } = new(Key.M, ModifierKeys.Windows);
    public static KeyGesture PlayAudioKeyGesture { get; private set; } = new(Key.P, ModifierKeys.Windows);
    public static KeyGesture KanjiModeKeyGesture { get; private set; } = new(Key.K, ModifierKeys.Windows);

    public static KeyGesture ShowManageDictionariesWindowKeyGesture { get; private set; } =
        new(Key.D, ModifierKeys.Windows);

    public static KeyGesture ShowManageFrequenciesWindowKeyGesture { get; private set; } =
        new(Key.F, ModifierKeys.Windows);

    public static KeyGesture ShowPreferencesWindowKeyGesture { get; private set; } = new(Key.L, ModifierKeys.Windows);
    public static KeyGesture ShowAddNameWindowKeyGesture { get; private set; } = new(Key.N, ModifierKeys.Windows);
    public static KeyGesture ShowAddWordWindowKeyGesture { get; private set; } = new(Key.W, ModifierKeys.Windows);
    public static KeyGesture SearchWithBrowserKeyGesture { get; private set; } = new(Key.S, ModifierKeys.Windows);
    public static KeyGesture MousePassThroughModeKeyGesture { get; private set; } = new(Key.T, ModifierKeys.Windows);
    public static KeyGesture InvisibleToggleModeKeyGesture { get; private set; } = new(Key.I, ModifierKeys.Windows);
    public static KeyGesture SteppedBacklogBackwardsKeyGesture { get; private set; } = new(Key.Left, ModifierKeys.Windows);
    public static KeyGesture SteppedBacklogForwardsKeyGesture { get; private set; } = new(Key.Right, ModifierKeys.Windows);
    public static KeyGesture InactiveLookupModeKeyGesture { get; private set; } = new(Key.Q, ModifierKeys.Windows);
    public static KeyGesture MotivationKeyGesture { get; private set; } = new(Key.O, ModifierKeys.Windows);
    public static KeyGesture ClosePopupKeyGesture { get; private set; } = new(Key.Escape, ModifierKeys.Windows);
    public static KeyGesture ShowStatsKeyGesture { get; private set; } = new(Key.Y, ModifierKeys.Windows);
    public static KeyGesture NextDictKeyGesture { get; private set; } = new(Key.PageDown, ModifierKeys.Windows);
    public static KeyGesture PreviousDictKeyGesture { get; private set; } = new(Key.PageUp, ModifierKeys.Windows);
    public static KeyGesture AlwaysOnTopKeyGesture { get; private set; } = new(Key.R, ModifierKeys.Windows);
    public static KeyGesture TextOnlyVisibleOnHoverKeyGesture { get; private set; } = new(Key.E, ModifierKeys.Windows);
    public static KeyGesture TextBoxIsReadOnlyKeyGesture { get; private set; } = new(Key.U, ModifierKeys.Windows);
    public static KeyGesture CaptureTextFromClipboardKeyGesture { get; private set; } = new(Key.F12, ModifierKeys.Windows);
    public static KeyGesture CaptureTextFromWebSocketdKeyGesture { get; private set; } = new(Key.F11, ModifierKeys.Windows);
    public static KeyGesture ReconnectToWebSocketServerKeyGesture { get; private set; } = new(Key.F9, ModifierKeys.Windows);
    public static KeyGesture DeleteCurrentLineKeyGesture { get; private set; } = new(Key.Delete, ModifierKeys.Windows);

    #endregion

    #region Advanced

    public static int MaxSearchLength { get; private set; } = 37;
    public static int MaxNumResultsNotInMiningMode { get; private set; } = 7;
    public static bool Precaching { get; private set; } = false;
    public static string SearchUrl { get; private set; } = "https://www.google.com/search?q={SearchTerm}&hl=ja";
    public static string BrowserPath { get; private set; } = "";

    #endregion

    public void ApplyPreferences()
    {
        string? minimumLogLevelStr = ConfigurationManager.AppSettings.Get("MinimumLogLevel");
        if (minimumLogLevelStr is null)
        {
            AddToConfig("MinimumLogLevel", "Error");
        }
        else
        {
            Utils.LoggingLevelSwitch.MinimumLevel = minimumLogLevelStr switch
            {
                "Fatal" => Serilog.Events.LogEventLevel.Fatal,
                "Error" => Serilog.Events.LogEventLevel.Error,
                "Warning" => Serilog.Events.LogEventLevel.Warning,
                "Information" => Serilog.Events.LogEventLevel.Information,
                "Debug" => Serilog.Events.LogEventLevel.Debug,
                "Verbose" => Serilog.Events.LogEventLevel.Verbose,
                _ => Serilog.Events.LogEventLevel.Error,
            };
        }

        MainWindow mainWindow = MainWindow.Instance;

        AutoAdjustFontSizesOnResolutionChange = GetValueFromConfig(AutoAdjustFontSizesOnResolutionChange, nameof(AutoAdjustFontSizesOnResolutionChange), bool.TryParse);
        HighlightLongestMatch = GetValueFromConfig(HighlightLongestMatch, nameof(HighlightLongestMatch), bool.TryParse);
        AutoPlayAudio = GetValueFromConfig(AutoPlayAudio, nameof(AutoPlayAudio), bool.TryParse);
        Precaching = GetValueFromConfig(Precaching, nameof(Precaching), bool.TryParse);
        CheckForJLUpdatesOnStartUp = GetValueFromConfig(CheckForJLUpdatesOnStartUp, nameof(CheckForJLUpdatesOnStartUp), bool.TryParse);
        AlwaysOnTop = GetValueFromConfig(AlwaysOnTop, nameof(AlwaysOnTop), bool.TryParse);
        mainWindow.Topmost = AlwaysOnTop;

        RequireLookupKeyPress = GetValueFromConfig(RequireLookupKeyPress, nameof(RequireLookupKeyPress), bool.TryParse);
        DisableHotkeys = GetValueFromConfig(DisableHotkeys, nameof(DisableHotkeys), bool.TryParse);
        Focusable = GetValueFromConfig(Focusable, nameof(Focusable), bool.TryParse);
        AnkiIntegration = GetValueFromConfig(AnkiIntegration, nameof(AnkiIntegration), bool.TryParse);
        KanjiMode = GetValueFromConfig(KanjiMode, nameof(KanjiMode), bool.TryParse);
        ForceSyncAnki = GetValueFromConfig(ForceSyncAnki, nameof(ForceSyncAnki), bool.TryParse);
        AllowDuplicateCards = GetValueFromConfig(AllowDuplicateCards, nameof(AllowDuplicateCards), bool.TryParse);
        PopupFocusOnLookup = GetValueFromConfig(PopupFocusOnLookup, nameof(PopupFocusOnLookup), bool.TryParse);
        ShowMiningModeReminder = GetValueFromConfig(ShowMiningModeReminder, nameof(ShowMiningModeReminder), bool.TryParse);
        DisableLookupsForNonJapaneseCharsInPopups = GetValueFromConfig(DisableLookupsForNonJapaneseCharsInPopups, nameof(DisableLookupsForNonJapaneseCharsInPopups), bool.TryParse);
        FixedPopupPositioning = GetValueFromConfig(FixedPopupPositioning, nameof(FixedPopupPositioning), bool.TryParse);
        ChangeMainWindowBackgroundOpacityOnUnhover = GetValueFromConfig(ChangeMainWindowBackgroundOpacityOnUnhover, nameof(ChangeMainWindowBackgroundOpacityOnUnhover), bool.TryParse);
        TextOnlyVisibleOnHover = GetValueFromConfig(TextOnlyVisibleOnHover, nameof(TextOnlyVisibleOnHover), bool.TryParse);
        TextBoxTrimWhiteSpaceCharacters = GetValueFromConfig(TextBoxTrimWhiteSpaceCharacters, nameof(TextBoxTrimWhiteSpaceCharacters), bool.TryParse);
        TextBoxRemoveNewlines = GetValueFromConfig(TextBoxRemoveNewlines, nameof(TextBoxRemoveNewlines), bool.TryParse);
        CaptureTextFromClipboard = GetValueFromConfig(CaptureTextFromClipboard, nameof(CaptureTextFromClipboard), bool.TryParse);
        CaptureTextFromWebSocket = GetValueFromConfig(CaptureTextFromWebSocket, nameof(CaptureTextFromWebSocket), bool.TryParse);
        OnlyCaptureTextWithJapaneseChars = GetValueFromConfig(OnlyCaptureTextWithJapaneseChars, nameof(OnlyCaptureTextWithJapaneseChars), bool.TryParse);
        DisableLookupsForNonJapaneseCharsInMainWindow = GetValueFromConfig(DisableLookupsForNonJapaneseCharsInMainWindow, nameof(DisableLookupsForNonJapaneseCharsInMainWindow), bool.TryParse);
        MainWindowFocusOnHover = GetValueFromConfig(MainWindowFocusOnHover, nameof(MainWindowFocusOnHover), bool.TryParse);
        SteppedBacklogWithMouseWheel = GetValueFromConfig(SteppedBacklogWithMouseWheel, nameof(SteppedBacklogWithMouseWheel), bool.TryParse);
        MainWindowDynamicHeight = GetValueFromConfig(MainWindowDynamicHeight, nameof(MainWindowDynamicHeight), bool.TryParse);
        MainWindowDynamicWidth = GetValueFromConfig(MainWindowDynamicWidth, nameof(MainWindowDynamicWidth), bool.TryParse);
        PopupDynamicHeight = GetValueFromConfig(PopupDynamicHeight, nameof(PopupDynamicHeight), bool.TryParse);
        PopupDynamicWidth = GetValueFromConfig(PopupDynamicWidth, nameof(PopupDynamicWidth), bool.TryParse);
        HideDictButtonsWithNoResults = GetValueFromConfig(HideDictButtonsWithNoResults, nameof(HideDictButtonsWithNoResults), bool.TryParse);
        AutoHidePopupIfMouseIsNotOverIt = GetValueFromConfig(AutoHidePopupIfMouseIsNotOverIt, nameof(AutoHidePopupIfMouseIsNotOverIt), bool.TryParse);

        TextBoxIsReadOnly = GetValueFromConfig(TextBoxIsReadOnly, nameof(TextBoxIsReadOnly), bool.TryParse);
        mainWindow.MainTextBox.IsReadOnly = TextBoxIsReadOnly;
        mainWindow.MainTextBox.IsUndoEnabled = !TextBoxIsReadOnly;

        TextBoxApplyDropShadowEffect = GetValueFromConfig(TextBoxApplyDropShadowEffect, nameof(TextBoxApplyDropShadowEffect), bool.TryParse);
        if (TextBoxApplyDropShadowEffect)
        {
            DropShadowEffect dropShadowEffect = new() { Direction = 320, BlurRadius = 4, ShadowDepth = 1.3, Opacity = 0.8, RenderingBias = RenderingBias.Quality };
            dropShadowEffect.Freeze();
            mainWindow.MainTextBox.Effect = dropShadowEffect;
        }

        else
        {
            mainWindow.MainTextBox.Effect = null;
        }

        MaxSearchLength = GetValueFromConfig(MaxSearchLength, nameof(MaxSearchLength), int.TryParse);
        LookupRate = GetValueFromConfig(LookupRate, nameof(LookupRate), int.TryParse);
        PrimarySpellingFontSize = GetValueFromConfig(PrimarySpellingFontSize, nameof(PrimarySpellingFontSize), int.TryParse);
        ReadingsFontSize = GetValueFromConfig(ReadingsFontSize, nameof(ReadingsFontSize), int.TryParse);
        AlternativeSpellingsFontSize = GetValueFromConfig(AlternativeSpellingsFontSize, nameof(AlternativeSpellingsFontSize), int.TryParse);
        DefinitionsFontSize = GetValueFromConfig(DefinitionsFontSize, nameof(DefinitionsFontSize), int.TryParse);
        FrequencyFontSize = GetValueFromConfig(FrequencyFontSize, nameof(FrequencyFontSize), int.TryParse);
        DeconjugationInfoFontSize = GetValueFromConfig(DeconjugationInfoFontSize, nameof(DeconjugationInfoFontSize), int.TryParse);
        DictTypeFontSize = GetValueFromConfig(DictTypeFontSize, nameof(DictTypeFontSize), int.TryParse);
        MaxNumResultsNotInMiningMode = GetValueFromConfig(MaxNumResultsNotInMiningMode, nameof(MaxNumResultsNotInMiningMode), int.TryParse);

        AutoHidePopupIfMouseIsNotOverItDelayInMilliseconds = GetValueFromConfig(AutoHidePopupIfMouseIsNotOverItDelayInMilliseconds, nameof(AutoHidePopupIfMouseIsNotOverItDelayInMilliseconds), int.TryParse);
        PopupWindow.PopupAutoHideTimer.Enabled = false;
        PopupWindow.PopupAutoHideTimer.Interval = AutoHidePopupIfMouseIsNotOverItDelayInMilliseconds;

        PopupXOffset = GetValueFromConfig(PopupXOffset, nameof(PopupXOffset), int.TryParse);
        WindowsUtils.DpiAwareXOffset = PopupXOffset / WindowsUtils.Dpi.DpiScaleX;

        PopupYOffset = GetValueFromConfig(PopupYOffset, nameof(PopupYOffset), int.TryParse);
        WindowsUtils.DpiAwareYOffset = PopupYOffset / WindowsUtils.Dpi.DpiScaleY;

        PopupMaxWidth = GetValueFromConfig(PopupMaxWidth, nameof(PopupMaxWidth), int.TryParse);
        WindowsUtils.DpiAwarePopupMaxWidth = PopupMaxWidth / WindowsUtils.Dpi.DpiScaleX;

        PopupMaxHeight = GetValueFromConfig(PopupMaxHeight, nameof(PopupMaxHeight), int.TryParse);
        WindowsUtils.DpiAwarePopupMaxHeight = PopupMaxHeight / WindowsUtils.Dpi.DpiScaleY;

        FixedPopupXPosition = GetValueFromConfig(FixedPopupXPosition, nameof(FixedPopupXPosition), int.TryParse);
        WindowsUtils.DpiAwareFixedPopupXPosition = FixedPopupXPosition / WindowsUtils.Dpi.DpiScaleX;

        FixedPopupYPosition = GetValueFromConfig(FixedPopupYPosition, nameof(FixedPopupYPosition), int.TryParse);
        WindowsUtils.DpiAwareFixedPopupYPosition = FixedPopupYPosition / WindowsUtils.Dpi.DpiScaleY;

        mainWindow.OpacitySlider.Value = GetNumberWithDecimalPointFromConfig(mainWindow.OpacitySlider.Value, "MainWindowOpacity", double.TryParse);
        mainWindow.FontSizeSlider.Value = GetNumberWithDecimalPointFromConfig(mainWindow.FontSizeSlider.Value, "MainWindowFontSize", double.TryParse);
        MainWindowBackgroundOpacityOnUnhover = GetNumberWithDecimalPointFromConfig(MainWindowBackgroundOpacityOnUnhover, nameof(MainWindowBackgroundOpacityOnUnhover), double.TryParse);

        MainWindowHeight = GetNumberWithDecimalPointFromConfig(MainWindowHeight, nameof(MainWindowHeight), double.TryParse);
        MainWindowWidth = GetNumberWithDecimalPointFromConfig(MainWindowWidth, nameof(MainWindowWidth), double.TryParse);
        MainWindowMaxDynamicWidth = GetNumberWithDecimalPointFromConfig(MainWindowMaxDynamicWidth, nameof(MainWindowMaxDynamicWidth), double.TryParse);
        MainWindowMaxDynamicHeight = GetNumberWithDecimalPointFromConfig(MainWindowMaxDynamicHeight, nameof(MainWindowMaxDynamicHeight), double.TryParse);
        WindowsUtils.SetSizeToContentForMainWindow(MainWindowDynamicWidth, MainWindowDynamicHeight, MainWindowMaxDynamicWidth, MainWindowMaxDynamicHeight, MainWindowWidth, MainWindowHeight, mainWindow);
        mainWindow.WidthBeforeResolutionChange = MainWindowWidth;
        mainWindow.HeightBeforeResolutionChange = MainWindowHeight;

        mainWindow.Top = GetNumberWithDecimalPointFromConfig(mainWindow.Top, "MainWindowTopPosition", double.TryParse);
        mainWindow.Left = GetNumberWithDecimalPointFromConfig(mainWindow.Left, "MainWindowLeftPosition", double.TryParse);

        mainWindow.TopPositionBeforeResolutionChange = mainWindow.Top;
        mainWindow.LeftPositionBeforeResolutionChange = mainWindow.Left;

        mainWindow.MainGrid.Opacity = TextOnlyVisibleOnHover && !mainWindow.IsMouseOver ? 0 : 1;

        // MAKE SURE YOU FREEZE ANY NEW COLOR OBJECTS YOU ADD
        // OR THE PROGRAM WILL CRASH AND BURN
        MainWindowTextColor = GetFrozenBrushFromConfig(MainWindowTextColor, nameof(MainWindowTextColor));

        MainWindowBacklogTextColor = GetFrozenBrushFromConfig(MainWindowBacklogTextColor, nameof(MainWindowBacklogTextColor));
        PrimarySpellingColor = GetFrozenBrushFromConfig(PrimarySpellingColor, nameof(PrimarySpellingColor));
        ReadingsColor = GetFrozenBrushFromConfig(ReadingsColor, nameof(ReadingsColor));
        AlternativeSpellingsColor = GetFrozenBrushFromConfig(AlternativeSpellingsColor, nameof(AlternativeSpellingsColor));
        DefinitionsColor = GetFrozenBrushFromConfig(DefinitionsColor, nameof(DefinitionsColor));
        FrequencyColor = GetFrozenBrushFromConfig(FrequencyColor, nameof(FrequencyColor));
        DeconjugationInfoColor = GetFrozenBrushFromConfig(DeconjugationInfoColor, nameof(DeconjugationInfoColor));
        SeparatorColor = GetFrozenBrushFromConfig(SeparatorColor, nameof(SeparatorColor));
        DictTypeColor = GetFrozenBrushFromConfig(DictTypeColor, nameof(DictTypeColor));

        HighlightColor = GetFrozenBrushFromConfig(HighlightColor, nameof(HighlightColor));
        mainWindow.MainTextBox.SelectionBrush = HighlightColor;

        PopupBackgroundColor = GetBrushFromConfig(PopupBackgroundColor, nameof(PopupBackgroundColor));
        PopupBackgroundColor.Opacity = GetNumberWithDecimalPointFromConfig(70.0, "PopupOpacity", double.TryParse) / 100;
        PopupBackgroundColor.Freeze();

        mainWindow.Background = GetBrushFromConfig(mainWindow.Background, "MainWindowBackgroundColor");

        mainWindow.Background.Opacity = ChangeMainWindowBackgroundOpacityOnUnhover && !mainWindow.IsMouseOver
            ? MainWindowBackgroundOpacityOnUnhover / 100
            : mainWindow.OpacitySlider.Value / 100;

        DisableHotkeysKeyGesture = WindowsUtils.SetKeyGesture(nameof(DisableHotkeysKeyGesture), DisableHotkeysKeyGesture);
        MiningModeKeyGesture = WindowsUtils.SetKeyGesture(nameof(MiningModeKeyGesture), MiningModeKeyGesture);
        PlayAudioKeyGesture = WindowsUtils.SetKeyGesture(nameof(PlayAudioKeyGesture), PlayAudioKeyGesture);
        KanjiModeKeyGesture = WindowsUtils.SetKeyGesture(nameof(KanjiModeKeyGesture), KanjiModeKeyGesture);
        LookupKeyKeyGesture = WindowsUtils.SetKeyGesture(nameof(LookupKeyKeyGesture), LookupKeyKeyGesture);
        ClosePopupKeyGesture = WindowsUtils.SetKeyGesture(nameof(ClosePopupKeyGesture), ClosePopupKeyGesture);
        ShowStatsKeyGesture = WindowsUtils.SetKeyGesture(nameof(ShowStatsKeyGesture), ShowStatsKeyGesture);
        NextDictKeyGesture = WindowsUtils.SetKeyGesture(nameof(NextDictKeyGesture), NextDictKeyGesture);
        PreviousDictKeyGesture = WindowsUtils.SetKeyGesture(nameof(PreviousDictKeyGesture), PreviousDictKeyGesture);
        AlwaysOnTopKeyGesture = WindowsUtils.SetKeyGesture(nameof(AlwaysOnTopKeyGesture), AlwaysOnTopKeyGesture);
        TextOnlyVisibleOnHoverKeyGesture = WindowsUtils.SetKeyGesture(nameof(TextOnlyVisibleOnHoverKeyGesture), TextOnlyVisibleOnHoverKeyGesture);
        TextBoxIsReadOnlyKeyGesture = WindowsUtils.SetKeyGesture(nameof(TextBoxIsReadOnlyKeyGesture), TextBoxIsReadOnlyKeyGesture);
        CaptureTextFromClipboardKeyGesture = WindowsUtils.SetKeyGesture(nameof(CaptureTextFromClipboardKeyGesture), CaptureTextFromClipboardKeyGesture);
        CaptureTextFromWebSocketdKeyGesture = WindowsUtils.SetKeyGesture(nameof(CaptureTextFromWebSocketdKeyGesture), CaptureTextFromWebSocketdKeyGesture);
        ReconnectToWebSocketServerKeyGesture = WindowsUtils.SetKeyGesture(nameof(ReconnectToWebSocketServerKeyGesture), ReconnectToWebSocketServerKeyGesture);
        DeleteCurrentLineKeyGesture = WindowsUtils.SetKeyGesture(nameof(DeleteCurrentLineKeyGesture), DeleteCurrentLineKeyGesture);

        ShowPreferencesWindowKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(ShowPreferencesWindowKeyGesture), ShowPreferencesWindowKeyGesture);
        ShowAddNameWindowKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(ShowAddNameWindowKeyGesture), ShowAddNameWindowKeyGesture);
        ShowAddWordWindowKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(ShowAddWordWindowKeyGesture), ShowAddWordWindowKeyGesture);
        SearchWithBrowserKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(SearchWithBrowserKeyGesture), SearchWithBrowserKeyGesture);
        MousePassThroughModeKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(MousePassThroughModeKeyGesture), MousePassThroughModeKeyGesture);
        InvisibleToggleModeKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(InvisibleToggleModeKeyGesture), InvisibleToggleModeKeyGesture);
        SteppedBacklogBackwardsKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(SteppedBacklogBackwardsKeyGesture), SteppedBacklogBackwardsKeyGesture);
        SteppedBacklogForwardsKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(SteppedBacklogForwardsKeyGesture), SteppedBacklogForwardsKeyGesture);
        InactiveLookupModeKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(InactiveLookupModeKeyGesture), InactiveLookupModeKeyGesture);
        MotivationKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(MotivationKeyGesture), MotivationKeyGesture);

        ShowManageDictionariesWindowKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(ShowManageDictionariesWindowKeyGesture),
                ShowManageDictionariesWindowKeyGesture);

        ShowManageFrequenciesWindowKeyGesture =
            WindowsUtils.SetKeyGesture(nameof(ShowManageFrequenciesWindowKeyGesture),
                ShowManageFrequenciesWindowKeyGesture);

        WindowsUtils.SetInputGestureText(mainWindow.AddNameButton, ShowAddNameWindowKeyGesture);
        WindowsUtils.SetInputGestureText(mainWindow.AddWordButton, ShowAddWordWindowKeyGesture);
        WindowsUtils.SetInputGestureText(mainWindow.SearchButton, SearchWithBrowserKeyGesture);
        WindowsUtils.SetInputGestureText(mainWindow.PreferencesButton, ShowPreferencesWindowKeyGesture);
        WindowsUtils.SetInputGestureText(mainWindow.ManageDictionariesButton, ShowManageDictionariesWindowKeyGesture);
        WindowsUtils.SetInputGestureText(mainWindow.ManageFrequenciesButton, ShowManageFrequenciesWindowKeyGesture);
        WindowsUtils.SetInputGestureText(mainWindow.StatsButton, ShowStatsKeyGesture);

        string? themeStr = ConfigurationManager.AppSettings.Get("Theme");
        if (themeStr is null)
        {
            themeStr = "Dark";
            AddToConfig("Theme", themeStr);
        }
        WindowsUtils.ChangeTheme(themeStr is "Dark" ? SkinType.Dark : SkinType.Default);

        string? ankiConnectUriStr = ConfigurationManager.AppSettings.Get(nameof(AnkiConnectUri));
        if (ankiConnectUriStr is null)
        {
            AddToConfig(nameof(AnkiConnectUri), AnkiConnectUri.OriginalString);
        }
        else if (Uri.TryCreate(ankiConnectUriStr, UriKind.Absolute, out Uri? ankiConnectUri))
        {
            AnkiConnectUri = ankiConnectUri;
        }
        else
        {
            Utils.Logger.Warning("Couldn't save AnkiConnect server address, invalid URL");
            Storage.Frontend.Alert(AlertLevel.Error, "Couldn't save AnkiConnect server address, invalid URL");
        }

        string? webSocketUriStr = ConfigurationManager.AppSettings.Get(nameof(WebSocketUri));
        if (webSocketUriStr is null)
        {
            AddToConfig(nameof(WebSocketUri), WebSocketUri.OriginalString);
        }
        else if (Uri.TryCreate(webSocketUriStr, UriKind.Absolute, out Uri? webSocketUri))
        {
            WebSocketUri = webSocketUri;
        }
        else
        {
            Utils.Logger.Warning("Couldn't save WebSocket address, invalid URL");
            Storage.Frontend.Alert(AlertLevel.Error, "Couldn't save WebSocket address, invalid URL");
        }
        WebSocketUtils.HandleWebSocket();

        string? searchUrlStr = ConfigurationManager.AppSettings.Get(nameof(SearchUrl));
        if (searchUrlStr is null)
        {
            AddToConfig(nameof(SearchUrl), SearchUrl);
        }
        else if (!Uri.IsWellFormedUriString(searchUrlStr.Replace("{SearchTerm}", ""), UriKind.Absolute))
        {
            Utils.Logger.Warning("Couldn't save Search URL, invalid URL");
            Storage.Frontend.Alert(AlertLevel.Error, "Couldn't save Search URL, invalid URL");
        }
        else
        {
            SearchUrl = searchUrlStr;
        }

        string? browserPathStr = ConfigurationManager.AppSettings.Get(nameof(BrowserPath));
        if (browserPathStr is null)
        {
            AddToConfig(nameof(BrowserPath), BrowserPath);
        }
        else if (!string.IsNullOrWhiteSpace(browserPathStr) && !Path.IsPathFullyQualified(browserPathStr))
        {
            Utils.Logger.Warning("Couldn't save Browser Path, invalid path");
            Storage.Frontend.Alert(AlertLevel.Error, "Couldn't save Browser Path, invalid path");
        }
        else
        {
            BrowserPath = browserPathStr;
        }

        string? mainWindowFontStr = ConfigurationManager.AppSettings.Get("MainWindowFont");
        if (mainWindowFontStr is null)
        {
            AddToConfig("MainWindowFont", "Meiryo");
            mainWindowFontStr = "Meiryo";
        }
        mainWindow.MainTextBox.FontFamily = new FontFamily(mainWindowFontStr);

        string? popupFlipStr = ConfigurationManager.AppSettings.Get("PopupFlip");
        if (popupFlipStr is null)
        {
            popupFlipStr = "Both";
            AddToConfig("PopupFlip", popupFlipStr);
        }

        switch (popupFlipStr)
        {
            case "X":
                PopupFlipX = true;
                PopupFlipY = false;
                break;

            case "Y":
                PopupFlipX = false;
                PopupFlipY = true;
                break;

            case "Both":
                PopupFlipX = true;
                PopupFlipY = true;
                break;

            default:
                PopupFlipX = true;
                PopupFlipY = true;
                break;
        }

        string? lookupModeStr = ConfigurationManager.AppSettings.Get("LookupMode");
        if (lookupModeStr is null)
        {
            lookupModeStr = "Hover";
            AddToConfig("LookupMode", lookupModeStr);
        }

        switch (lookupModeStr)
        {
            case "Hover":
                LookupOnLeftClickOnly = false;
                LookupOnSelectOnly = false;
                break;

            case "Click":
                LookupOnLeftClickOnly = true;
                LookupOnSelectOnly = false;
                break;

            case "Select":
                LookupOnLeftClickOnly = false;
                LookupOnSelectOnly = true;
                break;

            default:
                LookupOnLeftClickOnly = false;
                LookupOnSelectOnly = false;
                break;
        }

        string? popupFontStr = ConfigurationManager.AppSettings.Get(nameof(PopupFont));
        if (popupFontStr is null)
        {
            AddToConfig(nameof(PopupFont), PopupFont.Source);
        }
        else
        {
            PopupFont = new FontFamily(popupFontStr);
        }

        PopupWindow? currentPopupWindow = mainWindow.FirstPopupWindow;
        while (currentPopupWindow is not null)
        {
            currentPopupWindow.Background = PopupBackgroundColor;
            currentPopupWindow.Foreground = DefinitionsColor;
            currentPopupWindow.FontFamily = PopupFont;

            WindowsUtils.SetSizeToContentForPopup(PopupDynamicWidth, PopupDynamicHeight, WindowsUtils.DpiAwarePopupMaxHeight, WindowsUtils.DpiAwarePopupMaxWidth, currentPopupWindow);

            WindowsUtils.SetInputGestureText(currentPopupWindow.AddNameButton, ShowAddNameWindowKeyGesture);
            WindowsUtils.SetInputGestureText(currentPopupWindow.AddWordButton, ShowAddWordWindowKeyGesture);
            WindowsUtils.SetInputGestureText(currentPopupWindow.SearchButton, SearchWithBrowserKeyGesture);
            WindowsUtils.SetInputGestureText(currentPopupWindow.StatsButton, ShowStatsKeyGesture);

            currentPopupWindow = currentPopupWindow.ChildPopupWindow;
        }
    }

    public void LoadPreferences(PreferencesWindow preferenceWindow)
    {
        CreateDefaultAppConfig();

        MainWindow mainWindow = MainWindow.Instance;

        preferenceWindow.JLVersionTextBlock.Text = "v" + Storage.JLVersion;

        preferenceWindow.DisableHotkeysKeyGestureTextBox.Text = WindowsUtils.KeyGestureToString(DisableHotkeysKeyGesture);
        preferenceWindow.MiningModeKeyGestureTextBox.Text = WindowsUtils.KeyGestureToString(MiningModeKeyGesture);
        preferenceWindow.PlayAudioKeyGestureTextBox.Text = WindowsUtils.KeyGestureToString(PlayAudioKeyGesture);
        preferenceWindow.KanjiModeKeyGestureTextBox.Text = WindowsUtils.KeyGestureToString(KanjiModeKeyGesture);
        preferenceWindow.LookupKeyKeyGestureTextBox.Text = WindowsUtils.KeyGestureToString(LookupKeyKeyGesture);

        preferenceWindow.ShowManageDictionariesWindowKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ShowManageDictionariesWindowKeyGesture);
        preferenceWindow.ShowManageFrequenciesWindowKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ShowManageFrequenciesWindowKeyGesture);
        preferenceWindow.ShowPreferencesWindowKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ShowPreferencesWindowKeyGesture);
        preferenceWindow.ShowAddNameWindowKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ShowAddNameWindowKeyGesture);
        preferenceWindow.ShowAddWordWindowKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ShowAddWordWindowKeyGesture);
        preferenceWindow.SearchWithBrowserKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(SearchWithBrowserKeyGesture);
        preferenceWindow.MousePassThroughModeKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(MousePassThroughModeKeyGesture);
        preferenceWindow.InvisibleToggleModeKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(InvisibleToggleModeKeyGesture);
        preferenceWindow.SteppedBacklogBackwardsKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(SteppedBacklogBackwardsKeyGesture);
        preferenceWindow.SteppedBacklogForwardsKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(SteppedBacklogForwardsKeyGesture);
        preferenceWindow.InactiveLookupModeKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(InactiveLookupModeKeyGesture);
        preferenceWindow.MotivationKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(MotivationKeyGesture);
        preferenceWindow.ClosePopupKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ClosePopupKeyGesture);
        preferenceWindow.ShowStatsKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ShowStatsKeyGesture);
        preferenceWindow.NextDictKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(NextDictKeyGesture);
        preferenceWindow.PreviousDictKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(PreviousDictKeyGesture);
        preferenceWindow.AlwaysOnTopKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(AlwaysOnTopKeyGesture);
        preferenceWindow.TextOnlyVisibleOnHoverKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(TextOnlyVisibleOnHoverKeyGesture);
        preferenceWindow.TextBoxIsReadOnlyKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(TextBoxIsReadOnlyKeyGesture);
        preferenceWindow.CaptureTextFromClipboardKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(CaptureTextFromClipboardKeyGesture);
        preferenceWindow.CaptureTextFromWebSocketKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(CaptureTextFromWebSocketdKeyGesture);
        preferenceWindow.ReconnectToWebSocketServerKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(ReconnectToWebSocketServerKeyGesture);
        preferenceWindow.DeleteCurrentLineKeyGestureTextBox.Text =
            WindowsUtils.KeyGestureToString(DeleteCurrentLineKeyGesture);

        WindowsUtils.SetButtonColor(preferenceWindow.HighlightColorButton, HighlightColor);
        WindowsUtils.SetButtonColor(preferenceWindow.MainWindowBackgroundColorButton, mainWindow.Background.CloneCurrentValue());
        WindowsUtils.SetButtonColor(preferenceWindow.TextboxTextColorButton, MainWindowTextColor);
        WindowsUtils.SetButtonColor(preferenceWindow.TextboxBacklogTextColorButton, MainWindowBacklogTextColor);
        WindowsUtils.SetButtonColor(preferenceWindow.DeconjugationInfoColorButton, DeconjugationInfoColor);
        WindowsUtils.SetButtonColor(preferenceWindow.DefinitionsColorButton, DefinitionsColor);
        WindowsUtils.SetButtonColor(preferenceWindow.FrequencyColorButton, FrequencyColor);
        WindowsUtils.SetButtonColor(preferenceWindow.PrimarySpellingColorButton, PrimarySpellingColor);
        WindowsUtils.SetButtonColor(preferenceWindow.ReadingsColorButton, ReadingsColor);
        WindowsUtils.SetButtonColor(preferenceWindow.AlternativeSpellingsColorButton, AlternativeSpellingsColor);
        WindowsUtils.SetButtonColor(preferenceWindow.PopupBackgroundColorButton, PopupBackgroundColor);
        WindowsUtils.SetButtonColor(preferenceWindow.SeparatorColorButton, SeparatorColor);
        WindowsUtils.SetButtonColor(preferenceWindow.DictTypeColorButton, DictTypeColor);

        preferenceWindow.SearchUrlTextBox.Text = SearchUrl;
        preferenceWindow.BrowserPathTextBox.Text = BrowserPath;
        preferenceWindow.MaxSearchLengthNumericUpDown.Value = MaxSearchLength;
        preferenceWindow.AnkiUriTextBox.Text = AnkiConnectUri.OriginalString;
        preferenceWindow.WebSocketUriTextBox.Text = WebSocketUri.OriginalString;
        preferenceWindow.ForceSyncAnkiCheckBox.IsChecked = ForceSyncAnki;
        preferenceWindow.AllowDuplicateCardsCheckBox.IsChecked = AllowDuplicateCards;
        preferenceWindow.LookupRateNumericUpDown.Value = LookupRate;
        preferenceWindow.KanjiModeCheckBox.IsChecked = KanjiMode;
        preferenceWindow.AutoAdjustFontSizesOnResolutionChange.IsChecked = AutoAdjustFontSizesOnResolutionChange;
        preferenceWindow.HighlightLongestMatchCheckBox.IsChecked = HighlightLongestMatch;
        preferenceWindow.AutoPlayAudioCheckBox.IsChecked = AutoPlayAudio;
        preferenceWindow.CheckForJLUpdatesOnStartUpCheckBox.IsChecked = CheckForJLUpdatesOnStartUp;
        preferenceWindow.PrecachingCheckBox.IsChecked = Precaching;
        preferenceWindow.AlwaysOnTopCheckBox.IsChecked = AlwaysOnTop;
        preferenceWindow.RequireLookupKeyPressCheckBox.IsChecked = RequireLookupKeyPress;
        preferenceWindow.DisableHotkeysCheckBox.IsChecked = DisableHotkeys;
        preferenceWindow.FocusableCheckBox.IsChecked = Focusable;
        preferenceWindow.TextOnlyVisibleOnHoverCheckBox.IsChecked = TextOnlyVisibleOnHover;
        preferenceWindow.AnkiIntegrationCheckBox.IsChecked = AnkiIntegration;
        preferenceWindow.LookupRateNumericUpDown.Value = LookupRate;

        preferenceWindow.MainWindowDynamicWidthCheckBox.IsChecked = MainWindowDynamicWidth;
        preferenceWindow.MainWindowDynamicHeightCheckBox.IsChecked = MainWindowDynamicHeight;

        preferenceWindow.MainWindowMaxDynamicWidthNumericUpDown.Value = MainWindowMaxDynamicWidth;
        preferenceWindow.MainWindowMaxDynamicHeightNumericUpDown.Value = MainWindowMaxDynamicHeight;

        preferenceWindow.MainWindowHeightNumericUpDown.Value = MainWindowHeight;
        preferenceWindow.MainWindowWidthNumericUpDown.Value = MainWindowWidth;
        preferenceWindow.TextboxFontSizeNumericUpDown.Value = mainWindow.FontSizeSlider.Value;
        preferenceWindow.MainWindowOpacityNumericUpDown.Value = mainWindow.OpacitySlider.Value;

        preferenceWindow.ChangeMainWindowBackgroundOpacityOnUnhoverCheckBox.IsChecked = ChangeMainWindowBackgroundOpacityOnUnhover;
        preferenceWindow.MainWindowBackgroundOpacityOnUnhoverNumericUpDown.Value = MainWindowBackgroundOpacityOnUnhover;

        preferenceWindow.TextBoxIsReadOnlyCheckBox.IsChecked = TextBoxIsReadOnly;
        preferenceWindow.TextBoxTrimWhiteSpaceCharactersCheckBox.IsChecked = TextBoxTrimWhiteSpaceCharacters;
        preferenceWindow.TextBoxRemoveNewlinesCheckBox.IsChecked = TextBoxRemoveNewlines;
        preferenceWindow.TextBoxApplyDropShadowEffectCheckBox.IsChecked = TextBoxApplyDropShadowEffect;

        preferenceWindow.CaptureTextFromClipboardCheckBox.IsChecked = CaptureTextFromClipboard;
        preferenceWindow.CaptureTextFromWebSocketCheckBox.IsChecked = CaptureTextFromWebSocket;

        preferenceWindow.OnlyCaptureTextWithJapaneseCharsCheckBox.IsChecked = OnlyCaptureTextWithJapaneseChars;
        preferenceWindow.DisableLookupsForNonJapaneseCharsInMainWindowCheckBox.IsChecked = DisableLookupsForNonJapaneseCharsInMainWindow;
        preferenceWindow.MainWindowFocusOnHoverCheckBox.IsChecked = MainWindowFocusOnHover;
        preferenceWindow.SteppedBacklogWithMouseWheelCheckBox.IsChecked = SteppedBacklogWithMouseWheel;

        preferenceWindow.ThemeComboBox.SelectedValue = ConfigurationManager.AppSettings.Get("Theme");
        preferenceWindow.MinimumLogLevelComboBox.SelectedValue = ConfigurationManager.AppSettings.Get("MinimumLogLevel");

        preferenceWindow.MainWindowFontComboBox.ItemsSource = s_japaneseFonts;
        preferenceWindow.MainWindowFontComboBox.SelectedIndex = s_japaneseFonts.FindIndex(f =>
            f.Content.ToString() == mainWindow.MainTextBox.FontFamily.Source);

        if (preferenceWindow.MainWindowFontComboBox.SelectedIndex is -1)
        {
            preferenceWindow.MainWindowFontComboBox.SelectedIndex = 0;
        }


        preferenceWindow.PopupFontComboBox.ItemsSource = s_popupJapaneseFonts;
        preferenceWindow.PopupFontComboBox.SelectedIndex =
            s_popupJapaneseFonts.FindIndex(static f => f.Content.ToString() == PopupFont.Source);

        if (preferenceWindow.PopupFontComboBox.SelectedIndex is -1)
        {
            preferenceWindow.PopupFontComboBox.SelectedIndex = 0;
        }

        preferenceWindow.PopupMaxHeightNumericUpDown.Maximum = WindowsUtils.ActiveScreen.Bounds.Height;
        preferenceWindow.PopupMaxWidthNumericUpDown.Maximum = WindowsUtils.ActiveScreen.Bounds.Width;

        preferenceWindow.MaxNumResultsNotInMiningModeNumericUpDown.Value = MaxNumResultsNotInMiningMode;

        preferenceWindow.PopupMaxHeightNumericUpDown.Value = PopupMaxHeight;
        preferenceWindow.PopupMaxWidthNumericUpDown.Value = PopupMaxWidth;
        preferenceWindow.FixedPopupPositioningCheckBox.IsChecked = FixedPopupPositioning;
        preferenceWindow.FixedPopupXPositionNumericUpDown.Value = FixedPopupXPosition;
        preferenceWindow.FixedPopupYPositionNumericUpDown.Value = FixedPopupYPosition;
        preferenceWindow.PopupDynamicHeightCheckBox.IsChecked = PopupDynamicHeight;
        preferenceWindow.PopupDynamicWidthCheckBox.IsChecked = PopupDynamicWidth;

        preferenceWindow.AlternativeSpellingsFontSizeNumericUpDown.Value = AlternativeSpellingsFontSize;
        preferenceWindow.DeconjugationInfoFontSizeNumericUpDown.Value = DeconjugationInfoFontSize;
        preferenceWindow.DictTypeFontSizeNumericUpDown.Value = DictTypeFontSize;
        preferenceWindow.AutoHidePopupIfMouseIsNotOverItDelayInMillisecondsNumericUpDown.Value = AutoHidePopupIfMouseIsNotOverItDelayInMilliseconds;
        preferenceWindow.DefinitionsFontSizeNumericUpDown.Value = DefinitionsFontSize;
        preferenceWindow.FrequencyFontSizeNumericUpDown.Value = FrequencyFontSize;
        preferenceWindow.PrimarySpellingFontSizeNumericUpDown.Value = PrimarySpellingFontSize;
        preferenceWindow.ReadingsFontSizeNumericUpDown.Value = ReadingsFontSize;
        preferenceWindow.PopupOpacityNumericUpDown.Value = PopupBackgroundColor.Opacity * 100;
        preferenceWindow.PopupFocusOnLookupCheckBox.IsChecked = PopupFocusOnLookup;
        preferenceWindow.PopupXOffsetNumericUpDown.Value = PopupXOffset;
        preferenceWindow.PopupYOffsetNumericUpDown.Value = PopupYOffset;
        preferenceWindow.PopupFlipComboBox.SelectedValue = ConfigurationManager.AppSettings.Get("PopupFlip");

        preferenceWindow.LookupModeComboBox.SelectedValue = ConfigurationManager.AppSettings.Get("LookupMode");

        if (preferenceWindow.LookupModeComboBox.SelectedIndex is -1)
        {
            preferenceWindow.LookupModeComboBox.SelectedIndex = 0;
        }

        preferenceWindow.ShowMiningModeReminderCheckBox.IsChecked = ShowMiningModeReminder;
        preferenceWindow.DisableLookupsForNonJapaneseCharsInPopupsCheckBox.IsChecked = DisableLookupsForNonJapaneseCharsInPopups;
        preferenceWindow.HideDictButtonsWithNoResultsCheckBox.IsChecked = HideDictButtonsWithNoResults;
        preferenceWindow.AutoHidePopupIfMouseIsNotOverItCheckBox.IsChecked = AutoHidePopupIfMouseIsNotOverIt;
    }

    public async Task SavePreferences(PreferencesWindow preferenceWindow)
    {
        SaveKeyGesture(nameof(DisableHotkeysKeyGesture), preferenceWindow.DisableHotkeysKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(MiningModeKeyGesture), preferenceWindow.MiningModeKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(PlayAudioKeyGesture), preferenceWindow.PlayAudioKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(KanjiModeKeyGesture), preferenceWindow.KanjiModeKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(LookupKeyKeyGesture), preferenceWindow.LookupKeyKeyGestureTextBox.Text);

        SaveKeyGesture(nameof(ShowManageDictionariesWindowKeyGesture),
            preferenceWindow.ShowManageDictionariesWindowKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(ShowManageFrequenciesWindowKeyGesture),
            preferenceWindow.ShowManageFrequenciesWindowKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(ShowPreferencesWindowKeyGesture),
            preferenceWindow.ShowPreferencesWindowKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(ShowAddNameWindowKeyGesture),
            preferenceWindow.ShowAddNameWindowKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(ShowAddWordWindowKeyGesture),
            preferenceWindow.ShowAddWordWindowKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(SearchWithBrowserKeyGesture),
            preferenceWindow.SearchWithBrowserKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(MousePassThroughModeKeyGesture),
            preferenceWindow.MousePassThroughModeKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(InvisibleToggleModeKeyGesture),
            preferenceWindow.InvisibleToggleModeKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(SteppedBacklogBackwardsKeyGesture),
            preferenceWindow.SteppedBacklogBackwardsKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(SteppedBacklogForwardsKeyGesture),
            preferenceWindow.SteppedBacklogForwardsKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(InactiveLookupModeKeyGesture),
            preferenceWindow.InactiveLookupModeKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(MotivationKeyGesture),
            preferenceWindow.MotivationKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(ClosePopupKeyGesture),
            preferenceWindow.ClosePopupKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(ShowStatsKeyGesture),
            preferenceWindow.ShowStatsKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(NextDictKeyGesture),
            preferenceWindow.NextDictKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(PreviousDictKeyGesture),
            preferenceWindow.PreviousDictKeyGestureTextBox.Text);

        SaveKeyGesture(nameof(AlwaysOnTopKeyGesture),
            preferenceWindow.AlwaysOnTopKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(TextOnlyVisibleOnHoverKeyGesture),
            preferenceWindow.TextOnlyVisibleOnHoverKeyGestureTextBox.Text);

        SaveKeyGesture(nameof(TextBoxIsReadOnlyKeyGesture),
            preferenceWindow.TextBoxIsReadOnlyKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(CaptureTextFromClipboardKeyGesture),
            preferenceWindow.CaptureTextFromClipboardKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(CaptureTextFromWebSocketdKeyGesture),
            preferenceWindow.CaptureTextFromWebSocketKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(ReconnectToWebSocketServerKeyGesture),
            preferenceWindow.ReconnectToWebSocketServerKeyGestureTextBox.Text);
        SaveKeyGesture(nameof(DeleteCurrentLineKeyGesture),
            preferenceWindow.DeleteCurrentLineKeyGestureTextBox.Text);

        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        config.AppSettings.Settings[nameof(SearchUrl)].Value = preferenceWindow.SearchUrlTextBox.Text;

        config.AppSettings.Settings[nameof(BrowserPath)].Value = preferenceWindow.BrowserPathTextBox.Text;

        config.AppSettings.Settings[nameof(MaxSearchLength)].Value =
            preferenceWindow.MaxSearchLengthNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(AnkiConnectUri)].Value =
            preferenceWindow.AnkiUriTextBox.Text;

        config.AppSettings.Settings[nameof(WebSocketUri)].Value =
            preferenceWindow.WebSocketUriTextBox.Text;

        config.AppSettings.Settings[nameof(MainWindowDynamicWidth)].Value =
            preferenceWindow.MainWindowDynamicWidthCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(MainWindowDynamicHeight)].Value =
            preferenceWindow.MainWindowDynamicHeightCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(MainWindowMaxDynamicWidth)].Value =
            preferenceWindow.MainWindowMaxDynamicWidthNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(MainWindowMaxDynamicHeight)].Value =
            preferenceWindow.MainWindowMaxDynamicHeightNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings[nameof(MainWindowWidth)].Value =
            preferenceWindow.MainWindowWidthNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(MainWindowHeight)].Value =
            preferenceWindow.MainWindowHeightNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);

        // We want the opaque color here
        config.AppSettings.Settings["MainWindowBackgroundColor"].Value =
            preferenceWindow.MainWindowBackgroundColorButton.Background.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings[nameof(ChangeMainWindowBackgroundOpacityOnUnhover)].Value =
            preferenceWindow.ChangeMainWindowBackgroundOpacityOnUnhoverCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(MainWindowBackgroundOpacityOnUnhover)].Value =
            preferenceWindow.MainWindowBackgroundOpacityOnUnhoverNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(TextBoxIsReadOnly)].Value =
            preferenceWindow.TextBoxIsReadOnlyCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(TextBoxTrimWhiteSpaceCharacters)].Value =
            preferenceWindow.TextBoxTrimWhiteSpaceCharactersCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(TextBoxRemoveNewlines)].Value =
            preferenceWindow.TextBoxRemoveNewlinesCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(TextBoxApplyDropShadowEffect)].Value =
            preferenceWindow.TextBoxApplyDropShadowEffectCheckBox.IsChecked.ToString();


        config.AppSettings.Settings[nameof(CaptureTextFromClipboard)].Value =
            preferenceWindow.CaptureTextFromClipboardCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(CaptureTextFromWebSocket)].Value =
            preferenceWindow.CaptureTextFromWebSocketCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(OnlyCaptureTextWithJapaneseChars)].Value =
            preferenceWindow.OnlyCaptureTextWithJapaneseCharsCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(DisableLookupsForNonJapaneseCharsInMainWindow)].Value =
            preferenceWindow.DisableLookupsForNonJapaneseCharsInMainWindowCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(MainWindowFocusOnHover)].Value =
            preferenceWindow.MainWindowFocusOnHoverCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(SteppedBacklogWithMouseWheel)].Value =
    preferenceWindow.SteppedBacklogWithMouseWheelCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(MainWindowTextColor)].Value =
            preferenceWindow.TextboxTextColorButton.Tag.ToString();
        config.AppSettings.Settings[nameof(MainWindowBacklogTextColor)].Value =
            preferenceWindow.TextboxBacklogTextColorButton.Tag.ToString();
        config.AppSettings.Settings["MainWindowFontSize"].Value =
            preferenceWindow.TextboxFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["MainWindowOpacity"].Value =
            preferenceWindow.MainWindowOpacityNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["Theme"].Value =
            preferenceWindow.ThemeComboBox.SelectedValue.ToString();
        config.AppSettings.Settings["MinimumLogLevel"].Value =
            preferenceWindow.MinimumLogLevelComboBox.SelectedValue.ToString();
        config.AppSettings.Settings["MainWindowFont"].Value =
            preferenceWindow.MainWindowFontComboBox.SelectedValue.ToString();
        config.AppSettings.Settings[nameof(PopupFont)].Value =
            preferenceWindow.PopupFontComboBox.SelectedValue.ToString();

        config.AppSettings.Settings[nameof(KanjiMode)].Value =
            preferenceWindow.KanjiModeCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(ForceSyncAnki)].Value =
            preferenceWindow.ForceSyncAnkiCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(AllowDuplicateCards)].Value =
            preferenceWindow.AllowDuplicateCardsCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(LookupRate)].Value =
            preferenceWindow.LookupRateNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(AutoAdjustFontSizesOnResolutionChange)].Value =
            preferenceWindow.AutoAdjustFontSizesOnResolutionChange.IsChecked.ToString();
        config.AppSettings.Settings[nameof(HighlightLongestMatch)].Value =
            preferenceWindow.HighlightLongestMatchCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(AutoPlayAudio)].Value =
            preferenceWindow.AutoPlayAudioCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(Precaching)].Value =
            preferenceWindow.PrecachingCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(CheckForJLUpdatesOnStartUp)].Value =
            preferenceWindow.CheckForJLUpdatesOnStartUpCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(AlwaysOnTop)].Value =
            preferenceWindow.AlwaysOnTopCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(RequireLookupKeyPress)].Value =
            preferenceWindow.RequireLookupKeyPressCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(DisableHotkeys)].Value =
            preferenceWindow.DisableHotkeysCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(Focusable)].Value =
            preferenceWindow.FocusableCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(TextOnlyVisibleOnHover)].Value =
            preferenceWindow.TextOnlyVisibleOnHoverCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(AnkiIntegration)].Value =
            preferenceWindow.AnkiIntegrationCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(HighlightColor)].Value =
            preferenceWindow.HighlightColorButton.Tag.ToString();

        config.AppSettings.Settings[nameof(MaxNumResultsNotInMiningMode)].Value =
            preferenceWindow.MaxNumResultsNotInMiningModeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings[nameof(PopupMaxWidth)].Value =
            preferenceWindow.PopupMaxWidthNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(PopupMaxHeight)].Value =
            preferenceWindow.PopupMaxHeightNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(FixedPopupPositioning)].Value =
            preferenceWindow.FixedPopupPositioningCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(FixedPopupXPosition)].Value =
            preferenceWindow.FixedPopupXPositionNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(FixedPopupYPosition)].Value =
            preferenceWindow.FixedPopupYPositionNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(PopupDynamicHeight)].Value =
            preferenceWindow.PopupDynamicHeightCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(PopupDynamicWidth)].Value =
            preferenceWindow.PopupDynamicWidthCheckBox.IsChecked.ToString();

        // We want the opaque color here
        config.AppSettings.Settings[nameof(PopupBackgroundColor)].Value =
            preferenceWindow.PopupBackgroundColorButton.Background.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings[nameof(PrimarySpellingColor)].Value =
            preferenceWindow.PrimarySpellingColorButton.Tag.ToString();
        config.AppSettings.Settings[nameof(ReadingsColor)].Value =
            preferenceWindow.ReadingsColorButton.Tag.ToString();
        config.AppSettings.Settings[nameof(AlternativeSpellingsColor)].Value =
            preferenceWindow.AlternativeSpellingsColorButton.Tag.ToString();
        config.AppSettings.Settings[nameof(DefinitionsColor)].Value =
            preferenceWindow.DefinitionsColorButton.Tag.ToString();
        config.AppSettings.Settings[nameof(FrequencyColor)].Value =
            preferenceWindow.FrequencyColorButton.Tag.ToString();
        config.AppSettings.Settings[nameof(DeconjugationInfoColor)].Value =
            preferenceWindow.DeconjugationInfoColorButton.Tag.ToString();
        config.AppSettings.Settings["PopupOpacity"].Value =
            preferenceWindow.PopupOpacityNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(PrimarySpellingFontSize)].Value =
            preferenceWindow.PrimarySpellingFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(ReadingsFontSize)].Value =
            preferenceWindow.ReadingsFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(AlternativeSpellingsFontSize)].Value =
            preferenceWindow.AlternativeSpellingsFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(DefinitionsFontSize)].Value =
            preferenceWindow.DefinitionsFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(FrequencyFontSize)].Value =
            preferenceWindow.FrequencyFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(DeconjugationInfoFontSize)].Value =
            preferenceWindow.DeconjugationInfoFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(DictTypeFontSize)].Value =
            preferenceWindow.DictTypeFontSizeNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings[nameof(SeparatorColor)].Value =
            preferenceWindow.SeparatorColorButton.Tag.ToString();

        config.AppSettings.Settings[nameof(DictTypeColor)].Value =
            preferenceWindow.DictTypeColorButton.Tag.ToString();

        config.AppSettings.Settings[nameof(PopupFocusOnLookup)].Value =
            preferenceWindow.PopupFocusOnLookupCheckBox.IsChecked.ToString();
        config.AppSettings.Settings[nameof(PopupXOffset)].Value =
            preferenceWindow.PopupXOffsetNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings[nameof(PopupYOffset)].Value =
            preferenceWindow.PopupYOffsetNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["PopupFlip"].Value =
            preferenceWindow.PopupFlipComboBox.SelectedValue.ToString();

        config.AppSettings.Settings[nameof(ShowMiningModeReminder)].Value =
            preferenceWindow.ShowMiningModeReminderCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(DisableLookupsForNonJapaneseCharsInPopups)].Value =
            preferenceWindow.DisableLookupsForNonJapaneseCharsInPopupsCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(HideDictButtonsWithNoResults)].Value =
            preferenceWindow.HideDictButtonsWithNoResultsCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(AutoHidePopupIfMouseIsNotOverIt)].Value =
            preferenceWindow.AutoHidePopupIfMouseIsNotOverItCheckBox.IsChecked.ToString();

        config.AppSettings.Settings[nameof(AutoHidePopupIfMouseIsNotOverItDelayInMilliseconds)].Value =
            preferenceWindow.AutoHidePopupIfMouseIsNotOverItDelayInMillisecondsNumericUpDown.Value.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings["LookupMode"].Value =
            preferenceWindow.LookupModeComboBox.SelectedValue.ToString();

        MainWindow mainWindow = MainWindow.Instance;
        config.AppSettings.Settings["MainWindowTopPosition"].Value = mainWindow.Top.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["MainWindowLeftPosition"].Value = mainWindow.Left.ToString(CultureInfo.InvariantCulture);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");

        ApplyPreferences();

        if (preferenceWindow.SetAnkiConfig)
        {
            await preferenceWindow.SaveMiningSetup().ConfigureAwait(false);
        }
    }

    public static void SaveBeforeClosing()
    {
        CreateDefaultAppConfig();

        MainWindow mainWindow = MainWindow.Instance;

        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        config.AppSettings.Settings["MainWindowFontSize"].Value =
            mainWindow.FontSizeSlider.Value.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["MainWindowOpacity"].Value = mainWindow.OpacitySlider.Value.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings[nameof(MainWindowHeight)].Value = MainWindowHeight > mainWindow.MinHeight
            ? MainWindowHeight.ToString(CultureInfo.InvariantCulture)
            : mainWindow.MinHeight.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings[nameof(MainWindowWidth)].Value = MainWindowWidth > mainWindow.MinWidth
            ? MainWindowWidth.ToString(CultureInfo.InvariantCulture)
            : mainWindow.MinWidth.ToString(CultureInfo.InvariantCulture);

        config.AppSettings.Settings["MainWindowTopPosition"].Value = mainWindow.Top >= SystemParameters.VirtualScreenTop
            ? mainWindow.Top.ToString(CultureInfo.InvariantCulture)
            : "0";

        config.AppSettings.Settings["MainWindowLeftPosition"].Value = mainWindow.Left >= SystemParameters.VirtualScreenLeft
            ? mainWindow.Left.ToString(CultureInfo.InvariantCulture)
            : "0";

        // TODO: properties with public setters should be saved here?

        config.Save(ConfigurationSaveMode.Modified);
    }

    private static void CreateDefaultAppConfig()
    {
        string configPath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
        if (!File.Exists(configPath))
        {
            using (XmlWriter writer = XmlWriter.Create(configPath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("configuration");
                writer.WriteStartElement("appSettings");
                writer.WriteEndDocument();
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

    private delegate bool TryParseHandler<T>(string value, out T? result);
    private delegate bool TryParseHandlerWithCultureInfo<T>(string value, NumberStyles numberStyles, CultureInfo cultureInfo, out T result);

    private static T GetValueFromConfig<T>(T variable, string configKey, TryParseHandler<T> tryParseHandler) where T : struct
    {
        string? configValue = ConfigurationManager.AppSettings.Get(configKey);
        if (configValue is not null && tryParseHandler(configValue, out T value))
        {
            return value;
        }

        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if (ConfigurationManager.AppSettings.Get(configKey) is null)
        {
            config.AppSettings.Settings.Add(configKey, variable.ToString());
        }
        else
        {
            config.AppSettings.Settings[configKey].Value = variable.ToString();
        }

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");

        return variable;
    }

    private static T GetNumberWithDecimalPointFromConfig<T>(T number, string configKey, TryParseHandlerWithCultureInfo<T> tryParseHandler) where T : struct
    {
        string? configValue = ConfigurationManager.AppSettings.Get(configKey);
        if (configValue is not null && tryParseHandler(configValue, NumberStyles.Number, CultureInfo.InvariantCulture, out T value))
        {
            return value;
        }

        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if (ConfigurationManager.AppSettings.Get(configKey) is null)
        {
            config.AppSettings.Settings.Add(configKey, Convert.ToString(number, CultureInfo.InvariantCulture));
        }
        else
        {
            config.AppSettings.Settings[configKey].Value = Convert.ToString(number, CultureInfo.InvariantCulture);
        }

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");

        return number;
    }
    private static Brush GetBrushFromConfig(Brush solidColorBrush, string configKey)
    {
        Brush? brushFromConfig = null;
        string? configValue = ConfigurationManager.AppSettings.Get(configKey);

        if (configValue is not null)
        {
            brushFromConfig = WindowsUtils.BrushFromHex(configValue);
        }

        if (brushFromConfig is not null)
        {
            return brushFromConfig;
        }

        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if (ConfigurationManager.AppSettings.Get(configKey) is null)
        {
            config.AppSettings.Settings.Add(configKey, solidColorBrush.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            config.AppSettings.Settings[configKey].Value = solidColorBrush.ToString(CultureInfo.InvariantCulture);
        }

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");

        return solidColorBrush;
    }

    private static Brush GetFrozenBrushFromConfig(Brush solidColorBrush, string configKey)
    {
        Brush brush = GetBrushFromConfig(solidColorBrush, configKey);
        brush.Freeze();
        return brush;
    }

    private static void AddToConfig(string key, string value)
    {
        Configuration config =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings.Add(key, value);
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
    }

    private static void SaveKeyGesture(string key, string rawKeyGesture)
    {
        Configuration config =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        config.AppSettings.Settings[key].Value = rawKeyGesture.StartsWith("Win+", StringComparison.Ordinal)
            ? rawKeyGesture[4..]
            : rawKeyGesture;

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
    }
}
