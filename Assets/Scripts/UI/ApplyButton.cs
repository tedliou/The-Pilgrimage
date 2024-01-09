public class ApplyButton : GameButton
{
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        SaveSetting();
        ButtonClickSFX.Instance.Play();
    }
    

    private void SaveSetting()
    {
        SettingManager.Instance.Save();
    }
}