using FairyGUI;
using FairyGUI.Utils;
using Service;
using UnityEditor.UI;

namespace basic
{
    public partial class WindowLogin : BaseWindow
    {
        WndLogin content => (WndLogin) contentPane;

        protected override void OnInit()
        {
            base.OnInit();
            // BindServiceEvent();
            BindUIEvent();
        }

        private void BindUIEvent()
        {
            content.btnLogin.onClick.Add(onclick_btnLogin);
            content.btnRegister.onClick.Add(onclick_btnRegister);
            content.btnReturnLogin.onClick.Add(onclick_btnReturnLogin);
            content.btnRegisterConfirm.onClick.Add(onclick_btnRegisterConfirm);
        }


        private void onclick_btnLogin(EventContext context)
        {
            UserService.Instance.UserLogin(content.txtUserName.text, content.txtPassword.text);
        }


        private void onclick_btnRegister(EventContext context)
        {
            content.ctrlState.selectedIndex = 1;
        }

        private void onclick_btnReturnLogin(EventContext context)
        {
            content.ctrlState.selectedIndex = 0;
        }


        private void onclick_btnRegisterConfirm(EventContext context)
        {
            UserService.Instance.UserRegister(content.txtUserName.text, content.txtPassword.text);
        }
    }
}