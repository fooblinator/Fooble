using Fooble.Core;
using MediatR;
using System;
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
                throw new ArgumentNullException(nameof(mediator), "Mediator parameter was null");
            }

            if (keyGenerator == null)
            {
                throw new ArgumentNullException(nameof(keyGenerator), "Key generator parameter was null");
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
            _mediator.Send(command);

            return RedirectToAction("Detail", "Member", new { id = id.ToString() });
        }
    }
}
