using Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;
    
    
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        
        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        
        // Throw our custom exception if there are any validation failures
        if(failures.Count != 0)
        {
            // We will throw our own custom validation exception
            throw new CustomValidationException(failures);
        }
        
        
        return await next();
            
    }
}