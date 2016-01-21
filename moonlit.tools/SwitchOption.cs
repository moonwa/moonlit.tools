using System;

namespace Moonlit.Tools
{
    public enum SwitchOption
    {
         None,
         Yes,
         No
    }

    public static class SwitchOptionHelper
    {
        public static bool ToBoolean(this SwitchOption option)
        {
            switch (option)
            {
                case SwitchOption.None:
                    throw new Exception("option cannot be none");
                case SwitchOption.Yes:
                    return true;
                case SwitchOption.No:
                    return false;
                default:
                    throw new Exception("option cannot be other");
            }
        }
    }
}