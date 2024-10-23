# Introduction to Domain Events

## Learning Objectives 
- Understand what Domain Events are and why they are important in DDD.
- Learn about the characteristics of Domain Events and how they differ from other types of events.
- Explore how Domain Events are used to model significant changes in the business domain.

## What is a Domain Event?

A **Domain Event** is a significant occurrence or change in the state of the system, and it is something that other components or systems might care about. Domain events are fundamental in DDD because they capture meaningful changes in the business domain and help to model how different parts of the system should react to these changes.


#### Characteristics of an Event:

1. **Happened in the Past**:
   - Events describe something that has already occurred. They record historical actions or state changes within the system.
   - Example: An event such as `OrderPlaced` indicates that the order has been placed and completed in the past.

2. **Represents Something Important**:
   - Events usually represent significant moments in the business domain. These events often reflect important state changes, actions, or external signals that other components need to respond to.
   - Example: `ProductPriceChanged` is important because other systems (e.g., notifications, pricing engine) might need to respond to this change.

3. **Notification or Trigger**:
   - Events notify other parts of the system, prompting them to take some action or react. When something significant happens, the event serves as a trigger for further behavior within the system.
   - Example: When a `CustomerRegistered` event occurs, it might trigger a series of actions, such as sending a welcome email or provisioning a new account.

### Events in DDD

In **Domain-Driven Design (DDD)**, events are an essential part of modeling the business domain. They represent important changes that occur within the business and provide a way to decouple different parts of the system. By using events, different components can react to changes in a flexible, scalable manner.

- **Domain Events** represent changes that are meaningful to the business. When a domain event occurs, it often means something significant has happened that other parts of the system should respond to. This might be a change in state, such as an order being completed or a product being updated.

### Two Types of Events in DDD

1. **Domain Events**:
   - Domain Events are part of the **Domain Layer** in a DDD architecture.
   - They encapsulate changes within the domain itself and are meaningful to the business.
   - Example: `OrderCompleted`, `ProductPriceChanged`.

2. **Integration Events**:
   - Integration Events are used to communicate changes between different **bounded contexts** or **external systems**.
   - These events often trigger actions outside of the domain in which they occurred, facilitating communication and coordination between systems. (You will learn more about this later)

### How Events Trigger Reactions in the System

Events are not just passive records; they often trigger reactions within the system. For example:
- A `ProductPriceChanged` event might trigger a notification system to alert subscribers about the price change.
- A `ProductCreated` event might update the total product count for a brand.

### Events in GraphQL

It’s important to note that **events in GraphQL** are not necessarily the same as GraphQL **subscriptions**. While you can use domain events to trigger subscription updates in GraphQL, domain events themselves represent something that has happened in the business domain. Subscriptions in GraphQL can be tied to these events to inform clients of changes, but they serve different purposes.

- **Domain Events**: Focus on changes within the business domain, which are meaningful to the internal system.
- **GraphQL Subscriptions**: Can be used to notify clients when these domain events occur, but they are more about real-time updates in the client-server communication model.