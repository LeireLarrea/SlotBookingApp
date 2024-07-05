# Slot Booking App

## Introduction
The Slot Booking App provides a frontend that displays a fullCalendar to the user, 
and hided business logic that talks with a third party slot service to retrieve the schedule, 
then calculates available slots and offers them to the user to book.

## Getting Started
1. just run the application (no database required, no login: all config already loaded from appsettings.json)

## Debugging
Debug logging is enabled. 
Use breakpoints and look at code where required.

## Design Overview
This application follows the MVC software design pattern. 
It interacts with a ScheduleService to get the weekly availability
and with the BookingService to book the desired slots.

## Release Notes
### 2024-05-07
This is a very simple PoC that separates the third party calls from the frontend
Much work is still required: authentication and authorisation, connection to DB... 
and further use cases:
- should the user have a limit of slots they can book in a day?
- should booking made with at least 30 minutes in advance?
- should we allow the user to make booking for anybody (or should there be some validation)?
- should the user be over 18 years of age?
- should the Application allow amending bookings?
- should the Application provide a view with all future bookings?
- ...

