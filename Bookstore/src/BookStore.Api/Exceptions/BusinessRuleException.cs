namespace BookStore.Api.Exceptions;

public class BusinessRuleException(string message)
    : Exception(message);