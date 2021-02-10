<img src="C:\Work\_Portfolio\ASP.NETCore_and_Angular\MyWarehouse\src\WebUI\src\assets\logo-long-color.png" style="zoom:20%;" /> 

🍕 A Clean Architecture + DDD sample project in ASP.NET Core 🍕

---------

![.NET Core](https://github.com/baratgabor/MyWarehouse/workflows/.NET%20Core/badge.svg)

-------------------------

Hola, muchachos! What you have here is a sample project consisting of an ASP.NET Core web API backend service and an Angular frontend (with Bootstrap).

Finally decided it's time to have something on my GitHub that is actually relevant to my professional focus on ASP.NET Core, retiring those *god-awfully written*\* old classical ASP.NET projects I had on BitBucket. *(\*They had business logic in the controller actions. That's all I have to say about them.)*

## Motivation

This experimental/sample project is the product of my experiences and pains with traditional n-tier, anemic model, file type folder based ASP.NET Web APIs. The ones where you have a `Controllers`, `Models`, `Services` top-level segregation, sometimes as separate assemblies, and your models are essentially simple DTOs, massaged by countless "services", "handlers", "managers", "validators", etc. All of which lead to a code base which is relatively hard to navigate, hard to reason about what touches what, brittle, and prone to bugs.

Also, the Startup.cs madness, where you just put everything in, and a few months later you have a monster Startup where you have no idea anymore what is required for what, and it often seems anything you touch breaks something else.

I'm not saying I never want to work on a more classical project, since there are realities we have to cope with, but I wanted to push my skillset and understanding into a new direction.

##### Origin/disclaimer:

This project had an older version I originally developed for a job application, with functionality they specifically asked to implement. I actually know very little of warehouses as a business domain, and I have no contact with any business experts from this domain, so the domain aspect is not intended to model a real warehouse. What I explored doing, with respect to the domain, is introducing validation and logic in entities, locking them down by not exposing mutable properties, and using value objects like *Money* and *Mass* (instead of [primitive obsession](https://blog.ploeh.dk/2011/05/25/DesignSmellPrimitiveObsession/)) in the context of EF Core (i.e. owned types).

## Backend Design Paradigms

The current project's backend system uses the following design paradigms:

### Clean Architecture:

Uncle Bob's [Clean Architecture](https://www.oreilly.com/library/view/clean-architecture-a/9780134494272/) is one of the most well-known modern architectural paradigms, which focuses on creating an application core that is devoid of infrastructural implementation concerns, pushing those out to an infrastructure layer (including the persistence mechanism, which is quite famously argued to be just a 'detail').

Similarly to the [onion architecture](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/), the dependencies between layers are strictly **pointing inwards,** so you end up with a domain and application layer that is self-contained and decoupled from higher level, less stable components of the system. Application layer dependencies are declared locally as interfaces, implemented by concrete services in the infrastructure layer.

The basis of my project structure comes from Jason Taylor's excellent [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture) template, inspired by his relevant [conference talk](https://www.youtube.com/watch?v=dK4Yb6-LxAk). But, even though this is still just a small sample project, I vastly expanded on that template, implementing many structurally significant decisions; the most significant being the integration of DDD concerns in the Domain.

### Vertical Slicing:

Vertical slicing is the concept and technique of structuring your solution into more-or-less self-contained feature slices *instead of using generalized services from layers upon layers*.

The inspiration comes from the experience of brittleness that a highly service-oriented architecture can exhibit. E.g. when you try to implement a new feature or a change in a component, so you change the service it depends on, only to realize that **you broke another feature** that depended on the same service, because you didn't take into account all use cases of the given service component. Then you go and fix it, only to realize you broke another feature in a  component far away which was relying on a second service that was relying on some validator used by a fourth service.

And there comes the realization that *DRY – don't repeat yourself –* is a recipe for spaghetti code, unless you take something very-very seriously:

> *What matters is not whether code in two or more places look superficially similar, but whether you can guarantee that they have the same – preferably single – reason to change.*

This is how you end up with a design where each feature or [use case](https://martinfowler.com/bliki/UseCase.html) is more substantial than just a few calls on a bunch of generalized services, and actually might encapsulate proper business logic in a structurally separated manner.

Another great thing about vertical slicing that is support [SCREAMING ARCHITECTURE](https://blog.cleancoder.com/uncle-bob/2011/09/30/Screaming-Architecture.html), where you look at a solution and what you see is not a bunch of meaningless generic tech terms like models, services, dtos, validators... but you see *what the system actually does, what features it has*.

One great, modern take on vertical slicing comes from [Jimmy Bogard](https://jimmybogard.com/), author of the widely used AutoMapper package, and it uses another of his projects, [MediatR](https://github.com/jbogard/MediatR). Here is an excellent talk from him on [vertical slice architecture](https://www.youtube.com/watch?v=SUiWfhAhgQw) (notice how he is not calling it "CQRS").

In my humble opinion, there are two key things to understand about MediatR: 

1. ##### MediatR is not a mediator:

   It's just not. I think pretty much everybody who knows their [object-oriented design patterns](https://sourcemaking.com/design_patterns) are bound to see this. It indeed couples two objects with an intermediary, decoupling the direct connection between the two, but that alone doesn't make it a mediator; a bunch of behavioral design patterns do that.

   One classical example of the mediator pattern is chat rooms, where you have a bunch of people wanting to broadcast messages. And instead of making their object representation send a message to each one of them back and forth, you use an intermediary – a mediator – to facilitate distributed two-way communication between them. Another example is a taxi service where the dispatcher mediates between the taxis, instead of all of them having to know about all others. So, usually it refers to longer-term, more complex communication between more than two participators.

   But, there is another pattern, called commanding, where you encapsulate an operation into an object, and execute that object with a handler. *Which is exactly what MediatR does.* And how do you select the handler with MediatR? Through a *service locator*, a.k.a. the [service locator anti-pattern](https://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).

   TL;DR;: It's commanding with a service locator on top. Duh. But no matter how we call it, it's a pretty cool piece of code, and the service locator is arguably not a big deal here (although it does obfuscate what handler will actually execute).

2. ##### MediatR has little to do with CQRS: 

   This really has become one of my pet-peeves. For some reason 90% of people on the internet today seem to associate MediatR with CQRS, and they refer to "*doing CRQS with MediatR*", while their solution shows no concern or need to actually segregate queries from commands, and what they're doing is just *vertical slicing in a command-driven fashion*. Maybe they're confused by the 'command' word, I have no idea, but commanding is an unrelated, well-known pattern.

   *CQRS, or command-query responsibility segregation, is an orthogonal pattern*. You can do CQRS with MediatR, or you can decide not to do CQRS with MediatR. Just as you can do CQRS without MediatR, or decide not to do CQRS without MediatR. But yes, arguably it's easier to implement CQRS (and a lot of other things) if you have a vertical slice architecture.

   In fact, I felt that in my sample project creating an additional level of *Commands* and *Queries* folders (like in Jason Taylor's Clean Architecture template) don't add anything to my design, and even distracts from the goals of screaming architecture, so I simply placed all operations for a given aggregate in the same folder.

   Here is an article from someone who gets it: [No, MediatR Didn't Run Over My Dog](http://scotthannen.org/blog/2020/06/20/mediatr-didnt-run-over-dog.html).

Huh, this was a long section. 😅

### Domain Driven Design:

The alternative to those empty 'entities' that solely consist of public `{ get; set; }` properties; doing proper OOP in our entities. I actually learned proper OOP design, and it was *damn difficult* back then, because when I started to program I wrote procedural code in Commodore BASIC, then in QBasic, then in Turbo Pascal, then in PHP, and I was strongly conditioned to think in procedural terms. 

Though, by banging my head against the proverbial wall I did succeed in rewiring my neural pathways to think in terms of objects which were *responsible for their own valid state*, which exposed the operations that were their concern to execute on their own state, and which could use other objects to execute other operations. And I have seen *immense value* in my designs when I started to be able to properly implement these principles (which admittedly took years, to get through the initial super-convoluted designs that added plenty of complexity without much value, which I believe are actually the reason so many people are questioning the worth of OOP (plus the inheritance madness we did, leading to rigid systems, but that's another long topic)).

But, when I started to work with ASP.NET, for some reason I swiftly abandoned everything I learned about object-oriented design, and I adapted using a bunch of properties as my model. That seemed to be the ASP.NET way of doing things, and I was a newcomer to this strange and wonderful land. Not to mention being thoroughly **terrified** of what would happen to the ORM (EF) if I added anything else than properties to the entities. *But you can add. You can add a **lot**.* At least with the current versions of EF Core.

I intentionally kept this rather abstract, because DDD is a large field, and I'm still new to it.

## Diversions from and extensions to Jason Taylor's Clean Architecture Template

As I mentioned earlier, the structural basis of this sample project, and many of its good ideas, come from Jason Taylor's excellent [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture), which has been gaining some traction in the last two years in the ASP.NET community. He also has some great [conference talks on this subject](https://www.youtube.com/watch?v=dK4Yb6-LxAk).

But we all have to discover our own ways, based on our own experiences, following our own lead; there is not much value in just mindlessly copying someone.

Here is a list of my diversions and extensions. Part of it simply stem from the fact that this is not just a template, but instead a broader sample project with significantly more features. Another, smaller part are conscious disagreements.

- Domain Driven Design
  - Value objects (owned types)
- Paging, ordering and filtering
- Feature-rich, UX-focused Angular frontend
- Infrastructure layer modularization
- Improved, modular test helpers, test data factories
- Scaffolding for strongly typed configuration
  - Azure Key Vault inclusion
- Serilog structured logging integration
- No Identity Server integration (too heavy-weight)
- Not putting the frontend into the WebApi project. I just really, really, seriously didn't get that aspect of the design.
- Deemphasizing CQRS
- Repositories, DbContext strictly concerned DB-related aspects (mapping, conversions)
- .Net 5 features

## Frontend

It's Angular, updated to version 11, with Bootstrap for layout and design. I'm not really a frontend developer. 🤷‍♂️ 

I can implement features, and sometimes they feel neat, plus I have a decent sense of UX. But, based on my past experiences, I realize that this is probably the level where if an Angular pro looked into my code they'd be positively horrified. And I hardly know anything about RxJs; I can tap and map observables, that's all.

Though, there are some not too bad aspects of the frontend code presented here. I like, for example, how I automated the displaying of form validation errors via [custom validators](https://github.com/baratgabor/MyWarehouse/blob/master/src/WebUI/src/app/core/errorhandling/form-validation/validators/custom-validators.ts) and a component that [lists all reported validation errors](https://github.com/baratgabor/MyWarehouse/blob/master/src/WebUI/src/app/core/errorhandling/form-validation/components/show-validation-errors/show-validation-errors.component.ts).

I like Angular. But this was already enough talk of working with an underlying language where an empty string is a number, and it equals to zero.