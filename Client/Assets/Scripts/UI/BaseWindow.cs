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

        GObject fullWindow = contentPane.GetChild("FullWindow");
        if (fullWindow!=null)
        {
            //设置全屏窗口
            AddRelation(GRoot.inst, RelationType.Size);
            x = (GRoot.inst.width - width) / 2;
        }


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
