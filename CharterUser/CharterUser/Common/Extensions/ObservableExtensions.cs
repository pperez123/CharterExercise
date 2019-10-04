using System;
using System.Reactive.Disposables;

namespace CharterUser.Common.Extensions
{
    public static class ObservableExtensions
    {
        public static T AddToDisposable<T>(this T source, CompositeDisposable container) where T : IDisposable
        {
            container.Add(source);
            return source;
        }
    }
}