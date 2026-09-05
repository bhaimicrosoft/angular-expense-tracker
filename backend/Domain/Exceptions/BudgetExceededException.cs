namespace Domain.Exceptions;

public class BudgetExceededException : DomainException
{
    public BudgetExceededException(string category) : base($"The expense exceeds the allocated budget for {category}")
    {
    }
}