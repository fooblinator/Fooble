using Fooble.Core;
using MediatR;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Fooble.Web.Controllers
{
    public class SelfServiceController : Controller
    {
        readonly IMediator _mediator;

        public SelfServiceController(IMediator mediator)
        {
            // TODO: create key generator service and pass it in as dependency

            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator), "Mediator should not be null");

            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult Register()
        {
            var readModel = SelfServiceRegister.ReadModel.Empty;

            return View(readModel);
        }

        [HttpPost]
        public ActionResult Register(string name)
        {
            // TODO: need to modify the view pages to utilize the message display models appropriately

            var validationResult = Member.ValidateName(name);

            if (validationResult.IsInvalid)
            {
                ModelState.AddModelError(validationResult.ParamName, validationResult.Message);

                var readModel = SelfServiceRegister.ReadModel.Make(name);

                return View(readModel);
            }

            // TODO: utilize key generator service to generate id

            var id = Guid.NewGuid();
            var command = SelfServiceRegister.Command.Make(id, name);
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsDuplicateId)
            {
                return View("Register_DuplicateId", result.ToMessageDisplayReadModel());
            }

            return RedirectToAction("Detail", "Member", new { id = id.ToString() });
        }
    }
}
