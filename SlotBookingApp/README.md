# Slot Booking App

## Introduction
The Slot Booking App provides a frontend that displays a fullCalendar to the user, 
and hided business logic that talks with a third party slot service to retrieve the schedule, 
then calculates available slots and offers them to the user to book.

## Design Overview
This application follows the MVC software design pattern. 
It interacts with a ScheduleService to get the weekly availability
and with the BookingService to book the desired slots.

## Getting Started
1. Make sure your localhost port is set to 5230, or alrernatively update appsettings.json/LocalhostSettings/BaseUrl with your port number
2. Then run the application (no database required, no login: all config already loaded from appsettings.json)

## Debugging
Debug logging is enabled. 
For c# debugging we recoment to use http://localhost:5230/swagger/index.html to trigger the main methods and put breakpoints on your points of interest.
For JS debugging we recommend to use Console.log() messages.

## Supporting Documentation
## XML Documentation
You can find detailed information about the public methods on SlotBookingApp\SlotBookingApp\XMLdocumentation.xml

### Swagger
See below for example messages when using the Swagger UI

#### GetAvailableSlots(string date)
Sample requests:
from the UI: GET /home/GetAvailableSlots?date=2024-07-15
from swager: GET GetAvailableSlots   Date: 2024-07-15

#### BookEvent([FromBody] CalendarEventModel eventData)
Sample request from swagger:
from the UI: POST '/home/BookEvent'
	{
	  "start": "2024-07-12T08:40:00+01:00",
	  "end": "2024-07-12T08:50:00+01:00",
	  "name": "YourName",
	  "secondName": "YourSurname",
	  "email": "email@email.com",
	  "phone": "12345678901",
	  "comments": "I just like doctors",
	  "facilityId": "7960f43f-67dc-4bc5-a52b-9a3494306749"
	}
from swagger: POST 'api/swagger/book-event'
	{
	  "start": "2024-07-12T08:40:00+01:00",
	  "end": "2024-07-12T08:50:00+01:00",
	  "name": "YourName",
	  "secondName": "YourSurname",
	  "email": "email@email.com",
	  "phone": "12345678901",
	  "comments": "I just like doctors",
	  "facilityId": "7960f43f-67dc-4bc5-a52b-9a3494306749"
	}



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

