using System;

public class DataSavePrefsProvider : IDataSaveProvider
{
    private DataSave _dataSave;
    private SystemSave _systemSave;
    
    private readonly SaveManager<DataSave> _saveManager = new("game_save");
    private readonly SaveManager<SystemSave> _saveManagerSystemSave = new("system_save");

    public Action<EndRoundFadeEnum> OnSaveAnalyticsCallback { get; set; }
    
    public DataSave Get() => _dataSave ??= new DataSave();
    public SystemSave GetSystemSave() => _systemSave ??= _saveManagerSystemSave.Load();

    public void SaveSystemSave()
    {
        var result =_saveManagerSystemSave.Save(_systemSave);
    }

    public void Save(int slot, bool sendAnalytics, EndRoundFadeEnum endRoundType)
    {
        _dataSave ??= new DataSave();
        _dataSave.timeSinceStart = G.I.GameTimeManager.GetCurrentTime();
        var result =_saveManager.Save(_dataSave);
        
        if(sendAnalytics)
            OnSaveAnalyticsCallback?.Invoke(endRoundType);
    }

    public void CreateNew()
    {
        _dataSave = new DataSave();
        _dataSave.Validate();
    }

    public void ResetSystemSave()
    {
        _systemSave = new SystemSave();
        _saveManagerSystemSave.Delete();
    }

    public void ApplyFromSave(int slot)
    {
        _dataSave = _saveManager.Load(slot);
        _dataSave.Validate();
    }

    public bool IsExist()
    {
        return _saveManager.Exists();
    }
}
