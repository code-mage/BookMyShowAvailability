An quick and dirty script to notify you of new shows on BookMyShow

I made this when we couldn't get tickets to the new Avengers movie, because whenever we tried, it was always booked.
So, I decided to write a script to notify me when new shows became available, so I could book it then.

The script will take the following params
1. Dates - you can specfy the date/s on which you are want to watch the show.
2. Places - you can specify the venue/s where you want to watch it. You can get this code from the BookMyShow url for the venue. - https://in.bookmyshow.com/buytickets/pvr-inorbit-cyberabad/cinema-hyd-CXCB-MT/20180503 : CXCB is the code.
3. Format - This is a code to speicfy the moview and the lannguage and dimensions of the movie. You can get this code from the BookMyShow url for the movie - https://in.bookmyshow.com/buytickets/avengers-infinity-war-hyderabad/movie-hyd-ET00073462-MT/20180503 : ET00073462 is the code

You can give a list of email addresses, to which a message contaning details of the newly available shows will be mailed directly.

Look through the sample inputArgs file here.

You can use Windows scheduler to run it in a loop, to check this continuously.
Hopefully I'll improve the script on the weeked to automate this part.
