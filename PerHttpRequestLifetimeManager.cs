namespace Unity.LifetimeManager
{
    using System;
    using System.Web;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// Implements a Unity LifetimeManager to manage the lifecycle of a stateless http request.
    /// </summary>
    /// <remarks>This LifetimeManager disposes the resolved type after the http request has been ended.
    /// This LifetimeManager is not thread safe.</remarks>
    public class PerHttpRequestLifetimeManager : LifetimeManager
    {
        private readonly Guid key = Guid.NewGuid();

        public override void RemoveValue()
        {
            var obj = this.GetValue();
            HttpContext.Current.Items.Remove(obj);

            var disposable = obj as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public override object GetValue()
        {
            return HttpContext.Current.Items[this.key];
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[this.key] = newValue;
            HttpContext.Current.AddOnRequestCompleted(delegate { this.RemoveValue(); });
        }
    }
}
