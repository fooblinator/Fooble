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
        public ActionResult Detail(string id)
        {
            var query = MemberDetailQuery.Make(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result should not be null");

            if (result.IsFailure)
            {
                Debug.Assert(result.Status.IsNotFound, "Result status should be not found");
                return View("Detail_NotFound");
            }

            return View(result.Value);
        }

        [HttpGet]
        public ActionResult List()
        {
            var query = MemberListQuery.Make();
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result should not be null");

            if (result.IsFailure)
            {
                Debug.Assert(result.Status.IsNotFound, "Result status should be not found");
                return View("List_NotFound");
            }

            return View(result.Value);
        }
    }
}
