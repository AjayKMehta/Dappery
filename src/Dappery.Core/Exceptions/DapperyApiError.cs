namespace Dappery.Core.Exceptions
{
    public class DapperyApiError
    {
        public DapperyApiError(string errorMessage, string propertyName)
        {
            this.ErrorMessage = errorMessage;
            this.PropertyName = propertyName;
        }

        public string ErrorMessage { get; }

        public string PropertyName { get; }
    }
}
