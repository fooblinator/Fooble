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
            {
                throw new ArgumentNullException(nameof(mediator), "Mediator is required");
            }

            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            var validationResult = Member.ValidateId(id);

            if (validationResult.IsInvalid)
            {
                return View("MessageDisplay", validationResult.ToMessageDisplayReadModel());
            }

            var actualId = Guid.Parse(id);
            var query = MemberDetail.Query.Make(actualId);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
            {
                return View("MessageDisplay", result.ToMessageDisplayReadModel());
            }

            return View(result.ReadModel);
        }

        [HttpGet]
        public ActionResult List()
        {
            var query = MemberList.Query.Make();
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
            {
                return View("MessageDisplay", result.ToMessageDisplayReadModel());
            }

            return View(result.ReadModel);
        }
    }
}
