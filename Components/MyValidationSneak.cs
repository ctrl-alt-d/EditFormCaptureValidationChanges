using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MyComponents
{
    public class MyValidationSneak: ComponentBase, IDisposable
    {
        [CascadingParameter] EditContext CurrentEditContext { get; set; }
        [Parameter] public Action<EditContext> UpdateDelegate { get; set; }

        private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;
        public MyValidationSneak()
        {
            _validationStateChangedHandler = (sender, eventArgs) => GoUpdate();
        }

        private void GoUpdate() => UpdateDelegate(CurrentEditContext);

        private EditContext _previousEditContext;
        protected override void OnParametersSet()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(ValidationSummary)} requires a cascading parameter " +
                    $"of type {nameof(EditContext)}. For example, you can use {nameof(ValidationSummary)} inside " +
                    $"an {nameof(EditForm)}.");
            }

            if (CurrentEditContext != _previousEditContext)
            {
                DetachValidationStateChangedListener();
                CurrentEditContext.OnValidationStateChanged += _validationStateChangedHandler;
                GoUpdate();
                _previousEditContext = CurrentEditContext;
            }
        }

        protected virtual void Dispose(bool disposing) {}

        void IDisposable.Dispose()
        {
            DetachValidationStateChangedListener();
            this.Dispose(disposing: true);
        }
        private void DetachValidationStateChangedListener()
        {
            if (_previousEditContext != null)
            {
                _previousEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
                GoUpdate();
           }
        }                
    }

}