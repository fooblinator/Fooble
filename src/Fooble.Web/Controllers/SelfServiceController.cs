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
        readonly IKeyGenerator _keyGenerator;

        public SelfServiceController(IMediator mediator, IKeyGenerator keyGenerator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator), "Mediator was be null");

            if (keyGenerator == null)
                throw new ArgumentNullException(nameof(keyGenerator), "Key generator was null");

            _mediator = mediator;
            _keyGenerator = keyGenerator;
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

            var id = _keyGenerator.GenerateKey();
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
