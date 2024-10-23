using HotChocolate.AspNetCore;
using HotChocolate.Execution;

public sealed class SessionHttpIntercetpor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        OperationRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        // The cursomterid is currently hardcoded
        requestBuilder.SetGlobalState("customerId", "customerId");

        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}