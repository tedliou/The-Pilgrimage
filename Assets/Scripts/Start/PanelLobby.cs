public class PanelLobby : GamePanel
{
    protected override void OnPanelAction(PanelOption option)
    {
        base.OnPanelAction(option);
        SetDisplay(option);
    }

    private void SetDisplay(PanelOption option)
    {
        gameObject.SetActive(option == PanelOption.Lobby);
    }
}