using System;

namespace GMLoaded.Source.Interaces
{
    public interface IAppSystem
    {
        /// <param name='factory'></param>
        Boolean Connect(IntPtr factory);

        void Disconnect();

        /// <param name='pInterfaceName'></param>
        IntPtr QueryInterface(String pInterfaceName);

        IntPtr Init();

        void Shutdown();
    }
}
