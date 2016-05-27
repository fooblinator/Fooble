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
                throw new ArgumentNullException(nameof(mediator), "Mediator is required");

            if (keyGenerator == null)
                throw new ArgumentNullException(nameof(keyGenerator), "Key generator is required");

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
            Member.ValidateUsername(username)
                .AddModelErrorIfNotValid(ModelState);

            Member.ValidateName(name)
                .AddModelErrorIfNotValid(ModelState);

            if (!ModelState.IsValid)
                return View(SelfServiceRegister.ViewModel.Make(username, name));

            var id = _keyGenerator.GenerateKey();
            var command = SelfServiceRegister.Command.Make(id, username, name);
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result parameter was null");

            result.AddModelErrorIfNotSuccess(ModelState);

            if (!ModelState.IsValid)
                return View(SelfServiceRegister.ViewModel.Make(username, name));

            return RedirectToAction("Detail", "Member", new { id = id.ToString() });
        }
    }
}
