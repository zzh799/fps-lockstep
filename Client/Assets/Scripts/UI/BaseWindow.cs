using FairyGUI;


public abstract class BaseWindow : Window
{
    private WindowDefine Define;

    protected BaseWindow()
    {

    }

    public void SetInfo(WindowDefine info)
    {
        Define = info;
    }

    protected override void OnInit()
    {
        base.OnInit();
        contentPane = UIPackage.CreateObject(Define.packckageName, Define.resName).asCom;
    }

    protected override void OnHide()
    {
        base.OnHide();
        if (!Define.Cache)
        {
            Define.Instance = null;
            Dispose();
        }
    }

}
