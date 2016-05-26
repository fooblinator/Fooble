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
            {
                throw new ArgumentNullException(nameof(mediator), "Mediator is required");
            }

            if (keyGenerator == null)
            {
                throw new ArgumentNullException(nameof(keyGenerator), "Key generator is required");
            }

            _mediator = mediator;
            _keyGenerator = keyGenerator;
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(SelfServiceRegister.ViewModel.Empty);
        }

        [HttpPost]
        public ActionResult Register(string username, string name)
        {
            var usernameResult = Member.ValidateUsername(username);

            if (usernameResult.IsInvalid)
            {
                ModelState.AddModelError(usernameResult.ParamName, usernameResult.Message);
            }

            var nameResult = Member.ValidateName(name);

            if (nameResult.IsInvalid)
            {
                ModelState.AddModelError(nameResult.ParamName, nameResult.Message);
            }

            if (usernameResult.IsInvalid || nameResult.IsInvalid)
            {
                return View(SelfServiceRegister.ViewModel.Make(username, name));
            }

            var id = _keyGenerator.GenerateKey();
            var command = SelfServiceRegister.Command.Make(id, username, name);
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsUsernameUnavailable)
            {
                return View("MessageDisplay", result.ToMessageDisplayReadModel());
            }

            return RedirectToAction("Detail", "Member", new { id = id.ToString() });
        }
    }
}
