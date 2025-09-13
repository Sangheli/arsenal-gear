using System;

public interface IDataSaveProvider
{
    Action<EndRoundFadeEnum> OnSaveAnalyticsCallback { set; }
    DataSave Get();
    SystemSave GetSystemSave();
    void Save(int slot, bool sendAnalytics, EndRoundFadeEnum endRoundType);
    void SaveSystemSave();
    void CreateNew();
    void ResetSystemSave();
    void ApplyFromSave(int slot);
    bool IsExist();
}
