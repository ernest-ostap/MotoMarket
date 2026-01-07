using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.PageContent.Queries.GetAllPageContent
{
    public class GetAllPageContentsQueryHandler : IRequestHandler<GetAllPageContentsQuery, IEnumerable<PageContentDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllPageContentsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<PageContentDto>> Handle(GetAllPageContentsQuery request, CancellationToken token)
        {
            return await _context.PageContents.AsNoTracking()
                .Select(x => new PageContentDto
                {
                    Id = x.Id,
                    PageKey = x.PageKey,
                    Content = x.Content,
                    LastModified = x.LastModified
                })
                .ToListAsync(token);
        }
    }
}
