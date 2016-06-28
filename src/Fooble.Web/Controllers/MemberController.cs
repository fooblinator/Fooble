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
        readonly IdGenerator _idGenerator;
        readonly InitialMemberChangePasswordViewModelFactory _memberChangePasswordViewModelFactory;
        readonly InitialMemberDeactivateViewModelFactory _memberDeactivateViewModelFactory;
        readonly MemberDetailQueryFactory _memberDetailQueryFactory;
        readonly MemberEmailQueryFactory _memberEmailQueryFactory;
        readonly MemberExistsQueryFactory _memberExistsQueryFactory;
        readonly MemberListQueryFactory _memberListQueryFactory;
        readonly MemberOtherQueryFactory _memberOtherQueryFactory;
        readonly InitialMemberRegisterViewModelFactory _memberRegisterViewModelFactory;
        readonly MemberUsernameQueryFactory _memberUsernameQueryFactory;

        public MemberController(
            IMediator mediator,
            IdGenerator idGenerator,
            InitialMemberChangePasswordViewModelFactory memberChangePasswordViewModelFactory,
            InitialMemberDeactivateViewModelFactory memberDeactivateViewModelFactory,
            MemberDetailQueryFactory memberDetailQueryFactory,
            MemberEmailQueryFactory memberEmailQueryFactory,
            MemberExistsQueryFactory memberExistsQueryFactory,
            MemberListQueryFactory memberListQueryFactory,
            MemberOtherQueryFactory memberOtherQueryFactory,
            InitialMemberRegisterViewModelFactory memberRegisterViewModelFactory,
            MemberUsernameQueryFactory memberUsernameQueryFactory)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator),
                    "Mediator is required");

            if (idGenerator == null)
                throw new ArgumentNullException(nameof(idGenerator),
                    "Id generator is required");

            if (memberChangePasswordViewModelFactory == null)
                throw new ArgumentNullException(nameof(memberChangePasswordViewModelFactory),
                    "Member change password view model factory is required");

            if (memberDeactivateViewModelFactory == null)
                throw new ArgumentNullException(nameof(memberDeactivateViewModelFactory),
                    "Member deactivate view model factory is required");

            if (memberDetailQueryFactory == null)
                throw new ArgumentNullException(nameof(memberDetailQueryFactory),
                    "Member detail query factory is required");

            if (memberEmailQueryFactory == null)
                throw new ArgumentNullException(nameof(memberEmailQueryFactory),
                    "Member email query factory is required");

            if (memberExistsQueryFactory == null)
                throw new ArgumentNullException(nameof(memberExistsQueryFactory),
                    "Member exists query factory is required");

            if (memberListQueryFactory == null)
                throw new ArgumentNullException(nameof(memberListQueryFactory),
                    "Member list query factory is required");

            if (memberOtherQueryFactory == null)
                throw new ArgumentNullException(nameof(memberOtherQueryFactory),
                    "Member other query factory is required");

            if (memberRegisterViewModelFactory == null)
                throw new ArgumentNullException(nameof(memberRegisterViewModelFactory),
                    "Member register view model factory is required");

            if (memberUsernameQueryFactory == null)
                throw new ArgumentNullException(nameof(memberUsernameQueryFactory),
                    "Member username query factory is required");

            _mediator = mediator;
            _idGenerator = idGenerator;
            _memberChangePasswordViewModelFactory = memberChangePasswordViewModelFactory;
            _memberDeactivateViewModelFactory = memberDeactivateViewModelFactory;
            _memberDetailQueryFactory = memberDetailQueryFactory;
            _memberEmailQueryFactory = memberEmailQueryFactory;
            _memberExistsQueryFactory = memberExistsQueryFactory;
            _memberListQueryFactory = memberListQueryFactory;
            _memberOtherQueryFactory = memberOtherQueryFactory;
            _memberRegisterViewModelFactory = memberRegisterViewModelFactory;
            _memberUsernameQueryFactory = memberUsernameQueryFactory;
        }

        [HttpGet]
        public ActionResult ChangeEmail(Guid id)
        {
            var query = _memberEmailQueryFactory(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            return View(result.ViewModel);
        }

        [HttpPost]
        public ActionResult ChangeEmail(Guid id, IMemberChangeEmailViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.MapCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult ChangeOther(Guid id)
        {
            var query = _memberOtherQueryFactory(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            return View(result.ViewModel);
        }

        [HttpPost]
        public ActionResult ChangeOther(Guid id, IMemberChangeOtherViewModel viewModel, string submit)
        {
            Debug.Assert(viewModel != null, "View model is required");
            Debug.Assert(submit == "default" || submit == "random", "Submit is unexpected");

            if (submit == "random") return View(viewModel.RandomizeAvatarData());

            if (!ModelState.IsValid) return View(viewModel);

            var command = viewModel.MapCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            if (!ModelState.IsValid) return View(viewModel);

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult ChangePassword(Guid id)
        {
            var query = _memberExistsQueryFactory(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel("Change Password"));

            return View(_memberChangePasswordViewModelFactory(id));
        }

        [HttpPost]
        public ActionResult ChangePassword(Guid id, IMemberChangePasswordViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.MapCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult ChangeUsername(Guid id)
        {
            var query = _memberUsernameQueryFactory(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            return View(result.ViewModel);
        }

        [HttpPost]
        public ActionResult ChangeUsername(Guid id, IMemberChangeUsernameViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.MapCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }

        [HttpGet]
        public ActionResult Deactivate(Guid id)
        {
            var query = _memberExistsQueryFactory(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel("Deactivate"));

            return View(_memberDeactivateViewModelFactory(id));
        }

        [HttpPost]
        public ActionResult Deactivate(Guid id, IMemberDeactivateViewModel viewModel)
        {
            Debug.Assert(viewModel != null, "View model is required");

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var command = viewModel.MapCommand();
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Detail(Guid id)
        {
            var query = _memberDetailQueryFactory(id);
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            return View(result.ReadModel);
        }

        [HttpGet]
        public ActionResult List()
        {
            var query = _memberListQueryFactory();
            var result = _mediator.Send(query);

            Debug.Assert(result != null, "Result parameter was null");

            if (result.IsNotFound)
                return View("MessageDisplay", result.MapMessageDisplayReadModel());

            return View(result.ReadModel);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(_memberRegisterViewModelFactory());
        }

        [HttpPost]
        public ActionResult Register(IMemberRegisterViewModel viewModel, string submit)
        {
            Debug.Assert(viewModel != null, "View model is required");
            Debug.Assert(submit == "default" || submit == "random", "Submit is unexpected");

            if (submit == "random") return View(viewModel.RandomizeAvatarData());

            if (!ModelState.IsValid) return View(viewModel.Clean());

            var id = _idGenerator();
            var command = viewModel.MapCommand(id);
            var result = _mediator.Send(command);

            Debug.Assert(result != null, "Result was null");

            result.AddModelErrors(ModelState);

            if (!ModelState.IsValid) return View(viewModel.Clean());

            return RedirectToAction("Detail", "Member", new { id = id });
        }
    }
}
