using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.ParameterTypes
{
    public record UpdateParameterTypeCommand(int Id, string Name, string Unit, string InputType, string Category, bool IsRequired) : IRequest;

    public class UpdateParameterTypeCommandHandler : IRequestHandler<UpdateParameterTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateParameterTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateParameterTypeCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleParameterTypes.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                entity.Name = request.Name;
                entity.Unit = request.Unit;
                entity.InputType = request.InputType;
                entity.Category = request.Category;
                entity.IsRequired = request.IsRequired;
                await _context.SaveChangesAsync(token);
            }
        }
    }
}

