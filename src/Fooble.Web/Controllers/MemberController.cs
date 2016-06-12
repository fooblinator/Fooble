using Fooble.Core;
using Fooble.Presentation;
using MediatR;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Fooble.Web.Controllers
{
    public class MemberController : Controller
    {
        readonly IMediator _mediator;
        readonly KeyGenerator _keyGenerator;

        public MemberController(IMediator mediator, KeyGenerator keyGenerator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator), "Mediator is required");

            if (keyGenerator == null)
                throw new ArgumentNullException(nameof(keyGenerator), "Key generator is required");

            _mediator = mediator;
            _keyGenerator = keyGenerator;
        }

        [HttpGet]
        public ActionResult ChangeEmail(Guid id)
        {
            var query = MemberExistsQuery.Make(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel("Change Email"));

            return View(MemberChangeEmailViewModel.Make(id));
        }

        [HttpPost]
        public ActionResult ChangeEmail(Guid id,
            [ModelBinder(typeof(FoobleModelBinder))] IMemberChangeEmailViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.ToCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult ChangeOther(Guid id)
        {
            var query = MemberExistsQuery.Make(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel("Change Other"));

            return View(MemberChangeOtherViewModel.Make(id));
        }

        [HttpPost]
        public ActionResult ChangeOther(Guid id,
            [ModelBinder(typeof(FoobleModelBinder))] IMemberChangeOtherViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel);

            var command = viewModel.ToCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel());

            if (!ModelState.IsValid) return View(viewModel);

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult ChangePassword(Guid id)
        {
            var query = MemberExistsQuery.Make(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel("Change Password"));

            return View(MemberChangePasswordViewModel.Make(id));
        }

        [HttpPost]
        public ActionResult ChangePassword(Guid id,
            [ModelBinder(typeof(FoobleModelBinder))] IMemberChangePasswordViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.ToCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult ChangeUsername(Guid id)
        {
            var query = MemberExistsQuery.Make(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel("Change Username"));

            return View(MemberChangeUsernameViewModel.Make(id));
        }

        [HttpPost]
        public ActionResult ChangeUsername(Guid id,
            [ModelBinder(typeof(FoobleModelBinder))] IMemberChangeUsernameViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.ToCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult Deactivate(Guid id)
        {
            var query = MemberExistsQuery.Make(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel("Deactivate"));

            return View(MemberDeactivateViewModel.Make(id));
        }

        [HttpPost]
        public ActionResult Deactivate(Guid id,
            [ModelBinder(typeof(FoobleModelBinder))] IMemberDeactivateViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.ToCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Detail(Guid id)
        {
            var query = MemberDetailQuery.Make(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel());

            return View(result.ReadModel);
        }

        [HttpGet]
        public ActionResult List()
        {
            var query = MemberListQuery.Make();
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.ToMessageDisplayReadModel());

            return View(result.ReadModel);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(MemberRegisterViewModel.Empty);
        }

        [HttpPost]
        public ActionResult Register([ModelBinder(typeof(FoobleModelBinder))] IMemberRegisterViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var id = _keyGenerator.Invoke();
            var command = viewModel.ToCommand(id);
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }
    }
}
