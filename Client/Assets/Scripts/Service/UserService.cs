using System;
using Common;
using Network;
using Pb;

namespace Service
{
    public class UserService:Singleton<UserService>,IDisposable
    {
        public UserService()
        {
            MessageDistributor<Session>.Instance.Subscribe<UserLoginResponse>(OnUserLogin);
            MessageDistributor<Session>.Instance.Subscribe<UserRegisterResponse>(OnUserRegister);
        }
        
        public void Dispose()
        {
            MessageDistributor<Session>.Instance.Unsubscribe<UserLoginResponse>(OnUserLogin);
            MessageDistributor<Session>.Instance.Unsubscribe<UserRegisterResponse>(OnUserRegister);
        }
        

        public void UserLogin(string userName, string password)
        {

        }
        
        private void OnUserRegister(Session sender, UserRegisterResponse message)
        {
            throw new System.NotImplementedException();
        }

        public void UserRegister(string userName, string password)
        {
            
        }
        
        private void OnUserLogin<UserLoginResponse>(Session sender, UserLoginResponse message)
        {
            throw new System.NotImplementedException();

        }


    }
}