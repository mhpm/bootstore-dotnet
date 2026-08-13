namespace BookStore.Application.Exceptions;

public class BusinessRuleException(string message)
    : Exception(message);