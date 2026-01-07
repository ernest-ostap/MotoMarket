using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.PageContent.Queries.GetAllPageContent
{
    public record GetAllPageContentsQuery : IRequest<IEnumerable<PageContentDto>>;
}
