/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace basic
{
    public partial class WndLogin : GComponent
    {
        public Controller ctrlState;
        public GComponent FullWindow;
        public GTextInput txtUserName;
        public GTextInput txtPassword;
        public GButton btnLogin;
        public GButton btnRegister;
        public GTextInput txtPassword2;
        public GButton btnReturnLogin;
        public GButton btnRegisterConfirm;
        public const string URL = "ui://93tyfp33v3wk0";

        public static WndLogin CreateInstance()
        {
            return (WndLogin)UIPackage.CreateObject("basic", "WndLogin");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlState = GetController("ctrlState");
            FullWindow = (GComponent)GetChild("FullWindow");
            txtUserName = (GTextInput)GetChild("txtUserName");
            txtPassword = (GTextInput)GetChild("txtPassword");
            btnLogin = (GButton)GetChild("btnLogin");
            btnRegister = (GButton)GetChild("btnRegister");
            txtPassword2 = (GTextInput)GetChild("txtPassword2");
            btnReturnLogin = (GButton)GetChild("btnReturnLogin");
            btnRegisterConfirm = (GButton)GetChild("btnRegisterConfirm");
        }
    }
}