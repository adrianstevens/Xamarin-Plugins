using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace SecureLocalStorage
{
    class SecureLocalStorageIOS : SecureStorage
    {
        public override Task<bool> LoadAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}