using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;

namespace MotoMarket.Application.Dictionaries.Commands.ParameterTypes
{
    public record CreateParameterTypeCommand(string Name, string Unit, string InputType, string Category, bool IsRequired) : IRequest<int>;

    public class CreateParameterTypeCommandHandler : IRequestHandler<CreateParameterTypeCommand, int>
    {
        private readonly IApplicationDbContext _context;
        public CreateParameterTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<int> Handle(CreateParameterTypeCommand request, CancellationToken token)
        {
            var entity = new VehicleParameterType 
            { 
                Name = request.Name, 
                Unit = request.Unit,
                InputType = request.InputType,
                Category = request.Category,
                IsRequired = request.IsRequired,
                IsActive = true 
            };
            _context.VehicleParameterTypes.Add(entity);
            await _context.SaveChangesAsync(token);
            return entity.Id;
        }
    }
}

