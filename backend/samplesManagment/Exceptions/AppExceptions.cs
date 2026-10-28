namespace samplesManagment.Exceptions;

// 404
public class NotFoundException(string message) : Exception(message);

// 409: regla de negocio no cumplida
public class ConflictException(string message) : Exception(message);

// 400: error de validación asociado a un campo
public class BadRequestException(string field, string message) : Exception(message)
{
    public string Field { get; } = field;
}
