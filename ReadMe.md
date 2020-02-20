The purpose of this solution is to demonstration how through a Restful Web API client server infrastructure
we can perform the following actions.

-Create a new account
-Login
-Record a deposit
-Record a withdrawal
-Check balance
-See transaction history
-Log out

I have included two types of clients these are a dot net core Web MVC UI and dot net core console application. 

Please note that I am using Role based permissions so one user may not be able to do everything. (As at the bank I cannot log my own transactions)

As follows

-Create a new account (Teller/Admin/User)
-Login (Teller/Admin/User)
-Record a deposit (Teller/Admin)
-Record a withdrawal (Teller/Admin)
-Check balance (Teller/Admin/User)
-See transaction history (Teller/Admin/User)
-Log out (Teller/Admin/User)

Additionally, I tried to lock down this site as much as I could and still get it completed in a short amount of time. I 
am using token authentication and authorization on the web services and username and password to authenticate to retrieve it. 

The Web API really should be running on SSL but I decided to keep it as clear text for simplity sake. In real world it should be in SSL
I'm using a self signed cert on the WebUI because I have no wildcard cert. 

The users that have been created are:

Admin User:
UserName: GSmight@Avengers.com
Password: AvengersRule@

User:
UserName: Janet@Pizza.com
Password: PizzaTime

The Account Numbers in the system are:
99868786 
584752341
897562213
1847562247
