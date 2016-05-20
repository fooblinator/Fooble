using Fooble.Core;
using MediatR;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Fooble.Web.Controllers
{
    public class MemberController : Controller
    {
        readonly IMediator _mediator;

        public MemberController(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator), "Mediator should not be null");

            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult Detail(Guid id)
        {
            // TODO: handle invalid ids; prevent argument exceptions; redirect to message display

            var query = MemberDetail.MakeQuery(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
            {
                return View("Detail_NotFound", result.ToMessageDisplayReadModel());
            }

            return View(result.ReadModel);
        }

        [HttpGet]
        public ActionResult List()
        {
            var query = MemberList.MakeQuery();
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
            {
                return View("List_NotFound", result.ToMessageDisplayReadModel());
            }

            return View(result.ReadModel);
        }
    }
}
