for /r %%G in (*.sql) do sqlcmd -S (localdb)\MSSQLLocalDB -U "" -P "" -d CookIt -i"%%G"
pause