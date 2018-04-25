using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Macaria.Infrastructure.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Macaria.API.Features.Notes
{
    public class GetGetNoteBySlugQuery
    {
        public class Request : IRequest<Response> {
            public string Slug { get; set; }
        }

        public class Response
        {
            public NoteApiModel Note { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public IMacariaContext _context { get; set; }
            public Handler(IMacariaContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                => new Response()
                {
                    Note = NoteApiModel.FromNote(await _context.Notes
                    .Where(x =>x.Slug == request.Slug)
                    .SingleAsync())
                };
        }
    }
}