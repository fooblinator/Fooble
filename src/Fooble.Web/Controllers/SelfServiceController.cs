using Fooble.Core;
using Fooble.Presentation;
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
            return View(SelfServiceRegisterViewModel.Empty);
        }

        [HttpPost]
        public ActionResult Register(string username, string email, string nickname)
        {
            Member.ValidateUsername(username)
                .AddModelErrorIfNotValid(ModelState);

            Member.ValidateEmail(email)
                .AddModelErrorIfNotValid(ModelState);

            Member.ValidateNickname(nickname)
                .AddModelErrorIfNotValid(ModelState);

            if (!ModelState.IsValid)
                return View(SelfServiceRegisterViewModel.Make(username, email, nickname));

            var id = _keyGenerator.GenerateKey();
            var command = SelfServiceRegisterCommand.Make(id, username, email, nickname);
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result parameter was null");

            result.AddModelErrorIfNotSuccess(ModelState);

            if (!ModelState.IsValid)
                return View(SelfServiceRegisterViewModel.Make(username, email, nickname));

            return RedirectToAction("Detail", "Member", new { id = id.ToString() });
        }
    }
}
