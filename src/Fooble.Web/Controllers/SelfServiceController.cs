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
        readonly KeyGenerator _keyGenerator;

        public SelfServiceController(IMediator mediator, KeyGenerator keyGenerator)
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
        public ActionResult Register([ModelBinder(typeof(FoobleModelBinder))] ISelfServiceRegisterViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var id = _keyGenerator.Invoke();
            var command = viewModel.ToCommand(id);
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            result.AddModelErrorIfNotSuccess(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id.ToString() });
        }
    }
}
