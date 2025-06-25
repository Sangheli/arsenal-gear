using System;

public interface IDataSaveProvider
{
    Action OnSaveCallback { set; }
    DataSave Get();
    SystemSave GetSystemSave();
    void Save(int slot);
    void SaveSystemSave();
    void CreateNew();
    void ResetSystemSave();
    void ApplyFromSave(int slot);
    bool IsExist();
}
