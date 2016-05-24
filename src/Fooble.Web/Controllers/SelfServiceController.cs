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
        public ActionResult Register(string name)
        {
            var validationResult = Member.ValidateName(name);

            if (validationResult.IsInvalid)
            {
                ModelState.AddModelError(validationResult.ParamName, validationResult.Message);

                return View(SelfServiceRegister.ViewModel.Make(name));
            }

            var id = _keyGenerator.GenerateKey();
            var command = SelfServiceRegister.Command.Make(id, name);
            _mediator.Send(command);

            return RedirectToAction("Detail", "Member", new { id = id.ToString() });
        }
    }
}
