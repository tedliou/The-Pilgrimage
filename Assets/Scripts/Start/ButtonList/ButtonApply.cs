public class ButtonApply : GameButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        SaveSetting();
    }
    

    private void SaveSetting()
    {
        SettingManager.Instance.Save();
    }
}