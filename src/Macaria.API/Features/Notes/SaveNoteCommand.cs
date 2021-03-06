using FluentValidation;
using Macaria.Core.Models;
using Macaria.Core.Extensions;
using Macaria.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System;
using Macaria.Core.DomainEvents;

namespace Macaria.API.Features.Notes
{
    public class SaveNoteCommand
    {
        public class Validator: AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(request => request.Note.NoteId).NotNull();
                RuleFor(request => request.Note.Body).NotNull().NotEmpty();
            }
        }

        public class Request : IRequest<Response> {
            public NoteDto Note { get; set; }
        }

        public class Response
        {			
            public Guid NoteId { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var note = await _context.Notes
                    .Include(x => x.NoteTags)
                    .ThenInclude(x => x.Tag)
                    .SingleOrDefaultAsync(x => request.Note.NoteId == x.NoteId);

                if (note == null) _context.Notes.Add(note = new Note());

                note.Body = request.Note.Body;

                note.Title = request.Note.Title;

                note.Slug = request.Note.Title.ToSlug();
                
                note.NoteTags.Clear();

                foreach(var tag in request.Note.Tags)
                {
                    note.NoteTags.Add(new NoteTag()
                    {
                        Tag = (await _context.Tags.FindAsync(tag.TagId))
                    });
                }

                note.RaiseDomainEvent(new NoteSaved(note.NoteId));

                await _context.SaveChangesAsync(cancellationToken);

                return new Response() { NoteId = note.NoteId };
            }
        }
    }
}
