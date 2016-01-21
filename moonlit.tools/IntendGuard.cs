using System;

namespace Moonlit.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class IntendGuard : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IIntend _intend;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntendGuard"/> class.
        /// </summary>
        /// <param name="intend">The intend.</param>
        public IntendGuard(IIntend intend)
        {
            _intend = intend;
            _intend.Intend();
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _intend.Deintend();
        }

        #endregion
    }
}