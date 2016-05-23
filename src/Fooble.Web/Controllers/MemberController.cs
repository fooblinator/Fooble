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
            // TODO: need to modify the view pages to utilize the message display models appropriately

            var validationResult = Member.ValidateId(id);

            if (validationResult.IsInvalid)
            {
                return View("Detail_InvalidId", validationResult.ToMessageDisplayReadModel());
            }

            var actualId = Guid.Parse(id);
            var query = MemberDetail.Query.Make(actualId);
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
            // TODO: need to modify the view pages to utilize the message display models appropriately

            var query = MemberList.Query.Make();
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
