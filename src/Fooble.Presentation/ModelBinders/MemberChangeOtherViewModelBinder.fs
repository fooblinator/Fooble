namespace Fooble.Presentation

open Autofac.Integration.Mvc
open Fooble.Common
open System
open System.Web.Mvc

/// Provides model binding for member change other view models.
[<ModelBinderType(typeof<IMemberChangeOtherViewModel>)>]
type MemberChangeOtherViewModelBinder =
    inherit DefaultModelBinder

    val private ViewModelFactory:MemberChangeOtherViewModelFactory

    /// <summary>
    /// Validates and constructs view models based on the data submitted via HTTP request form variables, query string
    /// variables, and route data. Adds validation errors to model state.
    /// </summary>
    /// <param name="viewModelFactory">The member change other view model factory to use.</param>
    new(viewModelFactory:MemberChangeOtherViewModelFactory) =
        ensureWith (validateRequired viewModelFactory "viewModelFactory" "Member change other view model factory")
        { ViewModelFactory = viewModelFactory }

    override this.BindModel(controllerContext, bindingContext) =
#if DEBUG
        assertWith (validateRequired controllerContext "controllerContext" "Controller context")
        assertWith (validateRequired bindingContext "bindingContext" "Binding context")
#endif

        let routeData = controllerContext.RouteData
        let form = controllerContext.HttpContext.Request.Form

        match bindingContext.ModelType with

        | x when x = typeof<IMemberChangeOtherViewModel> ->
              let id = routeData.GetRequiredString("id")
              let nickname = form.Get("nickname")
              let avatarData = form.Get("avatarData")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.Parse(id)

              let modelState = bindingContext.ModelState

              match validateMemberNickname nickname with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              match validateMemberAvatarData avatarData with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              box (this.ViewModelFactory.Invoke(id, nickname, avatarData))

        | _ -> base.BindModel(controllerContext, bindingContext)
