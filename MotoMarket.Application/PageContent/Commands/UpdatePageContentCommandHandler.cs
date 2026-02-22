using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.PageContent.Commands
{
    public class UpdatePageContentCommandHandler : IRequestHandler<UpdatePageContentCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdatePageContentCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdatePageContentCommand request, CancellationToken token)
        {
            var entity = await _context.PageContents.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                entity.Content = request.Content;
                entity.LastModified = DateTime.UtcNow;
                await _context.SaveChangesAsync(token);
            }
        }
    }
}
