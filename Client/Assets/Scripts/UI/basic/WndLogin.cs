using FairyGUI;
using FairyGUI.Utils;
using UnityEditor.UI;

namespace basic
{
    public partial class WindowLogin:BaseWindow
    {
        WndLogin content =>  (WndLogin) contentPane;

        protected override void OnInit()
        {
            base.OnInit();
            // BindServiceEvent();
            BindUIEvent();
        }

        private void BindUIEvent()
        {
            content.btnLogin.onClick.Add(onclick_btnLogin);
        }

        private void onclick_btnLogin(EventContext context)
        {
            
        }
    }
}