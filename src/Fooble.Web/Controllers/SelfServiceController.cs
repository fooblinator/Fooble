using Fooble.Core;
using MediatR;
using System;
using System.Web.Mvc;

namespace Fooble.Web.Controllers
{
    public class SelfServiceController : Controller
    {
        readonly IMediator _mediator;

        public SelfServiceController(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator), "Mediator should not be null");

            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult Register()
        {
            var readModel = SelfServiceRegister.MakeInitialReadModel();

            return View(readModel);
        }
    }
}
